using HlidacStatu.Lib.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HlidacStatu.Lib.Data.VZ;

namespace HlidacStatu.Web.Controllers
{
    public partial class SubjektController : GenericAuthController
    {

        public ActionResult Index(string id)
        {
            TryGetCompany(id, out var firma, out var result);
            return result;
        }

        protected override void HandleUnknownAction(string actionName)
        {
            TryGetCompany(actionName, out var firma, out var result);

            result.ExecuteResult(this.ControllerContext);
        }
        public ActionResult Dotace(string id)
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
        public ActionResult ObchodySeSponzory(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                return View(firma);
            }

            return result;
        }
        public ActionResult DalsiDatabaze(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                return View(firma);
            }

            return result;
        }

        public ActionResult RegistrSmluv(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                return View((Firma: firma, Data: new List<int>() ));
            }

            return result;
        }

        public ActionResult VerejneZakazky(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                var verejneZakazky = VerejnaZakazka.Searching.GetVZForHolding(firma.ICO);

                return View((Firma: firma, Data: verejneZakazky));
            }

            return result;
        }

        public ActionResult Odberatele(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                return View(firma);
            }

            return result;
        }
        
        public ActionResult Dodavatele(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                return View(firma);
            }

            return result;
        }


        public ActionResult InsolvencniRejstrik(string id)
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

            actionResult = View("Index",firma);
            return true;
        }

    }
}