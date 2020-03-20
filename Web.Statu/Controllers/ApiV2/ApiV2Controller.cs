using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using System.Web.Http;
using HlidacStatu.Lib.Data;
using HlidacStatu.Web.Models.Apiv2;
using System.Net.Http;
using System.Net;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2")]
    public class ApiV2Controller : ApiController
    {
        // /api/v2/{id}
        //[AuthorizeAndAudit]
        [HttpGet, Route("ping/{text}")]
        public string Ping(string text)
        {
            return "pong " + text;
        }

    }
}
