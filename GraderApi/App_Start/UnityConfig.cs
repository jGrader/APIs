using Unity.WebApi;
using System.Web.Http;
using Microsoft.Practices.Unity;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Repositories;


namespace GraderApi
{
    using Microsoft.AspNet.Identity;

    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            container.RegisterType<ICourseRepository, CourseRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ISessionIdRepository, SessionIdRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAdminRepository, AdminRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICourseUserRepository, CourseUserRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEntityRepository, EntityRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGradeComponentRepository, GradeComponentRepository>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<ISubmissionRepository, SubmissionRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITaskRepository, TaskRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IUserRepository, UserRepository>(new ContainerControlledLifetimeManager());

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

            GlobalConfiguration.Configuration.DependencyResolver = new UnityResolver(container);
        }
    }
}