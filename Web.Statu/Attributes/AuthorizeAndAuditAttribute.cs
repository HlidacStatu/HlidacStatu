using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using HlidacStatu.Web.Framework;
using HlidacStatu.Web.Framework.Api;

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
        
        public async override void OnActionExecuting(HttpActionContext filterContext)
        {
            var controller = (IAuthenticableController)filterContext.ControllerContext.Controller;
            //filterContext.RequestContext.Principal = new GenericPrincipal()


            var stream = filterContext.Request.Content.ReadAsStreamAsync().Result;
            string datastring = Helpers.ReadRequestBody(stream);


            var parameters = filterContext.ActionArguments
                            .Select(ap => new Framework.ApiCall.CallParameter(ap.Key, ap.Value?.ToString()))
                            .ToList();

            if (!string.IsNullOrWhiteSpace(datastring))
            {
                parameters.Add(new Framework.ApiCall.CallParameter("requestBody", datastring));
            }

            var apiAuth = Framework.ApiAuth.IsApiAuth(controller,
                validRole: Roles,
                parameters: parameters);

            if (!apiAuth.Authentificated)
            {
                filterContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }

            controller.ApiAuth = apiAuth;

        }

    }
}