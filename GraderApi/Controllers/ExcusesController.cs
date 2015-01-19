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

    public class ExcusesController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Logger _logger;

        public ExcusesController(UnitOfWork unitOfWork, Logger log)
        {
            _logger = log;
            _unitOfWork = unitOfWork;
        }

        // GET: api/Excuses/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllExcuses)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _unitOfWork.ExcuseRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
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
                var result = await _unitOfWork.ExcuseRepository.GetByExpression(e => e.Entity.Task.CourseId == courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses/{courseId}/Excuses/Add?userId={int value}
        [HttpPost]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanGrantExcuses)]
        public async Task<HttpResponseMessage> Add(int courseId, int userId, [FromBody] ExcuseModel excuse)
        {
            excuse.Entity = await _unitOfWork.EntityRepository.Get(excuse.EntityId);
            if (courseId != excuse.Entity.Task.CourseId)
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
                var firstOrDefaultTeam = await GetCorrectTeamModel(true, excuse);
                if (firstOrDefaultTeam == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.NoTeamFound);
                }

                // This excuse can only be added directly by someone with permission
                // So they obviously want it to be granted
                excuse.IsGranted = true;
                var addedExcuses = new List<ExcuseModel>();
                foreach (var tm in firstOrDefaultTeam.TeamMembers)
                {
                    var tm1 = tm;
                    var enrollments = await _unitOfWork.CourseUserRepository.GetByExpression(cu => cu.UserId == tm1.Id && cu.CourseId == courseId);
                    var firstOrDefaultEnrollment = enrollments.FirstOrDefault();
                    if (firstOrDefaultEnrollment == null)
                    {
                        // This user is not enrolled in this course
                        // Something is fishy; revert and exit
                        var deleteResult = await DeleteAddedExcuses(addedExcuses);
                        return Request.CreateResponse(deleteResult == HttpStatusCode.OK ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                    }

                    excuse.UserId = tm.Id;
                    var result = await _unitOfWork.ExcuseRepository.Add(excuse);
                    if (result == null)
                    {
                        var deleteResult = await DeleteAddedExcuses(addedExcuses);
                        return Request.CreateResponse(deleteResult == HttpStatusCode.OK ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                    }
                    addedExcuses.Add(result);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                _logger.Log(e);
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
            excuse.Entity = await _unitOfWork.EntityRepository.Get(excuseId);
            if (courseId != excuse.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var firstOrDefaultTeam = await GetCorrectTeamModel(true, excuse);
                if (firstOrDefaultTeam == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.NoTeamFound);
                }

                var backUpValues = new List<ExcuseModel>();
                var addedExcuses = new List<ExcuseModel>();
                foreach (var tm in firstOrDefaultTeam.TeamMembers)
                {
                    var tm1 = tm;
                    var oldExcuse = await _unitOfWork.ExcuseRepository.GetByExpression(g => g.EntityId == excuse.EntityId && g.UserId == tm1.Id);
                    var firstOrDefaultOldGrade = oldExcuse.FirstOrDefault();
                    if (firstOrDefaultOldGrade != null)
                    {
                        backUpValues.Add(firstOrDefaultOldGrade);

                        excuse.UserId = tm.Id;
                        var result = await _unitOfWork.ExcuseRepository.Update(excuse);

                        if (result == null)
                        {
                            var revertResult1 = await UndoChangedExcuses(backUpValues);
                            var revertResult2 = await DeleteAddedExcuses(addedExcuses);
                            return Request.CreateResponse(revertResult1 == HttpStatusCode.OK && revertResult2 == HttpStatusCode.OK
                                ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                        }
                    }
                    else
                    {
                        excuse.UserId = tm.Id;
                        var result = await _unitOfWork.ExcuseRepository.Add(excuse);

                        if (result == null)
                        {
                            var revertResult1 = await UndoChangedExcuses(backUpValues);
                            var revertResult2 = await DeleteAddedExcuses(addedExcuses);
                            return Request.CreateResponse(revertResult1 == HttpStatusCode.OK && revertResult2 == HttpStatusCode.OK
                                ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                        }

                        addedExcuses.Add(result);
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

        // DELETE: api/Courses/{courseId}/Excuses/{excuseId}
        [HttpDelete]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanGrantExcuses)]
        public async Task<HttpResponseMessage> Delete(int courseId, int excuseId)
        {
            try
            {
                var existingExcuse = await _unitOfWork.ExcuseRepository.Get(excuseId);
                if (existingExcuse == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingExcuse.Entity.Task.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var firstOrDefaultTeam = await GetCorrectTeamModel(true, existingExcuse);
                if (firstOrDefaultTeam == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.NoTeamFound);
                }

                var deletedExcuses = new List<ExcuseModel>();
                foreach (var tm in firstOrDefaultTeam.TeamMembers)
                {
                    var tm1 = tm;
                    var oldExcuse = await _unitOfWork.ExcuseRepository.GetByExpression(g => g.EntityId == existingExcuse.EntityId && g.UserId == tm1.Id);
                    var firstOrDefaultOldExcuse = oldExcuse.FirstOrDefault();
                    if (firstOrDefaultOldExcuse != null)
                    {
                        var result = await _unitOfWork.GradeRepository.Delete(firstOrDefaultOldExcuse.Id);
                        if (result == false)
                        {
                            var undoResult = await AddDeletedExcuses(deletedExcuses);
                            return Request.CreateResponse(undoResult == HttpStatusCode.OK
                                ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                        }

                        deletedExcuses.Add(firstOrDefaultOldExcuse);
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
        private async Task<TeamModel> GetCorrectTeamModel(bool wholeTeam, ExcuseModel excuse)
        {
            var firstOrDefaultTeam = new TeamModel
            {
                EntityId = excuse.EntityId,
                TeamMembers = new Collection<UserModel> { await _unitOfWork.UserRepository.Get(excuse.UserId) }
            };

            if (wholeTeam)
            {
                var teams = await _unitOfWork.TeamRepository.GetByExpression(t => t.EntityId == excuse.EntityId
                                                                                  && t.TeamMembers.FirstOrDefault(tm => tm.Id == excuse.UserId) != null);
                firstOrDefaultTeam = teams.FirstOrDefault();
            }

            return firstOrDefaultTeam;
        }

        private async Task<HttpStatusCode> DeleteAddedExcuses(IEnumerable<ExcuseModel> addedExcuses)
        {
            foreach (var excuse in addedExcuses)
            {
                var result = await _unitOfWork.ExcuseRepository.Delete(excuse);

                if (result == false)
                {
                    return HttpStatusCode.InternalServerError;
                }
            }

            return HttpStatusCode.OK;
        }

        private async Task<HttpStatusCode> UndoChangedExcuses(IEnumerable<ExcuseModel> backUpValues)
        {
            foreach (var excuse in backUpValues)
            {
                var result = await _unitOfWork.ExcuseRepository.Update(excuse);

                if (result == null)
                {
                    return HttpStatusCode.InternalServerError;
                }
            }

            return HttpStatusCode.OK;
        }

        private async Task<HttpStatusCode> AddDeletedExcuses(IEnumerable<ExcuseModel> deletedExcuses)
        {
            foreach (var excuse in deletedExcuses)
            {
                var result = await _unitOfWork.ExcuseRepository.Add(excuse);

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