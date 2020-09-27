using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using System.Web.Http;
using HlidacStatu.Lib.Data;
using HlidacStatu.Web.Framework;

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
