namespace GraderApi.Controllers
{
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Repositories;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;


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

        // GET: api/Tasks/GetGradeComponent/3
        [HttpGet]
        [ResponseType(typeof (GradeComponentModel))]
        public async Task<HttpResponseMessage> GetGradeComponent(int taskId)
        {
            var task = await _taskRepository.Get(taskId);
            if (task == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, task.GradeComponent.ToJson());
        }

        // GET: api/Tasks/GetCourse/3
        [HttpGet]
        [ResponseType(typeof(CourseModel))]
        public async Task<HttpResponseMessage> GetCourse(int taskId)
        {
            var task = await _taskRepository.Get(taskId);
            if (task == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, task.Course.ToJson());
        }

        // POST: api/Tasks
        [HttpPost]
        [ResponseType(typeof(TaskModel))]
        public async Task<HttpResponseMessage> Add([FromBody]TaskModel task)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var result = await _taskRepository.Add(task);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
        }

        // PUT: api/Tasks/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> Update(int taskId, [FromBody]TaskModel task)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (taskId != task.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var result = await _taskRepository.Update(task);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE: api/Tasks/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> Delete(int taskId)
        {
            var result = await _taskRepository.Delete(taskId);
            return Request.CreateResponse(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
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