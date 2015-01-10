namespace GraderApi.Controllers
{
    using Filters;
    using Resources;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;

    public class FilesController : ApiController
    {
        private readonly IFileRepository _fileRepository;
        public FilesController(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        // GET: api/Files/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllFiles)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _fileRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
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
                var result = await _fileRepository.GetAllByCourseId(courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
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
                var file = await _fileRepository.Get(fileId);
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
                var result = await _fileRepository.Add(file);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
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
                var result = await _fileRepository.Update(file);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
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
                var existingfile = await _fileRepository.Get(fileId);
                if (existingfile == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingfile.Entity.Task.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var result = await _fileRepository.Delete(fileId);
                return Request.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}