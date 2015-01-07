namespace GraderApi.Controllers
{
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;


    public class SubmissionsController : ApiController
    {
        private readonly ISubmissionRepository _submissionRepository;
        public SubmissionsController(ISubmissionRepository submissionRepository)
        {
            _submissionRepository = submissionRepository;
        }

        // GET: api/Submissions
        [HttpGet]
        [ResponseType(typeof(IEnumerable<SubmissionModel>))]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _submissionRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Submissions/5
        [HttpGet]
        [ResponseType(typeof(SubmissionModel))]
        public async Task<HttpResponseMessage> Get(int submissionId)
        {
            try
            {
                var submission = await _submissionRepository.Get(submissionId);

                return submission != null
                    ? Request.CreateResponse(HttpStatusCode.OK, submission.ToJson())
                    : Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
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

            try
            {
                var result = await _submissionRepository.Add(submission);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Submissions/UploadFile/3/5
        [HttpPost]
        [ResponseType(typeof(string))]
        public async Task<HttpResponseMessage> UploadFile(int courseId, int entityId, [FromBody] FileModel file)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
                var result = await _submissionRepository.Add(file);

                return !String.IsNullOrEmpty(result)
                    ? Request.CreateResponse(HttpStatusCode.OK, result)
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // PUT: api/Submissions/5
        [HttpPut]
        [ResponseType(typeof(SubmissionModel))]
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

            try
            {
                var result = await _submissionRepository.Update(submission);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // DELETE: api/Submissions/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> Delete(int submissionId)
        {
            try
            {
                var result = await _submissionRepository.DeleteSubmission(submissionId);
                return Request.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}