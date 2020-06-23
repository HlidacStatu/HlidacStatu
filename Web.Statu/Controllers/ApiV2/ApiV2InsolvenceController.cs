using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Framework;
using HlidacStatu.Web.Models.Apiv2;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace HlidacStatu.Web.Controllers
{
    [SwaggerControllerTag("Insolvence")]

    [RoutePrefix("api/v2/insolvence")]
    public class ApiV2InsolvenceController : ApiV2AuthController
    {
        /// <summary>
        /// Vyhledá smlouvy v databázi smluv
        /// </summary>
        /// <param name="dotaz">fulltext dotaz dle <a href="https://www.hlidacstatu.cz/napoveda">syntaxe</a> </param>
        /// <param name="strana">stránka, max. hodnota je 250</param>
        /// <param name="razeni">
        /// pořadí výsledků:<br />
        /// 0: podle relevance<br />
        /// 1: nově zahájené první
        /// 2: nově zveřejněné poslední
        /// 3: naposled změněné první
        /// 4: naposled změněné poslední
        /// </param>
        /// <returns></returns>
        [HttpGet, Route("hledat")]
        [AuthorizeAndAudit]
        public SearchResultDTO<Lib.Data.Insolvence.Rizeni> Hledat([FromUri] string dotaz = null, [FromUri] int? strana = null, 
            [FromUri] int? razeni = null)
        {
            strana = strana ?? 1;
            razeni = razeni ?? 0;

            if (strana < 1)
                strana = 1;
            if (strana * ApiV2Controller.DefaultResultPageSize > ApiV2Controller.MaxResultsFromES)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota 'strana' nemůže být větší než {ApiV2Controller.MaxResultsFromES / ApiV2Controller.DefaultResultPageSize}"));
            }

            Lib.Searching.InsolvenceSearchResult result = null;

            if (string.IsNullOrWhiteSpace(dotaz))
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota dotaz chybí."));
            }

            bool isLimited = !(
                    this.ApiAuth.ApiCall.UserRoles.Contains("novinar")
                    ||
                    this.ApiAuth.ApiCall.UserRoles.Contains("Admin")
                    );

            result = Lib.Data.Insolvence.Insolvence.SimpleSearch(dotaz, strana.Value,
                ApiV2Controller.DefaultResultPageSize,razeni.Value,false, isLimited);

            if (result.IsValid == false)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Špatně nastavená hodnota dotaz=[{dotaz}]"));
            }
            else
            {
                var filtered = result.ElasticResults.Hits
                    .Select(m=>m.Source)
                    .ToArray();

                var ret = new SearchResultDTO<Lib.Data.Insolvence.Rizeni>(result.Total, result.Page, filtered);
                return ret;
            }
        }

        /// <summary>
        /// Vrátí detail jedné smlouvy.
        /// </summary>
        /// <param name="id">id smlouvy</param>
        /// <returns>detail smlouvy</returns>
        [HttpGet, Route("{id?}")]
        [AuthorizeAndAudit]
        public Lib.Data.Insolvence.Rizeni Detail(string id = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota id chybí."));
            }
            bool isLimited = !(
                    this.ApiAuth.ApiCall.UserRoles.Contains("novinar")
                    ||
                    this.ApiAuth.ApiCall.UserRoles.Contains("Admin")
                    );

            var ins = Lib.Data.Insolvence.Insolvence.LoadFromES(id,true,isLimited);
            if (ins == null)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Insolvence nenalezena"));
            }
            
            return ins.Rizeni;
        }

    }
}
