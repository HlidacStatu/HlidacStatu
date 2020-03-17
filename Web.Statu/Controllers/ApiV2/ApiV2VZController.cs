using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using Nest;
using Swashbuckle.Swagger.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace HlidacStatu.Web.Controllers
{
    public class ApiV2VZController : GenericAuthController
    {
        // /api/v2/verejnezakazky/detail/{id}
        [HttpGet]
        [AuthorizeAndAudit(Roles = "Admin")]
        [SwaggerOperation("Detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(Lib.Data.VZ.VerejnaZakazka), description: "Úspěšně vrácena veřejná zakázka")]
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

            var zakazka = Lib.Data.VZ.VerejnaZakazka.LoadFromES(id);
            if (zakazka == null)
            {
                Response.StatusCode = 404;
                return Content(new ErrorMessage($"Zakazka nenalezena").ToJson(), "application/json");
            }

            var zakazkaJson = JsonConvert.SerializeObject(zakazka);

            return Content(zakazkaJson, "application/json");
            
        }

        // /api/v2/verejnezakazky/hledat/?query=auto&page=1&order=0
        [HttpGet]
        [AuthorizeAndAudit(Roles = "Admin")]
        [SwaggerOperation("Hledat")]
        [SwaggerResponse(statusCode: 200, type: typeof(Lib.Data.VZ.VerejnaZakazka[]), description: "Úspěšně vrácen seznam smluv")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorMessage), description: "Některé z předaných parametrů byly zadané nesprávně")]
        [SwaggerResponse(statusCode: 401, type: typeof(ErrorMessage), description: "Nesprávný autorizační token")]
        [SwaggerResponse(statusCode: 404, description: "Žádná veřejná zakázka nenalezena")]
        [SwaggerResponse(statusCode: 500, description: "Došlo k interní chybě na serveru")]
        public ActionResult Hledat([Required]string query, int? page, int? order)
        {
            page = page ?? 1;
            order = order ?? 0;
            Lib.Searching.VerejnaZakazkaSearchData result = null;

            if (string.IsNullOrWhiteSpace(query))
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Hodnota query chybí.").ToJson(), "application/json");
            }


            result = Lib.Data.VZ.VerejnaZakazka.Searching.SimpleSearch(query, null, page.Value,
                Lib.Data.Smlouva.Search.DefaultPageSize,
                order.Value.ToString());


            if (result.IsValid == false)
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Špatně nastavená hodnota query=[{query}]").ToJson(),
                               "application/json");
            }
            else
            {
                var zakazky = result.Result.Hits
                    .Select(m => m.Source).ToArray();

                return Content(JsonConvert.SerializeObject(zakazky), "application/json");
            }

        }



    }
}
