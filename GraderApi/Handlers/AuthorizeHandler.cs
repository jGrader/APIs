namespace GraderApi.Handlers
{
    using Extensions;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Resources;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
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

    public class AuthorizeHandler : DelegatingHandler
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Logger _logger;

        public AuthorizeHandler()
        {
            _unitOfWork = new UnitOfWork();
            _logger = new Logger();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null || !String.Equals(request.RequestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                // Something is wrong with the request or the connection is not secure
                return await CreateTask(request, HttpStatusCode.BadRequest, Messages.NotHttps);
            }

            if (request.Headers.Contains(HeaderConstants.SessionIdHeader))
            {
                // This means that the request contains a SessionId which has to be checked first
                var sessionId = request.Headers.GetValues(HeaderConstants.SessionIdHeader).FirstOrDefault();
                if (String.IsNullOrEmpty(sessionId))
                {
                    // The sessionId parameter is unreadeable for some weird reason...
                    return await CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidSessionId);
                }

                try
                {
                    var searchResult = await _unitOfWork.SessionIdRepository.GetBySesionId(new Guid(sessionId));
                    if (searchResult == null)
                    {
                        // The sessionId is cannot be found in the database
                        return await CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidSessionId);
                    }

                    if (await _unitOfWork.SessionIdRepository.IsAuthorized(searchResult))
                    {
                        // The sessionId checks out
                        var user = await _unitOfWork.UserRepository.Get(searchResult.UserId);
                        if (user == null)
                        {
                            // Something is fishy about the user's login status
                            return await CreateTask(request, HttpStatusCode.Unauthorized, Messages.UserNotFound);
                        }

                        // We now know to which user it belongs so we set that user as the current UserPrincipal
                        UpdatePrincipal(user);
                    }
                    else
                    {
                        // The sessionId is expired
                        return await CreateTask(request, HttpStatusCode.Unauthorized, Messages.ExpiredSessionId);
                    }
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
            else if (request.Headers.Contains(HeaderConstants.AuthorizeHeader)) 
            {
                // This means that the request does not contain a SessionId and that it is a login request so we expect an Authorize header
                var authorizationData = request.Headers.GetValues(HeaderConstants.AuthorizeHeader).FirstOrDefault(); // get the Base64 encoded string of username:password
                if (authorizationData == null)
                {
                    return await CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCredentials);
                }

                var credentials = ParseAuthorizationHeader(authorizationData); // from the Base64 string (username:password), get the two individual parts
                if (credentials == null)
                {
                    return await CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCredentials);
                }

                try
                {
                    // Check with the LDAP server that the user's credentials are valid
                    if (!Membership.ValidateUser(credentials.Username, credentials.Password))
                    {
                        //If we reach this point, the user's credentials were invalid. Let him know about that!
                        return await CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidCredentials);
                    }

                    try
                    {
                        // Credentials check out; get his data from the server
                        var serverUser = ActiveDirectoryRoleProvider.GetUserEntry(credentials.Username);
                        if (serverUser == null)
                        {
                            // For some reason, no data was retreived for the user; why?!
                            return await CreateTask(request, HttpStatusCode.InternalServerError, Messages.InternalDatabaseError);
                        }

                        int userId;
                        // Now check if they are logging in for the first time or not
                        var foundUser = await _unitOfWork.UserRepository.GetByUsername(credentials.Username);
                        if (foundUser != null)
                        {
                            // If we reached this point, it's not the first time -- make sure the info is updated and move on
                            foundUser.Email = serverUser.Properties[LdapFields.Email].Value.ToString();
                            foundUser.GraduationYear = serverUser.Properties[LdapFields.Description].Value.ToString();

                            var result = await _unitOfWork.UserRepository.Update(foundUser);
                            if (result == null)
                            {
                                return await CreateTask(request, HttpStatusCode.InternalServerError, Messages.InternalDatabaseError);
                            }

                            userId = foundUser.Id;
                        }
                        else
                        {
                            // If we reached this point, it is the first time that the user logs in on this website;
                            // create a password-less local account 
                            var user = new UserModel
                            {
                                UserName = credentials.Username,
                                Name = serverUser.Properties[LdapFields.Name].Value.ToString(),
                                Surname = serverUser.Properties[LdapFields.Surname].Value.ToString(),
                                Email = serverUser.Properties[LdapFields.Email].Value.ToString(),
                                GraduationYear = serverUser.Properties[LdapFields.Description].Value.ToString()
                            };

                            var result = await _unitOfWork.UserRepository.Add(user);
                            if (result == null)
                            {
                                // We failed to add the new user to the database
                                return await CreateTask(request, HttpStatusCode.InternalServerError, Messages.InternalDatabaseError);
                            }

                            userId = result.Id;
                        }

                        if (userId == 0)
                        {
                            //If the userId is 0, the variable is uninitialized; something still went wrong; but what?
                            return await CreateTask(request, HttpStatusCode.InternalServerError, Messages.InternalDatabaseError);
                        }

                        //At this point, the user certainly has a local account and his credentials check out
                        var newSessionModel = await _unitOfWork.SessionIdRepository.Add(userId);
                        if (newSessionModel.SessionId == Guid.Empty)
                        {
                            // failed to register a new active session
                            return await CreateTask(request, HttpStatusCode.InternalServerError, Messages.InternalDatabaseError);
                        }

                        var userDb = await _unitOfWork.UserRepository.Get(userId);
                        if (userDb == null)
                        {
                            // The local database didn't find this user; why?!
                            return await CreateTask(request, HttpStatusCode.BadRequest, Messages.UserNotFound);
                        }

                        // Set the user as the current UserPrincipal
                        UpdatePrincipal(userDb);

                        //Everythin is fine; Hallelujah! Send the client the active sessionId so that it can use it in future requests
                        return await CreateTask(request, HttpStatusCode.OK, newSessionModel.SessionId);
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
                catch (Exception e)
                {
                    if (e.GetType() == typeof (ConfigurationErrorsException))
                    {
                        // VPN connection is not set up
                        var tsk = Task.Run(() => CreateTask(request, HttpStatusCode.ServiceUnavailable, Messages.CannotConnectToLdap), cancellationToken);
                        Task.WaitAll(tsk);

                        return tsk.Result;
                    }
                    else
                    {
                        // Unexpected error
                        _logger.Log(e);

                        var tsk = Task.Run(() => CreateTask(request, HttpStatusCode.InternalServerError, Messages.UnexpectedError), cancellationToken);
                        Task.WaitAll(tsk);

                        return tsk.Result;
                    }
                }
            }
            else
            {
                //If we got so far, something is wrong with the request; deny all access
                return await CreateTask(request, HttpStatusCode.BadRequest, Messages.InvalidRequest);
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
        private static void UpdatePrincipal(UserModel user)
        {
            var principal = new UserPrincipal(new GenericIdentity(user.UserName), new string[] { }, user);

            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        private class Credentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        private static Credentials ParseAuthorizationHeader(string authHeader)
        {
            var base64Credentials = authHeader.Split(' ')[1]; // Format is 'Basic string64Credentials'
            string[] credentials = Encoding.UTF8.GetString(Convert.FromBase64String(base64Credentials)).Split(new[] { ':' });
            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[1]))
            {
                return null;
            }

            return new Credentials { Username = credentials[0], Password = credentials[1] };
        }

        private static class ActiveDirectoryRoleProvider
        {
            private static readonly string ConnectionStringName = WebConfigurationManager.AppSettings[LdapFields.ConnectionStringField];
            private static readonly string ConnectionUsername = WebConfigurationManager.AppSettings[LdapFields.UsernameField];
            private static readonly string ConnectionPassword = WebConfigurationManager.AppSettings[LdapFields.PasswordField];
            private static readonly string AttributeMapUsername = WebConfigurationManager.AppSettings[LdapFields.AttributeMapUsernameField];

            public static DirectoryEntry GetUserEntry(string username)
            {
                var root = new DirectoryEntry(WebConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString, ConnectionUsername, ConnectionPassword);
                var searcher = new DirectorySearcher(root, string.Format(CultureInfo.InvariantCulture, LdapFields.UserQuery, AttributeMapUsername, username));

                var result = searcher.FindOne();

                if (result != null && !string.IsNullOrEmpty(result.Path))
                    return result.GetDirectoryEntry();

                return null;
            }

            private static IEnumerable<string> GetRolesForUser(string username)
            {
                // var allRoles = new List<string>();

                var root = new DirectoryEntry(WebConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString, ConnectionUsername, ConnectionPassword);

                var searcher = new DirectorySearcher(root, string.Format(CultureInfo.InvariantCulture, LdapFields.UserQuery, AttributeMapUsername, username));
                searcher.PropertiesToLoad.Add(LdapFields.Member); searcher.PropertiesToLoad.Add(LdapFields.EmployeeType);

                var result = searcher.FindOne();

                if (result != null && !string.IsNullOrEmpty(result.Path))
                {
                    var user = result.GetDirectoryEntry();

                    yield return user.Properties[LdapFields.EmployeeType].Value.ToString(); //This will return either 'Student', 'Professor', 'Technician' etc.
                    var groups = user.Properties[LdapFields.Member]; //This will return everything else, like mailing lists etc.

                    foreach (string path in groups)
                    {
                        var parts = path.Split(',');

                        if (parts.Length <= 0)
                        {
                            continue;
                        }

                        foreach (var part in parts)
                        {
                            var p = part.Split('=');

                            if (p.Length == 0)
                            {
                                continue;
                            }

                            if (p[0].Equals(LdapFields.CampusNet, StringComparison.OrdinalIgnoreCase))
                            {
                                yield return p[1];
                            }
                        }
                    }
                }
            }
            public static bool IsUserInRole(string username, string roleName)
            {
                IEnumerable<string> roles = GetRolesForUser(username);

                foreach (var role in roles)
                {
                    if (role.Equals(roleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        #endregion
    }
}