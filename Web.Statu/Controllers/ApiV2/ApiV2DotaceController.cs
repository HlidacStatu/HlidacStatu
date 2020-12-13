using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Framework;
using HlidacStatu.Web.Models.Apiv2;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace HlidacStatu.Web.Controllers
{
    [SwaggerControllerTag("Dotace")]

    [RoutePrefix("api/v2/dotace")]
    public class ApiV2DotaceController : ApiV2AuthController
    {
        /// <summary>
        /// Vyhledá dotace 
        /// </summary>
        /// <param name="dotaz">fulltext dotaz dle <a href="https://www.hlidacstatu.cz/napoveda">syntaxe</a> </param>
        /// <param name="strana">stránka, max. hodnota je 250</param>
        /// <param name="razeni">
        /// pořadí výsledků:<br />
        /// 0 Řadit podle relevance
        /// 1 Řadit podle data podpisu od nejnovějších
        /// 2 Řadit podle data podpisu od nejstarších
        /// 3 Řadit podle částky od největší
        /// 4 Řadit podle částky od nejmenší
        /// 5 Řadit podle IČO od největšího
        /// 6 Řadit podle IČO od nejmenšího
        /// </param>
        /// <returns></returns>
        [HttpGet, Route("hledat")]
        [AuthorizeAndAudit]
        public SearchResultDTO<Lib.Data.Dotace.Dotace> Hledat([FromUri] string dotaz = null, [FromUri] int? strana = null, [FromUri] int? razeni = null)
        {
            if (strana < 1)
                strana = 1;
            if (strana * ApiV2Controller.DefaultResultPageSize > ApiV2Controller.MaxResultsFromES)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota 'strana' nemůže být větší než {ApiV2Controller.MaxResultsFromES / ApiV2Controller.DefaultResultPageSize}"));
            }

            Lib.Searching.DotaceSearchResult result = null;

            if (string.IsNullOrWhiteSpace(dotaz))
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota dotaz chybí."));
            }


            result =new Lib.Data.Dotace.DotaceService().SimpleSearch(dotaz, strana.Value,
                ApiV2Controller.DefaultResultPageSize,
                (razeni ?? 0).ToString());

            if (result.IsValid == false)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Špatně nastavená hodnota dotaz=[{dotaz}]"));
            }
            else
            {
                var filtered = result.ElasticResults.Hits
                    .Select(m => m.Source)
                    .ToArray();

                return new SearchResultDTO<Lib.Data.Dotace.Dotace>(result.Total, result.Page, filtered);
            }
        }

        /// <summary>
        /// Vrátí detail jedné dotace.
        /// </summary>
        /// <param name="id">id dotace</param>
        /// <returns>detail dotace</returns>
        [HttpGet, Route("{id?}")]
        [AuthorizeAndAudit]
        public Lib.Data.Dotace.Dotace Detail(string id = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota id chybí."));
            }

            var dotace = new Lib.Data.Dotace.DotaceService().Get(id);
            if (dotace == null)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Dotace nenalezena"));
            }
            return dotace;
        }

        

    }
}
