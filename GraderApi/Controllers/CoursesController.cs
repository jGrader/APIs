using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using GraderApi.Models;
using System.Threading.Tasks;
using GraderDataAccessLayer.Models;
using GraderDataAccessLayer.Repositories;


namespace GraderApi.Controllers
{
    public class CoursesController : ApiController
    {
        private readonly CourseRepository _repository;

        public CoursesController(CourseRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Courses
        public IEnumerable<Course> GetCourses()
        {
            var result =  _repository.GetAll();
            return result.Result;
        }

        // GET: api/Courses/5
        [ResponseType(typeof(Course))]
        public async Task<IHttpActionResult> GetCourse(int id)
        {
            var course = await _repository.Get(id);
            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

        // PUT: api/Courses/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCourse(int id, Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != course.Id)
            {
                return BadRequest();
            }

            var result = await _repository.Update(course);
            return StatusCode(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
        }

        // POST: api/Courses
        [ResponseType(typeof(Course))]
        public async Task<IHttpActionResult> PostCourse(Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _repository.Add(course);

            if (!result)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return CreatedAtRoute("DefaultApi", new {id = course.Id}, course);
        }

        // DELETE: api/Courses/5
        [ResponseType(typeof(Course))]
        public async Task<IHttpActionResult> DeleteCourse(int id)
        {
            var result = await _repository.Remove(id);

            return StatusCode(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}