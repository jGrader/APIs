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


    public class GradeComponentsController : ApiController
    {
        private readonly GradeComponentRepository _gradeComponentRepository;

        public GradeComponentsController(GradeComponentRepository gradeComponentRepository)
        {
            _gradeComponentRepository = gradeComponentRepository;
        }

        // GET: api/GradeComponents
        [ResponseType(typeof(IEnumerable<GradeComponentModel>))]
        public async Task<HttpResponseMessage> GetGradeComponent()
        {
            var result = await _gradeComponentRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // GET: api/GradeComponents/5
        [ResponseType(typeof(GradeComponentModel))]
        [Authorize(Roles = "CanSeeGrades")]
        public async Task<HttpResponseMessage> GetGradeComponent(int gradeComponentId)
        {
            var gradeComponent = await _gradeComponentRepository.Get(gradeComponentId);
            if (gradeComponent == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, gradeComponent.ToJson());
        }

        // POST: api/GradeComponents
        [ResponseType(typeof(GradeComponentModel))]
        [Authorize(Roles = "CanCreateGradedPart")]
        public async Task<IHttpActionResult> PostGradeComponent(GradeComponentModel gradeComponentModel)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var result = await _gradeComponentRepository.Add(gradeComponentModel);
            if (result == null) {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return CreatedAtRoute("GradeComponentRoute", new { gradeComponentId = result.Id }, result.ToJson());
        }

        // PUT: api/GradeComponents/5
        [ResponseType(typeof(void))]
        [Authorize(Roles = "CanUpdateGradedPart")]
        public async Task<IHttpActionResult> PutGradeComponent(int gradeComponentId, GradeComponentModel gradeComponentModel)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (gradeComponentId != gradeComponentModel.Id) {
                return BadRequest();
            }

            var result = await _gradeComponentRepository.Update(gradeComponentModel);
            if (result == null) {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // DELETE: api/GradeComponents/5
        [ResponseType(typeof(GradeComponentModel))]
        [Authorize(Roles = "CanDeleteGradedPart")]
        public async Task<IHttpActionResult> DeleteGradeComponent(int gradeComponentId)
        {
            var result = await _gradeComponentRepository.Delete(gradeComponentId);
            return StatusCode(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _gradeComponentRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}