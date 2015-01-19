namespace GraderApi.Controllers
{
    using Filters;
    using Grader.ExtensionMethods;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Newtonsoft.Json.Linq;
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

    public class GradesController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Logger _logger;

        public GradesController(UnitOfWork unitOfWork, Logger log)
        {
            _logger = log;
            _unitOfWork = unitOfWork;
        }

        // GET: api/Grades/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllGrades)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _unitOfWork.GradeRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Grades/All
        [HttpGet]
        [PermissionsAuthorize(CoursePermissions.CanSeeGrades)]
        public async Task<HttpResponseMessage> All(int courseId)
        {
            try
            {
                var res = new List<IEnumerable<GradeModel>>();
                var entities = (await _unitOfWork.EntityRepository.GetByCourseId(courseId));
                foreach (var entity in entities)
                {
                    var tmp = await _unitOfWork.GradeRepository.GetByEntityId(entity.Id);
                    res.Add(tmp);
                }

                var jsonResult = new JArray();
                res.ForEach(g => jsonResult.Add(g.ToJson()));

                return Request.CreateResponse(HttpStatusCode.OK, jsonResult);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Grades/GetByUserId/{gradeId}
        [HttpGet]
        [PermissionsAuthorize(CoursePermissions.CanSeeGrades)]
        public async Task<HttpResponseMessage> Get(int gradeId)
        {
            try
            {
                var task = await _unitOfWork.GradeRepository.Get(gradeId);

                return task != null
                    ? Request.CreateResponse(HttpStatusCode.Accepted, task.ToJson())
                    : Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses/{courseId}/Grades/Add?wholeTeam={true/false}
        [HttpPost]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanGrade)]
        public async Task<HttpResponseMessage> Add(int courseId, bool wholeTeam, [FromBody] GradeModel grade)
        {
            grade.Entity = await _unitOfWork.EntityRepository.Get(grade.EntityId);
            grade.Entity.Task = await _unitOfWork.TaskRepository.Get(grade.Entity.TaskId);
            if (courseId != grade.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var firstOrDefaultTeam = await GetCorrectTeamModel(wholeTeam, grade);
                if (firstOrDefaultTeam == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.NoTeamFound);
                }

                var addedGrades = new List<GradeModel>();
                foreach (var teamMember in firstOrDefaultTeam.TeamMembers)
                {
                    grade.UserId = teamMember.Id;
                    var result = await _unitOfWork.GradeRepository.Add(grade);

                    if (result == null)
                    {
                        var revertResult = await DeleteAddedGrades(addedGrades);
                        return Request.CreateResponse(revertResult == HttpStatusCode.OK ? 
                            HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                    }

                    addedGrades.Add(result);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        
        // PUT: api//Courses/{courseId}/Grades/Update/{gradeId}?wholeTeam={true/false}
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanUpdateGrade)]
        public async Task<HttpResponseMessage> Update(int courseId, int gradeId, bool wholeTeam, [FromBody] GradeModel grade)
        {
            if (gradeId != grade.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            grade.Entity = await _unitOfWork.EntityRepository.Get(grade.EntityId);
            grade.Entity.Task = await _unitOfWork.TaskRepository.Get(grade.Entity.TaskId);
            if (courseId != grade.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var firstOrDefaultTeam = await GetCorrectTeamModel(wholeTeam, grade);
                if (firstOrDefaultTeam == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.NoTeamFound);
                }

                var backUpValues = new List<GradeModel>();
                var addedGrades = new List<GradeModel>();
                foreach (var tm in firstOrDefaultTeam.TeamMembers)
                {
                    var oldGrade = await _unitOfWork.GradeRepository.GetByExpression(g => g.EntityId == grade.EntityId && g.UserId == tm.Id);
                    var firstOrDefaultOldGrade = oldGrade.FirstOrDefault();
                    if (firstOrDefaultOldGrade != null)
                    {
                        backUpValues.Add(firstOrDefaultOldGrade);

                        grade.UserId = tm.Id;
                        var result = await _unitOfWork.GradeRepository.Update(grade);

                        if (result == null)
                        {
                            var revertResult = await UndoChangedGrades(backUpValues);
                            return Request.CreateResponse(revertResult == HttpStatusCode.OK
                                ? HttpStatusCode.NotModified
                                : HttpStatusCode.InternalServerError);
                        }
                    }
                    else
                    {
                        grade.UserId = tm.Id;
                        var result = await _unitOfWork.GradeRepository.Add(grade);

                        if (result == null)
                        {
                            var revertResult = await DeleteAddedGrades(addedGrades);
                            return Request.CreateResponse(revertResult == HttpStatusCode.OK 
                                ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                        }

                        addedGrades.Add(result);
                    }
                }

                Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // DELETE: api//Courses/{courseId}/Grades/Delete/{gradeId}?wholeTeam={true/false}
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanDeleteGrade)]
        public async Task<HttpResponseMessage> Delete(int courseId, int gradeId, bool wholeTeam)
        {
            try
            {
                var existingGrade = await _unitOfWork.GradeRepository.Get(gradeId);
                if (existingGrade == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingGrade.Entity.Task.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var firstOrDefaultTeam = await GetCorrectTeamModel(wholeTeam, existingGrade);
                if (firstOrDefaultTeam == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.NoTeamFound);
                }

                var deletedGrades = new List<GradeModel>();
                foreach (var tm in firstOrDefaultTeam.TeamMembers)
                {
                    var tm1 = tm;
                    var oldGrade = await _unitOfWork.GradeRepository.GetByExpression(g => g.EntityId == existingGrade.EntityId && g.UserId == tm1.Id);
                    var firstOrDefaultOldGrade = oldGrade.FirstOrDefault();
                    if (firstOrDefaultOldGrade != null)
                    {
                        var result = await _unitOfWork.GradeRepository.Delete(firstOrDefaultOldGrade.Id);
                        if (result == false)
                        {
                            var undoResult = await AddDeletedGrades(deletedGrades);
                            return Request.CreateResponse(undoResult == HttpStatusCode.OK 
                                ? HttpStatusCode.NotModified : HttpStatusCode.InternalServerError);
                        }

                        deletedGrades.Add(firstOrDefaultOldGrade);
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
        private async Task<TeamModel> GetCorrectTeamModel(bool wholeTeam, GradeModel grade)
        {
            var firstOrDefaultTeam = new TeamModel
            {
                EntityId = grade.EntityId,
                TeamMembers = new Collection<UserModel> { await _unitOfWork.UserRepository.Get(grade.UserId) }
            };

            if (wholeTeam)
            {
                var teams = await _unitOfWork.TeamRepository.GetByExpression(t => t.EntityId == grade.EntityId
                                                                                  && t.TeamMembers.FirstOrDefault(tm => tm.Id == grade.UserId) != null);
                firstOrDefaultTeam = teams.FirstOrDefault();
            }

            return firstOrDefaultTeam;
        }

        private async Task<HttpStatusCode> DeleteAddedGrades(IEnumerable<GradeModel> addedGrades)
        {
            foreach (var grade in addedGrades)
            {
                var result = await _unitOfWork.GradeRepository.Delete(grade);

                if (result == false)
                {
                    return HttpStatusCode.InternalServerError;
                }
            }

            return HttpStatusCode.OK;
        }

        private async Task<HttpStatusCode> UndoChangedGrades(IEnumerable<GradeModel> backUpValues)
        {
            foreach (var grade in backUpValues)
            {
                var result = await _unitOfWork.GradeRepository.Update(grade);

                if (result == null)
                {
                    return HttpStatusCode.InternalServerError;
                }
            }

            return HttpStatusCode.OK;
        }

        private async Task<HttpStatusCode> AddDeletedGrades(IEnumerable<GradeModel> deletedGrades)
        {
            foreach (var grade in deletedGrades)
            {
                var result = await _unitOfWork.GradeRepository.Add(grade);

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
