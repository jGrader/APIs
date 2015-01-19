namespace GraderApi.Controllers
{
    using Extensions;
    using Filters;
    using Grader.ExtensionMethods;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Services;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    public class UsersController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Logger _logger;
        private readonly UserModel _user;

        public UsersController(UnitOfWork unitOfWork, Logger log)
        {
            _logger = log;
            _unitOfWork = unitOfWork;

            if (HttpContext.Current == null)
            {
                _logger.Log(new Exception("The HTTP context was null in UsersController"));
                throw new NullReferenceException("HttpContext.Current");
            }

            var tmp = HttpContext.Current.User as UserPrincipal;
            if (tmp == null)
            {
                _logger.Log(new Exception("No principal was set."));
                throw new NullReferenceException("UserPrincipal");
            }
            _user = tmp.User;
        }

        //GET: api/Users/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllUsers)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _unitOfWork.UserRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Users/Login
        // Dummy action for Basic Authentication to call and let the Handler validate
        [HttpPost]
        public HttpResponseMessage Login()
        {
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        // POST: api/Users/Logout
        [HttpPost]
        public async Task<HttpResponseMessage> Logout()
        {
            var result = await _unitOfWork.SessionIdRepository.Delete(_user.Id);
            if (result)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        // DELETE: api/Users/{userId}
        [HttpDelete]
        [ValidateModelState]
        [PermissionsAuthorize(SuperUserPermissions.CanDeleteUser)]
        public async Task<HttpResponseMessage> Delete(int userId)
        {
            try
            {
                var result = await _unitOfWork.UserRepository.Delete(userId);
                return Request.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}