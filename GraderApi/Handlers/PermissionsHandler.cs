using GraderApi.Principals;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Repositories;
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


namespace GraderApi.Handlers
{
    public class PermissionsHandler : DelegatingHandler
    {
        public PermissionsHandler(HttpConfiguration httpConfiguration)
        {
            InnerHandler = new HttpControllerDispatcher(httpConfiguration); 
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var currentUser = HttpContext.Current.User as UserPrincipal;
            if (currentUser == null) 
            { 
                // Make sure that there is nothing fishy about the user's login status, 
                // i.e. that nothing went wrong in the AuthorizationHandler
                return await CreateTask(request, HttpStatusCode.BadRequest, Messages.UserNotFound);
            }


            // Check whether the currentUser is a super user or an admin
            var adminPermissions = new string[] { };
            using (IAdminRepository adminRepository = new AdminRepository())
            {
                var adminUser = await adminRepository.GetByUserId(currentUser.User.Id);
                if (adminUser != null) //This means that the user is an admin
                {
                    if (adminUser.IsSuperUser) // SuperUser which can do ANYTHING; just add that and exit
                    {
                        
                        var superUserPermission = GetSuperUserPermissions();
                        UpdatePrincipal(currentUser, superUserPermission);

                        return await base.SendAsync(request, cancellationToken);
                    }

                    //If we reach this point, he/she is an admin but not SuperUser; just load adminPermissions
                    adminPermissions = GetAdminPermissions();
                }
            }
            

            // Depending on incoming route, something else needs to be done to identify permissions
            var routeTemplate = request.GetRouteData().Route.RouteTemplate;
            var controllerName = request.GetRouteData().Values["controller"] as string;
            switch (routeTemplate)
            {
                //default 'Users' route
                case "api/{controller}/{action}/{userId}":
                {
                    switch (controllerName)
                    {
                        case "Users":
                        {
                            // ONLY SUPER USERS HAVE ACCESS TO THIS ROUTE 
                            // AND THEY HAVE BEEN REDIRECTED EARLIER IN THE CODE
                            return await CreateTask(request, HttpStatusCode.Forbidden, Messages.PermissionsInsufficient);
                        }
                        default:
                        {
                            // SOMEONE FORGOT TO UPDATE THIS HANDLER
                            return await CreateTask(request, HttpStatusCode.NotImplemented, Messages.NotImplemented);
                        }
                    }
                }
                    
                //default 'Courses' route
                case "api/{controller}/{action}/{courseId}":
                {
                    int courseId;
                    switch (controllerName)
                    {
                        case "Courses":
                        {
                            courseId = ExtractId(request, "courseId");
                            if (courseId == -1)
                            {
                                return await CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCourse);
                            }
                        }
                            break;
                        case "GradeComponents":
                        {
                            courseId = ExtractId(request, "courseId");
                            if (courseId == -1)
                            {
                                return await CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCourse);
                            }
                        }
                            break;
                        default:
                        {
                            // SOMEONE FORGOT TO UPDATE THIS HANDLER
                            return await CreateTask(request, HttpStatusCode.NotImplemented, Messages.NotImplemented);
                        }
                    }

                    // NOW LOAD PERMISSIONS BASED ON WHICH COURSE THE USER IS TRYING TO ACCESS
                    var isOwner = await IsCourseOwner(adminPermissions, courseId, currentUser);
                    return await LoadPermissionsForCourseRoutes(request, currentUser, courseId, isOwner, adminPermissions, cancellationToken);
                }

                //default 'CourseUsers' route
                case "api/{controller}/{action}/{courseUserId}":
                {
                    int courseUserId;
                    switch (controllerName)
                    {
                        case "CourseUsers":
                        {
                            //GET METHOD DOES NOT REQUIRE ANY PERMISSIONS
                            //THE OTHER ONES REQUIRE YOU TO BE THE OWNER
                            courseUserId = ExtractId(request, "courseUserId");
                            if (courseUserId == -1)
                            {
                                //
                                return await CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidEnrollment);
                            }
                        }
                            break;
                        default:
                        {
                            // SOMEONE FORGOT TO UPDATE THIS HANDLER
                            return await CreateTask(request, HttpStatusCode.NotImplemented, Messages.NotImplemented);
                        }
                    }

