namespace GraderApi.Controllers
{
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;

    public class UsersController : ApiController
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        //GET: api/Users/All
        [HttpGet]
        [ResponseType(typeof (IEnumerable<UserModel>))]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllUsers)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _userRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
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
                var result = await _userRepository.Delete(userId);
                return Request.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}