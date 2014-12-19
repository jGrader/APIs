using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Security;
using GraderApi.Principals;
using GraderDataAccessLayer;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Models;
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

        private Task<HttpResponseMessage> CreateTask(HttpRequestMessage request, HttpStatusCode code, Object data)
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
                var userId = -1;
                var username = request.Headers.GetValues(HeaderConstants.UsernameHeader).FirstOrDefault();
                var password = request.Headers.GetValues(HeaderConstants.PasswordHeader).FirstOrDefault();
                if (username == null || password == null)
                {
                    return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCredentials);
                }

                //Check with the LDAP server that the user's credentials are valid
                if (Membership.ValidateUser(username, password))
                {
                    //If they are, take his data from the server
                    var serverUser = ActiveDirectoryRoleProvider.GetUserEntry(username);

                    try
                    {
                        //Now check if they are logging in for the first time or not
                        var foundUser = _userRepository.GetByUsername(username);

                        //If we reached this point, it's not the first time -- make sure the info is updated and move on
                        foundUser.Email = serverUser.Properties["mail"].Value.ToString();
                        var result = _userRepository.Update(foundUser);
                        if (!result)
                        {
                            return CreateTask(request, HttpStatusCode.InternalServerError, Messages.InternalDatabaseError);
                        }

                        userId = foundUser.Id;
                    }
                    catch (ObjectNotFoundException)
                    {
                        //If this is the first time that the user logs in on this website, create a password-less local account 
                        var user = new UserModel
                        {
                            UserName = username,
                            Name = serverUser.Properties["givenName"].Value.ToString(),
                            Surname = serverUser.Properties["sn"].Value.ToString(),
                            Email = serverUser.Properties["mail"].Value.ToString(),
                            GraduationYear = serverUser.Properties["description"].Value.ToString()
                        };

                        var result = _userRepository.Add(user);
                        if (!result)
                        {
                            return CreateTask(request, HttpStatusCode.InternalServerError, Messages.InternalDatabaseError);
                        }

                        //The Id gets set by the database, so the 'user' variable does not know it
                        userId = _userRepository.GetByUsername(username).Id;
                    }


                    var newSessionId = _sessionIdRepository.Add(userId);
                    if (newSessionId == Guid.Empty)
                    {
                        return CreateTask(request, HttpStatusCode.InternalServerError, Messages.InternalDatabaseError);
                    }

                    return CreateTask(request, HttpStatusCode.Accepted, newSessionId);
                }
                else
                {
                    //If we reach this point, the user's credentials were invalid. Let him know about that!
                    return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCredentials);
                }
                
                /* OLD FILIP CODE
                var user = _userRepository.Get(1);
                var principal = new UserPrincipal(new GenericIdentity("filip"), new string[] {}, user );
                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }
                 */
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

        private static class ActiveDirectoryRoleProvider
        {
            private static string ConnectionStringName = WebConfigurationManager.AppSettings["LDAPConnectionString"];
            private static string ConnectionUsername = WebConfigurationManager.AppSettings["LDAPUsername"];
            private static string ConnectionPassword = WebConfigurationManager.AppSettings["LDAPPassword"];
            private static string AttributeMapUsername = WebConfigurationManager.AppSettings["LDAPAttributeMapUsername"];

            public static DirectoryEntry GetUserEntry(string username)
            {
                var root = new DirectoryEntry(WebConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString, ConnectionUsername, ConnectionPassword);
                var searcher = new DirectorySearcher(root, string.Format(CultureInfo.InvariantCulture, "(&(objectClass=user)({0}={1}))", AttributeMapUsername, username));

                SearchResult result = searcher.FindOne();

                if (result != null && !string.IsNullOrEmpty(result.Path))
                    return result.GetDirectoryEntry();

                return null;
            }

            public static string[] GetRolesForUser(string username)
            {
                var allRoles = new List<string>();

                var root = new DirectoryEntry(WebConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString, ConnectionUsername, ConnectionPassword);

                var searcher = new DirectorySearcher(root, string.Format(CultureInfo.InvariantCulture, "(&(objectClass=user)({0}={1}))", AttributeMapUsername, username));
                searcher.PropertiesToLoad.Add("memberOf"); searcher.PropertiesToLoad.Add("employeeType");

                SearchResult result = searcher.FindOne();

                if (result != null && !string.IsNullOrEmpty(result.Path))
                {
                    DirectoryEntry user = result.GetDirectoryEntry();

                    allRoles.Add(user.Properties["employeeType"].Value.ToString()); //This will return either 'Student', 'Professor', 'Technician' etc.
                    PropertyValueCollection groups = user.Properties["memberOf"]; //This will return everything else, like mailing lists etc.

                    foreach (string path in groups)
                    {
                        string[] parts = path.Split(',');

                        if (parts.Length > 0)
                        {
                            foreach (string part in parts)
                            {
                                string[] p = part.Split('=');

                                if (p[0].Equals("cn", StringComparison.OrdinalIgnoreCase))
                                {
                                    allRoles.Add(p[1]);
                                }
                            }
                        }
                    }
                }


                return allRoles.ToArray();
            }

            public static bool IsUserInRole(string username, string roleName)
            {
                string[] roles = GetRolesForUser(username);

                foreach (string role in roles)
                {
                    if (role.Equals(roleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}