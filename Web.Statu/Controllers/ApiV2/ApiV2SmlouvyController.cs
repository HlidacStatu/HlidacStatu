using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Framework;
using HlidacStatu.Web.Models.Apiv2;
using System.Linq;
using System.Web.Http;

namespace HlidacStatu.Web.Controllers
{
    [SwaggerControllerTag("Smlouvy")]

    [RoutePrefix("api/v2/smlouvy")]
    public class ApiV2SmlouvyController : ApiV2AuthController
    {
        /// <summary>
        /// Vyhledá smlouvy v databázi smluv
        /// </summary>
        /// <param name="dotaz">fulltext dotaz dle <a href="https://www.hlidacstatu.cz/napoveda">syntaxe</a> </param>
        /// <param name="strana">stránka, max. hodnota je 250</param>
        /// <param name="razeni">
        /// pořadí výsledků:<br />
        /// 0: podle relevance<br />
        /// 1: nově zveřejněné první<br />
        /// 2: nově zveřejněné poslední<br />
        /// 3: nejlevnější první<br />
        /// 4: nejdražší první<br />
        /// 5: nově uzavřené první<br />
        /// 6: nově uzavřené poslední<br />
        /// 7: nejvíce chybové první<br />
        /// 8: podle odběratele<br />
        /// 9: podle dodavatele<br />
        /// </param>
        /// <returns></returns>
        [HttpGet, Route("hledat")]
        [AuthorizeAndAudit]
        public SearchResultDTO<Lib.Data.Smlouva> Hledat([FromUri] string dotaz = null, [FromUri] int? strana = null, [FromUri] int? razeni = null)
        {
            strana = strana ?? 1;
            razeni = razeni ?? 0;
            Lib.Searching.SmlouvaSearchResult result = null;

            if (string.IsNullOrWhiteSpace(dotaz))
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota dotaz chybí."));
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
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Špatně nastavená hodnota dotaz=[{dotaz}]"));
            }
            else
            {
                var filtered = result.ElasticResults.Hits
                    .Select(m => 
                        Lib.Data.Smlouva.Export(m.Source, 
                            this.User.IsInRole("Admin"),
                            this.User.IsInRole("Admin")))
                    .ToArray();

                return new SearchResultDTO<Lib.Data.Smlouva>(result.Total, result.Page, filtered);
            }
        }

        /// <summary>
        /// Vrátí detail jedné smlouvy.
        /// </summary>
        /// <param name="id">id smlouvy</param>
        /// <returns>detail smlouvy</returns>
        [HttpGet, Route("{id?}")]
        [AuthorizeAndAudit]
        public Lib.Data.Smlouva Detail(string id = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota id chybí."));
            }

            var smlouva = Lib.Data.Smlouva.Load(id);
            if (smlouva == null)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Smlouva nenalezena"));
            }
            var s = Lib.Data.Smlouva.Export(smlouva, this.User.IsInRole("Admin"), this.User.IsInRole("Admin"));

            return s;
        }
    }
}
