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
        private readonly ICourseUserRepository _courseUserRepository;

        public PermissionsHandler(HttpConfiguration httpConfiguration)
        {
            _courseUserRepository = new CourseUserRepository();
            InnerHandler = new HttpControllerDispatcher(httpConfiguration); 
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var courseIdString = request.GetRouteData().Values["courseId"] as string;

            if (courseIdString == null)
            {
                return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            int courseId;
            if (!int.TryParse(courseIdString, out courseId))
            {
                return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            var curUser = HttpContext.Current.User as UserPrincipal;
            if (curUser == null)
            {
                return CreateTask(request, HttpStatusCode.BadRequest, Messages.UserNotFound);
            }
            
            
            try
            {
                var courseUser = _courseUserRepository.GetByLambda(cu => (cu.UserId == curUser.User.Id) && (cu.CourseId == courseId)).FirstOrDefault();
                if (courseUser == null) 
                {
                    //The user is not registered for this course
                    return CreateTask(request, HttpStatusCode.Forbidden, Messages.InvalidRequest);
                }

                var permissions = GetPermissions(courseUser.Permissions);
                var principal = new UserPrincipal(new GenericIdentity(curUser.User.UserName), permissions, curUser.User);

                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }
            }
            catch (Exception)
            {
                return CreateTask(request, HttpStatusCode.BadRequest, Messages.UserNotFound);
            }
              
            return base.SendAsync(request, cancellationToken);
        }

        private Task<HttpResponseMessage> CreateTask(HttpRequestMessage request, HttpStatusCode code, Object data)
        {
            var response = request.CreateResponse(code, data);
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            return tsc.Task;
        }

        // This has to be a string[] instead of IEnumerable because of the constructor in the GenericPrincipal
        private static string[] GetPermissions(int permissions)
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
    }
}