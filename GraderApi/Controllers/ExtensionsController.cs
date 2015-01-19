namespace GraderApi.Controllers
{
    using Filters;
    using Grader.ExtensionMethods;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Resources;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class ExtensionsController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Logger _logger;

        public ExtensionsController(UnitOfWork unitOfWork, Logger log)
        {
            _logger = log;
            _unitOfWork = unitOfWork;
        }

        // GET: api/Extensions/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllExtensions)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _unitOfWork.ExtensionRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Extensions/All
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanGrantExtensions)]
        public async Task<HttpResponseMessage> All(int courseId)
        {
            try
            {
                var result = await _unitOfWork.ExtensionRepository.GetByExpression(e => e.Entity.Task.CourseId == courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses/{courseId}/Extensions/Add?userId={int value}
        [HttpPost]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanGrantExtensions)]
        public async Task<HttpResponseMessage> Add(int courseId, int userId, [FromBody] ExtensionModel extension)
        {
            extension.Entity = await _unitOfWork.EntityRepository.Get(extension.EntityId);
            if (courseId != extension.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            var user = await _unitOfWork.UserRepository.Get(userId);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.UserNotFound);
            }

            try
            {
                var firstOrDefaultTeam = await GetCorrectTeamModel(true, extension);
                if (firstOrDefaultTeam == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.NoTeamFound);
                }

                // This extension can only be added directly by someone with permission
                // So they obviously want it to be granted
                extension.IsGranted = true;
                var addedExtensions = new List<ExtensionModel>();
                foreach (var tm in firstOrDefaultTeam.TeamMembers)
                {
                    var tm1 = tm;
                    var enrollments = await _unitOfWork.CourseUserRepository.GetByExpression(cu => cu.UserId == tm1.Id && cu.CourseId == courseId);
                    var firstOrDefaultEnrollment = enrollments.FirstOrDefault();
                    if (firstOrDefaultEnrollment == null)
                    {
                        // This user is not enrolled in this course
                        // Something is fishy; revert and exit
                        var deleteResult = await DeleteAddedExtensions(addedExtensions);
                        return Request.CreateResponse(deleteResult == HttpStatusCode.OK ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                    }

                    extension.UserId = tm.Id;
                    var result = await _unitOfWork.ExtensionRepository.Add(extension);
                    if (result == null)
                    {
                        var deleteResult = await DeleteAddedExtensions(addedExtensions);
                        return Request.CreateResponse(deleteResult == HttpStatusCode.OK ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                    }
                    addedExtensions.Add(result);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                _logger.Log(e);
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
            extension.Entity = await _unitOfWork.EntityRepository.Get(extensionId);
            if (courseId != extension.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var firstOrDefaultTeam = await GetCorrectTeamModel(true, extension);
                if (firstOrDefaultTeam == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.NoTeamFound);
                }

                var backUpValues = new List<ExtensionModel>();
                var addedExtensions = new List<ExtensionModel>();
                foreach (var tm in firstOrDefaultTeam.TeamMembers)
                {
                    var tm1 = tm;
                    var oldExtension = await _unitOfWork.ExtensionRepository.GetByExpression(g => g.EntityId == extension.EntityId && g.UserId == tm1.Id);
                    var firstOrDefaultOldGrade = oldExtension.FirstOrDefault();
                    if (firstOrDefaultOldGrade != null)
                    {
                        backUpValues.Add(firstOrDefaultOldGrade);

                        extension.UserId = tm.Id;
                        var result = await _unitOfWork.ExtensionRepository.Update(extension);

                        if (result == null)
                        {
                            var revertResult1 = await UndoChangedExtensions(backUpValues);
                            var revertResult2 = await DeleteAddedExtensions(addedExtensions);
                            return Request.CreateResponse(revertResult1 == HttpStatusCode.OK && revertResult2 == HttpStatusCode.OK
                                ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                        }
                    }
                    else
                    {
                        extension.UserId = tm.Id;
                        var result = await _unitOfWork.ExtensionRepository.Add(extension);

                        if (result == null)
                        {
                            var revertResult1 = await UndoChangedExtensions(backUpValues);
                            var revertResult2 = await DeleteAddedExtensions(addedExtensions);
                            return Request.CreateResponse(revertResult1 == HttpStatusCode.OK && revertResult2 == HttpStatusCode.OK
                                ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                        }

                        addedExtensions.Add(result);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                _logger.Log(e);
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
                var existingExtension = await _unitOfWork.ExtensionRepository.Get(extensionId);
                if (existingExtension == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingExtension.Entity.Task.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var firstOrDefaultTeam = await GetCorrectTeamModel(true, existingExtension);
                if (firstOrDefaultTeam == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.NoTeamFound);
                }

                var deletedExtensions = new List<ExtensionModel>();
                foreach (var tm in firstOrDefaultTeam.TeamMembers)
                {
                    var tm1 = tm;
                    var oldExtension = await _unitOfWork.ExtensionRepository.GetByExpression(g => g.EntityId == existingExtension.EntityId && g.UserId == tm1.Id);
                    var firstOrDefaultOldExtension = oldExtension.FirstOrDefault();
                    if (firstOrDefaultOldExtension != null)
                    {
                        var result = await _unitOfWork.GradeRepository.Delete(firstOrDefaultOldExtension.Id);
                        if (result == false)
                        {
                            var undoResult = await AddDeletedExtensions(deletedExtensions);
                            return Request.CreateResponse(undoResult == HttpStatusCode.OK
                                ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                        }

                        deletedExtensions.Add(firstOrDefaultOldExtension);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }


        #region Helpers
        private async Task<TeamModel> GetCorrectTeamModel(bool wholeTeam, ExtensionModel extension)
        {
            var firstOrDefaultTeam = new TeamModel
            {
                EntityId = extension.EntityId,
                TeamMembers = new Collection<UserModel> { await _unitOfWork.UserRepository.Get(extension.UserId) }
            };

            if (wholeTeam)
            {
                var teams = await _unitOfWork.TeamRepository.GetByExpression(t => t.EntityId == extension.EntityId
                                                                                  && t.TeamMembers.FirstOrDefault(tm => tm.Id == extension.UserId) != null);
                firstOrDefaultTeam = teams.FirstOrDefault();
            }

            return firstOrDefaultTeam;
        }

        private async Task<HttpStatusCode> DeleteAddedExtensions(IEnumerable<ExtensionModel> addedExtensions)
        {
            foreach (var extension in addedExtensions)
            {
                var result = await _unitOfWork.ExtensionRepository.Delete(extension);

                if (result == false)
                {
                    return HttpStatusCode.InternalServerError;
                }
            }

            return HttpStatusCode.OK;
        }

        private async Task<HttpStatusCode> UndoChangedExtensions(IEnumerable<ExtensionModel> backUpValues)
        {
            foreach (var extension in backUpValues)
            {
                var result = await _unitOfWork.ExtensionRepository.Update(extension);

                if (result == null)
                {
                    return HttpStatusCode.InternalServerError;
                }
            }

            return HttpStatusCode.OK;
        }

        private async Task<HttpStatusCode> AddDeletedExtensions(IEnumerable<ExtensionModel> deletedExtensions)
        {
            foreach (var extension in deletedExtensions)
            {
                var result = await _unitOfWork.ExtensionRepository.Add(extension);

                if (result == null)
                {
                    return HttpStatusCode.InternalServerError;
                }
            }

            return HttpStatusCode.OK;
        }
        #endregion
    }
}