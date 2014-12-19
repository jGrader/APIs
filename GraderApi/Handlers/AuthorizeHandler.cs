using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using GraderApi.Principals;
using GraderDataAccessLayer;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Repositories;


namespace GraderApi.Handlers
{
    public class AuthorizeHandler : DelegatingHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionIdRepository _sessionIdRepository;

        public AuthorizeHandler()
        {
            _userRepository = new UserRepository();
            _sessionIdRepository = new SessionIdRepository();
        }

        protected Task<HttpResponseMessage> CreateTask(HttpRequestMessage request, HttpStatusCode code, Object data)
        {
            var response = request.CreateResponse(code, data);
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            return tsc.Task;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!String.Equals(request.RequestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                return CreateTask(request, HttpStatusCode.BadRequest, Messages.NotHttps);
            }

            if (request.Headers.Contains(HeaderConstants.SessionIdHeader))
            {
                var sessionId = request.Headers.GetValues(HeaderConstants.SessionIdHeader).FirstOrDefault();
                if (sessionId == null)
                {
                    return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidSessionId);
                }
                try
                {
                    var searchResult = _sessionIdRepository.GetBySesionId(new Guid(sessionId));
                    if (_sessionIdRepository.IsAuthorized(searchResult))
                    {
                        try
                        {
                            var user = _userRepository.Get(searchResult.UserId);
                            var principal = new UserPrincipal(new GenericIdentity("filip"), new string[] { }, user);

                            Thread.CurrentPrincipal = principal;
                            if (HttpContext.Current != null)
                            {
                                HttpContext.Current.User = principal;
                            }
                        }
                        catch (Exception)
                        {
                            return CreateTask(request, HttpStatusCode.Unauthorized, Messages.UserNotFound);
                        }
                    }
                    else
                    {
                        return CreateTask(request, HttpStatusCode.Unauthorized, Messages.ExpiredSessionId);
                    }
                }
                catch (Exception)
                {
                    //Log
                    return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidSessionId);
                }

            }
            else
            {
                var user = _userRepository.Get(1);
                var principal = new UserPrincipal(new GenericIdentity("filip"), new string[] {}, user );
                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }
            }

            // var url = request.RequestUri.LocalPath.


            /*
            var sessionIdRepository = new SessionIdRepository();
            var query = request.RequestUri.ParseQueryString();
            int userId = int.TryParse(query["UserId"], out userId) ? userId : 0;

            var result = sessionIdRepository.IsAuthorized(userId);
            if (!result)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return tsc.Task;
            }
            */
            return base.SendAsync(request, cancellationToken);
        }
    }
}