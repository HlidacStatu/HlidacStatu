using HlidacStatu.Lib.Data;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    public partial class HomeController : GenericAuthController
    {
#if (!DEBUG)
        [OutputCache(VaryByParam = "id;embed;nameofview", Duration = 60 * 60 * 1)]
#endif

        [ChildActionOnly()]
        public ActionResult Subjekt_child(string id, string NameOfView, Firma firma)
        {
            return View(NameOfView, firma);
        }

        public ActionResult Subjekt(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            string ico = Util.ParseTools.NormalizeIco(id);

            if (string.IsNullOrEmpty(ico))
                return RedirectToAction("Report", new { id = "1" });

            //if (!Devmasters.TextUtil.IsNumeric(ico))
            //    ico = Devmasters.TextUtil.NormalizeToNumbersOnly(ico);

            Firma firma = Firmy.Get(ico);

            if (!Firma.IsValid(firma))
            {
                if (Util.DataValidators.IsFirmaIcoZahranicni(ico))
                    return View("Subjekt_zahranicni", new Firma() { ICO = ico, Jmeno = ico });
                else
                {
                    if (!Util.DataValidators.CheckCZICO(ico))

                        return View("Subjekt_err_spatneICO");
                    else
                        return View("Subjekt_err_nezname");
                }
            }
            if (Util.DataValidators.IsFirmaIcoZahranicni(ico))
                return View("Subjekt_zahranicni", firma);

            //Framework.Visit.AddVisit("/subjekt/" + ico, Visit.VisitChannel.Web);
            return View(new Models.SubjektReportModel() { firma = firma, ICO = ico });
        }
        public ActionResult Subjekt2(string id)
        {
            return _Subjekt2Data(id);
        }
        public ActionResult Subjekt2_dotace(string id)
        {
            return _Subjekt2Data(id);
        }

        [NonAction()]
        private ActionResult _Subjekt2Data(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            string ico = Util.ParseTools.NormalizeIco(id);

            if (string.IsNullOrEmpty(ico))
                return RedirectToAction("Report", new { id = "1" });


            Firma firma = Firmy.Get(ico);

            if (!Firma.IsValid(firma))
            {
                if (Util.DataValidators.IsFirmaIcoZahranicni(ico))
                    return View("Subjekt_zahranicni", new Firma() { ICO = ico, Jmeno = ico });
                else
                {
                    if (!Util.DataValidators.CheckCZICO(ico))

                        return View("Subjekt_err_spatneICO");
                    else
                        return View("Subjekt_err_nezname");
                }
            }
            if (Util.DataValidators.IsFirmaIcoZahranicni(ico))
                return View("Subjekt_zahranicni", firma);

            return View(firma);
        }
        
    }
}