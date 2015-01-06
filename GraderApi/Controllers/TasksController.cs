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
        [HttpGet]
        [ResponseType(typeof(IEnumerable<TaskModel>))]
        public async Task<HttpResponseMessage> All()
        {
            var result = await _taskRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // GET: api/Tasks/5
        [HttpGet]
        [ResponseType(typeof(TaskModel))]
        public async Task<HttpResponseMessage> Get(int taskId)
        {
            var task = await _taskRepository.Get(taskId);
            if (task == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, task.ToJson());
        }

        // POST: api/Tasks
        [HttpPost]
        [ResponseType(typeof(TaskModel))]
        public async Task<IHttpActionResult> Add([FromBody]TaskModel task)
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
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Add(int taskId, [FromBody]TaskModel task)
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
        [HttpDelete]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Delete(int taskId)
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