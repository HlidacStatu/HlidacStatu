using HlidacStatu.Web.Controllers;
using System.Linq;
using System.Web.Mvc;

namespace HlidacStatu.Web.Attributes
{
    public class AuthorizeAndAuditAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (GenericAuthController)filterContext.Controller;

            if (!Framework.ApiAuth.IsApiAuth(controller,
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