namespace GraderApi.Controllers
{
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;
    using Resources;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class CourseUsersController : ApiController
    {
        private readonly ICourseUserRepository _courseUserRepository;
        public CourseUsersController(ICourseUserRepository courseUserRepository)
        {
            _courseUserRepository = courseUserRepository;
        }

        // GET: api/CourseUsers/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllCourseUsers)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _courseUserRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/CourseUsers
        [HttpGet]
        [ValidateModelState]
        public async Task<HttpResponseMessage> All(int courseId)
        {
            try
            {
                var result = await _courseUserRepository.GetAllByCourseId(courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/CourseUsers/{courseUserId}
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CourseOwnerPermissions.CanSeeEnrollment)]
        public async Task<HttpResponseMessage> Get(int courseId, int courseUserId)
        {
            try
            {
                var courseUser = await _courseUserRepository.Get(courseUserId);
                if (courseUser == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }


                return courseId == courseUser.CourseId
                    ? Request.CreateResponse(HttpStatusCode.Accepted, courseUser.ToJson())
                    : Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses/{courseId}/CourseUsers
        [HttpPost]
        [ValidateModelState]
        [PermissionsAuthorize(CourseOwnerPermissions.CanAddEnrollment)]
        public async Task<HttpResponseMessage> Add(int courseId, [FromBody] CourseUserModel courseUser)
        {
            if (courseId != courseUser.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _courseUserRepository.Add(courseUser);

                return result != null ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // PUT: api/Courses/{courseId}/CourseUsers/{courseUserId}
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(CourseOwnerPermissions.CanUpdateEnrollment)]
        public async Task<HttpResponseMessage> Update(int courseId, int courseUserId, [FromBody] CourseUserModel courseUser)
        {
            if (courseUserId != courseUser.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            if (courseId != courseUser.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _courseUserRepository.Update(courseUser);

                return result != null ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // DELETE: api/Courses/{courseId}/CourseUsers/{courseUserId}
        [HttpDelete]
        [ValidateModelState]
        [PermissionsAuthorize(CourseOwnerPermissions.CanDeleteEnrollment)]
        public async Task<HttpResponseMessage> Delete(int courseId, int courseUserId)
        {
            try
            {
                var existingCourseUser = await _courseUserRepository.Get(courseUserId);
                if (existingCourseUser == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingCourseUser.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var result = await _courseUserRepository.Delete(courseUserId);
                return Request.CreateResponse(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}