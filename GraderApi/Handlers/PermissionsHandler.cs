using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using GraderApi.Principals;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Repositories;
using System.Linq.Expressions;

namespace GraderApi.Handlers
{
    public class PermissionsHandler : DelegatingHandler
    {
        private ICourseUserRepository _courseUserRepository;

        public PermissionsHandler(HttpConfiguration httpConfiguration)
        {
            _courseUserRepository = new CourseUserRepository();
            InnerHandler = new HttpControllerDispatcher(httpConfiguration); 
        }
        private Task<HttpResponseMessage> CreateTask(HttpRequestMessage request, HttpStatusCode code, Object data)
        {
            var response = request.CreateResponse(code, data);
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            return tsc.Task;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var courseIdString = request.GetRouteData().Values["courseId"] as string;

            if (courseIdString == null)
            {
                var res = CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCourse);
                return res;
            }
            int courseId;
            if (!int.TryParse(courseIdString, out courseId))
            {
                var res = CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCourse);
                return res;
            }

            var curUser = HttpContext.Current.User as UserPrincipal;
            if (curUser == null)
            {
                var res = CreateTask(request, HttpStatusCode.BadRequest, Messages.UserNotFound);
                return res;
            }
            
            
            try
            {
                var courseUser = _courseUserRepository.GetByLambda(cu => (cu.UserId == curUser.User.Id) && (cu.CourseId == courseId)).FirstOrDefault();
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

        private string[] GetPermissions(int permissions)
        {
            var res = new List<string>();
            var names = Enum.GetNames(typeof (CoursePermissions));
            foreach (var permission in names)
            {
                if ((uint) Enum.Parse(typeof (CoursePermissions), permission) == 0)
                {
                    continue;
                }

                if (permissions%2 == 1)
                {
                    res.Add(permission);
                }
                permissions = permissions >> 1;
            }
            if(res.Count == 0)
                res.Add(Enum.GetName(typeof(CoursePermissions), 0));
            return res.ToArray();
        }
    }
}