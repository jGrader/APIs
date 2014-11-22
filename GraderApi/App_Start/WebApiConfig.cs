using System.Web.Http;
using GraderApi.Handlers;
using System.Net.Http.Headers;


namespace GraderApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MessageHandlers.Add(new AuthorizeHandler());
            config.MessageHandlers.Add(new PermissionsHandler());
            // Web API configuration and services
            config.SuppressDefaultHostAuthentication();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html")); //to return JSON instead of XML in Chrome

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
