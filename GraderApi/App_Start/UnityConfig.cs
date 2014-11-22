using System.Configuration;
using GraderApi.Models;
using GraderDataAccessLayer.Interfaces;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using GraderDataAccessLayer.Repositories;

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