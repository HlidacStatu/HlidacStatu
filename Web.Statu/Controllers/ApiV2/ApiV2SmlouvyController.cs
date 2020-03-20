using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using HlidacStatu.Web.Models.Apiv2;
using System.Linq;
using System.Web.Http;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2/smlouvy")]
    public class ApiV2SmlouvyController : ApiController
    {
        // /api/v2/smlouvy/detail/{id}
        [HttpGet, Route("{id}")]
        [AuthorizeAndAudit]
        public Lib.Data.Smlouva Detail(string id, string nice = "")
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                //Response.StatusCode =400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota id chybí."));
            }

            var smlouva = Lib.Data.Smlouva.Load(id);
            if (smlouva == null)
            {
                //Response.StatusCode =404;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Smlouva nenalezena"));
            }
            var s = Lib.Data.Smlouva.Export(smlouva,this.User.IsInRole("Admin"));

            return s;
            
        }

        // /api/v2/Smlouvy/hledat/?query=auto&page=1&order=0
        [HttpGet, Route("hledat")]
        [AuthorizeAndAudit]
        public SearchResultDTO Hledat(string query, int? strana, int? razeni)
        {
            strana = strana ?? 1;
            razeni = razeni ?? 0;
            Lib.Searching.SmlouvaSearchResult result = null;

            if (string.IsNullOrWhiteSpace(query))
            {
                //Response.StatusCode =400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota query chybí."));
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
                //Response.StatusCode =400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Špatně nastavená hodnota query=[{query}]"));
            }
            else
            {
                var filtered = result.Result.Hits
                    .Select(m => new Newtonsoft.Json.Linq.JRaw(
                        Lib.Data.Smlouva.Export(m.Source, 
                            false, 
                            this.User.IsInRole("Admin"))))
                    .ToArray();

                return new SearchResultDTO(result.Total, result.Page, filtered);
//                    Newtonsoft.Json.JsonConvert.SerializeObject(new { total = result.Total, items = filtered }, Newtonsoft.Json.Formatting.None), "application/json");
            }

        }
    }
}
