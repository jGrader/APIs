using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Repositories;

namespace GraderApi.Constraints
{
    public class CourseConstraint : IHttpRouteConstraint
    {
        private readonly ICourseUserRepository _courseUserRepository ;

        public CourseConstraint()
        {
            _courseUserRepository = new CourseUserRepository();
        }
        
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values,
                          HttpRouteDirection routeDirection)
        {
            if (values.ContainsKey(parameterName))
            {
                var stringValue = values[parameterName] as string;
                int courseId;
                if (!int.TryParse(stringValue, out courseId))
                {
                    return false;
                }
                var result = _courseUserRepository.GetByCourseId(courseId);
                return (result != null);

            }

            return false;
        }
    }

}