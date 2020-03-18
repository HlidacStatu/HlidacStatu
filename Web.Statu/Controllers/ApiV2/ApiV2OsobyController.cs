using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using System.Web.Mvc;
using HlidacStatu.Lib.Data;
using HlidacStatu.Web.Models.apiv2;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2/osoby")]
    public class ApiV2OsobyController : GenericAuthController
    {
        // /api/v2/osoby/{id}
        [AuthorizeAndAudit]
        [HttpGet, Route("{id}")]
        public ActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Hodnota id chybí.").ToJson(), "application/json");
            }

            var osoba = Osoba.GetByNameId(id);

            if (osoba == null)
            {
                Response.StatusCode = 404;
                return Content(new ErrorMessage($"Osoba s id [{id}] nenalezena").ToJson(), "application/json");
            }

            OsobaDetailDTO OsobaDetail = new OsobaDetailDTO(osoba);

            return Content(OsobaDetail.ToJson(), "application/json");
            
        }

        //todo: replace with better function (after we load people to ES)
        // /api/v2/osoby/?jmeno=andrej&prijmeni=babis&narozen=1963-08-02
        //[AuthorizeAndAudit]
        //[HttpGet, Route()]
        //public ActionResult Hledat(string jmeno, string prijmeni, string narozen)
        //{
        //    DateTime? dt = ParseTools.ToDateTime(narozen
        //            , "yyyy-MM-dd");
        //    if (dt.HasValue == false)
        //    {
        //        Response.StatusCode = 400;
        //        return Content(
        //            new ErrorMessage($"Špatný formát data. Použijte formát ve tvaru [yyyy-MM-dd].").ToJson(), 
        //            "application/json");

        //    }
        //    var osoby = Osoba.GetAllByNameAscii(jmeno, prijmeni, dt.Value)
        //        .Select(o => new OsobaDTO(o))
        //        .ToArray();

        //    return Content(JsonConvert.SerializeObject(
        //        new { Total = osoby.Count(), Result = osoby }
        //        ), "application/json");

        //}

    }
}
