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
        
        // /api/v2/Smlouvy/hledat/?dotaz=auto&page=1&order=0
        [HttpGet, Route("hledat")]
        [AuthorizeAndAudit]
        public SearchResultDTO<Lib.Data.Smlouva> Hledat([FromUri] string dotaz = null, [FromUri] int? strana = null, [FromUri] int? razeni = null)
        {
            strana = strana ?? 1;
            razeni = razeni ?? 0;
            Lib.Searching.SmlouvaSearchResult result = null;

            if (string.IsNullOrWhiteSpace(dotaz))
            {
                //Response.StatusCode =400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota query chybí."));
            }

            bool? platnyzaznam = null; //1 - nic defaultne
            if (
                System.Text.RegularExpressions.Regex.IsMatch(dotaz.ToLower(), "(^|\\s)id:")
                ||
                dotaz.ToLower().Contains("idverze:")
                ||
                dotaz.ToLower().Contains("idsmlouvy:")
                ||
                dotaz.ToLower().Contains("platnyzaznam:")
                )
                platnyzaznam = null;

            result = Lib.Data.Smlouva.Search.SimpleSearch(dotaz, strana.Value,
                Lib.Data.Smlouva.Search.DefaultPageSize,
                (Lib.Data.Smlouva.Search.OrderResult)razeni.Value,
                platnyZaznam: platnyzaznam);


            if (result.IsValid == false)
            {
                //Response.StatusCode =400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Špatně nastavená hodnota query=[{dotaz}]"));
            }
            else
            {
                var filtered = result.Result.Hits
                    .Select(m => 
                        Lib.Data.Smlouva.Export(m.Source, 
                            false, 
                            this.User.IsInRole("Admin")))
                    .ToArray();

                //new Newtonsoft.Json.Linq.JRaw( 

                return new SearchResultDTO<Lib.Data.Smlouva>(result.Total, result.Page, filtered);
//                    Newtonsoft.Json.JsonConvert.SerializeObject(new { total = result.Total, items = filtered }, Newtonsoft.Json.Formatting.None), "application/json");
            }

        }


        // /api/v2/smlouvy/smlouva/detail/{id}
        [HttpGet, Route("{id?}")]
        [AuthorizeAndAudit]
        public Lib.Data.Smlouva Detail(string id =null ,[FromUri] string nice = "")
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
            var s = Lib.Data.Smlouva.Export(smlouva, this.User.IsInRole("Admin"));

            return s;

        }

    }
}
