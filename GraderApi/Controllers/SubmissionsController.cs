namespace GraderApi.Controllers
{
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Newtonsoft.Json;
    using Resources;
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
        public SubmissionsController(UnitOfWork unitOfWork)
        {
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
            var result = await Request.Content.ReadAsMultipartAsync(streamProvider);

            if (result.FormData[paramName] == null)
            {
                // We don't have the FileModels ... delete the TempFiles and return BadRequest
                Directory.Delete(tempPath, true);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            // The files have been successfully saved in a TempLocation and the FileModels are not null
            // Validate that everything else is fine with this command
            var fileModels = JsonConvert.DeserializeObject<IEnumerable<FileModelExtension>>(result.FormData[paramName]);
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

            foreach (var tempFile in result.FileData)
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

            // These variables are where we store old info in case smtg goes wrong to be able to revert
            var backUpInformation = new List<Tuple<SubmissionModel, string, string>>();
            var newlyAddedInformation = new List<Tuple<SubmissionModel, string>>();

            try
            {
                // Everything is awesome !!!
                // Start copying the files to their final location
                foreach (var tempFile in result.FileData)
                {
                    var originalFileName = tempFile.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                    var fileModel = fileModelExtensions.First(f => f.OriginalFileName == originalFileName);

                    foreach (var teamMember in currentTeam.TeamMembers)
                    {
                        // We have this for a weird possible bug between different versions of compiler
                        // for the GetByExpression call not getting the value from an inner-scope variable
                        var member = teamMember; 

                        var finalPath = Path.Combine(fileSavePath, member.UserName);
                        Directory.CreateDirectory(finalPath);

                        // Copy the file to the correct location with the correct name
                        var tempFilePath = tempFile.LocalFileName;
                        var finalFilePath = finalPath + "\\" + (fileModel.FileName + fileModel.Extension);
                        var backUpPath = finalPath + "\\Backup";
                        var finalBackUpFilePath = backUpPath + "\\" + (fileModel.FileName + fileModel.Extension);
                        if (File.Exists(finalFilePath))
                        {
                            //First make a backup
                            Directory.CreateDirectory(backUpPath);
                            File.Copy(finalFilePath, finalBackUpFilePath, true);
                        }
                        File.Copy(tempFilePath, finalFilePath, true);

                        // Now it's time to register a submission for this file in the Database
                        var submission = new SubmissionModel { FileId = fileModel.Id, FilePath = finalFilePath, TimeStamp = DateTime.UtcNow, UserId = member.Id };
                        var query = (await _unitOfWork.SubmissionRepository.GetByExpression(s => s.FileId == fileModel.Id && s.UserId == member.Id)).FirstOrDefault();
                        if (query != null)
                        {
                            // There exists an old submission for this file; update it!
                            backUpInformation.Add(new Tuple<SubmissionModel, string, string>(query, finalBackUpFilePath, finalFilePath));

                            submission.Id = query.Id;
                            var updateResult = await _unitOfWork.SubmissionRepository.Update(submission);
                            if (updateResult == null)
                            {
                                Directory.Delete(tempPath);
                                var revertResult =
                                    await RevertFailedSubmisionChanges(backUpInformation, newlyAddedInformation);

                                return Request.CreateResponse(revertResult == HttpStatusCode.OK ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                            }
                        }
                        else
                        {
                            // This is the very first submission
                            var addResult = await _unitOfWork.SubmissionRepository.Add(submission);
                            if (addResult == null)
                            {
                                Directory.Delete(tempPath);
                                var revertResult =
                                    await RevertFailedSubmisionChanges(backUpInformation, newlyAddedInformation);

                                return Request.CreateResponse(revertResult == HttpStatusCode.OK ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                            }

                            newlyAddedInformation.Add(new Tuple<SubmissionModel, string>(addResult, finalFilePath));
                        }
                    }
                }

                // Nothing went wrong; just delete the TempFolder and exit
                Directory.Delete(tempPath, true);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Directory.Delete(tempPath);
                var revertResult = Task.Run(() => RevertFailedSubmisionChanges(backUpInformation, newlyAddedInformation));
                Task.WaitAll(revertResult);

                return Request.CreateErrorResponse(revertResult.Result == HttpStatusCode.OK ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError, e);
            }
        }
        private async Task<HttpStatusCode> RevertFailedSubmisionChanges(IEnumerable<Tuple<SubmissionModel, string, string>> backUpInformation, 
            IEnumerable<Tuple<SubmissionModel, string>> newlyAddedInformation)
        {
            foreach (var x in newlyAddedInformation)
            {
                var result = await _unitOfWork.SubmissionRepository.Delete(x.Item1.Id);
                if (!result)
                {
                    return HttpStatusCode.InternalServerError;
                }

                File.Delete(x.Item2);
            }

            foreach (var y in backUpInformation)
            {
                var result = await _unitOfWork.SubmissionRepository.Update(y.Item1);
                if (result == null)
                {
                    return HttpStatusCode.InternalServerError;
                }

                File.Move(y.Item2, y.Item3);
            }

            return HttpStatusCode.OK;
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
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}