                    int courseId;
                    using (var courseUserRepository = new CourseUserRepository())
                    {
                        var courseUser = await courseUserRepository.Get(courseUserId);
                        if (courseUser == null)
                        {
                            //
                            return await CreateTask(request, HttpStatusCode.NotFound, Messages.InvalidEnrollment);
                        }

                        courseId = courseUser.CourseId;
                    }

                    // NOW LOAD PERMISSIONS 
                    var isOwner = await IsCourseOwner(adminPermissions, courseId, currentUser);
                    return await LoadPermissionsForCourseRoutes(request, currentUser, courseId, isOwner, adminPermissions, cancellationToken);
                }

                //default 'GradeComponents' route
                case "api/{controller}/{action}/{gradeComponentId}":
                {
                    int courseId;
                    switch (controllerName)
                    {
                        case "GradeComponents":
                        {
                            var gradeComponentId = ExtractId(request, "gradeComponentId");
                            if (gradeComponentId == -1)
                            {
                                return await CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidGradeComponentId);
                            }

                            //Initialize the courseId 
                            using (var gradeComponentRepository = new GradeComponentRepository())
                            {
                                var gradeComponent = await gradeComponentRepository.Get(gradeComponentId);
                                if (gradeComponent == null)
                                {
                                    //
                                    return await CreateTask(request, HttpStatusCode.NotFound, Messages.InvalidCourse);
                                }

                                courseId = gradeComponent.CourseId;
                            }      
                        }
                            break;
                        default:
                        {
                            // SOMEONE FORGOT TO UPDATE THIS HANDLER
                            return await CreateTask(request, HttpStatusCode.NotImplemented, Messages.NotImplemented);
                        }
                    }

