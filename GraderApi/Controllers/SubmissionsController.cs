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


    public class SubmissionsController : ApiController
    {
        private readonly SubmissionRepository _submissionRepository = new SubmissionRepository();

        public SubmissionsController(SubmissionRepository submissionRepository)
        {
            _submissionRepository = submissionRepository;
        }

        // GET: api/Submissions
        [HttpGet]
        [ResponseType(typeof(IEnumerable<SubmissionModel>))]
        public async Task<HttpResponseMessage> All()
        {
            var result = await _submissionRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // GET: api/Submissions/5
        [HttpGet]
        [ResponseType(typeof(SubmissionModel))]
        public async Task<HttpResponseMessage> Get(int submissionId)
        {
            var submission = await _submissionRepository.Get(submissionId);
            if (submission == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, submission.ToJson());
        }

        // POST: api/Submissions
        [HttpPost]
        [ResponseType(typeof(SubmissionModel))]
        public async Task<IHttpActionResult> Add([FromBody]SubmissionModel submission)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _submissionRepository.Add(submission);
            if (result == null)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return CreatedAtRoute("SubmissionRoute", new { submissionId = result.Id }, result.ToJson());
        }

        // PUT: api/Submissions/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Add(int submissionId, [FromBody]SubmissionModel submission)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (submissionId != submission.Id)
            {
                return BadRequest();
            }

            var result = await _submissionRepository.Update(submission);
            if (result == null)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // DELETE: api/Submissions/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Delete(int submissionId)
        {
            var result = await _submissionRepository.DeleteSubmission(submissionId);
            return StatusCode(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _submissionRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}