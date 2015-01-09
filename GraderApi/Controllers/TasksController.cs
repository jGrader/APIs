namespace GraderApi.Controllers
{
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;
    using Resources;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class TasksController : ApiController
    {
        private readonly ITaskRepository _taskRepository;
        public TasksController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        // GET: api/Tasks/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllTasks)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _taskRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Tasks
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeTasks)]
        public async Task<HttpResponseMessage> All(int courseId)
        {
            try
            {
                var result = await _taskRepository.GetAllByCourse(courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Tasks/{taskId}
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeTasks)]
        public async Task<HttpResponseMessage> Get(int taskId)
        {
            try
            {
                var task = await _taskRepository.Get(taskId);

                return task != null
                    ? Request.CreateResponse(HttpStatusCode.Accepted, task.ToJson())
                    : Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/Tasks/GetGradeComponent/{taskId}
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeGradedParts)]
        public async Task<HttpResponseMessage> GetGradeComponent(int courseId, int taskId)
        {
            try
            {
                var task = await _taskRepository.Get(taskId);
                if (task == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return task.GradeComponent.CourseId != courseId 
                    ? Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse) 
                    : Request.CreateResponse(HttpStatusCode.OK, task.GradeComponent.ToJson());
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses/{courseId}/Tasks
        [HttpPost]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanCreateTasks)]
        public async Task<HttpResponseMessage> Add(int courseId, [FromBody] TaskModel task)
        {
            if (courseId != task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _taskRepository.Add(task);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // PUT: api/Courses/{courseId}/Tasks/{taskId}
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanUpdateTasks)]
        public async Task<HttpResponseMessage> Update(int courseId, int taskId, [FromBody] TaskModel task)
        {
            if (taskId != task.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidTaskId);
            }
            if (courseId != task.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _taskRepository.Update(task);

                return result != null 
                    ? Request.CreateResponse(HttpStatusCode.OK, result.ToJson()) 
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // DELETE: api/Courses/{courseId}/Tasks/{taskId}
        [HttpDelete]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanDeleteTasks)]
        public async Task<HttpResponseMessage> Delete(int courseId, int taskId)
        {
            try
            {
                var existingTask = await _taskRepository.Get(taskId);
                if (existingTask == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingTask.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var result = await _taskRepository.Delete(taskId);
                return Request.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}