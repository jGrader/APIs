﻿namespace GraderDataAccessLayer
{
    using Repositories;
    using System;

    public class UnitOfWork : IDisposable
    {
        public DatabaseContext Context;
        private AdminRepository _adminRepository;
        private CourseRepository _courseRepository;
        private CourseUserRepository _courseUserRepository;
        private EntityRepository _entityRepository;
        private ExcuseRepository _excuseRepository;
        private ExtensionRepository _extensionRepository;
        private FileRepository _fileRepository;
        private GradeComponentRepository _gradeComponentRepository;
        private GradeRepository _gradeRepository;
        private SessionIdRepository _sessionIdRepository;
        private SubmissionRepository _submissionRepository;
        private TaskRepository _taskRepository;
        private TeamRepository _teamRepository;
        private UserRepository _userRepository;

        public UnitOfWork()
        {
            Context = new DatabaseContext();
        }

        public AdminRepository AdminRepository
        {
            get
            {
                _adminRepository = _adminRepository ?? new AdminRepository(Context);

                return _adminRepository;
            }
        }

        public CourseRepository CourseRepository
        {
            get
            {
                _courseRepository = _courseRepository ?? new CourseRepository(Context);

                return _courseRepository;
            }
        }

        public CourseUserRepository CourseUserRepository
        {
            get
            {
                _courseUserRepository = _courseUserRepository ?? new CourseUserRepository(Context);

                return _courseUserRepository;
            }
        }

        public EntityRepository EntityRepository
        {
            get
            {
                _entityRepository = _entityRepository ?? new EntityRepository(Context);
                return _entityRepository;
            }
        }

        public ExcuseRepository ExcuseRepository
        {
            get
            {
                _excuseRepository = _excuseRepository ?? new ExcuseRepository(Context);
                return _excuseRepository;
            }
        }

        public ExtensionRepository ExtensionRepository
        {
            get
            {
                _extensionRepository = _extensionRepository ?? new ExtensionRepository(Context);
                return _extensionRepository;
            }
        }

        public FileRepository FileRepository
        {
            get
            {
                _fileRepository = _fileRepository ?? new FileRepository(Context);
                return _fileRepository;
            }
        }

        public GradeComponentRepository GradeComponentRepository
        {
            get
            {
                _gradeComponentRepository = _gradeComponentRepository ?? new GradeComponentRepository(Context);
                return _gradeComponentRepository;
            }
        }

        public GradeRepository GradeRepository
        {
            get
            {
                _gradeRepository = _gradeRepository ?? new GradeRepository(Context);
                return _gradeRepository;
            }
        }

        public SessionIdRepository SessionIdRepository
        {
            get
            {
                _sessionIdRepository = _sessionIdRepository ?? new SessionIdRepository(Context);
                return _sessionIdRepository;
            }
        }

        public SubmissionRepository SubmissionRepository
        {
            get
            {
                _submissionRepository = _submissionRepository ?? new SubmissionRepository(Context);
                return _submissionRepository;
            }
        }

        public TaskRepository TaskRepository
        {
            get
            {
                _taskRepository = _taskRepository ?? new TaskRepository(Context);
                return _taskRepository;
            }
        }

        public TeamRepository TeamRepository
        {
            get
            {
                _teamRepository = _teamRepository ?? new TeamRepository(Context);
                return _teamRepository;
            }
        }

        public UserRepository UserRepository
        {
            get
            {
                _userRepository = _userRepository ?? new UserRepository(Context);
                return _userRepository;
            }
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

            DisposeAdminRepository();
            DisposeCourseRepository();
            DisposeCourseUserRepository();
            DisposeEntityRepository();
            DisposeExcuseRepository();
            DisposeExtensionRepository();
            DisposeFileRepository();
            DisposeGradeComponentRepository();
            DisposeGradeRepository();
            DisposeSessionIdRepository();
            DisposeSubmissionRepository();
            DisposeTaskRepository();
            DisposeTeamRepository();
            DisposeUserRepository();

            if (Context == null)
            {
                return;
            }

            Context.Dispose();
            Context = null;
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

        private void DisposeTeamRepository()
        {
            if (_teamRepository == null)
            {
                return;
            }

            _teamRepository.Dispose();
            _teamRepository = null;
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

        private void DisposeSubmissionRepository()
        {
            if (_submissionRepository == null)
            {
                return;
            }

            _submissionRepository.Dispose();
            _submissionRepository = null;
        }

        private void DisposeSessionIdRepository()
        {
            if (_sessionIdRepository == null)
            {
                return;
            }

            _sessionIdRepository.Dispose();
            _sessionIdRepository = null;
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

        private void DisposeGradeComponentRepository()
        {
            if (_gradeComponentRepository == null)
            {
                return;
            }

            _gradeComponentRepository.Dispose();
            _gradeComponentRepository = null;
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

        private void DisposeEntityRepository()
        {
            if (_entityRepository == null)
            {
                return;
            }

            _entityRepository.Dispose();
            _entityRepository = null;
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

        private void DisposeCourseRepository()
        {
            if (_courseRepository == null)
            {
                return;
            }

            _courseRepository.Dispose();
            _courseRepository = null;
        }

        private void DisposeAdminRepository()
        {
            if (_adminRepository == null)
            {
                return;
            }

            _adminRepository.Dispose();
            _adminRepository = null;
        }
    }
}
