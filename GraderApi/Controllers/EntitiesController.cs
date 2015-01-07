namespace GraderApi.Controllers
{
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Repositories;
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
            var result = await _entityRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // GET: api/Entities/Get/5
        [HttpGet]
        [ResponseType(typeof(EntityModel))]
        public async Task<HttpResponseMessage> Get(int entityId)
        {
            var entity = await _entityRepository.Get(entityId);
            if (entity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, entity.ToJson());
        }

        // POST: api/Entities
        [HttpPost]
        [ResponseType(typeof(EntityModel))]
        public async Task<HttpResponseMessage> Add([FromBody]EntityModel entity)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var result = await _entityRepository.Add(entity);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // PUT: api/Entities/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> Add(int entityId, [FromBody]EntityModel entity)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (entityId != entity.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var result = await _entityRepository.Update(entity);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE: api/Entities/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> Delete(int entityId)
        {
            var result = await _entityRepository.Delete(entityId);
            return Request.CreateResponse(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
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