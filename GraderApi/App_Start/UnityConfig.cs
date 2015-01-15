namespace GraderApi
{
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Repositories;
    using Microsoft.Practices.Unity;
    using System.Web.Http;

    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            container.RegisterType<ICourseRepository, CourseRepository>();
            container.RegisterType<ISessionIdRepository, SessionIdRepository>();
            container.RegisterType<IAdminRepository, AdminRepository>();
            container.RegisterType<ICourseUserRepository, CourseUserRepository>();
            container.RegisterType<IEntityRepository, EntityRepository>();
            container.RegisterType<IGradeComponentRepository, GradeComponentRepository>();
            container.RegisterType<ISubmissionRepository, SubmissionRepository>();
            container.RegisterType<ITaskRepository, TaskRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IFileRepository, FileRepository>();
            container.RegisterType<IGradeRepository, GradeRepository>();
            container.RegisterType<IExtensionRepository, ExtensionRepository>();
            container.RegisterType<IExcuseRepository, ExcuseRepository>();

            //var context = new DatabaseContext();

            var courseRepository = new CourseRepository();
            container.RegisterInstance<ICourseRepository>(courseRepository);

            var sessionIdRepository = new SessionIdRepository();
            container.RegisterInstance<ISessionIdRepository>(sessionIdRepository);

            var courseUserRepository = new CourseUserRepository();
            container.RegisterInstance<ICourseUserRepository>(courseUserRepository);

            var adminRepository = new AdminRepository();
            container.RegisterInstance<IAdminRepository>(adminRepository);

            var entityRepository = new EntityRepository();
            container.RegisterInstance<IEntityRepository>(entityRepository);

            var gradeComponentRepository = new GradeComponentRepository();
            container.RegisterInstance<IGradeComponentRepository>(gradeComponentRepository);

            var submissionRepository = new SubmissionRepository();
            container.RegisterInstance<ISubmissionRepository>(submissionRepository);

            var taskRepository = new TaskRepository();
            container.RegisterInstance<ITaskRepository>(taskRepository);

            var userRepository = new UserRepository();
            container.RegisterInstance<IUserRepository>(userRepository);

            var fileRepository = new FileRepository();
            container.RegisterInstance<IFileRepository>(fileRepository);

            var gradeRepository = new GradeRepository();
            container.RegisterInstance<IGradeRepository>(gradeRepository);

            var extensionRepository = new ExtensionRepository();
            container.RegisterInstance<IExtensionRepository>(extensionRepository);

            var excuseRepository = new ExcuseRepository();
            container.RegisterInstance<IExcuseRepository>(excuseRepository);

            GlobalConfiguration.Configuration.DependencyResolver = new UnityResolver(container);
        }
    }
}