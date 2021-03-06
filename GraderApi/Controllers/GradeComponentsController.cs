﻿namespace GraderApi.Controllers
{
    using Filters;
    using Grader.ExtensionMethods;
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Models;
    using Resources;
    using Services;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class GradeComponentsController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Logger _logger;

        public GradeComponentsController(UnitOfWork unitOfWork, Logger log)
        {
            _logger = log;
            _unitOfWork = unitOfWork;
        }

        // GET: api/GradeComponents/All
        [HttpGet]
        [PermissionsAuthorize(SuperUserPermissions.CanSeeAllGradedParts)]
        public async Task<HttpResponseMessage> All()
        {
            try
            {
                var result = await _unitOfWork.GradeComponentRepository.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/GradeComponents/All
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeGradedParts)]
        public async Task<HttpResponseMessage> All(int courseId)
        {
            try
            {
                var result = await _unitOfWork.GradeComponentRepository.GetByCourseId(courseId);
                return Request.CreateResponse(HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // GET: api/Courses/{courseId}/GradeComponents/{gradeComponentId}
        [HttpGet]
        [ValidateModelState]
        [PermissionsAuthorize(CoursePermissions.CanSeeGradedParts)]
        public async Task<HttpResponseMessage> Get(int courseId, int gradeComponentId)
        {
            try
            {
                var gradeComponent = await _unitOfWork.GradeComponentRepository.Get(gradeComponentId);
                if (gradeComponent == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);   
                }

                return gradeComponent.CourseId == courseId
                    ? Request.CreateResponse(HttpStatusCode.OK, gradeComponent.ToJson())
                    : Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // POST: api/Courses/{courseId}/GradeComponents
        [HttpPost]
        [ValidateModelState]
        [PermissionsAuthorize(CourseOwnerPermissions.CanCreateGradedPart)]
        public async Task<HttpResponseMessage> Add(int courseId, [FromBody] GradeComponentModel gradeComponent)
        {
            if (courseId != gradeComponent.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _unitOfWork.GradeComponentRepository.Add(gradeComponent);
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

        // PUT: api/Courses/{courseId}/GradeComponents/{gradeComponentId}
        [HttpPut]
        [ValidateModelState]
        [PermissionsAuthorize(CourseOwnerPermissions.CanUpdateGradedPart)]
        public async Task<HttpResponseMessage> Update(int courseId, int gradeComponentId, [FromBody] GradeComponentModel gradeComponent)
        {
            if (gradeComponentId != gradeComponent.Id) 
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidGradeComponentId);
            }
            if (courseId != gradeComponent.CourseId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
            }

            try
            {
                var result = await _unitOfWork.GradeComponentRepository.Update(gradeComponent);

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

        // DELETE: api/Courses/{courseId}/GradeComponents/{gradeComponentId}
        [HttpDelete]
        [ValidateModelState]
        [PermissionsAuthorize(CourseOwnerPermissions.CanDeleteGradedPart)]
        public async Task<HttpResponseMessage> Delete(int courseId, int gradeComponentId)
        {
            try
            {
                var existingGradeComponent = await _unitOfWork.GradeComponentRepository.Get(gradeComponentId);
                if (existingGradeComponent == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (existingGradeComponent.CourseId != courseId)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidCourse);
                }

                var result = await _unitOfWork.GradeComponentRepository.Delete(gradeComponentId);
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