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
            try
            {
                var result = await _gradeComponentRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/GradeComponents/5
        [HttpGet]
        [ResponseType(typeof(GradeComponentModel))]
        [PermissionsAuthorize(CoursePermissions.CanSeeGrades)]
        public async Task<HttpResponseMessage> Get(int gradeComponentId)
        {
            try
            {
                var gradeComponent = await _gradeComponentRepository.Get(gradeComponentId);

                return gradeComponent != null
                    ? Request.CreateResponse(HttpStatusCode.OK, gradeComponent.ToJson())
                    : Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: /api/GradeComponents/GetCourse/3
        [HttpGet]
        [ResponseType(typeof (CourseModel))]
        [PermissionsAuthorize(CoursePermissions.CanSeeGrades)]
        public async Task<HttpResponseMessage> GetCourse(int gradeComponentId)
        {
            try
            {
                var gradeComponent = await _gradeComponentRepository.Get(gradeComponentId);

                return gradeComponent != null 
                    ? Request.CreateResponse(HttpStatusCode.OK, gradeComponent.Course.ToJson()) 
                    : Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: /api/GradeComponents/GetForCourse/3
        [HttpGet]
        [ResponseType(typeof (IEnumerable<GradeComponentModel>))]
        [PermissionsAuthorize(CoursePermissions.CanSeeGrades)]
        public async Task<HttpResponseMessage> GetForCourse(int courseId)
        {
            try
            {
                var gradeComponents = await _gradeComponentRepository.GetAllByCourse(courseId);

                return gradeComponents != null
                    ? Request.CreateResponse(HttpStatusCode.OK, gradeComponents.ToJson())
                    : Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/GradeComponents
        [HttpPost]
        [ResponseType(typeof(GradeComponentModel))]
        [PermissionsAuthorize(CourseOwnerPermissions.CanCreateGradedPart)]
        public async Task<HttpResponseMessage> Add([FromBody] GradeComponentModel gradeComponent)
        {
            if (!ModelState.IsValid) 
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
                var result = await _gradeComponentRepository.Add(gradeComponent);
                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // PUT: api/GradeComponents/5
        [HttpPut]
        [ResponseType(typeof(GradeComponentModel))]
        [PermissionsAuthorize(CourseOwnerPermissions.CanUpdateGradedPart)]
        public async Task<HttpResponseMessage> Update(int gradeComponentId, [FromBody] GradeComponentModel gradeComponent)
        {
            if (!ModelState.IsValid) 
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (gradeComponentId != gradeComponent.Id) 
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                var result = await _gradeComponentRepository.Update(gradeComponent);

                return result != null 
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson()) 
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // DELETE: api/GradeComponents/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        [PermissionsAuthorize(CourseOwnerPermissions.CanDeleteGradedPart)]
        public async Task<HttpResponseMessage> Delete(int gradeComponentId)
        {
            try
            {
                var result = await _gradeComponentRepository.Delete(gradeComponentId);
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
                _gradeComponentRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}