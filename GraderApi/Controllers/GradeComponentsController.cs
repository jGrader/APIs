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


    public class GradeComponentsController : ApiController
    {
        private readonly GradeComponentRepository _gradeComponentRepository;

        public GradeComponentsController(GradeComponentRepository gradeComponentRepository)
        {
            _gradeComponentRepository = gradeComponentRepository;
        }

        // GET: api/GradeComponents
        [HttpGet]
        [ResponseType(typeof(IEnumerable<GradeComponentModel>))]
        public async Task<HttpResponseMessage> All()
        {
            var result = await _gradeComponentRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // GET: api/GradeComponents/5
        [HttpGet]
        [ResponseType(typeof(GradeComponentModel))]
        [PermissionsAuthorize(CoursePermissions.CanSeeGrades)]
        public async Task<HttpResponseMessage> Get(int gradeComponentId)
        {
            var gradeComponent = await _gradeComponentRepository.Get(gradeComponentId);
            if (gradeComponent == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, gradeComponent.ToJson());
        }

        // POST: api/GradeComponents
        [HttpPost]
        [ResponseType(typeof(GradeComponentModel))]
        [PermissionsAuthorize(AdminPermissions.CanCreateGradedPart)]
        public async Task<HttpResponseMessage> Add(GradeComponentModel gradeComponentModel)
        {
            if (!ModelState.IsValid) {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var result = await _gradeComponentRepository.Add(gradeComponentModel);
            if (result == null) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // PUT: api/GradeComponents/5
        [HttpPut]
        [ResponseType(typeof(void))]
        [PermissionsAuthorize(AdminPermissions.CanUpdateGradedPart)]
        public async Task<HttpResponseMessage> Add(int gradeComponentId, [FromBody]GradeComponentModel gradeComponentModel)
        {
            if (!ModelState.IsValid) {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (gradeComponentId != gradeComponentModel.Id) {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var result = await _gradeComponentRepository.Update(gradeComponentModel);
            if (result == null) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE: api/GradeComponents/5
        [HttpDelete]
        [ResponseType(typeof(GradeComponentModel))]
        [PermissionsAuthorize(AdminPermissions.CanDeleteGradedPart)]
        public async Task<HttpResponseMessage> Delete(int gradeComponentId)
        {
            var result = await _gradeComponentRepository.Delete(gradeComponentId);
            return Request.CreateResponse(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
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