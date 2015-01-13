using GraderApi.Filters;
using GraderApi.Handlers;
using GraderDataAccessLayer;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Mvc;

namespace GraderApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, DatabaseContext context)
        {
            config.MessageHandlers.Add(new AuthorizeHandler(context));
            // Web API configuration and services
            config.SuppressDefaultHostAuthentication();
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html")); //to return JSON instead of XML in Chrome
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));


            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "GradeRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{gradeId}",
                defaults: new { gradeId = UrlParameter.Optional},
                constraints: new { controller = "Grades", courseId = new ApiRouteConstraints(context), gradeId = new ApiRouteConstraints(context)},
                handler: new PermissionsHandler(GlobalConfiguration.Configuration, context)
            );

            config.Routes.MapHttpRoute(
                name: "CurrentUserRoute",
                routeTemplate: "api/{controller}/{action}/{courseId}",
                defaults: new { courseId = UrlParameter.Optional },
                constraints: new { controller = "CurrentUser", courseId = new ApiRouteConstraints(context) },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration, context)
            );

            config.Routes.MapHttpRoute(
                name: "UserRoute",
                routeTemplate: "api/{controller}/{action}/{userId}",
                defaults: new { userId = UrlParameter.Optional },
                constraints: new { controller = "Users", userId = new ApiRouteConstraints(context) },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration, context)
            );

            config.Routes.MapHttpRoute(
                name: "CourseRoute",
                routeTemplate: "api/{controller}/{action}/{courseId}",
                defaults: new { courseId = UrlParameter.Optional },
                constraints: new { controller = "Courses", courseId = new ApiRouteConstraints(context) },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration, context)
            );

            config.Routes.MapHttpRoute(
                name: "CourseUserRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{courseUserId}",
                defaults: new { courseUserId = UrlParameter.Optional },
                constraints: new { controller = "CourseUsers", courseId = new ApiRouteConstraints(context), courseUserId = new ApiRouteConstraints(context) },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration, context)
            );

            config.Routes.MapHttpRoute(
                name: "GradeComponentRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{gradeComponentId}",
                defaults: new { gradeComponentId = UrlParameter.Optional },
                constraints: new { controller = "GradeComponents", courseId = new ApiRouteConstraints(context), gradeComponentId = new ApiRouteConstraints(context) },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration, context)
            );

            config.Routes.MapHttpRoute(
                name: "TaskRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{taskId}",
                defaults: new { taskId = UrlParameter.Optional },
                constraints: new { controller = "Tasks", courseId = new ApiRouteConstraints(context), taskId = new ApiRouteConstraints(context) },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration, context)
            );

            config.Routes.MapHttpRoute(
                name: "EntityRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{entityId}",
                defaults: new { entityId = UrlParameter.Optional },
                constraints: new { controller = "Entities", courseId = new ApiRouteConstraints(context), entityId = new ApiRouteConstraints(context) },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration, context)
            );

            config.Routes.MapHttpRoute(
                name: "FileRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{fileId}",
                defaults: new { entityId = UrlParameter.Optional },
                constraints: new { controller = "Files", courseId = new ApiRouteConstraints(context), fileId = new ApiRouteConstraints(context) },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration, context)
            );

            config.Routes.MapHttpRoute(
                name: "SubmissionRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{submissionId}",
                defaults: new { submissionId = UrlParameter.Optional },
                constraints: new { controller = "Submissions", courseId = new ApiRouteConstraints(context), submissionId = new ApiRouteConstraints(context) },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration, context)
            );

            config.Routes.MapHttpRoute(
                name: "TeamRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{teamId}",
                defaults: new { teamId = UrlParameter.Optional },
                constraints: new { controller = "Teams", courseId = new ApiRouteConstraints(context), teamId = new ApiRouteConstraints(context) },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration, context)
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration, context)
            );
        }
    }
}
