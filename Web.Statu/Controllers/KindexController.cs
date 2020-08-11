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
            if (!(this.User.Identity.IsAuthenticated == true))
            {
                return Redirect("/");
            }
            if (
                !(this.User.IsInRole("Admin") || this.User.IsInRole("BetaTester"))
                )
            {
                return Redirect("/");
            }
            return View();
        }

        public ActionResult Detail(string id, int? rok = null)
        {
            if (!(this.User.Identity.IsAuthenticated == true))
            {
                return Redirect("/");
            }
            if (
                !(this.User.IsInRole("Admin") || this.User.IsInRole("BetaTester"))
                )
            {
                return Redirect("/");
            }
            
            if (Util.DataValidators.CheckCZICO(Util.ParseTools.NormalizeIco(id)))
            {
                var kdata = Lib.Analysis.KorupcniRiziko.KIndexData.Get(Util.ParseTools.NormalizeIco(id));
                ViewBag.ICO = id;

                var AvailableKindexYears = kdata.roky.Select(r => r.Rok).OrderByDescending(r => r).ToList();
                ViewBag.AvailableKindexYears = AvailableKindexYears;

                int? selectedYear = null;
                if (rok is null || !AvailableKindexYears.Exists(y => y == rok))
                {
                    selectedYear = AvailableKindexYears.First();
                }
                else
                {
                    selectedYear = AvailableKindexYears.Where(y => y == rok).FirstOrDefault();
                }

                ViewBag.SelectedYear = selectedYear;

                var result = kdata.roky.Where(y => y.Rok == selectedYear).FirstOrDefault();
                return View(result);
            }

            return View();
        }

        public ActionResult Debug(string id, string ico = "", int? rok = null)
        {
            if (!(this.User.Identity.IsAuthenticated == true))
            {
                return Redirect("/");
            }
            if (
                !(this.User.IsInRole("Admin") || this.User.IsInRole("BetaTester"))
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
                HlidacStatu.Lib.Analysis.KorupcniRiziko.KIndexData kdata = HlidacStatu.Lib.Analysis.KorupcniRiziko.KIndexData.Get(Util.ParseTools.NormalizeIco(id));
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
                        var kidx = HlidacStatu.Lib.Analysis.KorupcniRiziko.KIndexData.Get(Util.ParseTools.NormalizeIco(i));
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