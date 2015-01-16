namespace GraderApi.Controllers
{
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Services;
    using System;
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
                throw new NullReferenceException("Something went wrong");
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
    }
}
