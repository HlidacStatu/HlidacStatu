using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HlidacStatu.Web.Framework
{
    public static class InsolvenceLimitedView
    {
        static string[] defaultRolesWithoutLimitation = new string[] { "Admin", "novinar" };
        public static bool IsLimited(Controllers.GenericAuthController c, string[] validRoles = null)
        {
            validRoles = validRoles ?? new string[] { "Admin", "novinar" };
            var usrmgr = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            if (c.User?.Identity?.IsAuthenticated == true)
            {
                if (validRoles.Count() == 0)
                    return false;

                foreach (var role in validRoles)
                {
                    if (c.User.IsInRole(role.Trim()))
                        return false;
                }
                return true;
            }
            return true;

        }
    }
}