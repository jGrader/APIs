﻿namespace GraderApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.UI;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;
    using Principals;

    public class CurrentUserController : ApiController
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseUserRepository _courseUserRepository;
        private readonly IEntityRepository _entityRepository;
        private readonly IFileRepository _fileRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly UserModel _user;

        public CurrentUserController(
            ICourseRepository courseRepository,
            ICourseUserRepository coureUserRepository,
            IEntityRepository entityRepository,
            IFileRepository fileRepository,
            ISubmissionRepository submissionRepository,
            ITaskRepository taskRepository
            )
        {
            _courseRepository = courseRepository;
            _courseUserRepository = coureUserRepository;
            _entityRepository = entityRepository;
            _fileRepository = fileRepository;
            _submissionRepository = submissionRepository;
            _taskRepository = taskRepository;

            var principal = HttpContext.Current.User as UserPrincipal;
            if (principal == null)
            {
                throw new NullReferenceException("Something went wrong");
            }
            _user = principal.User;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> Courses()
        {
            try
            {
                var courses = await _courseUserRepository.GetAllByUser(_user.Id);
                var tasks = (from c in courses select _courseRepository.Get(c.CourseId));
                var result = await Task.WhenAll(tasks);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());

            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpGet]
        public async Task<HttpResponseMessage> Submissions(int courseId)
        {
            try
            {
                var submissions = await _submissionRepository.GetAllByUserId(_user.Id);
                return Request.CreateResponse(HttpStatusCode.OK, submissions.ToJson());

            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
