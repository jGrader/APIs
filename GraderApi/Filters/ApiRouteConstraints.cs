namespace GraderApi.Filters
{
    using GraderDataAccessLayer;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http.Routing;

    public class ApiRouteConstraints : IHttpRouteConstraint, IDisposable
    {
        private UnitOfWork _unitOfWork;

        public ApiRouteConstraints()
        {
            _unitOfWork = new UnitOfWork();
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

            switch (parameterName)
            {
                case "userId":
                {
                    int userId;
                    if (!int.TryParse(stringValue, out userId))
                    {
                        //Validate that userId is truly an integer number
                        return false;
                    }

                    // GetByUserId the user with that Id; if null, then route is invalid
                    var result = Task.Run(() => _unitOfWork.UserRepository.Get(userId));
                    Task.WaitAll(result);
                    return (result.Result != null);
                }
                case "courseId":
                {
                    int courseId;
                    if (!int.TryParse(stringValue, out courseId))
                    {
                        // Validate that the courseId is truly an integer number
                        return false;
                    }

                    // GetByUserId the course with that Id; if null, then route is invalid
                    var result = Task.Run(() => _unitOfWork.CourseRepository.Get(courseId));
                    Task.WaitAll(result); // That is an async operation in a synchronous environment - 'await' for it
                    return (result.Result != null);
                }
                case "courseUserId":
                {
                    int courseUserId;
                    if (!int.TryParse(stringValue, out courseUserId))
                    {
                        // Validate that the courseUserId is truly an integer number
                        return false;
                    }

                    //GetByUserId the courseUserwith that Id; if null, then route is invalid
                    var result = Task.Run(() => _unitOfWork.CourseUserRepository.Get(courseUserId));
                    Task.WaitAll(result);
                    return (result.Result != null);
                }
                case "gradeComponentId":
                {
                    int gradeComponentId;
                    if (!int.TryParse(stringValue, out gradeComponentId))
                    {
                        // Validate that the gradeComponentId is truly an integer number
                        return false;
                    }

                    // GetByUserId the gradeComponent with that Id; if null, then route is invalid
                    var result = Task.Run(() => _unitOfWork.GradeComponentRepository.Get(gradeComponentId));
                    Task.WaitAll(result); // That is an async operation in a synchronous environment - 'await' for it
                    return (result.Result != null);
                }
                case "taskId":
                {
                    int taskId;
                    if (!int.TryParse(stringValue, out taskId))
                    {
                        return false;
                    }

                    //
                    var result = Task.Run(() => _unitOfWork.TaskRepository.Get(taskId));
                    Task.WaitAll(result);
                    return (result.Result != null);
                }
                case "entityId":
                {
                    int entityId;
                    if (!int.TryParse(stringValue, out entityId))
                    {
                        return false;
                    }

                    //
                    var result = Task.Run(() => _unitOfWork.EntityRepository.Get(entityId));
                    Task.WaitAll(result);
                    return (result.Result != null);
                }
                case "submissionId":
                {
                    int submissionId;
                    if (!int.TryParse(stringValue, out submissionId))
                    {
                        return false;
                    }

                    //
                    var result = Task.Run(() => _unitOfWork.SubmissionRepository.Get(submissionId));
                    Task.WaitAll(result);
                    return (result.Result != null);
                }
                case "fileId":
                {
                    int fileId;
                    if (!int.TryParse(stringValue, out fileId))
                    {
                        return false;
                    }

                    //
                    var result = Task.Run(() => _unitOfWork.FileRepository.Get(fileId));
                    Task.WaitAll(result);
                    return (result.Result != null);
                }
                case "teamId":
                {
                    int teamId;
                    if (!int.TryParse(stringValue, out teamId))
                    {
                        return false;
                    }

                    //
                    var result = Task.Run(() => _unitOfWork.TeamRepository.Get(teamId));
                    Task.WaitAll(result);
                    return (result.Result != null);
                }
                case "gradeId":
                {
                    int gradeId;
                    if (!int.TryParse(stringValue, out gradeId))
                    {
                        return false;
                    }

                    //
                    var result = Task.Run(() => _unitOfWork.GradeRepository.Get(gradeId));
                    Task.WaitAll(result);
                    return (result.Result != null);
                }
                case "extensionId":
                {
                    int extensionId;
                    if (!int.TryParse(stringValue, out extensionId))
                    {
                        return false;
                    }

                    //
                    var result = Task.Run(() => _unitOfWork.ExtensionRepository.Get(extensionId));
                    Task.WaitAll(result);
                    return (result.Result != null);
                }
                case "excuseId":
                {
                    int excuseId;
                    if (!int.TryParse(stringValue, out excuseId))
                    {
                        return false;
                    }

                    //
                    var result = Task.Run(() => _unitOfWork.ExcuseRepository.Get(excuseId));
                    Task.WaitAll(result);
                    return (result.Result != null);
                }
            }

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
            _unitOfWork.Dispose();
        }
        #endregion
    }
}