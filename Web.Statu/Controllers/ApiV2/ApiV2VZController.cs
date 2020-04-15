using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Framework;
using HlidacStatu.Web.Models.Apiv2;
using System.Linq;
using System.Web.Http;

namespace HlidacStatu.Web.Controllers
{
    [SwaggerControllerTag("Veřejné zakázky")]
    [RoutePrefix("api/v2/verejnezakazky")]
    public class ApiV2VZController : ApiV2AuthController
    {
        /// <summary>
        /// Detail veřejné zakázky
        /// </summary>
        /// <param name="id">Id veřejné zakázky</param>
        /// <returns>detail veřejné zakázky</returns>
        [AuthorizeAndAudit(Roles = "Admin")]
        [HttpGet, Route("{id?}")]
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

        /// <summary>
        /// Vyhledá veřejné zakázky v databázi Hlídače smluv
        /// </summary>
        /// <param name="dotaz">fulltext dotaz dle <a href="https://www.hlidacstatu.cz/napoveda">syntaxe</a></param>
        /// <param name="strana">stránka, max. hodnota je 250</param>
        /// <param name="razeni">
        /// pořadí výsledků: <br />
        /// 0: podle relevance<br />
        /// 1: nově zveřejněné první<br />
        /// 2: nově zveřejněné poslední<br />
        /// 3: nejlevnější první<br />
        /// 4: nejdražší první<br />
        /// 5: nově uzavřené první<br />
        /// 6: nově uzavřené poslední<br />
        /// 8: podle odběratele<br />
        /// 9: podle dodavatele<br />
        /// </param>
        /// <returns>nalezené veřejné zakázky</returns>
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
                var zakazky = result.ElasticResults.Hits
                    .Select(m => m.Source).ToArray();

                return new SearchResultDTO<Lib.Data.VZ.VerejnaZakazka>(result.Total, result.Page, zakazky);
            }
        }
    }
}
