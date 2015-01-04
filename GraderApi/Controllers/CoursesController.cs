namespace GraderApi.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Repositories;


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
        [ResponseType(typeof(IEnumerable<CourseModel>))]
        public async Task<HttpResponseMessage> GetCourse()
        {
            var result = await _courseRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // GET: api/Courses/5
        [ResponseType(typeof(CourseModel))]
        [PermissionsAuthorize(CoursePermissions.CanSeeGrades)]
        public async Task<HttpResponseMessage> GetCourse(int courseId)
        {
            var course = await _courseRepository.Get(courseId);
            if (course == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, course.ToJson());
        }

        // POST: api/Courses
        [ResponseType(typeof(CourseModel))]
        [PermissionsAuthorize(AdminPermissions.CanCreateCourse)]
        public async Task<IHttpActionResult> PostCourseModel(CourseModel course)
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
        [ResponseType(typeof(void))]
        [PermissionsAuthorize(AdminPermissions.CanUpdateCourse)]
        public async Task<IHttpActionResult> PutCourse(int courseId, CourseModel course)
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
        [ResponseType(typeof(void))]
        [PermissionsAuthorize(AdminPermissions.CanDeleteCourse)]
        public async Task<IHttpActionResult> DeleteCourseModel(int courseId)
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