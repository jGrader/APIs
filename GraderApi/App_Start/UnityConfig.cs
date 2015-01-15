namespace GraderApi
{
    using GraderDataAccessLayer;
    using Microsoft.Practices.Unity;
    using System.Web.Http;

    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container.RegisterType<UnitOfWork>();
            container.RegisterInstance(new UnitOfWork());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityResolver(container);
        }
    }
}