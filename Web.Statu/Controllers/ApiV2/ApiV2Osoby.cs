using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using Swashbuckle.Swagger.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using HlidacStatu.Util;
using System;
using HlidacStatu.Lib.Data;
using HlidacStatu.Web.Models.apiv2;

namespace HlidacStatu.Web.Controllers
{
    public class ApiV2OsobyController : GenericAuthController
    {
        // /api/v2/verejnezakazky/detail/{id}
        [HttpGet]
        [AuthorizeAndAudit]
        [SwaggerOperation("Detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(OsobaDetailDTO), description: "Úspěšně vrácena veřejná zakázka")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorMessage), description: "Některé z předaných parametrů byly zadané nesprávně")]
        [SwaggerResponse(statusCode: 401, type: typeof(ErrorMessage), description: "Nesprávný autorizační token")]
        [SwaggerResponse(statusCode: 404, description: "Požadovaný dokument nebyl nalezen")]
        [SwaggerResponse(statusCode: 500, description: "Došlo k interní chybě na serveru")]
        public ActionResult Detail([Required]string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Hodnota id chybí.").ToJson(), "application/json");
            }

            var osoba = Osoba.GetByNameId(id);

            if (osoba == null)
            {
                Response.StatusCode = 404;
                return Content(new ErrorMessage($"Osoba s id [{id}] nenalezena").ToJson(), "application/json");
            }

            OsobaDetailDTO OsobaDetail = new OsobaDetailDTO(osoba);

            return Content(OsobaDetail.ToJson(), "application/json");
            
        }

        // /api/v2/verejnezakazky/hledat/?query=auto&page=1&order=0
        [HttpGet]
        [AuthorizeAndAudit]
        [SwaggerOperation("Hledat")]
        [SwaggerResponse(statusCode: 200, type: typeof(OsobaDTO), description: "Úspěšně vrácen seznam smluv")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorMessage), description: "Některé z předaných parametrů byly zadané nesprávně")]
        [SwaggerResponse(statusCode: 401, type: typeof(ErrorMessage), description: "Nesprávný autorizační token")]
        [SwaggerResponse(statusCode: 404, description: "Žádná veřejná zakázka nenalezena")]
        [SwaggerResponse(statusCode: 500, description: "Došlo k interní chybě na serveru")]
        public ActionResult Hledat(string jmeno, string prijmeni, string narozen)
        {
            DateTime? dt = ParseTools.ToDateTime(narozen
                    , "yyyy-MM-dd");
            if (dt.HasValue == false)
            {
                Response.StatusCode = 400;
                return Content(
                    new ErrorMessage($"Špatný formát data. Použijte formát ve tvaru [yyyy-MM-dd].").ToJson(), 
                    "application/json");
                
            }
            var osoby = Osoba.GetAllByNameAscii(jmeno, prijmeni, dt.Value)
                .Select(o => new OsobaDTO(o))
                .ToArray();

            return Content(JsonConvert.SerializeObject(
                new { Total = osoby.Count(), Result = osoby }
                ), "application/json");

        }

    }
}
