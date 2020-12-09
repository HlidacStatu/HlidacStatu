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
    public partial class SubjektController : GenericAuthController
    {

        public ActionResult Index(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                (Firma firma, string viewName, string title) model = (firma, "Index", $"{firma.Jmeno}");
                return View("_subjektLayout", model);
            }

            return result;
            
        }

        protected override void HandleUnknownAction(string actionName)
        {
            //TryGetCompany(actionName, out var firma, out var result);
            if (TryGetCompany(actionName, out var firma, out var result))
            {
                (Firma firma, string viewName, string title) model = (firma, "Index", $"{firma.Jmeno}");
                ActionResult res = View("_subjektLayout", model);
                res.ExecuteResult(this.ControllerContext);
                return;
            }

            result.ExecuteResult(this.ControllerContext);
        }
        public ActionResult Dotace(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                (Firma firma, string viewName, string title) model = (firma, "Dotace", $"{firma.Jmeno}: Dotace");
                return View("_subjektLayout", model);
            }

            return result;
        }
        public ActionResult ObchodySeSponzory(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                (Firma firma, string viewName, string title) model = (firma, "ObchodySeSponzory", $"{firma.Jmeno}: Smlouvy se sponzory politických stran");
                return View("_subjektLayout", model);
            }

            return result;
        }
        public ActionResult DalsiDatabaze(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                (Firma firma, string viewName, string title) model = (firma, "DalsiDatabaze", $"{firma.Jmeno} v dalších databázích");
                return View("_subjektLayout", model);
            }

            return result;
        }

        public ActionResult RegistrSmluv(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                (Firma firma, string viewName, string title) model = (firma, "RegistrSmluv", $"{firma.Jmeno}: Registr smluv");
                return View("_subjektLayout", model);
            }

            return result;
        }

        public ActionResult VerejneZakazky(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                (Firma firma, string viewName, string title) model = (firma, "VerejneZakazky", $"{firma.Jmeno}: Veřejné zakázky");
                return View("_subjektLayout", model);
            }

            return result;
        }

        public ActionResult Odberatele(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                (Firma firma, string viewName, string title) model = (firma, "Odberatele", $"{firma.Jmeno}: Odběratelé");
                return View("_subjektLayout", model);
            }

            return result;
        }
        
        public ActionResult Dodavatele(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                (Firma firma, string viewName, string title) model = (firma, "Dodavatele", $"{firma.Jmeno}: Dodavatelé");
                return View("_subjektLayout", model);
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
                dataHierarchy = ossu.FirstOrDefault().GenerateD3DataHierarchy();
            }

            return dataHierarchy is null ? RedirectToAction("Index") : (ActionResult)View(dataHierarchy);
        }

        public ActionResult DalsiInformace(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                (Firma firma, string viewName, string title) model = (firma, "DalsiInformace", $"{firma.Jmeno}: Informace z registrů");
                return View("_subjektLayout", model);
            }
            return result;

        }


        public ActionResult InsolvencniRejstrik(string id)
        {
            if (TryGetCompany(id, out var firma, out var result))
            {
                (Firma firma, string viewName, string title) model = (firma, "InsolvencniRejstrik", $"{firma.Jmeno}: Insolvenční rejstřík" );
                return View("_subjektLayout", model);
                //return View((Firma: firma, Data: new List<int>()));
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