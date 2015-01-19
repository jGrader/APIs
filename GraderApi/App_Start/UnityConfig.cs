namespace GraderApi
{
    using GraderDataAccessLayer;
    using Microsoft.Practices.Unity;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Dependencies;

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

    public class UnityResolver : IDependencyResolver
    {
        protected IUnityContainer container;

        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new UnityResolver(child);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            if (container == null)
            {
                return;
            }

            container.Dispose();
            container = null;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }   
}