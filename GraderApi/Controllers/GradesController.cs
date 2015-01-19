namespace GraderApi.Controllers
{
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Newtonsoft.Json.Linq;
    using Resources;
    using Services;
    using System;
    using System.Collections.Generic;
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

        // POST: api/Courses/{courseId}/Grades/Add
        [HttpPost]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanGrade)]
        public async Task<HttpResponseMessage> Add(int courseId, [FromBody] GradeModel grade)
        {
            grade.Entity = await _unitOfWork.EntityRepository.Get(grade.EntityId);
            grade.Entity.Task = await _unitOfWork.TaskRepository.Get(grade.Entity.TaskId);
            if (courseId != grade.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _unitOfWork.GradeRepository.Add(grade);

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

        // PUT: api//Courses/{courseId}/Grades/Update/{gradeId}
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanUpdateGrade)]
        public async Task<HttpResponseMessage> Update(int courseId, int gradeId, [FromBody] GradeModel grade)
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
                var result = await _unitOfWork.GradeRepository.Update(grade);

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

        // DELETE: api//Courses/{courseId}/Grades/Delete/{gradeId}
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanDeleteGrade)]
        public async Task<HttpResponseMessage> Delete(int courseId, int gradeId)
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

                var result = await _unitOfWork.EntityRepository.Delete(gradeId);
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
