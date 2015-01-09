namespace GraderApi.Controllers
{
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;
    using Principals;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Description;

    public class UsersController : ApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ICourseUserRepository _courseUserRepository;
        public UsersController(IUserRepository userRepository, ICourseUserRepository courseUserRepository)
        {
            _userRepository = userRepository;
            _courseUserRepository = courseUserRepository;
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
        
        // GET: api/Users/Courses
        // No permissions needed because it only gets the courses of the currently logged in user
        [HttpGet]
        public async Task<HttpResponseMessage> Courses()
        {
            var currentUser = HttpContext.Current.User as UserPrincipal;
            if (currentUser == null || currentUser.User == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Messages.UserNotFound);
            }

            try
            {
                var courses = await _courseUserRepository.GetAllByUser(currentUser.User.Id);
                return Request.CreateResponse(HttpStatusCode.OK, courses.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
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