using System.Data.Entity;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GraderDataAccessLayer;


namespace GraderApi
{
    public class WebApiApplication : HttpApplication
    {
        private DatabaseContext _context;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            Database.SetInitializer(new DatabaseInitializer());

            _context = new DatabaseContext();
            UnityConfig.RegisterComponents(_context);
            GlobalConfiguration.Configure(Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //
            
        }

        protected void Register(HttpConfiguration config)
        {
            WebApiConfig.Register(config, _context);
        }

    }
}
