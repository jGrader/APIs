namespace GraderApi.Controllers
{
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Repositories;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;


    public class CoursesController : ApiController
    {
        private readonly CourseRepository _courseRepository;
        private readonly CourseUserRepository _courseUserRepository;

        public CoursesController(CourseRepository courseRepository, CourseUserRepository courseUserRepository)
        {
            _courseRepository = courseRepository;
            _courseUserRepository = courseUserRepository;
        }

        // GET: api/Courses
        [HttpGet]
        [ResponseType(typeof(IEnumerable<CourseModel>))]
        public async Task<HttpResponseMessage> All()
        {
            var result = await _courseRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // GET: api/Courses/5
        [HttpGet]
        [ResponseType(typeof(CourseModel))]
        public async Task<HttpResponseMessage> Get(int courseId)
        {
            var course = await _courseRepository.Get(courseId);
            if (course == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, course.ToJson());
        }

        // POST: api/Courses
        [HttpPost]
        [ResponseType(typeof(CourseModel))]
        [PermissionsAuthorize(AdminPermissions.CanCreateCourse)]
        public async Task<IHttpActionResult> Add([FromBody] CourseModel course)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var result = await _courseRepository.Add(course);
            if (result == null) {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return CreatedAtRoute("CourseRoute", new { courseId = result.Id }, result.ToJson());
        }

        // PUT: api/Courses/5
        [HttpPut]
        [ResponseType(typeof(void))]
        [PermissionsAuthorize(AdminPermissions.CanUpdateCourse)]
        public async Task<IHttpActionResult> Add(int courseId, [FromBody] CourseModel course)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (courseId != course.Id) {
                return BadRequest();
            }

            var result = await _courseRepository.Update(course);
            if (result == null) {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // DELETE: api/Courses/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        [PermissionsAuthorize(AdminPermissions.CanDeleteCourse)]
        public async Task<IHttpActionResult> Remove(int courseId)
        {
            var result = await _courseRepository.Delete(courseId);
            return StatusCode(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _courseRepository.Dispose();
                _courseUserRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}