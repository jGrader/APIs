namespace GraderApi.Controllers
{
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Repositories;
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

        // GET: api/Tasks
        [HttpGet]
        [ResponseType(typeof(IEnumerable<TaskModel>))]
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

        // GET: api/Tasks/5
        [HttpGet]
        [ResponseType(typeof(TaskModel))]
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

        // GET: api/Tasks/GetGradeComponent/3
        [HttpGet]
        [ResponseType(typeof (GradeComponentModel))]
        public async Task<HttpResponseMessage> GetGradeComponent(int taskId)
        {
            try
            {
                var task = await _taskRepository.Get(taskId);

                return task != null
                    ? Request.CreateResponse(HttpStatusCode.OK, task.GradeComponent.ToJson())
                    : Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Tasks/GetCourse/3
        [HttpGet]
        [ResponseType(typeof(CourseModel))]
        public async Task<HttpResponseMessage> GetCourse(int taskId)
        {
            try
            {
                var task = await _taskRepository.Get(taskId);

                return task != null
                    ? Request.CreateResponse(HttpStatusCode.OK, task.Course.ToJson())
                    : Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Tasks
        [HttpPost]
        [ResponseType(typeof(TaskModel))]
        [PermissionsAuthorize(CoursePermissions.CanCreateTasks)]
        public async Task<HttpResponseMessage> Add([FromBody] TaskModel task)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
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

        // PUT: api/Tasks/5
        [HttpPut]
        [ResponseType(typeof(TaskModel))]
        [PermissionsAuthorize(CoursePermissions.CanUpdateTasks)]
        public async Task<HttpResponseMessage> Update(int taskId, [FromBody] TaskModel task)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (taskId != task.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
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

        // DELETE: api/Tasks/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        [PermissionsAuthorize(CoursePermissions.CanDeleteTasks)]
        public async Task<HttpResponseMessage> Delete(int taskId)
        {
            try
            {
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