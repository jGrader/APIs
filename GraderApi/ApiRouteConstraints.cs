using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using System.Collections.Generic;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Repositories;


namespace GraderApi.Constraints
{
    public class ApiRouteConstraints : IHttpRouteConstraint, IDisposable
    {
        private ICourseRepository _courseRepository ;
        private IGradeComponentRepository _gradeComponentRepository;

        public ApiRouteConstraints()
        {
            _courseRepository = new CourseRepository();
            _gradeComponentRepository = new GradeComponentRepository();
        }
        
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (!values.ContainsKey(parameterName)) { // The parameter checked for is not present
                return false;
            }

            var stringValue = values[parameterName] as string;

            switch (parameterName)
            {
                case "courseId":
                    {
                        int courseId;
                        if (!int.TryParse(stringValue, out courseId)) { // Validate that the courseId is truly an integer number
                            return false;
                        }

                        // Get the course with that Id; if null, then route is invalid
                        var result = Task.Run(() => _courseRepository.Get(courseId));
                        Task.WaitAll(result); // That is an async operation in a synchronous environment - 'await' for it
                        return (result.Result != null);
                    } 
                case "gradeComponentId":
                    {
                        int gradeComponentId;
                        if (!int.TryParse(stringValue, out gradeComponentId)) { // Validate that the gradeComponentId is truly an integer number
                            return false;
                        }

                        // Get the gradeComponent with that Id; if null, then route is invalid
                        var result = Task.Run(() => _gradeComponentRepository.Get(gradeComponentId));
                        Task.WaitAll(result); // That is an async operation in a synchronous environment - 'await' for it
                        return (result != null);
                    }
            }

            return false;
        }

        public void Dispose()
        {
            DisposeCourseRepository(true);
            DisposeGradeComponentRepository(true);
            GC.SuppressFinalize(this);
        }
        private void DisposeCourseRepository(bool disposing)
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
        private void DisposeGradeComponentRepository(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            if (_gradeComponentRepository == null)
            {
                return;
            }

            _gradeComponentRepository.Dispose();
            _gradeComponentRepository = null;
        }
    }
}