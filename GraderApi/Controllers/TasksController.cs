namespace GraderApi.Controllers
{
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;


    public class TasksController : ApiController
    {
        private readonly ITaskRepository _taskRepository;
        public TasksController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        // GET: api/Tasks/All
        [HttpGet]
        [ResponseType(typeof(IEnumerable<TaskModel>))]
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
        [ResponseType(typeof(IEnumerable<TaskModel>))]
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
        [ResponseType(typeof(TaskModel))]
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
        [ResponseType(typeof (GradeComponentModel))]
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
        [ResponseType(typeof(TaskModel))]
        [PermissionsAuthorize(CoursePermissions.CanCreateTasks)]
        public async Task<HttpResponseMessage> Add(int courseId, [FromBody] TaskModel task)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
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
        [ResponseType(typeof(TaskModel))]
        [PermissionsAuthorize(CoursePermissions.CanUpdateTasks)]
        public async Task<HttpResponseMessage> Update(int courseId, int taskId, [FromBody] TaskModel task)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
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
        [ResponseType(typeof(void))]
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