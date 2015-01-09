using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using GraderDataAccessLayer.Models;

namespace GraderApi.Principals
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