namespace GraderApi
{
    using System;
    using System.Web;
    using GraderDataAccessLayer;
    using Microsoft.Practices.Unity;
    using System.Web.Http;
    using Services;

    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container.RegisterType<UnitOfWork>();
            container.RegisterInstance(new UnitOfWork());

            container.RegisterType<Logger>(new ContainerControlledLifetimeManager());
            container.RegisterInstance(new Logger());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityResolver(container);
        }
    }
}