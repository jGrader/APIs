using GraderApi.Filters;
using GraderApi.Handlers;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Mvc;

namespace GraderApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MessageHandlers.Add(new AuthorizeHandler());
            // Web API configuration and services
            config.SuppressDefaultHostAuthentication();
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html")); //to return JSON instead of XML in Chrome
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            #region ROUTES WHICH DON'T HAVE THE {courseId} PARAMETER

            config.Routes.MapHttpRoute(
                name: "CurrentUserRoute",
                routeTemplate: "api/{controller}/{action}/{courseId}",
                defaults: new { courseId = UrlParameter.Optional },
                constraints: new { controller = "CurrentUser", courseId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "CurrentUserGradesRoute",
                routeTemplate: "api/{controller}/{action}/{courseId}/{isPredicted}",
                defaults: new { },
                constraints: new { controller = "CurrentUser", courseId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "UserRoute",
                routeTemplate: "api/{controller}/{action}/{userId}",
                defaults: new { userId = UrlParameter.Optional },
                constraints: new { controller = "Users", userId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "CourseRoute",
                routeTemplate: "api/{controller}/{action}/{courseId}",
                defaults: new { courseId = UrlParameter.Optional },
                constraints: new { controller = "Courses", courseId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            #endregion


            config.Routes.MapHttpRoute(
                name: "GradeRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{gradeId}",
                defaults: new { gradeId = UrlParameter.Optional },
                constraints: new { controller = "Grades", courseId = new ApiRouteConstraints(), gradeId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "GradeTeamRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{wholeTeam}/{gradeId}",
                defaults: new { gradeId = UrlParameter.Optional, wholeTeam = false },
                constraints: new { controller = "Grades", courseId = new ApiRouteConstraints(), gradeId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "CourseUserRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{courseUserId}",
                defaults: new { courseUserId = UrlParameter.Optional },
                constraints: new { controller = "CourseUsers", courseId = new ApiRouteConstraints(), courseUserId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "GradeComponentRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{gradeComponentId}",
                defaults: new { gradeComponentId = UrlParameter.Optional },
                constraints: new { controller = "GradeComponents", courseId = new ApiRouteConstraints(), gradeComponentId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "TaskRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{taskId}",
                defaults: new { taskId = UrlParameter.Optional },
                constraints: new { controller = "Tasks", courseId = new ApiRouteConstraints(), taskId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "EntityRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{entityId}",
                defaults: new { entityId = UrlParameter.Optional },
                constraints: new { controller = "Entities", courseId = new ApiRouteConstraints(), entityId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "FileRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{fileId}",
                defaults: new { entityId = UrlParameter.Optional },
                constraints: new { controller = "Files", courseId = new ApiRouteConstraints(), fileId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "SubmissionRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{submissionId}",
                defaults: new { submissionId = UrlParameter.Optional },
                constraints: new { controller = "Submissions", courseId = new ApiRouteConstraints(), submissionId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "TeamRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{teamId}",
                defaults: new { teamId = UrlParameter.Optional },
                constraints: new { controller = "Teams", courseId = new ApiRouteConstraints(), teamId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "ExtensionRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{extensionId}",
                defaults: new { extensionId = UrlParameter.Optional },
                constraints: new { controller = "Extensions", courseId = new ApiRouteConstraints(), extensionId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "ExcuseRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{excuseId}",
                defaults: new { excuseId = UrlParameter.Optional },
                constraints: new { controller = "Excuses", courseId = new ApiRouteConstraints(), excuseId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "CustomExtensionAndExcusesRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{userId}",
                defaults: new { },
                constraints: new { controller = "/^(Extensions|Excuses)$/", courseId = new ApiRouteConstraints(), userId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );

            config.Routes.MapHttpRoute(
                name: "PermissionRoute",
                routeTemplate: "api/Courses/{courseId}/{controller}/{action}/{userId}",
                defaults: new { userId = UrlParameter.Optional },
                constraints: new { controller = "Permissions", courseId = new ApiRouteConstraints(), userId = new ApiRouteConstraints() },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );


            // IF NOTHING ELSE FITS, THIS GETS CALLED; 
            // TRY NOT TO REACH THIS POINT
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { },
                handler: new PermissionsHandler(GlobalConfiguration.Configuration)
            );
        }
    }
}
