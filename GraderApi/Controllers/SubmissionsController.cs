namespace GraderApi.Controllers
{
    using Extensions;
    using Filters;
    using Grader.ExtensionMethods;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Newtonsoft.Json;
    using Resources;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    public class SubmissionsController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Logger _logger;

        public SubmissionsController(UnitOfWork unitOfWork, Logger log)
        {
            _logger = log;
            _unitOfWork = unitOfWork;
        }

        // GET: api/Submissions/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllSubmissions)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _unitOfWork.SubmissionRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Submissions/All
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeSubmissions)]
        public async Task<HttpResponseMessage> All(int courseId)
        {
            try
            {
                var result = await _unitOfWork.SubmissionRepository.GetByCourseId(courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Submissions/GetByUserId/{submissionId}
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeSubmissions)]
        public async Task<HttpResponseMessage> Get(int courseId, int submissionId)
        {
            try
            {
                var submission = await _unitOfWork.SubmissionRepository.Get(submissionId);
                if (submission == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return submission.File.Entity.Task.CourseId == courseId
                    ? Request.CreateResponse(HttpStatusCode.OK, submission.ToJson())
                    : Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses/{courseId}/Submissions/Add
        [HttpPost]
        [ValidateModelState]
        [ValidateMimeMultipartContent]
        [PermissionsAuthorize(CoursePermissions.CanCreateSubmissions)]
        public async Task<HttpResponseMessage> Add(int courseId)
        {
            const string paramName = "fileModels";

            // Put the files in a temporary file
            var tempPath = HttpContext.Current.Server.MapPath("~/App_Data/Temp/" + Guid.NewGuid());
            Directory.CreateDirectory(tempPath);
           
            var streamProvider = new MultipartFormDataStreamProvider(tempPath);
            var readResult = await Request.Content.ReadAsMultipartAsync(streamProvider);

            if (readResult.FormData[paramName] == null)
            {
                // We don't have the FileModels ... delete the TempFiles and return BadRequest
                Directory.Delete(tempPath, true);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            // The files have been successfully saved in a TempLocation and the FileModels are not null
            // Validate that everything else is fine with this command
            var fileModels = JsonConvert.DeserializeObject<IEnumerable<FileModelExtension>>(readResult.FormData[paramName]);
            var fileModelExtensions = fileModels as IList<FileModelExtension> ?? fileModels.ToList();
            foreach (var file in fileModelExtensions)
            {
                file.Entity = await _unitOfWork.EntityRepository.Get(file.EntityId);
            }

            var firstOrDefault = fileModelExtensions.FirstOrDefault();
            if (firstOrDefault == null)
            {
                // There were no files to submit (HOW?! - we checked for this earlier already)
                // Anyway, return error
                Directory.Delete(tempPath, true);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Messages.NoFiles);
            }

            var entityId = firstOrDefault.EntityId;
            if (fileModelExtensions.Any(file => file.EntityId != entityId))
            {
                // Not all files belong to the same entity, exit; how can that happen?
                Directory.Delete(tempPath, true);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (fileModelExtensions.Any(file => file.Entity.Task.CourseId != courseId))
            {
                // If the files uploaded do not belong to this course, exit; how can that happen ?!
                Directory.Delete(tempPath, true);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            if (fileModelExtensions.Count != HttpContext.Current.Request.Files.Count)
            {
                // The number of files is not the same with the number of FileModels submitted
                Directory.Delete(tempPath, true);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Messages.FileNumberDoesNotMatch);
            }

            foreach (var tempFile in readResult.FileData)
            {
                var originalFileName = tempFile.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                var fileModel = fileModelExtensions.FirstOrDefault(f => f.OriginalFileName == originalFileName);
                if (fileModel == null)
                {
                    // One of the uploaded files is not for any of the submitted FileModels
                    // Delete everything and exit
                    Directory.Delete(tempPath);
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            // All the initial inconsistencies checking is over; things seem to be fine
            var currentUser = HttpContext.Current.User as UserPrincipal;
            if (currentUser == null)
            {
                // Something is fishy about the user's login status, exit;
                Directory.Delete(tempPath, true);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, Messages.UserNotFound);
            }

            // Find out whether the user is working in a team
            var currentTeam = currentUser.User.Teams.FirstOrDefault(t => t.EntityId == entityId) ?? new TeamModel
            {
                // If the user is not working in a team, 
                // create a dummy team consisting of only the current user
                EntityId = entityId,
                TeamMembers = new Collection<UserModel> {currentUser.User}
            };

            var task = firstOrDefault.Entity.Task;
            var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/UploadedFiles/"), task.CourseId + "_" + task.Course.Name,
                task.Id + "_" + task.Name, firstOrDefault.Entity.Id + "_" + firstOrDefault.Entity.Name);

            try
            {
                // Start copying the files to their final location
                var addedSubmissions = new List<SubmissionModel>();
                foreach (var tempFile in readResult.FileData)
                {
                    var originalFileName = tempFile.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                    var fileModel = fileModelExtensions.First(f => f.OriginalFileName == originalFileName);

                    var result = await _unitOfWork.SubmissionRepository.AddSubmissionToTeam(fileSavePath, tempFile.LocalFileName, fileModel, currentTeam.TeamMembers);
                    if (result != null)
                    {
                        addedSubmissions.AddRange(result);
                    }
                }

                // Nothing went wrong; just delete the TempFolder and exit
                Directory.Delete(tempPath, true);
                return Request.CreateResponse(HttpStatusCode.OK, addedSubmissions.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);

                Directory.Delete(tempPath);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        private class FileModelExtension : FileModel
        {
            public string OriginalFileName { get; set; }
        }

        // DELETE: api/Courses/{courseId}/Submissions/Delete/{submissionId}
        [HttpDelete]
        [ValidateModelState]
        [PermissionsAuthorize(SuperUserPermissions.CanDeleteSubmission)]
        public async Task<HttpResponseMessage> Delete(int courseId, int submissionId)
        {
            try
            {
                var existingSubmission = await _unitOfWork.SubmissionRepository.Get(submissionId);
                if (existingSubmission == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingSubmission.File.Entity.Task.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }


                var result = await _unitOfWork.SubmissionRepository.Delete(submissionId);
                if (!result)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                // DELETE THE FILE FROM THE SERVER
                if (File.Exists(existingSubmission.FilePath))
                {
                    File.Delete(existingSubmission.FilePath);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}