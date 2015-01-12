using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GraderApi.Controllers
{
    using System.Threading.Tasks;
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;
    using Newtonsoft.Json.Linq;
    using Resources;
    using WebGrease.Css.Extensions;

    public class GradesController : ApiController
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IEntityRepository _entityRepository;
        private readonly ITaskRepository _taskRepository;

        public GradesController(
            IGradeRepository gradeRepository,
            IEntityRepository entityRepository,
            ITaskRepository taskRepository)
        {
            _gradeRepository = gradeRepository;
            _entityRepository = entityRepository;
            _taskRepository = taskRepository;
        }

        // GET: api/Grades
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllGrades)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _gradeRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Grades/5
        [HttpGet]
        [PermissionsAuthorize(CoursePermissions.CanSeeGrades)]
        public async Task<HttpResponseMessage> All(int courseId)
        {
            try
            {
                var res = new List<IEnumerable<GradeModel>>();
                var entities = (await _entityRepository.GetAllByCourseId(courseId));
                foreach (var entity in entities)
                {
                    var tmp = await _gradeRepository.GetGradesByEntityId(entity.Id);
                    res.Add(tmp);
                }

                var jsonResult = new JArray();
                res.ForEach(g => jsonResult.Add(g.ToJson()));

                return Request.CreateResponse(HttpStatusCode.OK, jsonResult);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Grades
        [HttpPost]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanGrade)]
        public async Task<HttpResponseMessage> Add(int courseId, [FromBody] GradeModel grade)
        {
            grade.Entity = await _entityRepository.Get(grade.EntityId);
            grade.Entity.Task = await _taskRepository.Get(grade.Entity.TaskId);
            if (courseId != grade.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _gradeRepository.Add(grade);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // PUT: api/Grades/5
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanGrade)]
        public async Task<HttpResponseMessage> Update(int courseId, int gradeId, [FromBody]GradeModel grade)
        {
            if (gradeId != grade.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            grade.Entity = await _entityRepository.Get(grade.EntityId);
            grade.Entity.Task = await _taskRepository.Get(grade.Entity.TaskId);
            if (courseId != grade.Entity.Task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _gradeRepository.Update(grade);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // DELETE: api/Grades/5
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanGrade)]
        public async Task<HttpResponseMessage> Delete(int courseId, int gradeId)
        {
            try
            {
                var existingGrade = await _gradeRepository.Get(gradeId);
                if (existingGrade == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingGrade.Entity.Task.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var result = await _entityRepository.Delete(gradeId);
                return Request.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
