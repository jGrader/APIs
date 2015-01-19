namespace GraderApi.Controllers
{
    using Extensions;
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Resources;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    public class CurrentUserController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserModel _user;
        private readonly Logger _logger;

        public CurrentUserController(UnitOfWork unitOfWork, Logger log)
        {
            _logger = log;
            _unitOfWork = unitOfWork;

            var principal = HttpContext.Current.User as UserPrincipal;
            if (principal == null)
            {
                throw new NullReferenceException(Messages.UnexpectedError);
            }
            _user = principal.User;
        }

        // GET: api/CurrentUser/Courses
        [HttpGet]
        public async Task<HttpResponseMessage> Courses()
        {
            try
            {
                var courses = await _unitOfWork.CourseUserRepository.GetByUserId(_user.Id);
                var tasks = (from c in courses select _unitOfWork.CourseRepository.Get(c.CourseId));
                var result = await Task.WhenAll(tasks);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());

            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/CurrentUser/Submissions/{courseId}
        [HttpGet]
        [ValidateModelState]
        public async Task<HttpResponseMessage> Submissions(int courseId)
        {
            try
            {
                var submissions = await _unitOfWork.SubmissionRepository.GetByExpression(s => s.UserId == _user.Id && s.File.Entity.Task.CourseId == courseId);
                return Request.CreateResponse(HttpStatusCode.OK, submissions.ToJson());

            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/CurrentUser/Grades/{courseId}
        [HttpGet]
        [ValidateModelState]
        public async Task<HttpResponseMessage> Grades(int courseId)
        {
            try
            {
                var entities = from e in (await _unitOfWork.EntityRepository.GetByCourseId(courseId)) select e.Id;
                var grades = (await _unitOfWork.GradeRepository.GetByUserId(_user.Id)).Where(g => entities.Contains(g.EntityId));

                return Request.CreateResponse(HttpStatusCode.OK, grades.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/CurrentUser/CourseGrade/{courseId}
        [HttpGet]
        [ValidateModelState]
        public async Task<HttpResponseMessage> CourseGrade(int courseId, [FromBody] bool isPredicted)
        {
            try
            {
                var courseUser = await _unitOfWork.CourseUserRepository.GetByExpression(cu => cu.CourseId == courseId && cu.UserId == _user.Id);
                var courseUserModels = courseUser as IList<CourseUserModel> ?? courseUser.ToList();
                var firstOrDefaultEnrollment = courseUserModels.FirstOrDefault();
                if (firstOrDefaultEnrollment == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, Messages.InvalidEnrollment);
                }

                //
                var gradeComponents = await _unitOfWork.GradeComponentRepository.GetByCourseId(courseId);
                var entities = await _unitOfWork.EntityRepository.GetByCourseId(courseId);
                var entityModels = entities as IList<EntityModel> ?? entities.ToList();

                var finalGrade = 0;
                foreach (var gradeComponent in gradeComponents)
                {
                    var sum = 0;
                    var count = 0;

                    var component = gradeComponent; // If this isn't here, we might get a bug depending on compiler version
                    var filteredEntitiesByGradeComponent = entityModels.Where(e => e.Task.GradeComponentId == component.Id);
                    var entitiesByGradeComponent = filteredEntitiesByGradeComponent as IList<EntityModel> ?? filteredEntitiesByGradeComponent.ToList();

                    foreach (var entity in entitiesByGradeComponent)
                    {
                        var entity1 = entity; // If this isn't here, we might get a bug depending on compiler version
                        var grade = await _unitOfWork.GradeRepository.GetByExpression(g => g.EntityId == entity1.Id && g.UserId == _user.Id);
                        var firstOrDefault = grade.FirstOrDefault();
                        if (firstOrDefault == null)
                        {
                            continue;
                        }

                        count++;
                        sum += firstOrDefault.Grade + firstOrDefault.BonusGrade;
                    }

                    if (isPredicted)
                    {
                        finalGrade += (sum / count);
                    }
                    else
                    {
                        finalGrade += (sum / entitiesByGradeComponent.Count());
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, finalGrade);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
