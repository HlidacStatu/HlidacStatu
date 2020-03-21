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

            string datastring = null;
            if(filterContext.HttpContext.Request.ContentLength > 0)
            {
                datastring = Framework.Api.Helpers.ReadRequestBody(filterContext.HttpContext.Request);
            }

            var parameters = filterContext.ActionParameters
                            .Select(ap => new Framework.ApiCall.CallParameter(ap.Key, ap.Value?.ToString()))
                            .ToList();

            if (!string.IsNullOrWhiteSpace(datastring))
            {
                
                parameters.Add(new Framework.ApiCall.CallParameter("requestBody", datastring));
            }

            if (!Framework.ApiAuth.IsApiAuth(controller,
                validRole: Roles,
                parameters: parameters)
                .Authentificated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }

        }

    }
}