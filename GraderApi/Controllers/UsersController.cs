namespace GraderApi.Controllers
{
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Repositories;
    using Principals;
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
        private readonly UserRepository _userRepository;
        private readonly CourseUserRepository _courseUserRepository;
        public UsersController(UserRepository userRepository, CourseUserRepository courseUserRepository)
        {
            _userRepository = userRepository;
            _courseUserRepository = courseUserRepository;
        }

        // POST: /api/Users/Authentication
        // Dummy action for Basic Authentication to call and let the Handler validate
        [HttpPost]
        [ResponseType(typeof (void))]
        public HttpResponseMessage Authentication()
        {
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
        
        // GET: api/Users/Courses
        [HttpGet]
        [ResponseType(typeof(IEnumerable<CourseModel>))]
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

        // DELETE: api/Users/5
        [HttpDelete]
        [ResponseType(typeof(void))]
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


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userRepository.Dispose();
                _courseUserRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}