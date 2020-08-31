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
            if (!Framework.HtmlExtensions.ShowKIndex(this.User)    || string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/");
            }

            if (Util.DataValidators.CheckCZICO(Util.ParseTools.NormalizeIco(id)))
            {
                ViewBag.ICO = id;

                ViewBag.SelectedYear = rok;

                (string id, int? rok) model = (id, rok);
                return View(model);
            }
            else
                return View("Index");


        }

        [ChildActionOnly()]
#if (!DEBUG)
        [OutputCache(VaryByParam = "id;rok;group;kraj", Duration = 60 * 60 * 1)]
#endif
        public ActionResult Detail_child(string id, int? rok = null)
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

            var results = new List<SubjectWithKIndexAnnualData>();
            if (Enum.TryParse(id, true, out Firma.Zatrideni.StatniOrganizaceObor oborFromId))
            {
                var subjects = Statistics.GetStatistics(rok.Value)
                            .SubjektOrderedListKIndexCompanyAsc(Firma.Zatrideni.Subjekty(oborFromId), showNone: true);

                foreach (var subject in subjects)
                {
                    SubjectWithKIndexAnnualData data = new SubjectWithKIndexAnnualData((Firma.Zatrideni.Item)subject);
                    try
                    {
                        data.PopulateWithAnnualData(rok.Value);
                    }
                    catch (Exception)
                    {
                        // chybí ičo v objeku data
                        continue;
                    }
                    results.Add(data);
                }
            }
            else
            {
                foreach (var i in id.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var f = Firmy.Get(Util.ParseTools.NormalizeIco(i));
                    if (f.Valid)
                    {
                        SubjectWithKIndexAnnualData data = new SubjectWithKIndexAnnualData()
                        {
                            Ico = f.ICO,
                            Jmeno = f.Jmeno
                        };
                        try
                        {
                            data.PopulateWithAnnualData(rok.Value);
                        }
                        catch (Exception)
                        {
                            // chybí ičo v objeku data
                            continue;
                        }
                        results.Add(data);
                    }
                }
            }

            return View(results);
        }
        public ActionResult Zebricek(string id, int? rok = null, string group = null, string kraj = null, string part = null)
        {
            if (!Framework.HtmlExtensions.ShowKIndex(this.User))
            {
                return Redirect("/");
            }

            ViewBag.SelectedYear = rok;
            ViewBag.SelectedLadder = id;
            ViewBag.SelectedGroup = group;
            ViewBag.SelectedKraj = kraj;


            Lib.Data.Firma.Zatrideni.StatniOrganizaceObor oborFromId;
            if (Enum.TryParse<Firma.Zatrideni.StatniOrganizaceObor>(id, true, out oborFromId))
                id = "obor";


            if (Enum.TryParse<KIndexData.KIndexParts>(part, out KIndexData.KIndexParts ePart))
            {
                part = ePart.ToString();
            }
            else
                part = "";

            switch (id?.ToLower())
            {
                case "obor":
                    ViewBag.LadderTopic = oborFromId.ToNiceDisplayName();
                    ViewBag.LadderTitle = oborFromId.ToNiceDisplayName() + " podle K–Indexu";
                    break;

                case "nejlepsi":
                    ViewBag.LadderTopic = "Top 100 nejlepších subjektů";
                    ViewBag.LadderTitle = "Top 100 nejlepších subjektů podle K–Indexu";
                    break;

                case "nejhorsi":
                    ViewBag.LadderTopic = "Nejhůře hodnocené úřady a organizace";
                    ViewBag.LadderTitle = "Nejhůře hodnocené úřady a organizace podle K–Indexu";
                    break;

                case "celkovy":
                    ViewBag.LadderTopic = "Kompletní žebříček úřadů a organizací";
                    ViewBag.LadderTitle = "Kompletní žebříček úřadů a organizací podle K–Indexu";
                    break;

                case "skokani":
                    ViewBag.LadderTitle = "Úřady a organizace, kterým se hodnocení K-Indexu meziročně nejvíce změnilo";
                    break;
                default:
                    break;
            }


            (string id, int? rok, string group, string kraj) model = ((string)ViewBag.SelectedLadder, rok, 
                group,kraj );
            return View(model);
        }

        [ChildActionOnly()]
#if (!DEBUG)
        [OutputCache(VaryByParam = "id;rok;group;kraj;part", Duration = 60 * 60 * 1)]
