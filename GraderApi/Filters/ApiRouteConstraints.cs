namespace GraderApi.Filters
{
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Resources;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http.Routing;

    public class ApiRouteConstraints : IHttpRouteConstraint, IDisposable
    {
        private UnitOfWork _unitOfWork;
        private readonly Logger _logger;

        public ApiRouteConstraints()
        {
            _unitOfWork = new UnitOfWork(ConfigurationManager.ConnectionStrings[DatabaseConnections.MySQL].ConnectionString);
            _logger = new Logger();
        }
        
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (!values.ContainsKey(parameterName)) 
            { 
                // The parameter checked for is not present
                return false;
            }

            var stringValue = values[parameterName] as string;
            if (stringValue == null)
            {
                // There is nothing to be checked about the parameters;
                // move forward
                return true;
            }

            int parameterId;
            if (!int.TryParse(stringValue, out parameterId))
            {
                //Validate that stringValue is truly an integer number
                return false;
            }

            try
            {
                // If the result of the Get is null, then route is invalid
                switch (parameterName)
                {
                    case "userId":
                    {

                        var result = Task.Run(() => _unitOfWork.UserRepository.Get(parameterId));
                        Task.WaitAll(result);
                        return (result.Result != null);
                    }
                    case "courseId":
                    {
                        var result = Task.Run(() => _unitOfWork.CourseRepository.Get(parameterId));
                        Task.WaitAll(result);
                        return (result.Result != null);
                    }
                    case "courseUserId":
                    {
                        var result = Task.Run(() => _unitOfWork.CourseUserRepository.Get(parameterId));
                        Task.WaitAll(result);
                        return (result.Result != null);
                    }
                    case "gradeComponentId":
                    {
                        var result = Task.Run(() => _unitOfWork.GradeComponentRepository.Get(parameterId));
                        Task.WaitAll(result);
                        return (result.Result != null);
                    }
                    case "taskId":
                    {
                        var result = Task.Run(() => _unitOfWork.TaskRepository.Get(parameterId));
                        Task.WaitAll(result);
                        return (result.Result != null);
                    }
                    case "entityId":
                    {
                        var result = Task.Run(() => _unitOfWork.EntityRepository.Get(parameterId));
                        Task.WaitAll(result);
                        return (result.Result != null);
                    }
                    case "submissionId":
                    {
                        var result = Task.Run(() => _unitOfWork.SubmissionRepository.Get(parameterId));
                        Task.WaitAll(result);
                        return (result.Result != null);
                    }
                    case "fileId":
                    {
                        var result = Task.Run(() => _unitOfWork.FileRepository.Get(parameterId));
                        Task.WaitAll(result);
                        return (result.Result != null);
                    }
                    case "teamId":
                    {
                        var result = Task.Run(() => _unitOfWork.TeamRepository.Get(parameterId));
                        Task.WaitAll(result);
                        return (result.Result != null);
                    }
                    case "gradeId":
                    {
                        var result = Task.Run(() => _unitOfWork.GradeRepository.Get(parameterId));
                        Task.WaitAll(result);
                        return (result.Result != null);
                    }
                    case "extensionId":
                    {
                        var result = Task.Run(() => _unitOfWork.ExtensionRepository.Get(parameterId));
                        Task.WaitAll(result);
                        return (result.Result != null);
                    }
                    case "excuseId":
                    {
                        var result = Task.Run(() => _unitOfWork.ExcuseRepository.Get(parameterId));
                        Task.WaitAll(result);
                        return (result.Result != null);
                    }
                }
            }
            catch (Exception e)
            {
                // Unexpected error
                _logger.Log(e);
                return false;
            }

            // SOMEONE PROBABLY FORGOT TO UPDATE THIS FILE
            // WITH THE LATEST ADDED ROUTES
            return false;
        }

        #region Dispose
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

            if (_unitOfWork == null)
            {
                return;
            }

            _unitOfWork.Dispose();
            _unitOfWork = null;
        }
        #endregion
    }
}