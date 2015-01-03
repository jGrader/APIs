using System.Web.Routing;
using GraderApi.Principals;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Models;
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
using Microsoft.Ajax.Utilities;


namespace GraderApi.Handlers
{
    public class PermissionsHandler : DelegatingHandler
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseUserRepository _courseUserRepository;
        private readonly IGradeComponentRepository _gradeComponentRepository;

        public PermissionsHandler(HttpConfiguration httpConfiguration)
        {
            _adminRepository = new AdminRepository();
            _courseRepository = new CourseRepository();
            _courseUserRepository = new CourseUserRepository();
            _gradeComponentRepository = new GradeComponentRepository();

            InnerHandler = new HttpControllerDispatcher(httpConfiguration); 
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var curUser = HttpContext.Current.User as UserPrincipal;
            if (curUser == null) { // Make sure that there is nothing fishy about the user's login status
                return CreateTask(request, HttpStatusCode.BadRequest, Messages.UserNotFound);
            }


            int courseId = -1, gradeComponentId = -1;
            var routeTemplate = request.GetRouteData().Route.RouteTemplate;
            switch (routeTemplate)
            {
                case "api/Courses/{courseId}":
                {
                    var courseIdString = request.GetRouteData().Values["courseId"] as string;

                    if (courseIdString == null) { // Make sure that the route is valid 
                        return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCourse);
                    }

                    if (!int.TryParse(courseIdString, out courseId)) { // Make sure that the courseId is valid; the CourseConstraint class does this already, but just to be safe
                        return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCourse);
                    }
                }
                    break;
                case "api/GradeComponents/{gradeComponentId}":
                {
                    var gradeComponentIdString = request.GetRouteData().Values["gradeComponentId"] as string;

                    if (gradeComponentIdString == null) { // Make sure that the route is valid 
                        return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCourse);
                    }

                    if (!int.TryParse(gradeComponentIdString, out gradeComponentId)) { // Make sure that the courseId is valid; the CourseConstraint class does this already, but just to be safe
                        return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCourse);
                    }
                    //Initialize the courseId ... for the following lines
                    var gradeComponent = Task.Run(() => _gradeComponentRepository.Get(gradeComponentId));
                    Task.WaitAll(gradeComponent);
                    if (gradeComponent.Result == null) {
                        return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCourse);
                    }
                    courseId = gradeComponent.Result.CourseId;
                }
                    break;
            }

            
            var adminUser = Task.Run(() => _adminRepository.GetByUserId(curUser.User.Id));
            Task.WaitAll(adminUser);
            if (adminUser.Result != null) //This means that the user is an admin
            {
                if (adminUser.Result.IsSuperUser) // SuperUser which can do ANYTHING; just add that and exit
                {
                    var superUserPermission = GetOwnerPermissions();
                    UpdatePrincipal(curUser, superUserPermission);

                    return base.SendAsync(request, cancellationToken);
                }

                //If we reach this point, he/she is an admin but not SuperUser; find out if (s)he owns this course!
                var adminCourse = Task.Run(() => _courseRepository.Get(courseId));
                Task.WaitAll(adminCourse);
                if (adminCourse.Result == null) { // Just to be safe
                    return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                if (adminCourse.Result.OwnerId == curUser.User.Id) { // Current user is the owner; give all possible rights and exit
                    var ownerPermissions = GetOwnerPermissions();
                    UpdatePrincipal(curUser, ownerPermissions);

                    return base.SendAsync(request, cancellationToken);
                }
            }

            // If we reached this point, the user is either not an admin / superUser or doesn't own the course; treat as a regular user no matter what
            var courseUser = _courseUserRepository.GetByLambda(cu => (cu.UserId == curUser.User.Id) && (cu.CourseId == courseId)).FirstOrDefault();
            if (courseUser == null) { //The user is not registered for this course         
                return CreateTask(request, HttpStatusCode.Forbidden, Messages.InvalidRequest);
            }

            var permissions = GetUserPermissions(courseUser.Permissions);
            UpdatePrincipal(curUser, permissions);
              
            return base.SendAsync(request, cancellationToken);
        }

        private Task<HttpResponseMessage> CreateTask(HttpRequestMessage request, HttpStatusCode code, Object data)
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
        private static string[] GetOwnerPermissions()
        {
            var res = Enum.GetNames(typeof (CoursePermissions)).ToList(); //Add all regular CoursePermissions
            res.AddRange(Enum.GetNames(typeof (AdminPermissions))); //Add all AdminPermissions

            return res.ToArray();
        }
    }
}