namespace GraderApi.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Grader.JsonSerializer;
    using GraderApi.Principals;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Repositories;


    public class UsersController : ApiController
    {
        private readonly UserRepository _userRepository;
        private readonly CourseUserRepository _courseUserRepository;

        public UsersController(UserRepository userRepository, CourseUserRepository courseUserRepository)
        {
            _userRepository = userRepository;
            _courseUserRepository = courseUserRepository;
        }


        // GET: api/Users
        [ResponseType(typeof(IEnumerable<UserModel>))]
        public async Task<HttpResponseMessage> GetUser()
        {
            var result = await _userRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // GET: api/Users/5
        [ResponseType(typeof(UserModel))]
        public async Task<HttpResponseMessage> GetUser(int userId)
        {
            var user = await _userRepository.Get(userId);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, user.ToJson());
        }

        // GET: api/Users/GetAllCoursesForCurrentUser
        [ResponseType(typeof(IEnumerable<CourseModel>))]
        public async Task<HttpResponseMessage> GetAllCoursesForCurrentUser()
        {
            var currentUser = HttpContext.Current.User as UserPrincipal;
            if (currentUser == null || currentUser.User == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Messages.UserNotFound);
            }

            var courses = await _courseUserRepository.GetAllByUser(currentUser.User.Id);
            return Request.CreateResponse(HttpStatusCode.Accepted, courses.ToJson());
        }

        // GET: api/Users/GetAllCoursesForCurrentUser/5
        [ResponseType(typeof(IEnumerable<CourseModel>))]
        public async Task<HttpResponseMessage> GetAllCoursesForCurrentUser(int userId)
        {
            var user = await _userRepository.Get(userId);
            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Messages.UserNotFound);
            }

            var courses = await _courseUserRepository.GetAllByUser(userId);
            return Request.CreateResponse(HttpStatusCode.Accepted, courses.ToJson());
        }

        // POST: api/Users
        [ResponseType(typeof(UserModel))]
        public async Task<IHttpActionResult> PostUserModel(UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userRepository.Add(user);
            if (result == null)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return CreatedAtRoute("UserRoute", new { userId = result.Id }, result.ToJson());
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(int userId, UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userId != user.Id)
            {
                return BadRequest();
            }

            var result = await _userRepository.Update(user);
            if (result == null)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteUserModel(int userId)
        {
            var result = await _userRepository.Delete(userId);
            return StatusCode(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
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