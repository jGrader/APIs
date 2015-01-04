using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Grader.JsonSerializer;
using GraderApi.Principals;
using GraderDataAccessLayer.Repositories;

namespace GraderApi.Controllers
{
    public class UserController : ApiController
    {
        private readonly CourseRepository _courseRepository;
        private readonly CourseUserRepository _courseUserRepository;

        public UserController(CourseRepository courseRepository, CourseUserRepository courseUserRepository)
        {
            _courseRepository = courseRepository;
            _courseUserRepository = courseUserRepository;
        }

        public async Task<HttpResponseMessage> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> GetAllCourses()
        {
            var currentUser = HttpContext.Current.User as UserPrincipal;
            if (currentUser == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Messages.UserNotFound);
            }
            if (currentUser.User == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Messages.UserNotFound);
            }

            var courses = await _courseUserRepository.GetAllByUser(currentUser.User.Id);
            return Request.CreateResponse(HttpStatusCode.Accepted, courses.ToJson());
        }
    }
}
