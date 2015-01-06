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
            var result = await _courseUserRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // GET: api/CourseUsers/5
        [HttpGet]
        [ResponseType(typeof(CourseUserModel))]
        public async Task<HttpResponseMessage> Get(int courseUserId)
        {
            var courseUser = await _courseUserRepository.Get(courseUserId);
            if (courseUser == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, courseUser.ToJson());
        }

        // POST: api/CourseUsers
        [HttpPost]
        [ResponseType(typeof(CourseUserModel))]
        public async Task<IHttpActionResult> Add([FromBody]CourseUserModel courseUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _courseUserRepository.Add(courseUser);
            if (result == null)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return CreatedAtRoute("CourseUserRoute", new { courseUserId = result.Id }, result.ToJson());
        }

        // PUT: api/CourseUsers/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Update(int courseUserId, [FromBody]CourseUserModel courseUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (courseUserId != courseUser.Id)
            {
                return BadRequest();
            }

            var result = await _courseUserRepository.Update(courseUser);
            if (result == null)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // DELETE: api/CourseUsers/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Delete(int courseUserId)
        {
            var result = await _courseUserRepository.Delete(courseUserId);
            return StatusCode(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
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