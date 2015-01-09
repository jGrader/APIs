namespace GraderApi.Controllers
{
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class CoursesController : ApiController
    {
        private readonly ICourseRepository _courseRepository;
        public CoursesController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        // GET: api/Courses
        [HttpGet]
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

        // GET: api/Courses/{courseId}
        [HttpGet]
        [ValidateModelState]
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
        [ValidateModelState]
        [PermissionsAuthorize(AdminPermissions.CanCreateCourse)]
        public async Task<HttpResponseMessage> Add([FromBody] CourseModel course)
        {
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

        // PUT: api/Courses/{courseId}
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(AdminPermissions.CanUpdateCourse)]
        public async Task<HttpResponseMessage> Update(int courseId, [FromBody] CourseModel course)
        {
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

        // DELETE: api/Courses/{courseId}
        [HttpDelete]
        [ValidateModelState]
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
    }
}