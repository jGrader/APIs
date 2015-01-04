namespace GraderApi.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Repositories;


    public class TasksController : ApiController
    {
        private readonly TaskRepository _taskRepository;

        public TasksController(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        // GET: api/Tasks
        [ResponseType(typeof(IEnumerable<TaskModel>))]
        public async Task<HttpResponseMessage> GetTask()
        {
            var result = await _taskRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // GET: api/Tasks/5
        [ResponseType(typeof(TaskModel))]
        public async Task<HttpResponseMessage> GetTask(int taskId)
        {
            var task = await _taskRepository.Get(taskId);
            if (task == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, task.ToJson());
        }

        // POST: api/Tasks
        [ResponseType(typeof(TaskModel))]
        public async Task<IHttpActionResult> PostTask(TaskModel task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _taskRepository.Add(task);
            if (result == null)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return CreatedAtRoute("TaskRoute", new { taskId = result.Id }, result.ToJson());
        }

        // PUT: api/Tasks/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTask(int taskId, TaskModel task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (taskId != task.Id)
            {
                return BadRequest();
            }

            var result = await _taskRepository.Update(task);
            if (result == null)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // DELETE: api/Tasks/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteTask(int taskId)
        {
            var result = await _taskRepository.Delete(taskId);
            return StatusCode(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _taskRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}