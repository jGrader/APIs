namespace GraderApi.Controllers
{
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Resources;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class CourseUsersController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public CourseUsersController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/CourseUsers/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllCourseUsers)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _unitOfWork.CourseUserRepository.GetAll();
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
                var result = await _unitOfWork.CourseUserRepository.GetByCourseId(courseId);
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
                var courseUser = await _unitOfWork.CourseUserRepository.Get(courseUserId);
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
                var result = await _unitOfWork.CourseUserRepository.Add(courseUser);

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
                var result = await _unitOfWork.CourseUserRepository.Update(courseUser);

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
                var existingCourseUser = await _unitOfWork.CourseUserRepository.Get(courseUserId);
                if (existingCourseUser == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingCourseUser.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var result = await _unitOfWork.CourseUserRepository.Delete(courseUserId);
                return Request.CreateResponse(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}