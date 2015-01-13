using System.Web.Http;
using Microsoft.Practices.Unity;
using GraderDataAccessLayer;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Repositories;

namespace GraderApi
{
    public static class UnityConfig
    {
        public static void RegisterComponents(DatabaseContext context)
        {
			var container = new UnityContainer();
            container.RegisterType<ICourseRepository, CourseRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ISessionIdRepository, SessionIdRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAdminRepository, AdminRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICourseUserRepository, CourseUserRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEntityRepository, EntityRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGradeComponentRepository, GradeComponentRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISubmissionRepository, SubmissionRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITaskRepository, TaskRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IUserRepository, UserRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFileRepository, FileRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGradeRepository, GradeRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IExtensionRepository, ExtensionRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IExcuseRepository, ExcuseRepository>(new HierarchicalLifetimeManager());

            //var context = new DatabaseContext();

            var courseRepository = new CourseRepository(context);
            container.RegisterInstance<ICourseRepository>(courseRepository);

            var sessionIdRepository = new SessionIdRepository(context);
            container.RegisterInstance<ISessionIdRepository>(sessionIdRepository);

            var courseUserRepository = new CourseUserRepository(context);
            container.RegisterInstance<ICourseUserRepository>(courseUserRepository);

            var adminRepository = new AdminRepository(context);
            container.RegisterInstance<IAdminRepository>(adminRepository);

            var entityRepository = new EntityRepository(context);
            container.RegisterInstance<IEntityRepository>(entityRepository);

            var gradeComponentRepository = new GradeComponentRepository(context);
            container.RegisterInstance<IGradeComponentRepository>(gradeComponentRepository);

            var submissionRepository = new SubmissionRepository(context);
            container.RegisterInstance<ISubmissionRepository>(submissionRepository);

            var taskRepository = new TaskRepository(context);
            container.RegisterInstance<ITaskRepository>(taskRepository);

            var userRepository = new UserRepository(context);
            container.RegisterInstance<IUserRepository>(userRepository);

            var fileRepository = new FileRepository(context);
            container.RegisterInstance<IFileRepository>(fileRepository);

            var gradeRepository = new GradeRepository(context);
            container.RegisterInstance<IGradeRepository>(gradeRepository);

            var extensionRepository = new ExtensionRepository(context);
            container.RegisterInstance<IExtensionRepository>(extensionRepository);

            var excuseRepository = new ExcuseRepository(context);
            container.RegisterInstance<IExcuseRepository>(excuseRepository);

            GlobalConfiguration.Configuration.DependencyResolver = new UnityResolver(container);
        }
    }
}