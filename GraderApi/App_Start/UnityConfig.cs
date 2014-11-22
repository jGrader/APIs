using System.Configuration;
using GraderApi.Models;
using GraderDataAccessLayer.Repositories;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using GraderDataAccessLayer.Interfaces;

namespace GraderApi
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            container.RegisterType<ICourseRepository, CourseRepository>(new HierarchicalLifetimeManager());
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}