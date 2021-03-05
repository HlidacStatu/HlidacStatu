using HlidacStatu.Lib.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HlidacStatu.Lib.Data.VZ;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Xml;
using HlidacStatu.Lib.Data.OrgStrukturyStatu;
using HlidacStatu.Lib;

namespace HlidacStatu.Web.Controllers
{
    public partial class OsobaController : GenericAuthController
    {

        public ActionResult Index(string id)
        {
            if (TryGetOsoba(id, out var osoba, out var result))
            {
                (Osoba osoba, string viewName, string title) model = (osoba, "Index", $"{osoba.FullName()}");
                return View("_osobaLayout", model);
            }

            return result;
            
        }

        protected override void HandleUnknownAction(string actionName)
        {
            //TryGetCompany(actionName, out var osoba, out var result);
            if (TryGetOsoba(actionName, out var osoba, out var result))
            {
                (Osoba osoba, string viewName, string title) model = (osoba, "Index", $"{osoba.FullName()}");
                ActionResult res = View("_osobaLayout", model);
                res.ExecuteResult(this.ControllerContext);
                return;
            }

            result.ExecuteResult(this.ControllerContext);
        }
        public ActionResult Dotace(string id)
        {
            if (TryGetOsoba(id, out var osoba, out var result))
            {
                (Osoba osoba, string viewName, string title) model = (osoba, "Dotace", $"Dotace");
                return View("_osobaLayout", model);
            }

            return result;
        }
        public ActionResult ObchodySeSponzory(string id)
        {
            if (TryGetOsoba(id, out var osoba, out var result))
            {
                (Osoba osoba, string viewName, string title) model = (osoba, "ObchodySeSponzory", $"Smlouvy se sponzory politických stran");
                return View("_osobaLayout", model);
            }

            return result;
        }
        public ActionResult DalsiDatabaze(string id)
        {
            if (TryGetOsoba(id, out var osoba, out var result))
            {
                (Osoba osoba, string viewName, string title) model = (osoba, "DalsiDatabaze", $"Další databáze");
                return View("_osobaLayout", model);
            }

            return result;
        }

        public ActionResult RegistrSmluv(string id)
        {
            if (TryGetOsoba(id, out var osoba, out var result))
            {
                (Osoba osoba, string viewName, string title) model = (osoba, "RegistrSmluv", $"Registr smluv");
                return View("_osobaLayout", model);
            }

            return result;
        }

        public ActionResult VerejneZakazky(string id)
        {
            if (TryGetOsoba(id, out var osoba, out var result))
            {
                (Osoba osoba, string viewName, string title) model = (osoba, "VerejneZakazky", $"Veřejné zakázky");
                return View("_osobaLayout", model);
            }

            return result;
        }


        public ActionResult Vazby(string id, HlidacStatu.Lib.Data.Relation.AktualnostType? aktualnost)
        {
            if (TryGetOsoba(id, out var osoba, out var result))
            {
                var popis = "Dceřinné společnosti";
                //if (firma.JsemOVM())
                //    popis = "Podřízené organizace";

                if (aktualnost.HasValue == false)
                    aktualnost = HlidacStatu.Lib.Data.Relation.AktualnostType.Nedavny;

                ViewBag.Aktualnost = aktualnost;

                (Osoba osoba, string viewName, string title) model = (osoba, "Vazby", $"{popis}");
                return View("_osobaLayout", model);
            }

            return result;
        }

        public ActionResult Odberatele(string id)
        {
            if (TryGetOsoba(id, out var osoba, out var result))
            {
                (Osoba osoba, string viewName, string title) model = (osoba, "Odberatele", $"Odběratelé");
                return View("_osobaLayout", model);
            }

            return result;
        }
        
        public ActionResult Dodavatele(string id)
        {
            if (TryGetOsoba(id, out var osoba, out var result))
            {
                (Osoba osoba, string viewName, string title) model = (osoba, "Dodavatele", $"Dodavatelé");
                return View("_osobaLayout", model);
            }

            return result;
        }

        public ActionResult OrganizacniStruktura(string id, string orgId)
        {
            //ico => id translation!
            if (!StaticData.OrganizacniStrukturyUradu.Get().TryGetValue(id, out var ossu))
            {
                return RedirectToAction("Index");
            }

            D3GraphHierarchy dataHierarchy;

            if (ossu.Count > 1)
            {
                dataHierarchy = ossu.Where(o => o.id == orgId).FirstOrDefault()?.GenerateD3DataHierarchy();
            }
            else
            {
                dataHierarchy = ossu.FirstOrDefault()?.GenerateD3DataHierarchy();
            }

            return dataHierarchy is null ? RedirectToAction("Index") : (ActionResult)View(dataHierarchy);
        }

        public ActionResult DalsiInformace(string id)
        {
            if (TryGetOsoba(id, out var osoba, out var result))
            {
                (Osoba osoba, string viewName, string title) model = (osoba, "DalsiInformace", $"Informace z registrů");
                return View("_osobaLayout", model);
            }
            return result;

        }


        public ActionResult InsolvencniRejstrik(string id)
        {
            if (TryGetOsoba(id, out var osoba, out var result))
            {
                (Osoba osoba, string viewName, string title) model = (osoba, "InsolvencniRejstrik", $"Insolvenční rejstřík" );
                return View("_osobaLayout", model);
                //return View((Firma: firma, Data: new List<int>()));
            }

            return result;
        }

        [NonAction()]
        private bool TryGetOsoba(string id, out Osoba osoba, out ActionResult actionResult)
        {
            osoba = null;
            
            if (string.IsNullOrWhiteSpace(id))
            {
                actionResult = RedirectToAction("Index", "Osoby");
                return false;
            }
                

            osoba = Osoby.GetByNameId.Get(id);

            if (osoba == null)
            {
                actionResult = View("Osoba_err_neznama");
                return false;
            }


            actionResult = View("Index",osoba);
            return true;
        }

    }
}