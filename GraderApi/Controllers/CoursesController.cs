namespace GraderApi.Controllers
{
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;


    public class CoursesController : ApiController
    {
        private readonly CourseRepository _courseRepository;
        public CoursesController(CourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        // GET: api/Courses
        [HttpGet]
        [ResponseType(typeof(IEnumerable<CourseModel>))]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _courseRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/5
        [HttpGet]
        [ResponseType(typeof(CourseModel))]
        public async Task<HttpResponseMessage> Get(int courseId)
        {
            try
            {
                var course = await _courseRepository.Get(courseId);

                return (course != null) ? Request.CreateResponse(HttpStatusCode.OK, course.ToJson()) : 
                    Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses
        [HttpPost]
        [ResponseType(typeof(CourseModel))]
        [PermissionsAuthorize(AdminPermissions.CanCreateCourse)]
        public async Task<HttpResponseMessage> Add([FromBody] CourseModel course)
        {
            if (!ModelState.IsValid) 
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
                var result = await _courseRepository.Add(course);

                return result != null ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson()) : 
                    Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // PUT: api/Courses/5
        [HttpPut]
        [ResponseType(typeof(CourseModel))]
        [PermissionsAuthorize(AdminPermissions.CanUpdateCourse)]
        public async Task<HttpResponseMessage> Update(int courseId, [FromBody] CourseModel course)
        {
            if (!ModelState.IsValid) 
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (courseId != course.Id) 
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                var result = await _courseRepository.Update(course);

                return result != null ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson()) : 
                    Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // DELETE: api/Courses/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        [PermissionsAuthorize(AdminPermissions.CanDeleteCourse)]
        public async Task<HttpResponseMessage> Delete(int courseId)
        {
            try
            {
                var result = await _courseRepository.Delete(courseId);
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
                _courseRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}