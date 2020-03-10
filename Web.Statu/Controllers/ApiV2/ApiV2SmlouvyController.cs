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
    public class ApiV2SmlouvyController : GenericAuthController
    {
        // /api/v2/smlouvy/SmlouvyDetailIdGet/{id}
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
            if (string.IsNullOrWhiteSpace(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Missing Id");

            var model = HlidacStatu.Lib.Data.Smlouva.Load(id);
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Smlouva not found");
            }
            var smodel = HlidacStatu.Lib.Data.Smlouva.ExportToJson(model,
                !string.IsNullOrWhiteSpace(Request.QueryString["nice"]),
                this.User.IsInRole("Admin")
                );

            return Content(smodel, "application/json");
            
        }

    }
}
