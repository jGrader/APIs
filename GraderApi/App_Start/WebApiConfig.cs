using System.Collections.Generic;
using GraderApi.Constraints;
using GraderApi.Handlers;
using System.Net.Http.Headers;
using System.Web.Http;
using GraderDataAccessLayer.Models;


namespace GraderApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MessageHandlers.Add(new AuthorizeHandler());
            // config.MessageHandlers.Add(new PermissionsHandler());
            // Web API configuration and services
            config.SuppressDefaultHostAuthentication();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html")); //to return JSON instead of XML in Chrome

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "CourseRoute",
                routeTemplate: "api/{controller}/{courseId}",
                defaults: new { },
                constraints: new { courseId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );
            
            /*config.Routes.MapHttpRoute(
                name: "GradeComponentRoute",
                routeTemplate: "api/GradeComponents/{gradeComponentId}",
                defaults: new { },
                constraints: new { gradeComponentId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );*/

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
