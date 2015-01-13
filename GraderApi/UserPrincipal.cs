using System.Security.Principal;
using GraderDataAccessLayer.Models;

namespace GraderApi
{
    public class UserPrincipal : GenericPrincipal
    {
        public UserModel User;
        public UserPrincipal(IIdentity identity, string[] roles, UserModel user) : base(identity, roles)
        {
            User = user;
        }
    }
}