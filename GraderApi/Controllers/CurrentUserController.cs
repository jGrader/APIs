namespace GraderApi.Controllers
{
    using Extensions;
    using Filters;
    using Grader.ExtensionMethods;
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
                /*var entities = from e in (await _unitOfWork.EntityRepository.GetByCourseId(courseId)) select e.Id;
                var grades = (await _unitOfWork.GradeRepository.GetByUserId(_user.Id)).Where(g => entities.Contains(g.EntityId));*/
                var tmp = _user.Grades.Where(c => c.Entity.Task.CourseId == courseId);
                return Request.CreateResponse(HttpStatusCode.OK, tmp.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/CurrentUser/CourseGrade/{courseId}/{isPredicted}
        [HttpGet]
        [ValidateModelState]
        public async Task<HttpResponseMessage> CourseGrade(int courseId, bool isPredicted)
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

        // GET: api/CurrentUser/ExtensionRequests/{courseId}
        [HttpGet]
        [ValidateModelState]
        public async Task<HttpResponseMessage> ExtensionRequests(int courseId)
        {
            try
            {
                var enrollments = await _unitOfWork.CourseUserRepository.GetByExpression(cu => cu.CourseId == courseId & cu.UserId == _user.Id);
                var firstOrDefaultEnrollment = enrollments.FirstOrDefault();
                if (firstOrDefaultEnrollment == null)
                {
                    // The user is not enrolled in this course
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidEnrollment);
                }

                // The user is enrolled in this course
                var extensions = await _unitOfWork.ExtensionRepository.GetByExpression(e => e.UserId == _user.Id && e.Entity.Task.CourseId == courseId);
                return Request.CreateResponse(HttpStatusCode.OK, extensions.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Messages.UnexpectedError);
            }
        }

        // POST: api/CurrentUser/ExtensionRequest/{courseId}
        [HttpPost]
        [ValidateModelState]
        public async Task<HttpResponseMessage> ExtensionRequest(int courseId, [FromBody] ExtensionModel extension)
        {
            extension.Entity = await _unitOfWork.EntityRepository.Get(extension.EntityId);
            if (courseId != extension.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var enrollment = await _unitOfWork.CourseUserRepository.GetByExpression(cu => cu.UserId == _user.Id && cu.CourseId == courseId);
                var courseUserModels = enrollment as IList<CourseUserModel> ?? enrollment.ToList();
                var firstOrDefault = courseUserModels.FirstOrDefault();
                if (firstOrDefault == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidEnrollment);
                }
                if (firstOrDefault.ExtensionNumber >= extension.Entity.Task.Course.ExtensionLimit)
                {
                    // The user already reached the maximum number of allowed extensions
                    return Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ExtensionNumberExceeded);
                }

                extension.IsGranted = false;
                var result = await _unitOfWork.ExtensionRepository.Add(extension);
                if (result == null)
                {
                    Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                // Update the number of extensions asked for
                firstOrDefault.ExtensionNumber += 1;
                var query = await _unitOfWork.CourseUserRepository.Update(firstOrDefault);
                if (query == null)
                {
                    // Revert the extension request
                    if (result != null)
                    {
                        await _unitOfWork.ExtensionRepository.Delete(result.Id);
                    }
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/CurrentUser/ExcuseRequests/{courseId}
        [HttpGet]
        [ValidateModelState]
        public async Task<HttpResponseMessage> ExcuseRequests(int courseId)
        {
            try
            {
                var enrollments = await _unitOfWork.CourseUserRepository.GetByExpression(cu => cu.CourseId == courseId & cu.UserId == _user.Id);
                var firstOrDefaultEnrollment = enrollments.FirstOrDefault();
                if (firstOrDefaultEnrollment == null)
                {
                    // The user is not enrolled in this course
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidEnrollment);
                }

                // The user is enrolled in this course
                var excuses = await _unitOfWork.ExcuseRepository.GetByExpression(e => e.UserId == _user.Id && e.Entity.Task.CourseId == courseId);
                return Request.CreateResponse(HttpStatusCode.OK, excuses.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Messages.UnexpectedError);
            }
        }

        // POST: api/CurrentUser/ExcuseRequest/{courseId}
        [HttpPost]
        [ValidateModelState]
        public async Task<HttpResponseMessage> ExcuseRequest(int courseId, [FromBody] ExcuseModel excuse)
        {
            excuse.Entity = await _unitOfWork.EntityRepository.Get(excuse.EntityId);
            if (courseId != excuse.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var enrollment = await _unitOfWork.CourseUserRepository.GetByExpression(cu => cu.UserId == _user.Id && cu.CourseId == courseId);
                var courseUserModels = enrollment as IList<CourseUserModel> ?? enrollment.ToList();
                var firstOrDefault = courseUserModels.FirstOrDefault();
                if (firstOrDefault == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidEnrollment);
                }
                if (firstOrDefault.ExcuseNumber >= excuse.Entity.Task.Course.ExcuseLimit)
                {
                    // The user already reached the maximum number of allowed Excuses
                    return Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ExcuseNumberExceeded);
                }

                excuse.IsGranted = false;
                var result = await _unitOfWork.ExcuseRepository.Add(excuse);
                if (result == null)
                {
                    Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                // Update the number of Excuses asked for
                firstOrDefault.ExcuseNumber += 1;
                var query = await _unitOfWork.CourseUserRepository.Update(firstOrDefault);
                if (query == null)
                {
                    // Revert the excuse request
                    if (result != null)
                    {
                        await _unitOfWork.ExcuseRepository.Delete(result.Id);
                    }
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
