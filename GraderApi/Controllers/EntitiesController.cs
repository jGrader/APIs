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


    public class EntitiesController : ApiController
    {
        private readonly IEntityRepository _entityRepository;
        public EntitiesController(IEntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        // GET: api/Entities/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllEntities)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _entityRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Entities
        [HttpGet]
        [ValidateModelState]
        public async Task<HttpResponseMessage> All(int courseId)
        {
            try
            {
                var result = await _entityRepository.GetAllByCourseId(courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Entities/Get/{entityId}
        [HttpGet]
        [ValidateModelState]
        public async Task<HttpResponseMessage> Get(int courseId, int entityId)
        {
            try
            {
                var entity = await _entityRepository.Get(entityId);
                if (entity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return courseId == entity.CourseId
                    ? Request.CreateResponse(HttpStatusCode.OK, entity.ToJson())
                    : Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: /api/Courses/{courseId}/Entities/GetTask/{entityId}
        [HttpGet]
        [ValidateModelState]
        public async Task<HttpResponseMessage> GetTask(int courseId, int entityId)
        {
            try
            {
                var entity = await _entityRepository.Get(entityId);
                if (entity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return courseId == entity.CourseId
                    ? Request.CreateResponse(HttpStatusCode.OK, entity.Task.ToJson())
                    : Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses/{courseId}/Entities
        [HttpPost]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanCreateEntities)]
        public async Task<HttpResponseMessage> Add(int courseId, [FromBody] EntityModel entity)
        {
            if (courseId != entity.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _entityRepository.Add(entity);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
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
            if (courseId != entity.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _entityRepository.Update(entity);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
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
                var existingEntity = await _entityRepository.Get(entityId);
                if (existingEntity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingEntity.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var result = await _entityRepository.Delete(entityId);
                return Request.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}