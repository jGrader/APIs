﻿namespace GraderApi.Controllers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Description;

    using GraderApi.Models;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Repositories;

    using Grader.JsonSerializer;

    public class CoursesController : ApiController
    {
        private readonly CourseRepository _repository;

        public CoursesController(CourseRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Courses
        public HttpResponseMessage GetCourses()
        {
            var courses =  _repository.GetAll();
            return Request.CreateResponse(HttpStatusCode.Accepted, courses.ToJson());
        }

        // GET: api/Courses/5
        public HttpResponseMessage GetCourse(int id)
        {
            var course = _repository.Get(id);
            if (course == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, course);
        }

        // PUT: api/Courses/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCourse(int id, CourseModel course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != course.Id)
            {
                return BadRequest();
            }

            var result = await _repository.Update(course);
            return StatusCode(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
        }

        // POST: api/Courses
        [ResponseType(typeof(CourseModel))]
        public async Task<IHttpActionResult> PostCourse(CourseModel course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _repository.Add(course);

            if (!result)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return CreatedAtRoute("DefaultApi", new {id = course.Id}, course);
        }

        // DELETE: api/Courses/5
        [ResponseType(typeof(CourseModel))]
        public async Task<IHttpActionResult> DeleteCourse(int id)
        {
            var result = await _repository.Remove(id);

            return StatusCode(!result ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}