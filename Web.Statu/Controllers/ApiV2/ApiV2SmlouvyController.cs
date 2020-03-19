using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.apiv2;
using HlidacStatu.Web.Models.Apiv2;
using System.Linq;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2/smlouvy")]
    public class ApiV2SmlouvyController : GenericAuthController
    {
        // /api/v2/smlouvy/detail/{id}
        [HttpGet, Route("{id}")]
        [AuthorizeAndAudit]
        public ActionResult Detail(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Hodnota id chybí.").ToJson(), "application/json");
            }

            var smlouva = Lib.Data.Smlouva.Load(id);
            if (smlouva == null)
            {
                Response.StatusCode = 404;
                return Content(new ErrorMessage($"Smlouva nenalezena").ToJson(), "application/json");
            }
            var smlouvaJson = Lib.Data.Smlouva.ExportToJson(smlouva,
                !string.IsNullOrWhiteSpace(Request.QueryString["nice"]),
                this.User.IsInRole("Admin")
                );

            return Content(smlouvaJson, "application/json");
            
        }

        // /api/v2/Smlouvy/hledat/?query=auto&page=1&order=0
        [HttpGet, Route("hledat")]
        [AuthorizeAndAudit]
        public ActionResult Hledat(string query, int? strana, int? razeni)
        {
            strana = strana ?? 1;
            razeni = razeni ?? 0;
            Lib.Searching.SmlouvaSearchResult result = null;

            if (string.IsNullOrWhiteSpace(query))
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Hodnota query chybí.").ToJson(), "application/json");
            }

            bool? platnyzaznam = null; //1 - nic defaultne
            if (
                System.Text.RegularExpressions.Regex.IsMatch(query.ToLower(), "(^|\\s)id:")
                ||
                query.ToLower().Contains("idverze:")
                ||
                query.ToLower().Contains("idsmlouvy:")
                ||
                query.ToLower().Contains("platnyzaznam:")
                )
                platnyzaznam = null;

            result = Lib.Data.Smlouva.Search.SimpleSearch(query, strana.Value,
                Lib.Data.Smlouva.Search.DefaultPageSize,
                (Lib.Data.Smlouva.Search.OrderResult)razeni.Value,
                platnyZaznam: platnyzaznam);


            if (result.IsValid == false)
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Špatně nastavená hodnota query=[{query}]").ToJson(),
                               "application/json");
            }
            else
            {
                var filtered = result.Result.Hits
                    .Select(m => new Newtonsoft.Json.Linq.JRaw(
                        Lib.Data.Smlouva.ExportToJson(m.Source, 
                            false, 
                            this.User.IsInRole("Admin"))))
                    .ToArray();

                return Content(new SearchResultDTO(result.Total, result.Page, filtered).ToJson(), "application/json");
//                    Newtonsoft.Json.JsonConvert.SerializeObject(new { total = result.Total, items = filtered }, Newtonsoft.Json.Formatting.None), "application/json");
            }

        }
    }
}
