using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HlidacStatu.Lib.Data;

namespace HlidacStatu.Web.Controllers
{
    public class KindexController : GenericAuthController
    {
        public ActionResult Index()
        {
            if (!Framework.HtmlExtensions.ShowKIndex(this.User))
            {
                return Redirect("/");
            }
            else
                return View();
        }

        public ActionResult Detail(string id, int? rok = null)
        {
            if (!Framework.HtmlExtensions.ShowKIndex(this.User)
                || string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/");
            }

            
            if (Util.DataValidators.CheckCZICO(Util.ParseTools.NormalizeIco(id)))
            {
                Lib.Analysis.KorupcniRiziko.KIndexData kdata = Lib.Analysis.KorupcniRiziko.KIndex.Get(Util.ParseTools.NormalizeIco(id));
                ViewBag.ICO = id;


                ViewBag.SelectedYear = rok ?? DateTime.Now.Year - 1;
                if (rok < 2017)
                    rok = 2017;
                if (rok >= DateTime.Now.Year)
                    rok = DateTime.Now.Year - 1;

                return View(kdata);
            }

            return View();
        }

        public ActionResult Debug(string id, string ico = "", int? rok = null)
        {
            if (!Framework.HtmlExtensions.ShowKIndex(this.User))
            {
                return Redirect("/");
            }
            if (
                !(this.User.IsInRole("Admin") || this.User.IsInRole("KIndex"))
                )
            {
                return Redirect("/");
            }

            if (string.IsNullOrEmpty(id))
            {
                return View("Debug.Start");
            }

            if (HlidacStatu.Util.DataValidators.CheckCZICO(Util.ParseTools.NormalizeIco(id)))
            {
                HlidacStatu.Lib.Analysis.KorupcniRiziko.KIndexData kdata = HlidacStatu.Lib.Analysis.KorupcniRiziko.KIndex.Get(Util.ParseTools.NormalizeIco(id));
                ViewBag.ICO = id;
                return View("Debug", kdata);
            }
            else if (id?.ToLower() == "porovnat")
            {
                List<HlidacStatu.Lib.Analysis.KorupcniRiziko.KIndexData> kdata = new List<Lib.Analysis.KorupcniRiziko.KIndexData>();

                foreach (var i in ico.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var f = Firmy.Get(Util.ParseTools.NormalizeIco(i));
                    if (f.Valid)
                    {
                        var kidx = HlidacStatu.Lib.Analysis.KorupcniRiziko.KIndex.Get(Util.ParseTools.NormalizeIco(i));
                        if (kidx != null)
                            kdata.Add(kidx);
                    }
                }
                return View("Debug.Porovnat", kdata);
            }
            else
                return NotFound();
        }

    }
}