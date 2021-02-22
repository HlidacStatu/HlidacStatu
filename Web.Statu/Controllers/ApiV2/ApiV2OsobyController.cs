using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using System.Web.Http;
using HlidacStatu.Lib.Data;
using HlidacStatu.Web.Framework;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Linq.Expressions;

namespace HlidacStatu.Web.Controllers
{


    [SwaggerControllerTag("Osoby")]

    [RoutePrefix("api/v2/osoby")]
    public class ApiV2OsobyController : ApiV2AuthController
    {
        /// <summary>
        /// Vypíše detail osoby na základě "osobaId" parametru.
        /// </summary>
        /// <param name="osobaId">Id osoby v Hlídači Státu</param>
        /// <returns></returns>
        [AuthorizeAndAudit]
        [HttpGet, Route("{osobaId}")]
        public OsobaDetailDTO Detail(string osobaId)
        {
            if (string.IsNullOrEmpty(osobaId))
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota id chybí."));
            }

            var osoba = Osoba.GetByNameId(osobaId);

            if (osoba == null)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Osoba s id [{osobaId}] nenalezena"));
            }

            OsobaDetailDTO OsobaDetail = new OsobaDetailDTO(osoba);

            return OsobaDetail;
        }

        [AuthorizeAndAudit]
        [HttpGet, Route("hledat")]
        public List<OsobaDTO> Search([FromUri] string dotaz = null, [FromUri] int? strana = null)
        {
            if (string.IsNullOrEmpty(dotaz))
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Chybí query."));
            }

            if (strana is null || strana < 1)
                strana = 1;
            var osoby = Osoba.Search.SimpleSearch(dotaz, strana.Value, 30, Osoba.Search.OrderResult.Relevance);

            var result = osoby.Results.Select(o => new OsobaDTO(o)).ToList();

            return result;
        }

        // /api/v2/osoby/social?typ=instagram&typ=twitter
        [AuthorizeAndAudit]
        [HttpGet, Route("social")]
        public List<OsobaSocialDTO> Social([FromUri] OsobaEvent.SocialNetwork[] typ)
        {

            var socials = (typ is null || typ.Length == 0)
                ? Enum.GetNames(typeof(OsobaEvent.SocialNetwork))
                : typ.Select(t => t.ToString("G"));

            Expression<Func<OsobaEvent, bool>> socialNetworkFilter = e =>
                e.Type == (int)OsobaEvent.Types.SocialniSite
                && socials.Contains(e.Organizace);

            var osobaSocialDTOs = Osoba.GetByEvent(socialNetworkFilter)
                .Select(o => new OsobaSocialDTO(o, socialNetworkFilter))
                .ToList();

            return osobaSocialDTOs;
        }


        //todo: replace with better function (after we load people to ES)
        // /api/v2/osoby/?jmeno=andrej&prijmeni=babis&narozen=1963-08-02
        //[AuthorizeAndAudit]
        //[HttpGet, Route()]
        //public ActionResult Hledat(string jmeno, string prijmeni, string narozen)
        //{
        //    DateTime? dt = Devmasters.DT.Util.ToDateTime(narozen
        //            , "yyyy-MM-dd");
        //    if (dt.HasValue == false)
        //    {
        //        //Response.StatusCode =400;
        //        return Content(
        //            new ErrorMessage($"Špatný formát data. Použijte formát ve tvaru [yyyy-MM-dd].").ToJson(), 
        //            "application/json");

        //    }
        //    var osoby = Osoba.Searching.GetAllByNameAscii(jmeno, prijmeni, dt.Value)
        //        .Select(o => new OsobaDTO(o))
        //        .ToArray();

        //    return Content(JsonConvert.SerializeObject(
        //        new { Total = osoby.Count(), Result = osoby }
        //        ), "application/json");

        //}

    }
}
