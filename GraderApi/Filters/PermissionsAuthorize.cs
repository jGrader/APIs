namespace GraderApi.Filters
{
    using System;
    using System.Linq;
    using System.Web.Http;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class PermissionsAuthorize : AuthorizeAttribute
    { 
        public PermissionsAuthorize(params object[] roles)
        {
            if (roles.Any(r => r.GetType().BaseType != typeof(Enum)))
                throw new ArgumentException("roles");

            this.Roles = string.Join(",", roles.Select(r => Enum.GetName(r.GetType(), r)));
        }
    }
}