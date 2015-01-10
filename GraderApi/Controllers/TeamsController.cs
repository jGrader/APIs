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

    public class TeamsController : ApiController
    {
        private readonly ITeamRepository _teamRepository;
        public TeamsController(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        // GET: api/Teams/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllTeams)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _teamRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
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
                var result = await _teamRepository.GetAllForCourse(courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses/{courseId}/Teams/Add
        [HttpPost]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanCreateTeams)]
        public async Task<HttpResponseMessage> Add(int courseId, [FromBody] TeamModel team)
        {
            if (courseId != team.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _teamRepository.Add(team);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
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
            if (courseId != team.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _teamRepository.Update(team);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
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
                var existingTeam = await _teamRepository.Get(teamId);
                if (existingTeam == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingTeam.Entity.Task.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var result = await _teamRepository.Delete(teamId);
                return Request.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}