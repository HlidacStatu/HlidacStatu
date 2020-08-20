using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HlidacStatu.Lib.Analysis.KorupcniRiziko;
using HlidacStatu.Lib.Data;
using HlidacStatu.Lib.OCR.Api;

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
                KIndexData kdata = KIndex.Get(Util.ParseTools.NormalizeIco(id));
                ViewBag.ICO = id;

                rok = FixKindexYear(rok);
                ViewBag.SelectedYear = rok;

                return View(kdata);
            }

            return View();
        }

        public ActionResult Porovnat(string id, int? rok = null)
        {
            if (!Framework.HtmlExtensions.ShowKIndex(this.User)
                || string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/");
            }

            rok = FixKindexYear(rok);
            ViewBag.SelectedYear = rok;

            var kindexes = new List<KIndexData>();
            foreach (var i in id.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var f = Firmy.Get(Util.ParseTools.NormalizeIco(i));
                if (f.Valid)
                {
                    var kidx = KIndex.Get(Util.ParseTools.NormalizeIco(i));
                    if (kidx != null)
                        kindexes.Add(kidx);
                }
            }

            return View(kindexes);
        }

        public ActionResult Zebricek(string id, int? rok = null)
        {
            if (!Framework.HtmlExtensions.ShowKIndex(this.User)
                || string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/");
            }

            rok = FixKindexYear(rok);
            ViewBag.SelectedYear = rok;
            ViewBag.SelectedLadder = id;

            var stat = Statistics.GetStatistics(rok.Value).SubjektOrderedListKIndexCompanyAsc();

            IEnumerable<Company> result = null; 
            switch (id)
            {
                case "nejlepsi":
                    result = stat.Take(100);
                    break;
                case "nejhorsi":
                    result = stat.OrderByDescending(k => k.Value4Sort).Take(100);
                    break;
                default:
                    result = stat;
                    break;
            }
            

            return View(result);
        }

        public JsonResult FindCompany(string id)
        {
            return Json(Company.FullTextSearch(id, 10), JsonRequestBehavior.AllowGet);
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


        private int FixKindexYear(int? year)
        {
            if (year < 2017)
                return 2017;
            if (year is null || year >= DateTime.Now.Year)
                return DateTime.Now.Year - 1;
            
            return year.Value;
        }
    }
}