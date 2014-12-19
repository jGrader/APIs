using Unity.WebApi;
using System.Web.Http;
using Microsoft.Practices.Unity;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Repositories;


namespace GraderApi
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            container.RegisterType<ICourseRepository, CourseRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ISessionIdRepository, SessionIdRepository>(new ContainerControlledLifetimeManager());

            var courseRepository = new CourseRepository();
            container.RegisterInstance<ICourseRepository>(courseRepository);

            var sessionIdRepository = new SessionIdRepository();
            container.RegisterInstance<ISessionIdRepository>(sessionIdRepository);

            var courseUserRepository = new CourseUserRepository();
            container.RegisterInstance<ICourseUserRepository>(courseUserRepository);

            GlobalConfiguration.Configuration.DependencyResolver = new UnityResolver(container);
        }
    }
}