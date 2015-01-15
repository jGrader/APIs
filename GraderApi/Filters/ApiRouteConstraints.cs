﻿namespace GraderApi.Filters
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
        private IUserRepository _userRepository;
        private ICourseRepository _courseRepository;
        private ICourseUserRepository _courseUserRepository;
        private IGradeComponentRepository _gradeComponentRepository;
        private ITaskRepository _taskRepository;
        private IEntityRepository _entityRepository;
        private IFileRepository _fileRepository;
        private ISubmissionRepository _submissionRepository;
        private ITeamRepository _teamRepository;
        private IGradeRepository _gradeRepository;
        private IExtensionRepository _extensionRepository;
        private IExcuseRepository _excuseRepository;

        public ApiRouteConstraints(DatabaseContext context)
        {
            _userRepository = new UserRepository(context);
            _courseRepository = new CourseRepository(context);
            _courseUserRepository = new CourseUserRepository(context);
            _gradeComponentRepository = new GradeComponentRepository(context);
            _taskRepository = new TaskRepository(context);
            _entityRepository = new EntityRepository(context);
            _fileRepository = new FileRepository(context);
            _submissionRepository = new SubmissionRepository(context);
            _teamRepository = new TeamRepository(context);
            _gradeRepository = new GradeRepository(context);
            _extensionRepository = new ExtensionRepository(context);
            _excuseRepository = new ExcuseRepository(context);
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

                    // Get the user with that Id; if null, then route is invalid
                    var result = Task.Run(() => _userRepository.Get(userId));
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

                    // Get the course with that Id; if null, then route is invalid
                    var result = Task.Run(() => _courseRepository.Get(courseId));
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

                    //Get the courseUserwith that Id; if null, then route is invalid
                    var result = Task.Run(() => _courseUserRepository.Get(courseUserId));
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

                    // Get the gradeComponent with that Id; if null, then route is invalid
                    var result = Task.Run(() => _gradeComponentRepository.Get(gradeComponentId));
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
                    var result = Task.Run(() => _taskRepository.Get(taskId));
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
                    var result = Task.Run(() => _entityRepository.Get(entityId));
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
                    var result = Task.Run(() => _submissionRepository.Get(submissionId));
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
                    var result = Task.Run(() => _fileRepository.Get(fileId));
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
                    var result = Task.Run(() => _teamRepository.Get(teamId));
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
                    var result = Task.Run(() => _gradeRepository.Get(gradeId));
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
                    var result = Task.Run(() => _extensionRepository.Get(extensionId));
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
                    var result = Task.Run(() => _excuseRepository.Get(excuseId));
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
            DisposeUserRepository();
            DisposeCourseRepository();
            DisposeCourseUserRepository();
            DisposeGradeComponentRepository();
            DisposeTaskRepository();
            DisposeEntityRepository();
            DisposeFileRepository();
            DisposeSubmissionRepository();
            DisposeTeamRepository();
            DisposeGradeRepository();
            DisposeExtensionRepository();
            DisposeExcuseRepository();
        }

        private void DisposeGradeRepository()
        {
            if (_gradeRepository == null)
            {
                return;
            }

            _gradeRepository.Dispose();
            _gradeRepository = null;
        }
        private void DisposeUserRepository()
        {
            if (_userRepository == null)
            {
                return;
            }

            _userRepository.Dispose();
            _userRepository = null;
        }
        private void DisposeCourseRepository()
        {
            if (_courseRepository == null)
            {
                return;
            }

            _courseRepository.Dispose();
            _courseRepository = null;
        }
        private void DisposeCourseUserRepository()
        {
            if (_courseUserRepository == null)
            {
                return;
            }

            _courseUserRepository.Dispose();
            _courseUserRepository = null;
        }
        private void DisposeGradeComponentRepository()
        {
            if (_gradeComponentRepository == null)
            {
                return;
            }

            _gradeComponentRepository.Dispose();
            _gradeComponentRepository = null;
        }
        private void DisposeTaskRepository()
        {
            if (_taskRepository == null)
            {
                return;
            }

            _taskRepository.Dispose();
            _taskRepository = null;
        }
        private void DisposeEntityRepository()
        {
            if (_entityRepository == null)
            {
                return;
            }

            _entityRepository.Dispose();
            _entityRepository = null;
        }
        private void DisposeFileRepository()
        {
            if (_fileRepository == null)
            {
                return;
            }

            _fileRepository.Dispose();
            _fileRepository = null;
        }
        private void DisposeSubmissionRepository()
        {
            if (_submissionRepository == null)
            {
                return;
            }

            _submissionRepository.Dispose();
            _submissionRepository = null;
        }
        private void DisposeTeamRepository()
        {
            if (_teamRepository == null)
            {
                return;
            }

            _teamRepository.Dispose();
            _teamRepository = null;
        }
        private void DisposeExtensionRepository()
        {
            if (_extensionRepository == null)
            {
                return;
            }

            _extensionRepository.Dispose();
            _extensionRepository = null;
        }
        private void DisposeExcuseRepository()
        {
            if (_excuseRepository == null)
            {
                return;
            }

            _excuseRepository.Dispose();
            _excuseRepository = null;
        }
        #endregion
    }
}