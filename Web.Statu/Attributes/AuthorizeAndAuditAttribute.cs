using HlidacStatu.Web.Controllers;
using System.Linq;
using System.Web.Mvc;

namespace HlidacStatu.Web.Attributes
{


    /// <summary>
    /// Tests if user is logged in (in correct role) and audits a request.
    /// </summary>
    public class AuthorizeAndAuditAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets user roles which are authorized to access method.
        /// </summary>
        public string Roles { get; set; }
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (GenericAuthController)filterContext.Controller;

            if (!Framework.ApiAuth.IsApiAuth(controller,
                validRole: Roles,
                parameters: filterContext.ActionParameters
                            .Select(ap => new Framework.ApiCall.CallParameter(ap.Key, ap.Value?.ToString()))
                            .ToArray())
                .Authentificated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }

        }

    }
}