using HlidacStatu.Web.Framework;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;

namespace HlidacStatu.Web.Controllers
{
    public class ApiV2AuthController : ApiController, IAuthenticableController
    {
        public string HostIpAddress => HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";

        public string AuthToken
        {
            get
            {

                if (this.Request.Headers != null)
                {
                    return this.Request.Headers.Authorization.ToString();
                }

                return this.Request.GetQueryNameValuePairs().LastOrDefault(k => k.Key == "Authorization").Value;
                
            }
        }

        public ApiAuth.Result ApiAuth { get; set; }
        
    }
}