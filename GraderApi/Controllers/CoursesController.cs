using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

using System.Threading.Tasks;
using Grader.JsonSerializer;
using GraderApi.Principals;
using GraderDataAccessLayer.Models;
using GraderDataAccessLayer.Repositories;


namespace GraderApi.Controllers
{
    public class CoursesController : ApiController
    {
        private readonly CourseRepository _courseRepository;
        private readonly CourseUserRepository _courseUserRepository;

        public CoursesController(
                CourseRepository courseRepository,
                CourseUserRepository courseUserRepository
            )

        {
            _courseRepository = courseRepository;
            _courseUserRepository = courseUserRepository;
        }

        // GET: api/Courses
        public HttpResponseMessage GetCourses()
        {
            var currentUser = HttpContext.Current.User as UserPrincipal;
            if (currentUser == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Messages.UserNotFound);
            }
            if (currentUser.User == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Messages.UserNotFound);
            }
            var courses = _courseUserRepository.GetByUser(currentUser.User.Id);
            return Request.CreateResponse(HttpStatusCode.Accepted, courses.ToJson());
        }

        // GET: api/Courses/5
        public HttpResponseMessage GetCourse(int id)
        {
            var course = _courseRepository.Get(id);
            if (course == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, course);
        }

        // PUT: api/Courses/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCourse(int id, CourseModel course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != course.Id)
            {
                return BadRequest();
            }

            var result = await _courseRepository.Update(course);
            return StatusCode(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
        }

        // POST: api/Courses
        [ResponseType(typeof(CourseModel))]
        public async Task<IHttpActionResult> PostCourse(CourseModel course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _courseRepository.Add(course);

            if (!result)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return CreatedAtRoute("DefaultApi", new {id = course.Id}, course);
        }

        // DELETE: api/Courses/5
        [ResponseType(typeof(CourseModel))]
        public async Task<IHttpActionResult> DeleteCourse(int id)
        {
            var result = await _courseRepository.Remove(id);

            return StatusCode(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _courseRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}