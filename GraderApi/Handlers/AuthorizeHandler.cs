using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using GraderApi.Principals;
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

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null || !String.Equals(request.RequestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                return CreateTask(request, HttpStatusCode.BadRequest, Messages.NotHttps);
            }

            if (request.Headers.Contains(HeaderConstants.SessionIdHeader))
            {
                //This means that the request contains a SessionId which has to be checked first
                var sessionId = request.Headers.GetValues(HeaderConstants.SessionIdHeader).FirstOrDefault();
                if (sessionId == null)
                {
                    return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidSessionId);
                }
                try
                {
                    var searchResult = _sessionIdRepository.GetBySesionId(new Guid(sessionId));
                    if (_sessionIdRepository.IsAuthorized(searchResult)) //If the sessionId is active, then slide the expiration time and log the user in
                    {
                        try
                        {
                            var user = _userRepository.Get(searchResult.UserId);
                            var principal = new UserPrincipal(new GenericIdentity(user.UserName), new string[] { }, user);

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
                    else //The sessionId is not active; let the client know in order to redirect to the login screen
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
            else if (request.Headers.Contains(HeaderConstants.AuthorizeHeader)) //This means that the request does not contain a SessionId and that it is a login request so we expect an Authorize header
            {
                var authorizationData = request.Headers.GetValues(HeaderConstants.AuthorizeHeader).FirstOrDefault(); //get the Base64 encoded string of username:password
                if (authorizationData == null)
                {
                    return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCredentials);
                }

                var credentials = ParseAuthorizationHeader(authorizationData);
                    //from the Base64 string (username:password), get the two individual parts
                if (credentials == null)
                {
                    return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCredentials);
                }

                //Check with the LDAP server that the user's credentials are valid
                if (Membership.ValidateUser(credentials.Username, credentials.Password))
                {
                    //If they are, take his data from the server
                    var serverUser = ActiveDirectoryRoleProvider.GetUserEntry(credentials.Username);
                    if (serverUser == null)
                    {
                        return CreateTask(request, HttpStatusCode.InternalServerError, Messages.InternalDatabaseError);
                    }

                    var userId = -1;
                    try
                    {
                        //Now check if they are logging in for the first time or not
                        var foundUser = _userRepository.GetByUsername(credentials.Username);

                        //If we reached this point, it's not the first time -- make sure the info is updated and move on
                        foundUser.Email = serverUser.Properties["mail"].Value.ToString();
                        foundUser.GraduationYear = serverUser.Properties["description"].Value.ToString();

                        var result = _userRepository.Update(foundUser);
                        if (!result)
                        {
                            return CreateTask(request, HttpStatusCode.InternalServerError,
                                Messages.InternalDatabaseError);
                        }

                        userId = foundUser.Id;
                    }
                    catch (ObjectNotFoundException)
                    {
                        //If we reached this point, it is the first time that the user logs in on this website, create a password-less local account 
                        var user = new UserModel
                        {
                            UserName = credentials.Username,
                            Name = serverUser.Properties["givenName"].Value.ToString(),
                            Surname = serverUser.Properties["sn"].Value.ToString(),
                            Email = serverUser.Properties["mail"].Value.ToString(),
                            GraduationYear = serverUser.Properties["description"].Value.ToString()
                        };

                        var result = _userRepository.Add(user);
                        if (!result)
                        {
                            return CreateTask(request, HttpStatusCode.InternalServerError,
                                Messages.InternalDatabaseError);
                        }

                        //The Id gets set by the database, so the 'user' variable does not know it
                        userId = _userRepository.GetByUsername(credentials.Username).Id;
                    }

                    if (userId == -1) //If the userId is -1, then something still went wrong; but what?
                    {
                        return CreateTask(request, HttpStatusCode.InternalServerError,
                            Messages.InternalDatabaseError);
                    }

                    //At this point, the user certainly has a local account and his credentials check out
                    var newSessionId = _sessionIdRepository.Add(userId);
                    if (newSessionId == Guid.Empty)
                    {
                        return CreateTask(request, HttpStatusCode.InternalServerError,
                            Messages.InternalDatabaseError);
                    }

                    try
                    {
                        var user = _userRepository.Get(userId);
                        var principal = new UserPrincipal(new GenericIdentity(user.UserName), new string[] {}, user);

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

                    //Everythin is fine; Hallelujah! Send the client the active sessionId so that it can use it in future requests
                    return CreateTask(request, HttpStatusCode.Accepted, newSessionId);
                }

                //If we reach this point, the user's credentials were invalid. Let him know about that!
                return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCredentials);
            }
            else
            {
                //If we got so far, something is wrong with the request; deny all access
                return CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidRequest);
            }

            return base.SendAsync(request, cancellationToken);

            //FILIP WANTS THE CODE BELOW TO STAY HERE COMMENTED OUT
            /*
            var url = request.RequestUri.LocalPath.
             
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
        }

        private static Task<HttpResponseMessage> CreateTask(HttpRequestMessage request, HttpStatusCode code, Object data)
        {
            var response = request.CreateResponse(code, data);

            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);

            return tsc.Task;
        }

        private class Credentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        private static Credentials ParseAuthorizationHeader(string authHeader)
        {
            string[] credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader)).Split(new[] { ':' });
            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[1]))
            {
                return null;
            }

            return new Credentials() { Username = credentials[0], Password = credentials[1] };
        }

        private static class ActiveDirectoryRoleProvider
        {
            private static readonly string ConnectionStringName = WebConfigurationManager.AppSettings["LDAPConnectionString"];
            private static readonly string ConnectionUsername = WebConfigurationManager.AppSettings["LDAPUsername"];
            private static readonly string ConnectionPassword = WebConfigurationManager.AppSettings["LDAPPassword"];
            private static readonly string AttributeMapUsername = WebConfigurationManager.AppSettings["LDAPAttributeMapUsername"];

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