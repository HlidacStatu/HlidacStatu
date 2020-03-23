using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using System.Linq;
using System.Web.Http;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2/verejnezakazky")]
    public class ApiV2VZController : ApiController
    {
        [AuthorizeAndAudit(Roles = "Admin")]
        [HttpGet, Route("detail/{id?}")]
        public Lib.Data.VZ.VerejnaZakazka Detail(string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota id chybí."));
            }

            var zakazka = Lib.Data.VZ.VerejnaZakazka.LoadFromES(id);
            if (zakazka == null)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Zakazka nenalezena"));
            }

            return zakazka;
        }

        [AuthorizeAndAudit(Roles = "Admin")]
        [HttpGet, Route("hledat")]
        public SearchResultDTO<Lib.Data.VZ.VerejnaZakazka>Hledat(string dotaz = null, int? strana=null, int? razeni=null)
        {
            strana = strana ?? 1;
            razeni = razeni ?? 0;
            Lib.Searching.VerejnaZakazkaSearchData result = null;

            if (string.IsNullOrWhiteSpace(dotaz))
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota dotaz chybí."));
            }

            result = Lib.Data.VZ.VerejnaZakazka.Searching.SimpleSearch(dotaz, null, strana.Value,
                Lib.Data.Smlouva.Search.DefaultPageSize,
                razeni.Value.ToString());

            if (result.IsValid == false)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Špatně nastavená hodnota dotaz=[{dotaz}]"));
            }
            else
            {
                var zakazky = result.Result.Hits
                    .Select(m => m.Source).ToArray();

                return new SearchResultDTO<Lib.Data.VZ.VerejnaZakazka>(result.Total, result.Page, zakazky);
            }
        }
    }
}
