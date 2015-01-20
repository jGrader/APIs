namespace GraderApi.Controllers
{
    using Filters;
    using Grader.ExtensionMethods;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Resources;
    using Services;
    using System;
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

        // POST: api/Courses/{courseId}/Excuses/Add/{userId}
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

                foreach (var tm in firstOrDefaultTeam.TeamMembers)
                {
                    var tm1 = tm;
                    var enrollments = await _unitOfWork.CourseUserRepository.GetByExpression(cu => cu.UserId == tm1.Id && cu.CourseId == courseId);
                    var firstOrDefaultEnrollment = enrollments.FirstOrDefault();
                    if (firstOrDefaultEnrollment == null)
                    {
                        // This user is not enrolled in this course; revert and exit
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }

                // This excuse can only be added directly by someone with permission
                // So they obviously want it to be granted
                excuse.IsGranted = true;

                var result = await _unitOfWork.ExcuseRepository.AddToTeam(excuse, firstOrDefaultTeam.TeamMembers);
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

                var result = await _unitOfWork.ExcuseRepository.UpdateForTeam(excuse, firstOrDefaultTeam.TeamMembers);
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

                var result = await _unitOfWork.ExcuseRepository.DeleteForTeam(existingExcuse, firstOrDefaultTeam.TeamMembers);
                return result
                    ? Request.CreateResponse(HttpStatusCode.OK)
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            } 
        }

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
    }
}