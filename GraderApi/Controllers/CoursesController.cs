using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using GraderApi.Models;
using System.Threading.Tasks;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Models;
using GraderDataAccessLayer.Repositories;


namespace GraderApi.Controllers
{
    public class CoursesController : ApiController
    {
        private readonly ICourseRepository _repository;

        public CoursesController(ICourseRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Courses
        public HttpResponseMessage GetCourses()
        {
            var courses =  _repository.GetAll();
            var result = Request.CreateResponse(HttpStatusCode.Accepted, courses);
            return result;
        }

        // GET: api/Courses/5
        public Course GetCourse(int id)
        {
            var course = _repository.Get(id);
            return course;
        }

        // PUT: api/Courses/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCourse(int id, Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != course.Id)
            {
                return BadRequest();
            }

            var result = _repository.Update(course);
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

            var result = _repository.Add(course);

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
            var result = _repository.Remove(id);

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