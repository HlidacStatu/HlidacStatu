using Devmasters.Enums;
using HlidacStatu.Lib.Analysis.KorupcniRiziko;
using HlidacStatu.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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

                rok = Consts.FixKindexYear(rok);
                ViewBag.SelectedYear = rok;

                return View(kdata);
            }

            return View();
        }

        public ActionResult Porovnat(string id, int? rok = null)
        {
            if (!Framework.HtmlExtensions.ShowKIndex(this.User))
            {
                //todo: přidat base view, kde pokud nebude žádná hodnota v id, tak zobrazíme základní porovnání
                return Redirect("/");
            }

            if (string.IsNullOrWhiteSpace(id))
                return View("Porovnat.Index");

            rok = Consts.FixKindexYear(rok);
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
                    else
                        kindexes.Add(KIndexData.Empty(f.ICO, f.Jmeno));
                }
            }

            return View(kindexes);
        }

        public ActionResult Zebricek(string id, int? rok = null, string group = null, string kraj = null)
        {
            if (!Framework.HtmlExtensions.ShowKIndex(this.User))
            {
                return Redirect("/");
            }

            rok = Consts.FixKindexYear(rok);
            ViewBag.SelectedYear = rok;
            ViewBag.SelectedLadder = id;
            ViewBag.SelectedGroup = group;
            ViewBag.SelectedKraj = kraj;


            IEnumerable<SubjectWithKIndex> result = null;
            Lib.Data.Firma.Zatrideni.StatniOrganizaceObor oborFromId;
            if (Enum.TryParse<Firma.Zatrideni.StatniOrganizaceObor>(id, true, out oborFromId))
                id = "obor";
            switch (id?.ToLower())
            {
                case "obor":
                    result = Statistics.GetStatistics(rok.Value)
                        .SubjektOrderedListKIndexCompanyAsc(Firma.Zatrideni.Subjekty(oborFromId), showNone: true);
                    ViewBag.LadderTopic = oborFromId.ToNiceDisplayName();
                    ViewBag.LadderTitle = oborFromId.ToNiceDisplayName() + " podle K–Indexu";
                    break;

                case "nejlepsi":
                    result = Statistics.GetStatistics(rok.Value).SubjektOrderedListKIndexCompanyAsc()
                        .Take(100);
                    ViewBag.LadderTopic = "Top 100 nejlepších subjektů";
                    ViewBag.LadderTitle = "Top 100 nejlepších subjektů podle K–Indexu";
                    break;

                case "nejhorsi":
                    result = Statistics.GetStatistics(rok.Value).SubjektOrderedListKIndexCompanyAsc()
                        .OrderByDescending(k => k.KIndex)
                        .Take(100);
                    ViewBag.LadderTopic = "Top 100 nejhorších subjektů";
                    ViewBag.LadderTitle = "Top 100 nejhorších subjektů podle K–Indexu";
                    break;

                case "celkovy":
                    result = Statistics.GetStatistics(rok.Value).SubjektOrderedListKIndexCompanyAsc();
                    ViewBag.LadderTopic = "Top 100 nejlepších subjektů";
                    ViewBag.LadderTitle = "Žebříček K–Indexu";
                    break;

                case "skokani":
                    ViewBag.LadderTitle = "Top změny K–Indexu";
                    return View("Zebricek.Skokani", Statistics.GetJumpersFromBest(rok.Value).Take(100));

                default:
                    return View("Zebricek.Index");
            }

            return View(result);
        }

        // Used for searching
        public JsonResult FindCompany(string id)
        {
            return Json(SubjectNameCache.FullTextSearch(id, 10), JsonRequestBehavior.AllowGet);
        }

        public JsonResult KindexForIco(string id, int? rok = null)
        {
            rok = Consts.FixKindexYear(rok);
            var f = Firmy.Get(Util.ParseTools.NormalizeIco(id));
            if (f.Valid)
            {
                var kidx = KIndex.Get(Util.ParseTools.NormalizeIco(id));

                if (kidx != null)
                {
                    
                    var radky = kidx.ForYear(rok.Value).KIndexVypocet.Radky
                        .Select(r => new
                        {
                            VelicinaName = r.VelicinaName,
                            Label = $"<img title='K–Index {KIndexData.KIndexLabelForPart(r.VelicinaPart, r.Hodnota).ToString()} - Index korupčního rizika'  src='{KIndexData.KIndexLabelIconUrl(KIndexData.KIndexLabelForPart(r.VelicinaPart, r.Hodnota), showNone: true)}' class='kindex' style='height:25px'>",
                            Comment = KIndexData.KIndexCommentForPart(r.VelicinaPart, kidx.ForYear(rok.Value))
                        }).ToList();

                    var result = new
                    {
                        Ico = kidx.Ico,
                        Jmeno = Devmasters.Core.TextUtil.ShortenText(kidx.Jmeno, 55),
                        Kindex = $"<img title='K–Index {kidx.ForYear(rok.Value).KIndexLabel.ToString()} - Index korupčního rizika'  src='{KIndexData.KIndexLabelIconUrl(kidx.ForYear(rok.Value).KIndexLabel, showNone: true)}' class='kindex' style='width: 30px'>",
                        Radky = radky
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);

                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        //todo: What are we going to do with this?
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

            if (Util.DataValidators.CheckCZICO(Util.ParseTools.NormalizeIco(id)))
            {
                KIndexData kdata = KIndex.Get(Util.ParseTools.NormalizeIco(id));
                ViewBag.ICO = id;
                return View("Debug", kdata);
            }
            else if (id?.ToLower() == "porovnat")
            {
                List<KIndexData> kdata = new List<KIndexData>();

                foreach (var i in ico.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var f = Firmy.Get(Util.ParseTools.NormalizeIco(i));
                    if (f.Valid)
                    {
                        var kidx = KIndex.Get(Util.ParseTools.NormalizeIco(i));
                        if (kidx != null)
                            kdata.Add(kidx);
                    }
                }
                return View("Debug.Porovnat", kdata);
            }
            else
                return NotFound();
        }

        [ValidateInput(false)]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult Banner(string id, int? rok = null)
        {
            rok = Consts.FixKindexYear(rok);
            byte[] data = null;
            var kidx = KIndex.Get(id);
            if (kidx != null)
            {
                KIndexGenerator.IndexLabel img = new KIndexGenerator.IndexLabel(Lib.StaticData.App_Data_Path);
                data = img.GenerateImageByteArray(kidx.Jmeno,
                    Util.InfoFact.RenderInfoFacts(kidx.InfoFacts(), 3, true, false, " "),
                    kidx.LastKIndexLabel().ToString(),
                    rok.Value
                    );
            }
            else
            {
                KIndexGenerator.IndexLabel img = new KIndexGenerator.IndexLabel(Lib.StaticData.App_Data_Path);
                data = img.GenerateImageByteArray(kidx.Jmeno,
                    Util.InfoFact.RenderInfoFacts(kidx.InfoFacts(), 3, true, false, " "),
                    KIndexData.KIndexLabelValues.None.ToString(),
                    rok.Value
                    );
            }
            return File(data, "image/png");
        }
        
    }
}