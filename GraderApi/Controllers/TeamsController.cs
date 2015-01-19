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

        // GET: api/Courses/{courseId}/Teams/GetByUserId/{teamId}
        [HttpGet]
        [PermissionsAuthorize(CoursePermissions.CanSeeGrades)]
        public async Task<HttpResponseMessage> Get(int teamId)
        {
            try
            {
                var task = await _unitOfWork.TeamRepository.Get(teamId);

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
                var result = await _unitOfWork.TeamRepository.Add(team);

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
    }
}