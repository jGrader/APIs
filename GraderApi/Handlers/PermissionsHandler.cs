namespace GraderApi.Handlers
{
    using System.Configuration;
    using Extensions;
    using Grader.ExtensionMethods;
    using GraderDataAccessLayer;
    using Resources;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Dispatcher;

    public class PermissionsHandler : DelegatingHandler
    {
        private readonly Logger _logger;

        public PermissionsHandler(HttpConfiguration httpConfiguration)
        {
            _logger = new Logger();
            InnerHandler = new HttpControllerDispatcher(httpConfiguration);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (var unitOfWork = new UnitOfWork(ConfigurationManager.ConnectionStrings[DatabaseConnections.MySQL].ConnectionString))
            {
                var currentUser = HttpContext.Current.User as UserPrincipal;
                if (currentUser == null)
                {
                    // Make sure that there is nothing fishy about the user's login status, 
                    // i.e. that nothing went wrong in the AuthorizationHandler
                    return await CreateTask(request, HttpStatusCode.BadRequest, Messages.UserNotFound);
                }

                try
                {
                    // Check whether the currentUser is a super user or an admin
                    var adminPermissions = new string[] {};
                    var adminUser = await unitOfWork.AdminRepository.GetByUserId(currentUser.User.Id);
                    if (adminUser != null) //This means that the user is an admin
                    {
                        if (adminUser.IsSuperUser)
                        {
                            // SuperUser which can do ANYTHING; just add that and exit 
                            var superUserPermission = GetSuperUserPermissions();
                            UpdatePrincipal(currentUser, superUserPermission);

                            return await base.SendAsync(request, cancellationToken);
                        }

                        //If we reach this point, (s)he is an admin but not SuperUser; just load adminPermissions
                        adminPermissions = GetAdminPermissions();
                    }

                    // Depending on incoming route, something else needs to be done to identify permissions
                    var courseIdString = request.GetRouteData().Values["courseId"] as string;
                    if (courseIdString == null)
                    {
                        // Find out if the user is trying to access a method of the 'Courses' controller
                        var controller = request.GetRouteData().Values["controller"] as string;
                        if (controller != "Courses")
                        {
                            // If it's not the 'Courses' controller, right now it can only be the 'Users' or 'CurrentUser' controller, none of which have Permission requirements
                            // Everything is public there except for SuperUser permissions which got cleared a few lines above; just move forward
                            return await base.SendAsync(request, cancellationToken);
                        }

                        // If it is the 'Courses' controller and there is no 'courseId' parameter,
                        // the only non public action which qualifies is 'Add' which is in adminPermissions
                        UpdatePrincipal(currentUser, adminPermissions);
                        return await base.SendAsync(request, cancellationToken);
                    }

                    int courseId;
                    if (!int.TryParse(courseIdString, out courseId))
                    {
                        // Make sure that the id is valid; the CourseConstraint class does this already, but just to be safe
                        return await CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCourse);
                    }

                    // Now find out if the user is the CourseOwner
                    if (adminPermissions.Count() != 0)
                    {
                        // Only admins can be CourseOwners, 
                        // so this test only applies to those who have adminPermissions
                        var course = await unitOfWork.CourseRepository.Get(courseId);
                        if (course == null)
                        {
                            // Somehow the course can still not be found ...
                            return await CreateTask(request, HttpStatusCode.NotFound, Messages.InvalidCourse);
                        }

                        if (course.OwnerId == currentUser.User.Id)
                        {
                            // The user is the CourseOwner; 
                            // load all permissions (except SuperUser) and continue
                            var ownerPermissions = GetOwnerPermissions();
                            UpdatePrincipal(currentUser, ownerPermissions);

                            return await base.SendAsync(request, cancellationToken);
                        }
                    }

                    // If the user is not the CourseOwner, check that (s)he is actually enrolled for this course 
                    var courseUser = await unitOfWork.CourseUserRepository.GetByExpression( cu => (cu.UserId == currentUser.User.Id) && (cu.CourseId == courseId));
                    var firstOrDefault = courseUser.FirstOrDefault();
                    if (firstOrDefault == null)
                    {
                        //The user is not registered for this course         
                        return await CreateTask(request, HttpStatusCode.Forbidden, Messages.InvalidRequest);
                    }

                    var permissions = GetUserPermissions(firstOrDefault.Permissions);
                    //In case the user is also an admin, add those roles also - just in case
                    ExtensionMethods.Concat(permissions, adminPermissions);

                    UpdatePrincipal(currentUser, permissions);
                    return await base.SendAsync(request, cancellationToken);
                }
                catch (Exception e)
                {
                    // Unexpected error
                    _logger.Log(e);

                    var tsk = Task.Run(() => CreateTask(request, HttpStatusCode.InternalServerError, Messages.UnexpectedError), cancellationToken);
                    Task.WaitAll(tsk);

                    return tsk.Result;
                } 
            }
        }

        #region Helpers
        private static Task<HttpResponseMessage> CreateTask(HttpRequestMessage request, HttpStatusCode code, Object data)
        {
            var response = request.CreateResponse(code, data);
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            return tsc.Task;
        }
        private static void UpdatePrincipal(UserPrincipal curUser, string[] newPermissions)
        {
            var principal = new UserPrincipal(new GenericIdentity(curUser.User.UserName), newPermissions, curUser.User);

            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        // This has to be a string[] instead of IEnumerable because of the constructor in the GenericPrincipal
        private static string[] GetSuperUserPermissions()
        {
            var res = Enum.GetNames(typeof(SuperUserPermissions)).ToList();
            res.AddRange(GetOwnerPermissions());

            return res.ToArray();
        }
        private static string[] GetOwnerPermissions()
        {
            var res = Enum.GetNames(typeof(CourseOwnerPermissions)).ToList(); 
            res.AddRange(Enum.GetNames(typeof(CoursePermissions)));
            res.AddRange(GetAdminPermissions()); 

            return res.ToArray();
        }
        private static string[] GetAdminPermissions()
        {
            var res = Enum.GetNames(typeof(AdminPermissions)).ToList();
            
            return res.ToArray();
        }
        private static string[] GetUserPermissions(int permissions)
        {
            var res = new List<string>();
            var names = Enum.GetNames(typeof (CoursePermissions));

            //Search through all the permissions which are not 'Nothing'
            foreach (var permission in names.Where(permission => (long) Enum.Parse(typeof (CoursePermissions), permission) != 0))
            {
                if (permissions % 2 == 1)
                {
                    res.Add(permission);
                }
                permissions = permissions >> 1;
            }

            if (res.Count == 0)
            {
                //If the user has no permissions, simply add 'Nothing' - 0 in the enum
                res.Add(Enum.GetName(typeof (CoursePermissions), 0));
            }
            return res.ToArray();
        }
        #endregion
    }
}