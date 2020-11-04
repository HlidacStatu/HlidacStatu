using HlidacStatu.Lib.Data;
using System.Collections.Generic;
using System.Linq;
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
            TryGetCompany(id, out var firma, out var result);
            return result;
        }
        
        public ActionResult Subjekt2_Dotace(string id)
        {
            if(TryGetCompany(id, out var firma, out var result))
            {
                var dotaceService = new HlidacStatu.Lib.Data.Dotace.DotaceService();
                var holdingSubsidies = dotaceService.GetDotaceForHolding(firma.ICO).ToList();

                var cerp = holdingSubsidies
                    .SelectMany(s => s.Rozhodnuti
                        .SelectMany(r => r.Cerpani
                            .Select(c => 
                            (
                                Ico: s.Prijemce.Ico,
                                Rok: c.GuessedYear ?? 0,
                                Cerpano: c.CastkaSpotrebovana ?? 0
                            ))
                        )
                    ).ToList();

                return View((Firma: firma, Cerpani: cerp));
            }

            return result;
        }

        public ActionResult Subjekt2_RegistrSmluv(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                return View((Firma: firma, Data: new List<int>() ));
            }

            return result;
        }

        public ActionResult Subjekt2_VerejneZakazky(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                //var verejneZakazky = Lib.Data.VZ.VerejnaZakazka.Searching.SimpleSearch($"holding:{firma.ICO}");

                return View((Firma: firma, Data: new List<int>()));
            }

            return result;
        }

        public ActionResult Subjekt2_InsolvencniRejstrik(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                return View((Firma: firma, Data: new List<int>()));
            }

            return result;
        }

        [NonAction()]
        private bool TryGetCompany(string id, out Firma firma, out ActionResult actionResult)
        {
            firma = null;
            
            if (string.IsNullOrWhiteSpace(id))
            {
                actionResult = RedirectToAction("Index");
                return false;
            }
                
            string ico = Util.ParseTools.NormalizeIco(id);

            if (string.IsNullOrEmpty(ico))
            {
                actionResult = RedirectToAction("Report", new { id = "1" }); ;
                return false;
            }

            firma = Firmy.Get(ico);

            if (!Firma.IsValid(firma))
            {
                if (Util.DataValidators.IsFirmaIcoZahranicni(ico))
                    actionResult = View("Subjekt_zahranicni", new Firma() { ICO = ico, Jmeno = ico });
                else
                {
                    if (!Util.DataValidators.CheckCZICO(ico))
                        actionResult = View("Subjekt_err_spatneICO");
                    else
                        actionResult = View("Subjekt_err_nezname");
                }
                return false;
            }
            if (Util.DataValidators.IsFirmaIcoZahranicni(ico))
            {
                actionResult = View("Subjekt_zahranicni", firma);
                return false;
            }

            actionResult = View(firma);
            return true;
        }

    }
}