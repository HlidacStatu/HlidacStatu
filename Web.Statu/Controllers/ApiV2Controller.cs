using HlidacStatu.Lib.Data.External.DataSets;
using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    public class ApiV2Controller : GenericAuthController
    {
        [HttpGet]
        //[Route("/api/v2/smlouvy/detail/{id}")]
        [AuthorizeAndAudit]
        [SwaggerOperation("SmlouvyDetailIdGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(Smlouva), description: "Úspěšně vrácena smlouva")]
        [SwaggerResponse(statusCode: 400, type: typeof(Chyba), description: "Některé z předaných parametrů byly zadané nesprávně")]
        [SwaggerResponse(statusCode: 401, type: typeof(Chyba), description: "Nesprávný autorizační token")]
        [SwaggerResponse(statusCode: 404, type: typeof(Chyba), description: "Požadovaný dokument nebyl nalezen")]
        [SwaggerResponse(statusCode: 500, type: typeof(Chyba), description: "Došlo k interní chybě na serveru")]
        public ActionResult SmlouvyDetailIdGet([Required]string id)
        {
            //if (!Framework.ApiAuth.IsApiAuth(this,
            //    parameters: new Framework.ApiCall.CallParameter[] {
            //        new Framework.ApiCall.CallParameter("id", id)
                    
            //    })
            //    .Authentificated)
            //{
            //    return new HttpUnauthorizedResult();
            //}
            //else
            //{
            return Content(null, "application/json");
            //}
        }

    }
}
