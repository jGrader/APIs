

namespace GraderApi.Controllers
{
    using Filters;
    using Grader.ExtensionMethods;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Resources;
    using Services;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class FilesController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Logger _logger;

        public FilesController(UnitOfWork unitOfWork, Logger log)
        {
            _logger = log;
            _unitOfWork = unitOfWork;
        }

        // GET: api/Files/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllFiles)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _unitOfWork.FileRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Files/All
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeFiles)]
        public async Task<HttpResponseMessage> All(int courseId)
        {
            try
            {
                var result = await _unitOfWork.FileRepository.GetByCourseId(courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Files/{fileId}
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeFiles)]
        public async Task<HttpResponseMessage> Get(int courseId, int fileId)
        {
            try
            {
                var file = await _unitOfWork.FileRepository.Get(fileId);
                if (file == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return courseId == file.Entity.Task.CourseId
                    ? Request.CreateResponse(HttpStatusCode.OK, file.ToJson())
                    : Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses/{courseId}/Files/Add
        [HttpPost]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanCreateFiles)]
        public async Task<HttpResponseMessage> Add(int courseId, [FromBody] FileModel file)
        {
            if (courseId != file.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _unitOfWork.FileRepository.Add(file);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // PUT: api/Courses/{courseId}/Files/{fileId}
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanUpdateFiles)]
        public async Task<HttpResponseMessage> Update(int courseId, int fileId, [FromBody] FileModel file)
        {
            if (fileId != file.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            if (courseId != file.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _unitOfWork.FileRepository.Update(file);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // DELETE: api/Courses/{courseId}/Files/{fileId}
        [HttpDelete]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanDeleteFiles)]
        public async Task<HttpResponseMessage> DeleteFileModel(int courseId, int fileId)
        {
            try
            {
                var existingfile = await _unitOfWork.FileRepository.Get(fileId);
                if (existingfile == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingfile.Entity.Task.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var result = await _unitOfWork.FileRepository.Delete(fileId);
                return Request.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}