namespace GraderApi.Controllers
{
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    public class ExcusesController : ApiController
    {
        private readonly IExcuseRepository _excuseRepository;
        private readonly IEntityRepository _entityRepository;
        private readonly ICourseUserRepository _courseUserRepository;
        public ExcusesController(IExcuseRepository excuseRepository, IEntityRepository entityRepository, ICourseUserRepository courseUserRepository)
        {
            _excuseRepository = excuseRepository;
            _entityRepository = entityRepository;
            _courseUserRepository = courseUserRepository;
        }

        // GET: api/Excuses/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllExcuses)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _excuseRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Excuses/All
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanGrantExcuses)]
        public async Task<HttpResponseMessage> All(int courseId)
        {
            try
            {
                var result = await _excuseRepository.GetAllByLambda(e => e.Entity.Task.CourseId == courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses/{courseId}/Excuses/Add
        [HttpPost]
        [ValidateModelState]
        public async Task<HttpResponseMessage> Add(int courseId, [FromBody] ExcuseModel excuse)
        {
            excuse.Entity = await _entityRepository.Get(excuse.EntityId);
            if (courseId != excuse.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            var currentUser = HttpContext.Current.User as UserPrincipal;
            if (currentUser == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.UserNotFound);
            }

            try
            {
                var enrollment = await _courseUserRepository.GetAllByLambda(cu => cu.UserId == currentUser.User.Id && cu.CourseId == courseId);
                var courseUserModels = enrollment as IList<CourseUserModel> ?? enrollment.ToList();
                var firstOrDefault = courseUserModels.FirstOrDefault();
                if (firstOrDefault == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidEnrollment);
                }
                if (firstOrDefault.ExcuseNumber >= excuse.Entity.Task.Course.ExcuseLimit)
                {
                    // The user already reached the maximum number of allowed Excuses
                    return Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ExcuseNumberExceeded);
                }

                excuse.IsGranted = false;
                var result = await _excuseRepository.Add(excuse);
                if (result == null)
                {
                    Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                // Update the number of Excuses asked for
                firstOrDefault.ExcuseNumber += 1;
                var query = await _courseUserRepository.Update(firstOrDefault);
                if (query == null)
                {
                    // Revert the excuse request
                    if (result != null)
                    {
                        await _excuseRepository.Delete(result.Id);
                    }
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // PUT: api/Courses/{courseId}/Excuses/{excuseId}
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanGrantExcuses)]
        public async Task<HttpResponseMessage> Update(int courseId, int excuseId, [FromBody] ExcuseModel excuse)
        {
            if (excuseId != excuse.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            excuse.Entity = await _entityRepository.Get(excuseId);
            if (courseId != excuse.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _excuseRepository.Update(excuse);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // DELETE: api/Courses/{courseId}/Excuses/{excuseId}
        [HttpDelete]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanGrantExcuses)]
        public async Task<HttpResponseMessage> Delete(int courseId, int excuseId)
        {
            try
            {
                var existingexcuse = await _excuseRepository.Get(excuseId);
                if (existingexcuse == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingexcuse.Entity.Task.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var result = await _excuseRepository.Delete(excuseId);
                return Request.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}