using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.Ajax.Utilities;

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

    public class TeamsController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Logger _logger;

        public TeamsController(UnitOfWork unitOfWork, Logger log)
        {
            _logger = log;
            _unitOfWork = unitOfWork;
        }

        // GET: api/Teams/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllTeams)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _unitOfWork.TeamRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Teams/All
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeTeams)]
        public async Task<HttpResponseMessage> All(int courseId)
        {
            try
            {
                var result = await _unitOfWork.TeamRepository.GetByCoureId(courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Teams/Get/{teamId}
        [HttpGet]
        [PermissionsAuthorize(CoursePermissions.CanSeeGrades)]
        public async Task<HttpResponseMessage> Get(int courseId, int teamId)
        {
            try
            {
                var team = await _unitOfWork.TeamRepository.Get(teamId);
                if (team == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                team.Entity = await _unitOfWork.EntityRepository.Get(team.EntityId);
                team.Entity.Task = await _unitOfWork.TaskRepository.Get(team.Entity.TaskId);
                if (team.Entity.Task.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                return Request.CreateResponse(HttpStatusCode.OK, team.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        #region UNSAFE - Context used directly
        // POST: api/Courses/{courseId}/Teams/Add
        [HttpPost]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanCreateTeams)]
        public async Task<HttpResponseMessage> Add(int courseId, [FromBody] TeamModel team)
        {
            team.Entity = await _unitOfWork.EntityRepository.Get(team.EntityId);
            team.Entity.Task = await _unitOfWork.TaskRepository.Get(team.Entity.TaskId);
            if (courseId != team.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            if (team.TeamMembers == null || team.TeamMembers.Count < 2)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                foreach (var tm in team.TeamMembers)
                {
                    // Make sure that the team members are not already members of other teams
                    var tm1 = tm;
                    var teams = await _unitOfWork.TeamRepository.GetByExpression(e => e.EntityId == team.EntityId && e.TeamMembers.FirstOrDefault(f => f.Id == tm1.Id) != null);
                    if (teams.FirstOrDefault() != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, String.Format(Messages.TeamMemberAlreadyInTeam, tm1.UserName));   
                    }
                }

                foreach (var tm in team.TeamMembers)
                {
                    // In case one of the users in the team already has 
                    // a submission, excuse, or extension for this entity, delete ALL of them
                    var tm1 = tm;
                    var submissions = await _unitOfWork.SubmissionRepository.GetByExpression(e => e.File.EntityId == team.EntityId && e.UserId == tm1.Id);
                    foreach (var s in submissions)
                    {
                        _unitOfWork.Context.Entry(s).State = EntityState.Deleted;
                    }

                    var extensions = await _unitOfWork.ExtensionRepository.GetByExpression(e => e.EntityId == team.EntityId && e.UserId == tm1.Id);
                    foreach (var e in extensions)
                    {
                        _unitOfWork.Context.Entry(e).State = EntityState.Deleted;
                    }

                    var excuses = await _unitOfWork.ExcuseRepository.GetByExpression(e => e.EntityId == team.EntityId && e.UserId == tm1.Id);
                    foreach (var e in excuses)
                    {
                        _unitOfWork.Context.Entry(e).State = EntityState.Deleted;
                    }
                }

                _unitOfWork.Context.Entry(team).State = EntityState.Added;
                await _unitOfWork.Context.SaveChangesAsync();
                
                return Request.CreateResponse(HttpStatusCode.OK, team.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // PUT: api/Courses/{courseId}/Teams/Update/{teamId}
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanUpdateTeams)]
        public async Task<HttpResponseMessage> Update(int courseId, int teamId, TeamModel team)
        {
            if (teamId != team.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            team.Entity = await _unitOfWork.EntityRepository.Get(team.EntityId);
            team.Entity.Task = await _unitOfWork.TaskRepository.Get(team.Entity.TaskId);
            if (courseId != team.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var oldTeam = await _unitOfWork.TeamRepository.Get(team.Id);
                var deletedMembers = oldTeam.TeamMembers.Where(tm => ! team.TeamMembers.Contains(tm)).ToList();
                if (deletedMembers.Count == oldTeam.TeamMembers.Count)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "You cannot remove all members of the old team!");
                }
                var addedMembers = team.TeamMembers.Where(tm => !oldTeam.TeamMembers.Contains(tm)).ToList();

                foreach (var tm in deletedMembers)
                {
                    // Delete all submission, extensions, excuses of this team for the deleted team members
                    var tm1 = tm;
                    var submissions = await _unitOfWork.SubmissionRepository.GetByExpression(e => e.File.EntityId == team.EntityId && e.UserId == tm1.Id);
                    foreach (var s in submissions)
                    {
                        _unitOfWork.Context.Entry(s).State = EntityState.Deleted;
                    }

                    var extensions = await _unitOfWork.ExtensionRepository.GetByExpression(e => e.EntityId == team.EntityId && e.UserId == tm1.Id);
                    foreach (var e in extensions)
                    {
                        _unitOfWork.Context.Entry(e).State = EntityState.Deleted;
                    }

                    var excuses = await _unitOfWork.ExcuseRepository.GetByExpression(e => e.EntityId == team.EntityId && e.UserId == tm1.Id);
                    foreach (var e in excuses)
                    {
                        _unitOfWork.Context.Entry(e).State = EntityState.Deleted;
                    }
                }

                var constantMember = oldTeam.TeamMembers.FirstOrDefault(tm => !deletedMembers.Contains(tm));
                var teamSubmissions = await _unitOfWork.SubmissionRepository.GetByExpression(e => e.File.EntityId == team.EntityId && e.UserId == constantMember.Id);
                var teamExtensions = await _unitOfWork.ExtensionRepository.GetByExpression(e => e.EntityId == team.EntityId && e.UserId == constantMember.Id);
                var teamExcuses = await _unitOfWork.ExcuseRepository.GetByExpression(e => e.EntityId == team.EntityId && e.UserId == constantMember.Id);
                foreach (var tm in addedMembers)
                {
                    // Add all submissions, extensions, excuses of the team to the new team members
                    var tm1 = tm;
                    foreach (var s in teamSubmissions)
                    {
                        s.UserId = tm1.Id;
                        s.Id = new int();
                       
                        _unitOfWork.Context.Entry(s).State = EntityState.Added;
                    }
                    
                }

                foreach (var tm in team.TeamMembers)
                {
                    // Make sure that the team members are not already members of other teams
                    var tm1 = tm;
                    var teams = await _unitOfWork.TeamRepository.GetByExpression(e => e.EntityId == team.EntityId && e.TeamMembers.FirstOrDefault(f => f.Id == tm1.Id) != null);
                    if (teams.FirstOrDefault() != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, String.Format(Messages.TeamMemberAlreadyInTeam, tm1.UserName));
                    }
                }

                var result = await _unitOfWork.TeamRepository.Update(team);

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

        // DELETE: api/Courses/{courseId}/Teams/Delete/{teamId}
        [HttpDelete]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanDeleteTeams)]
        public async Task<HttpResponseMessage> Delete(int courseId, int teamId)
        {
            try
            {
                var existingTeam = await _unitOfWork.TeamRepository.Get(teamId);
                if (existingTeam == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingTeam.Entity.Task.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var result = await _unitOfWork.TeamRepository.Delete(teamId);
                return Request.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        #endregion
    }
}