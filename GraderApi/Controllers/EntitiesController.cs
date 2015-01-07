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


    public class EntitiesController : ApiController
    {
        private readonly EntityRepository _entityRepository;
        public EntitiesController(EntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        // GET: api/Entities
        [HttpGet]
        [ResponseType(typeof(IEnumerable<EntityModel>))]
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

        // GET: api/Entities/Get/5
        [HttpGet]
        [ResponseType(typeof(EntityModel))]
        public async Task<HttpResponseMessage> Get(int entityId)
        {
            try
            {
                var entity = await _entityRepository.Get(entityId);

                return entity != null
                    ? Request.CreateResponse(HttpStatusCode.OK, entity.ToJson())
                    : Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: /api/Entities/GetTask/3
        [HttpGet]
        [ResponseType(typeof (TaskModel))]
        public async Task<HttpResponseMessage> GetTask(int entityId)
        {
            try
            {
                var entity = await _entityRepository.Get(entityId);
                return entity != null
                    ? Request.CreateResponse(HttpStatusCode.OK, entity.Task.ToJson())
                    : Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Entities
        [HttpPost]
        [ResponseType(typeof(EntityModel))]
        [PermissionsAuthorize(CoursePermissions.CanCreateEntities)]
        public async Task<HttpResponseMessage> Add([FromBody] EntityModel entity)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
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

        // PUT: api/Entities/5
        [HttpPut]
        [ResponseType(typeof(EntityModel))]
        [PermissionsAuthorize(CoursePermissions.CanUpdateEntities)]
        public async Task<HttpResponseMessage> Update(int entityId, [FromBody] EntityModel entity)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (entityId != entity.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
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

        // DELETE: api/Entities/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        [PermissionsAuthorize(CoursePermissions.CanDeleteEntities)]
        public async Task<HttpResponseMessage> Delete(int entityId)
        {
            try
            {
                var result = await _entityRepository.Delete(entityId);
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
                _entityRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}