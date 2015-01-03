using System;
using System.Net.Http;
using System.Web.Http.Routing;
using System.Collections.Generic;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Repositories;


namespace GraderApi.Constraints
{
    public class CourseConstraint : IHttpRouteConstraint, IDisposable
    {
        private ICourseRepository _courseRepository ;

        public CourseConstraint()
        {
            _courseRepository = new CourseRepository();
        }
        
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (!values.ContainsKey(parameterName)) { // The parameter 'courseId' is not present
                return false;
            }

            var stringValue = values[parameterName] as string;
            int courseId;
            if (!int.TryParse(stringValue, out courseId)) { // Validate that the courseId is truly an integer number
                return false;
            }

            // Get the course with that Id; if null, then route is invalid
            var result = _courseRepository.Get(courseId);
            return (result != null);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            if (_courseRepository == null)
            {
                return;
            }

            _courseRepository.Dispose();
            _courseRepository = null;
        }
    }
}