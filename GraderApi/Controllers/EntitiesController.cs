namespace GraderApi.Controllers
{
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Resources;
    using Services;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class EntitiesController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Logger _logger;

        public EntitiesController(UnitOfWork unitOfWork, Logger log)
        {
            _logger = log;
            _unitOfWork = unitOfWork;
        }

        // GET: api/Entities/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllEntities)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _unitOfWork.EntityRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Entities/All
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeExtensions)]
        public async Task<HttpResponseMessage> All(int courseId)
        {
            try
            {
                var result = await _unitOfWork.EntityRepository.GetByCourseId(courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Entities/GetByUserId/{entityId}
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeEntities)]
        public async Task<HttpResponseMessage> Get(int courseId, int entityId)
        {
            try
            {
                var entity = await _unitOfWork.EntityRepository.Get(entityId);
                if (entity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return courseId == entity.Task.CourseId
                    ? Request.CreateResponse(HttpStatusCode.OK, entity.ToJson())
                    : Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: /api/Courses/{courseId}/Entities/GetTask/{entityId}
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeTasks)]
        public async Task<HttpResponseMessage> GetTask(int courseId, int entityId)
        {
            try
            {
                var entity = await _unitOfWork.EntityRepository.Get(entityId);
                if (entity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return courseId == entity.Task.CourseId
                    ? Request.CreateResponse(HttpStatusCode.OK, entity.Task.ToJson())
                    : Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses/{courseId}/Entities
        [HttpPost]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanCreateEntities)]
        public async Task<HttpResponseMessage> Add(int courseId, [FromBody] EntityModel entity)
        {
            entity.Task = await _unitOfWork.TaskRepository.Get(entity.TaskId);
            if (courseId != entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _unitOfWork.EntityRepository.Add(entity);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // PUT: api/Courses/{courseId}/Entities/{entityId}
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanUpdateEntities)]
        public async Task<HttpResponseMessage> Update(int courseId, int entityId, [FromBody] EntityModel entity)
        {
            if (entityId != entity.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            entity.Task = await _unitOfWork.TaskRepository.Get(entityId);
            if (courseId != entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _unitOfWork.EntityRepository.Update(entity);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // DELETE: api/Courses/{courseId}/Entities/{entityId}
        [HttpDelete]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanDeleteEntities)]
        public async Task<HttpResponseMessage> Delete(int courseId, int entityId)
        {
            try
            {
                var existingEntity = await _unitOfWork.EntityRepository.Get(entityId);
                if (existingEntity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingEntity.Task.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var result = await _unitOfWork.EntityRepository.Delete(entityId);
                return Request.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}