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

    public class ExtensionsController : ApiController
    {
        private readonly IExtensionRepository _extensionRepository;
        private readonly IEntityRepository _entityRepository;
        private readonly ICourseUserRepository _courseUserRepository;
        public ExtensionsController(IExtensionRepository extensionRepository, IEntityRepository entityRepository, ICourseUserRepository courseUserRepository)
        {
            _extensionRepository = extensionRepository;
            _entityRepository = entityRepository;
            _courseUserRepository = courseUserRepository;
        }

        // GET: api/Extensions/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllExtensions)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _extensionRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Extensions/All
        [HttpGet]
        [ValidateModelState]
        public async Task<HttpResponseMessage> All(int courseId)
        {
            try
            {
                var result = await _extensionRepository.GetAllByLambda(e => e.Entity.Task.CourseId == courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses/{courseId}/Extensions/Add
        [HttpPost]
        [ValidateModelState]
        public async Task<HttpResponseMessage> Add(int courseId, [FromBody] ExtensionModel extension)
        {
            extension.Entity = await _entityRepository.Get(extension.EntityId);
            if (courseId != extension.Entity.Task.CourseId)
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
                if (firstOrDefault.ExtensionNumber >= extension.Entity.Task.Course.ExtensionLimit)
                {
                    // The user already reached the maximum number of allowed extensions
                    return Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ExtensionNumberExceeded);
                }

                extension.IsGranted = false;
                var result = await _extensionRepository.Add(extension);
                if (result == null)
                {
                    Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                // Update the number of extensions asked for
                firstOrDefault.ExtensionNumber += 1;
                var query = await _courseUserRepository.Update(firstOrDefault);
                if (query == null)
                {
                    // Revert the extension request
                    if (result != null)
                    {
                        await _extensionRepository.Delete(result.Id);
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

        // PUT: api/Courses/{courseId}/Extensions/{extensionId}
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanGrantExtensions)]
        public async Task<HttpResponseMessage> Update(int courseId, int extensionId, [FromBody] ExtensionModel extension)
        {
            if (extensionId != extension.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            extension.Entity = await _entityRepository.Get(extensionId);
            if (courseId != extension.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _extensionRepository.Update(extension);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // DELETE: api/Courses/{courseId}/Extensions/{extensionId}
        [HttpDelete]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanGrantExtensions)]
        public async Task<HttpResponseMessage> Delete(int courseId, int extensionId)
        {
            try
            {
                var existingExtension = await _extensionRepository.Get(extensionId);
                if (existingExtension == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingExtension.Entity.Task.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var result = await _extensionRepository.Delete(extensionId);
                return Request.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}