#endif
        public ActionResult Zebricek_child(string id, int? rok = null, string group = null, string kraj = null, string part = null)
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

            KIndexData.KIndexParts? selectedPart = null;
            if (Enum.TryParse<KIndexData.KIndexParts>(part, out KIndexData.KIndexParts ePart))
                selectedPart = (KIndexData.KIndexParts)ePart;
            ViewBag.SelectedPart = selectedPart;

            IEnumerable<SubjectWithKIndex> result = null;
            Lib.Data.Firma.Zatrideni.StatniOrganizaceObor oborFromId;
            if (Enum.TryParse<Firma.Zatrideni.StatniOrganizaceObor>(id, true, out oborFromId))
                id = "obor";

            switch (id?.ToLower())
            {
                case "obor":
                    if (selectedPart.HasValue)
                        result = Statistics.GetStatistics(rok.Value)
                            .SubjektOrderedListPartsCompanyAsc(selectedPart.Value, Firma.Zatrideni.Subjekty(oborFromId), showNone: true);
                    else
                        result = Statistics.GetStatistics(rok.Value)
                            .SubjektOrderedListKIndexCompanyAsc(Firma.Zatrideni.Subjekty(oborFromId), showNone: true);
                    ViewBag.LadderTopic = oborFromId.ToNiceDisplayName();
                    ViewBag.LadderTitle = oborFromId.ToNiceDisplayName() + " podle K–Indexu";
                    break;

                case "nejlepsi":
                    if (selectedPart.HasValue)
                        result = Statistics.GetStatistics(rok.Value)
                            .SubjektOrderedListPartsCompanyAsc(selectedPart.Value)
                            .Take(100);
                    else
                        result = Statistics.GetStatistics(rok.Value).SubjektOrderedListKIndexCompanyAsc()
                        .Take(100);
                    ViewBag.LadderTopic = "Top 100 nejlepších subjektů";
                    ViewBag.LadderTitle = "Top 100 nejlepších subjektů podle K–Indexu";
                    break;

                case "nejhorsi":
                    if (selectedPart.HasValue)
                        result = Statistics.GetStatistics(rok.Value)
                            .SubjektOrderedListPartsCompanyAsc(selectedPart.Value)
                            .OrderByDescending(k => k.KIndex)
                            .Take(100);
                    else
                        result = Statistics.GetStatistics(rok.Value).SubjektOrderedListKIndexCompanyAsc()
                        .OrderByDescending(k => k.KIndex)
                        .Take(100);
                    ViewBag.LadderTopic = "Nejhůře hodnocené úřady a organizace";
                    ViewBag.LadderTitle = "Nejhůře hodnocené úřady a organizace podle K–Indexu";
                    break;

                case "celkovy":
                    if (selectedPart.HasValue)
                        result = Statistics.GetStatistics(rok.Value)
                            .SubjektOrderedListPartsCompanyAsc(selectedPart.Value)
                            .Take(100);
                    else
                        result = Statistics.GetStatistics(rok.Value).SubjektOrderedListKIndexCompanyAsc();
                    ViewBag.LadderTopic = "Kompletní žebříček úřadů a organizací";
                    ViewBag.LadderTitle = "Kompletní žebříček úřadů a organizací podle K–Indexu";
                    break;

                case "skokani":
                    ViewBag.LadderTitle = "Úřady a organizace, kterým se hodnocení K-Indexu meziročně nejvíce změnilo";
                    return View("Zebricek.Skokani", Statistics.GetJumpersFromBest(rok.Value).Take(200));

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
                            Label = KIndexData.KindexImageIcon(KIndexData.KIndexLabelForPart(r.VelicinaPart, r.Hodnota),
                                "height: 25px",
                                showNone: true,
                                KIndexData.KIndexCommentForPart(r.VelicinaPart, kidx.ForYear(rok.Value))),
                        }).ToList();

                    var result = new
                    {
                        Ico = kidx.Ico,
                        Jmeno = Devmasters.Core.TextUtil.ShortenText(kidx.Jmeno, 55),
                        Kindex = KIndexData.KindexImageIcon(kidx.ForYear(rok.Value).KIndexLabel,
                                "height: 40px",
                                showNone: true),
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
        public ActionResult PercentileBanner(string id, int? part = null, int? rok = null)
        {
            rok = Consts.FixKindexYear(rok);
            var kidx = KIndex.Get(id);
            if (kidx != null)
            {

                Statistics stat = Statistics.GetStatistics(rok.Value);

                KIndexData.KIndexParts? kpart = (KIndexData.KIndexParts?)part;
                if (kpart.HasValue)
                {
                    var val = kidx.ForYear(rok.Value).KIndexVypocet.Radky.FirstOrDefault(m => m.VelicinaPart == kpart.Value)?.Hodnota ?? 0;

                    return Content(
                        new HlidacStatu.KIndexGenerator.PercentileBanner(
                            val,
                            stat.Percentil(1, kpart.Value),
                            stat.Percentil(10, kpart.Value),
                            stat.Percentil(25, kpart.Value),
                            stat.Percentil(50, kpart.Value),
                            stat.Percentil(75, kpart.Value),
                            stat.Percentil(90, kpart.Value),
                            stat.Percentil(99, kpart.Value),
                            Lib.StaticData.App_Data_Path).Svg()
                        , "image/svg+xml");


                }
                else
                {
                    var val = kidx.ForYear(rok.Value).KIndex;

                    return Content(
                        new HlidacStatu.KIndexGenerator.PercentileBanner(
                            val,
                            stat.Percentil(1),
                            stat.Percentil(10),
                            stat.Percentil(25),
                            stat.Percentil(50),
                            stat.Percentil(75),
                            stat.Percentil(90),
                            stat.Percentil(99),
                            Lib.StaticData.App_Data_Path).Svg()
                        , "image/svg+xml");

                }

            }
            else
                return Content("", "image/svg+xml");
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
                    Util.InfoFact.RenderInfoFacts(kidx.InfoFacts(), 3, false, false, " "),
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