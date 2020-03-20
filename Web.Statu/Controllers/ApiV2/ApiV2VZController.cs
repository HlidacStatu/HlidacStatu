using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using Nest;
using Swashbuckle.Swagger.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;
using HlidacStatu.Web.Models.Apiv2;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2/verejnezakazky")]
    public class ApiV2VZController : ApiController
    {
        // /api/v2/verejnezakazky/detail/{id}
        [AuthorizeAndAudit(Roles = "Admin")]
        [HttpGet, Route("{id}")]
        public Lib.Data.VZ.VerejnaZakazka Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                //Response.StatusCode =400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota id chybí."));
            }

            var zakazka = Lib.Data.VZ.VerejnaZakazka.LoadFromES(id);
            if (zakazka == null)
            {
                //Response.StatusCode =404;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Zakazka nenalezena"));
            }

            return zakazka;
            
        }

        // /api/v2/verejnezakazky/hledat/?query=auto&page=1&order=0
        [AuthorizeAndAudit(Roles = "Admin")]
        [HttpGet, Route("hledat")]
        public SearchResultDTO Hledat(string query, int? strana, int? razeni)
        {
            strana = strana ?? 1;
            razeni = razeni ?? 0;
            Lib.Searching.VerejnaZakazkaSearchData result = null;

            if (string.IsNullOrWhiteSpace(query))
            {
                //Response.StatusCode =400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota query chybí."));
            }


            result = Lib.Data.VZ.VerejnaZakazka.Searching.SimpleSearch(query, null, strana.Value,
                Lib.Data.Smlouva.Search.DefaultPageSize,
                razeni.Value.ToString());


            if (result.IsValid == false)
            {
                //Response.StatusCode =400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Špatně nastavená hodnota query=[{query}]"));
            }
            else
            {
                var zakazky = result.Result.Hits
                    .Select(m => m.Source).ToArray();

                return new SearchResultDTO(result.Total, result.Page, zakazky);
                //return Content(JsonConvert.SerializeObject(zakazky), "application/json");
            }

        }



    }
}
