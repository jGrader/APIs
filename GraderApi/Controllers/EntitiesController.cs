namespace GraderApi.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Repositories;


    public class EntitiesController : ApiController
    {
        private readonly EntityRepository _entityRepository;

        public EntitiesController(EntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        // GET: api/Entities
        [ResponseType(typeof(IEnumerable<EntityModel>))]
        public async Task<HttpResponseMessage> GetEntity()
        {
            var result = await _entityRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // GET: api/Entities/5
        [ResponseType(typeof(EntityModel))]
        public async Task<HttpResponseMessage> GetEntity(int entityId)
        {
            var entity = await _entityRepository.Get(entityId);
            if (entity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, entity.ToJson());
        }

        // POST: api/Entities
        [ResponseType(typeof(EntityModel))]
        public async Task<IHttpActionResult> PostEntity(EntityModel entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _entityRepository.Add(entity);
            if (result == null)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return CreatedAtRoute("EntityRoute", new { entityId = result.Id }, result.ToJson());
        }

        // PUT: api/Entities/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutEntity(int entityId, EntityModel entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (entityId != entity.Id)
            {
                return BadRequest();
            }

            var result = await _entityRepository.Update(entity);
            if (result == null)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // DELETE: api/Entities/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteEntityModel(int entityId)
        {
            var result = await _entityRepository.Delete(entityId);
            return StatusCode(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
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