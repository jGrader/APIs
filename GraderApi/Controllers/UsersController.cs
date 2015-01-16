namespace GraderApi.Controllers
{
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer;
    using Services;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class UsersController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Logger _logger;

        public UsersController(UnitOfWork unitOfWork, Logger log)
        {
            _logger = log;
            _unitOfWork = unitOfWork;
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

        // POST: api/Users/Authentication
        // Dummy action for Basic Authentication to call and let the Handler validate
        [HttpPost]
        public HttpResponseMessage Authentication()
        {
            return Request.CreateResponse(HttpStatusCode.NoContent);
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