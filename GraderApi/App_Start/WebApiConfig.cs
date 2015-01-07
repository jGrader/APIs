namespace GraderApi
{
    using Handlers;
    using System.Net.Http.Headers;
    using System.Web.Http;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MessageHandlers.Add(new AuthorizeHandler());
            // config.MessageHandlers.Add(new PermissionsHandler());
            // Web API configuration and services
            config.SuppressDefaultHostAuthentication();
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html")); //to return JSON instead of XML in Chrome
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "UserRoute",
                routeTemplate: "api/{controller}/{action}/{userId}",
                defaults: new { },
                constraints: new { controller = "Users", userId = new ApiRouteConstraints() }
            );

            config.Routes.MapHttpRoute(
                name: "CourseRoute",
                routeTemplate: "api/{controller}/{action}/{courseId}",
                defaults: new { },
                constraints: new { controller = "Courses", courseId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "CourseUserRoute",
                routeTemplate: "api/{controller}/{action}/{courseUserId}",
                defaults: new { },
                constraints: new { controller = "CourseUsers", courseUserId = new ApiRouteConstraints() }
            );
            
            config.Routes.MapHttpRoute(
                name: "GradeComponentRoute",
                routeTemplate: "api/{controller}/{action}/{gradeComponentId}",
                defaults: new { },
                constraints: new { controller = "GradeComponents", gradeComponentId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "TaskRoute",
                routeTemplate: "api/{controller}/{action}/{taskId}",
                defaults: new { },
                constraints: new { controller = "Tasks", taskId = new ApiRouteConstraints() }
            );

            config.Routes.MapHttpRoute(
                name: "EntityRoute",
                routeTemplate: "api/{controller}/{action}/{entityId}",
                defaults: new { },
                constraints: new { controller = "Entities", entityId = new ApiRouteConstraints() }
            );

            config.Routes.MapHttpRoute(
                name: "SubmissionRoute",
                routeTemplate: "api/{controller}/{action}/{submissionId}",
                defaults: new { },
                constraints: new { controller = "Submissions", submissionId = new ApiRouteConstraints() }
            );

            config.Routes.MapHttpRoute(
                name: "SubmitFile",
                routeTemplate: "api/{controller}/{action}/{courseId}/{entityId}",
                defaults: new { },
                constraints: new { controller = "Submissions", courseId = new ApiRouteConstraints(), entityId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
