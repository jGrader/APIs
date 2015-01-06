namespace GraderApi.Controllers
{
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Repositories;
    using Principals;
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
        public async Task<IHttpActionResult> Authentication()
        {
            return StatusCode(HttpStatusCode.NoContent);
        }
        
        /*
        No one(!) should have the permissions for this.
        // GET: api/Users/All
        [HttpGet]
        [ResponseType(typeof(IEnumerable<UserModel>))]
        public async Task<HttpResponseMessage> All()
        {
            var result = await _userRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        
        // GET: api/Users/GetUser/5
        [HttpGet]
        [ResponseType(typeof(UserModel))]
        public async Task<HttpResponseMessage> GetUser(int userId)
        {
            var user = await _userRepository.Get(userId);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, user.ToJson());
        }*/

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

            var courses = await _courseUserRepository.GetAllByUser(currentUser.User.Id);
            return Request.CreateResponse(HttpStatusCode.Accepted, courses.ToJson());
        }

        /*
        Same here. This controller is for the logged in user ONLY!
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
        }*/

        // POST: api/Users
        [HttpPost]
        [ResponseType(typeof(UserModel))]
        public async Task<IHttpActionResult> Add([FromBody] UserModel user)
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
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Update(int userId,[FromBody] UserModel user)
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
        [HttpDelete]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Delete(int userId)
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