                    // NOW LOAD PERMISSIONS 
                    var isOwner = await IsCourseOwner(adminPermissions, courseId, currentUser);
                    return await LoadPermissionsForCourseRoutes(request, currentUser, courseId, isOwner, adminPermissions, cancellationToken);
                }

                //default 'Tasks' route
                case "api/{controller}/{action}/{taskId}":
                {
                    int courseId;
                    switch (controllerName)
                    {
                        case "Tasks":
                        {
                            var taskId = ExtractId(request, "taskId");
                            if (taskId == -1)
                            {
                                return await CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidTaskId);
                            }

                            //Initialize the courseId
                            using (var taskRepository = new TaskRepository())
                            {
                                var task = await taskRepository.Get(taskId);
                                if (task == null)
                                {
                                    //
                                    return await CreateTask(request, HttpStatusCode.NotFound, Messages.InvalidTaskId);
                                }

                                courseId = task.CourseId;
                            }
                        }
                            break;
                        default:
                        {
                            // SOMEONE FORGOT TO UPDATE THIS HANDLER
                            return await CreateTask(request, HttpStatusCode.NotImplemented, Messages.NotImplemented);
                        }
                    }

                    // NOW LOAD PERMISSIONS 
                    var isOwner = await IsCourseOwner(adminPermissions, courseId, currentUser);
                    return await LoadPermissionsForCourseRoutes(request, currentUser, courseId, isOwner, adminPermissions, cancellationToken);
                }

                //default 'Entity' route
                case "api/{controller}/{action}/{entityId}":
                {
                    int courseId;
                    switch (controllerName)
                    {
                        case "Entities":
                        {
                            var entityId = ExtractId(request, "entityId");
                            if (entityId == -1)
                            {
                                return await CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidEntity);
                            }

                            //Initialize the courseId

                            using (var entityRepository = new EntityRepository())
                            {
                                var entity = await entityRepository.Get(entityId);
                                if (entity == null)
                                {
                                    //
                                    return await CreateTask(request, HttpStatusCode.NotFound, Messages.InvalidEntity);
                                }

                                courseId = entity.Task.CourseId;
                            }
                        }
                            break;
                        default:
                        {
                            // SOMEONE FORGOT TO UPDATE THIS HANDLER
                            return await CreateTask(request, HttpStatusCode.NotImplemented, Messages.NotImplemented);
                        }
                    }

                    // NOW LOAD PERMISSIONS 
                    var isOwner = await IsCourseOwner(adminPermissions, courseId, currentUser);
                    return await LoadPermissionsForCourseRoutes(request, currentUser, courseId, isOwner, adminPermissions, cancellationToken);
                }

                //default 'Submission' route
                case "api/{controller}/{action}/{submissionId}":
                {
                    int courseId;
                    switch (controllerName)
                    {
                        case "Submissions":
                        {
                            var submissionId = ExtractId(request, "submissionId");
                            if (submissionId == -1)
                            {
                                return await CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidSubmissionId);
                            }

                            using (var submissionRepository = new SubmissionRepository())
                            {
                                var submission = await submissionRepository.Get(submissionId);
                                if (submission == null)
                                {
                                    return await CreateTask(request, HttpStatusCode.NotFound, Messages.InvalidSubmissionId);
                                }

                                courseId = submission.Entity.Task.CourseId;
                            }
                        }
                            break;
                        default:
                        {
                            // SOMEONE FORGOT TO UPDATE THIS HANDLER
                            return await CreateTask(request, HttpStatusCode.NotImplemented, Messages.NotImplemented);
                        }
                    }

                    // NOW LOAD PERMISSIONS 
                    var isOwner = await IsCourseOwner(adminPermissions, courseId, currentUser);
                    return await LoadPermissionsForCourseRoutes(request, currentUser, courseId, isOwner, adminPermissions, cancellationToken);
                }

                //default 'Submissions' route for file upload
                case "api/{controller}/{action}/{courseId}/{entityId}":
                {
                    int courseId;
                    switch (controllerName)
                    {
                        case "Submissions":
                        {
                            courseId = ExtractId(request, "courseId");
                        }
                            break;
                        default:
                        {
                            // SOMEONE FORGOT TO UPDATE THIS HANDLER
                            return await CreateTask(request, HttpStatusCode.NotImplemented, Messages.NotImplemented);
                        }
                    }

                    // NOW LOAD PERMISSIONS
                    var isOwner = await IsCourseOwner(adminPermissions, courseId, currentUser);
                    return await LoadPermissionsForCourseRoutes(request, currentUser, courseId, isOwner, adminPermissions, cancellationToken);
                }
            }

            //If we reached this point, something is wrong
            return await CreateTask(request, HttpStatusCode.Forbidden, Messages.InvalidRoute);
        }

        private async Task<HttpResponseMessage> LoadPermissionsForCourseRoutes(HttpRequestMessage request, UserPrincipal currentUser, 
            int courseId, bool isOwner, string[] adminPermissions, CancellationToken cancellationToken)
        {
            if (isOwner)
            {
                var permissions = GetOwnerPermissions();
                UpdatePrincipal(currentUser, permissions);
            }
            else
            {
                using (var courseUserRepository = new CourseUserRepository())
                {
                    var courseUser =
                        (await
                            courseUserRepository.GetAllByLambda(
                                cu => (cu.UserId == currentUser.User.Id) && (cu.CourseId == courseId))).FirstOrDefault();
                    if (courseUser == null)
                    {
                        //The user is not registered for this course         
                        return await CreateTask(request, HttpStatusCode.Forbidden, Messages.InvalidRequest);
                    }

                    var permissions = GetUserPermissions(courseUser.Permissions);
                    permissions.Concat(adminPermissions);
                        //In case the user is also an admin, add those roles also - just in case
                    UpdatePrincipal(currentUser, permissions);
                }
            }

            return await base.SendAsync(request, cancellationToken);
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

        private static async Task<bool> IsCourseOwner(string[] adminPermissions, int courseId, UserPrincipal currentUser)
        {
            if (adminPermissions.Count() != 0) // USER IS ADMIN
            {
                using (var courseRepository = new CourseRepository())
                {
                    var course = await courseRepository.Get(courseId);
                    if (course == null)
                    {
                        return false;
                    }

                    if (course.OwnerId == currentUser.User.Id)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private static int ExtractId(HttpRequestMessage request, string parameterName)
        {
            var parameterString = request.GetRouteData().Values[parameterName] as string;
            if (parameterString == null)
            {
                // Make sure that the route is valid 
                return -1;
            }

            int parameterId;
            if (!int.TryParse(parameterString, out parameterId))
            {
                // Make sure that the id is valid; the CourseConstraint class does this already, but just to be safe
                return -1;
            }

            return parameterId;
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
            foreach (var permission in names.Where(permission => (uint) Enum.Parse(typeof (CoursePermissions), permission) != 0))
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