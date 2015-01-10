﻿namespace GraderApi.Controllers
{
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;
    using Principals;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    public class SubmissionsController : ApiController
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IEntityRepository _entityRepository;
        public SubmissionsController(ISubmissionRepository submissionRepository, IEntityRepository entityRepository)
        {
            _submissionRepository = submissionRepository;
            _entityRepository = entityRepository;
        }

        // GET: api/Submissions/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllSubmissions)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _submissionRepository.GetAll();
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
                var result = await _submissionRepository.GetAllByCourseId(courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Submissions/Get/{submissionId}
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeSubmissions)]
        public async Task<HttpResponseMessage> Get(int courseId, int submissionId)
        {
            try
            {
                var submission = await _submissionRepository.Get(submissionId);
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
        public async Task<HttpResponseMessage> Add(int courseId, [FromUri] IEnumerable<FileModel> files)
        {
            var fileModels = files as IList<FileModel> ?? files.ToList();
            foreach (var file in fileModels)
            {
                file.Entity = await _entityRepository.Get(file.EntityId);
            }
            if (fileModels.Any(file => file.Entity.Task.CourseId != courseId))
            {
                // If the files uploaded do not belong to this course, exit; how can that happen ?!
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            if (fileModels.Count != HttpContext.Current.Request.Files.Count)
            {
                // The number of files is not the same with the number of FileModels submitted
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Messages.FileNumberDoesNotMatch);
            }

            var currentUser = HttpContext.Current.User as UserPrincipal;
            if (currentUser == null)
            {
                // Something is fishy about the user's login status, exit;
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, Messages.UserNotFound);
            }

            var firstOrDefault = fileModels.FirstOrDefault();
            if (firstOrDefault == null)
            {
                // There were no files to submit, return error
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Messages.NoFiles);
            }

            var task = firstOrDefault.Entity.Task;
            var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/UploadedFiles/"), task.CourseId + "_" + task.Course.Name,
                fileModels.First().Entity.TaskId + "_" + task.Name, fileModels.First().Entity.Id + "_" + fileModels.First().Entity.Name, currentUser.User.UserName);

            if (!Directory.Exists(fileSavePath))
            {
                Directory.CreateDirectory(fileSavePath);
            }

            // HttpContext.Current MIGHT be null, need to research on this.
            var finalFileModels = new List<FileModelExtension>();
            for (var i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                finalFileModels.Add(new FileModelExtension(fileModels[i]) { UploadedFileName = HttpContext.Current.Request.Files[i].FileName });
            }

            
            var streamProvider = new CustomMultipartFormDataStreamProvider(fileSavePath, finalFileModels);
            // Read the MIME multipart content using the stream provider we just created.
            var work = await Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith(async t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                // At this point the files have been uploaded with the proper FileName
                for (var index = 0; index < streamProvider.FileData.Count; index++)
                {
                    var fileId = fileModels[index].Id;
                    var file = streamProvider.FileData[index];
                    var submission = new SubmissionModel {UserId = currentUser.User.Id, FileId = fileId, TimeStamp = DateTime.UtcNow, FilePath = Path.Combine(fileSavePath, file.LocalFileName) };

                    // Delete any old submission for the same file
                    
                    var query = (await _submissionRepository.GetAllByLambda(s => s.FileId == fileId && s.UserId == currentUser.User.Id)).FirstOrDefault();
                    if (query != null)
                    {
                        var isDeleted = await _submissionRepository.DeleteSubmission(query.Id);
                        if (!isDeleted)
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError);
                        }

                        // Delete the file itself also
                        var filePath = Path.Combine(fileSavePath, fileModels[index].FileName);
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }

                    // Add the submission
                    var result = await _submissionRepository.Add(submission);
                    if (result == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }

                    // Now add the submission to all the other TeamMembers
                    var team = currentUser.User.Teams.FirstOrDefault(w => w.EntityId == firstOrDefault.EntityId);
                    if (team != null)
                    {
                        // The user is working in a Team; add the submissions to the other TeamMembers
                        foreach (var teamMember in team.TeamMembers)
                        {
                            var s = new SubmissionModel { UserId = teamMember.Id, FileId = fileId, TimeStamp = DateTime.UtcNow, FilePath = Path.Combine(fileSavePath, file.LocalFileName) };
                            var r = await _submissionRepository.Add(s);
                            if (r == null)
                            {
                                return Request.CreateResponse(HttpStatusCode.InternalServerError);
                            }
                        }
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            });

            return Request.CreateResponse(work.Result.StatusCode);
        }
        private class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            private readonly IEnumerable<FileModelExtension> _fileModels;

            public CustomMultipartFormDataStreamProvider(string path, IEnumerable<FileModelExtension> fileModels) : base(path)
            {
                _fileModels = fileModels;
            }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                var uploadedFileName = headers.ContentDisposition.FileName.Replace("\"", string.Empty); //this is here because Chrome submits files in quotation marks which get treated as part of the filename and get escaped
                var fileModel = _fileModels.FirstOrDefault(f => f.UploadedFileName == uploadedFileName);
                if (fileModel == null)
                {
                    throw new ObjectNotFoundException(Messages.FileNamesDoNotMatch);
                }

                return fileModel.FileName + fileModel.Extension;
            }
        }
        private class FileModelExtension : FileModel
        {
            public string UploadedFileName { get; set; }

            public FileModelExtension(FileModel f)
            {
                Id = f.Id;
                EntityId = f.EntityId;
                Extension = f.Extension;
                FileName = f.FileName;
            }
        }

        // DELETE: api/Courses/{courseId}/Submissions/Delete/{submissionId}
        [HttpDelete]
        [ValidateModelState]
        [PermissionsAuthorize(SuperUserPermissions.CanDeleteSubmission)]
        public async Task<HttpResponseMessage> Delete(int courseId, int submissionId)
        {
            try
            {
                var existingSubmission = await _submissionRepository.Get(submissionId);
                if (existingSubmission == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingSubmission.File.Entity.Task.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }


                var result = await _submissionRepository.DeleteSubmission(submissionId);
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