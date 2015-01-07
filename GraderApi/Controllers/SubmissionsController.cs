using System;

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
        public async Task<HttpResponseMessage> Add([FromBody]SubmissionModel submission)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var result = await _submissionRepository.Add(submission);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // POST: api/Submissions/UploadFile
        [HttpPost]
        [ResponseType(typeof(string))]
        public async Task<HttpResponseMessage> UploadFile(int courseId, int entityId, [FromBody] FileModel file)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var result = await _submissionRepository.Add(file);
            if (String.IsNullOrEmpty(result))
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // PUT: api/Submissions/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> Add(int submissionId, [FromBody]SubmissionModel submission)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (submissionId != submission.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var result = await _submissionRepository.Update(submission);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE: api/Submissions/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> Delete(int submissionId)
        {
            var result = await _submissionRepository.DeleteSubmission(submissionId);
            return Request.CreateResponse(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
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