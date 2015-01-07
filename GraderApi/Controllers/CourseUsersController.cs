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


    public class CourseUsersController : ApiController
    {
        private readonly CourseUserRepository _courseUserRepository;
        public CourseUsersController(CourseUserRepository courseUserRepository)
        {
            _courseUserRepository = courseUserRepository;
        }

        // GET: api/CourseUsers
        [HttpGet]
        [ResponseType(typeof(IEnumerable<CourseUserModel>))]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _courseUserRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/CourseUsers/5
        [HttpGet]
        [ResponseType(typeof(CourseUserModel))]
        public async Task<HttpResponseMessage> Get(int courseUserId)
        {
            try
            {
                var courseUser = await _courseUserRepository.Get(courseUserId);

                return courseUser != null ? Request.CreateResponse(HttpStatusCode.Accepted, courseUser.ToJson())
                    : Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/CourseUsers
        [HttpPost]
        [ResponseType(typeof(CourseUserModel))]
        [PermissionsAuthorize(CourseOwnerPermissions.CanAddEnrollment)]
        public async Task<HttpResponseMessage> Add([FromBody] CourseUserModel courseUser)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
                var result = await _courseUserRepository.Add(courseUser);

                return result != null ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // PUT: api/CourseUsers/5
        [HttpPut]
        [ResponseType(typeof(CourseUserModel))]
        [PermissionsAuthorize(CourseOwnerPermissions.CanUpdateEnrollment)]
        public async Task<HttpResponseMessage> Update(int courseUserId, [FromBody] CourseUserModel courseUser)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (courseUserId != courseUser.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                var result = await _courseUserRepository.Update(courseUser);

                return result != null ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // DELETE: api/CourseUsers/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        [PermissionsAuthorize(CourseOwnerPermissions.CanDeleteEnrollment)]
        public async Task<HttpResponseMessage> Delete(int courseUserId)
        {
            try
            {
                var result = await _courseUserRepository.Delete(courseUserId);
                return Request.CreateResponse(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
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
                _courseUserRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}