using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using Nest;
using Swashbuckle.Swagger.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using HlidacStatu.Web.Models.apiv2;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2/verejnezakazky")]
    public class ApiV2VZController : GenericAuthController
    {
        // /api/v2/verejnezakazky/detail/{id}
        [AuthorizeAndAudit(Roles = "Admin")]
        [HttpGet, Route("{id}")]
        public ActionResult Detail(string id)
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
        [AuthorizeAndAudit(Roles = "Admin")]
        [HttpGet, Route("hledat")]
        public ActionResult Hledat(string query, int? strana, int? razeni)
        {
            strana = strana ?? 1;
            razeni = razeni ?? 0;
            Lib.Searching.VerejnaZakazkaSearchData result = null;

            if (string.IsNullOrWhiteSpace(query))
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Hodnota query chybí.").ToJson(), "application/json");
            }


            result = Lib.Data.VZ.VerejnaZakazka.Searching.SimpleSearch(query, null, strana.Value,
                Lib.Data.Smlouva.Search.DefaultPageSize,
                razeni.Value.ToString());


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

                return Content(new SearchResultDTO(result.Total, result.Page, zakazky).ToJson(), "application/json");
                //return Content(JsonConvert.SerializeObject(zakazky), "application/json");
            }

        }



    }
}
