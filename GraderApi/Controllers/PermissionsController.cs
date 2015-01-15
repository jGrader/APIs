namespace GraderApi.Controllers
{
    using Filters;
    using Grader.JsonSerializer;
    using GraderDataAccessLayer;
    using Newtonsoft.Json;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class PermissionsController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public PermissionsController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Permissions/All
        [HttpGet]
        [PermissionsAuthorize(CourseOwnerPermissions.CanGrantPermissions)]
        public HttpResponseMessage All()
        {
            var allGrantablePermissions = Enum.GetNames(typeof(CoursePermissions));
            return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(allGrantablePermissions));
        }

        // POST: api/Courses/{courseId}/Permissions/Set/{userId}
        [HttpPost]
        [ValidateModelState]
        [PermissionsAuthorize(CourseOwnerPermissions.CanGrantPermissions)]
        public async Task<HttpResponseMessage> Set(int courseId, int userId, [FromBody] IEnumerable<string> permissions)
        {
            var enrollment = await _unitOfWork.CourseUserRepository.GetByExpression(cu => cu.CourseId == courseId && cu.UserId == userId);
            var firstOrDefault = enrollment.FirstOrDefault();
            if (firstOrDefault == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.InvalidEnrollment);
            }

            try
            {
                // SET THE PERMISSIONS
                firstOrDefault.Permissions = GetUserPermissions(permissions);
                var result = await _unitOfWork.CourseUserRepository.Update(firstOrDefault);

                return result != null
                    ? Request.CreateResponse(HttpStatusCode.OK, firstOrDefault.ToJson())
                    : Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        private static int GetUserPermissions(IEnumerable<string> permissions)
        {
            var finalPermissions = Enumerable.Repeat('0', 32).ToArray();
            foreach (var permission in permissions)
            {
                CoursePermissions enumPermission;
                if (!Enum.TryParse(permission, out enumPermission))
                {
                    throw new ArgumentOutOfRangeException("permissions", Messages.PermissionsParseError);
                }

                var currentPermission = Convert.ToString((int)enumPermission, 2);
                finalPermissions[currentPermission.Length - 1] = '1';
            }

            // We have to do .Reverse() because otherwise the 1s will start on the left
            return Convert.ToInt32(new string(finalPermissions.Reverse().ToArray()), 2);
        }
    }
}
