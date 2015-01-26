namespace GraderApi.Controllers
{
    using Filters;
    using Grader.ExtensionMethods;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Resources;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class CourseUsersController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Logger _logger;

        public CourseUsersController(UnitOfWork unitOfWork, Logger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/CourseUsers/All
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
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/CourseUsers/Get/{courseUserId}
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
                    ? Request.CreateResponse(HttpStatusCode.OK, courseUser.ToJson())
                    : Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/CourseUsers/{courseUserId}
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeFinalGrades)]
        public async Task<HttpResponseMessage> GetEnrollmentGrade(int courseId, int courseUserId, bool isPredicted)
        {
            try
            {
                var courseUser = await _unitOfWork.CourseUserRepository.Get(courseUserId);
                if (courseUser == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (courseUser.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                //
                var gradeComponents = await _unitOfWork.GradeComponentRepository.GetByCourseId(courseId);
                var entities = await _unitOfWork.EntityRepository.GetByCourseId(courseId);
                var entityModels = entities as IList<EntityModel> ?? entities.ToList();

                var finalGrade = 0;
                foreach (var gradeComponent in gradeComponents)
                {
                    var sum = 0;
                    var count = 0;

                    var component = gradeComponent; // If this isn't here, we might get a bug depending on compiler version
                    var filteredEntitiesByGradeComponent = entityModels.Where(e => e.Task.GradeComponentId == component.Id);
                    var entitiesByGradeComponent = filteredEntitiesByGradeComponent as IList<EntityModel> ?? filteredEntitiesByGradeComponent.ToList();

                    foreach (var entity in entitiesByGradeComponent)
                    {
                        var entity1 = entity; // If this isn't here, we might get a bug depending on compiler version
                        var grade = await _unitOfWork.GradeRepository.GetByExpression(g => g.EntityId == entity1.Id && g.UserId == courseUser.UserId);
                        var firstOrDefault = grade.FirstOrDefault();
                        if (firstOrDefault == null)
                        {
                            continue;
                        }

                        count++;
                        sum += firstOrDefault.Grade + firstOrDefault.BonusGrade;
                    }

                    if (isPredicted)
                    {
                        finalGrade += (sum / count);
                    }
                    else
                    {
                        finalGrade += (int)((gradeComponent.Percentage / 100.0) * (sum /(double) entitiesByGradeComponent.Count()));
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, finalGrade);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses/{courseId}/CourseUsers/Add
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
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // PUT: api/Courses/{courseId}/CourseUsers/Update/{courseUserId}
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
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // DELETE: api/Courses/{courseId}/CourseUsers/Delete/{courseUserId}
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
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}