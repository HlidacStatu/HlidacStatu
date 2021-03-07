using Devmasters.Enums;
using HlidacStatu.Lib.Analysis.KorupcniRiziko;
using HlidacStatu.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FullTextSearch;

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
            if (!Framework.HtmlExtensions.ShowKIndex(this.User) || string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/");
            }

            if (Util.DataValidators.CheckCZICO(Util.ParseTools.NormalizeIco(id)))
            {
                ViewBag.ICO = id;

                SetViewbagSelectedYear(ref rok);

                (string id, int? rok) model = (id, rok);
                return View(model);
            }
            else
                return View("Index");


        }

        [ChildActionOnly()]
#if (!DEBUG)
        [OutputCache(VaryByParam = "id;rok", Duration = 60 * 60 * 1)]
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

                SetViewbagSelectedYear(ref rok);

                return View(kdata);
            }

            return View();
        }

        public ActionResult Backup(string id, int? rok = null)
        {
            if (!Framework.HtmlExtensions.ShowKIndex(this.User)
                || string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/");
            }

            Backup backup = KIndexData.GetPreviousVersion(id);

            SetViewbagSelectedYear(ref rok);
            ViewBag.BackupCreated = backup.Created.ToString("dd.MM.yyyy");
            ViewBag.BackupComment = backup.Comment;

            return View(backup.KIndex);

        }

        private void SetViewbagSelectedYear(ref int? rok)
        {
            rok = Consts.FixKindexYear(rok);
            ViewBag.SelectedYear = rok;
        }

        public ActionResult Porovnat(string id, int? rok = null)
        {
            if (!Framework.HtmlExtensions.ShowKIndex(this.User))
            {
                //todo: přidat base view, kde pokud nebude žádná hodnota v id, tak zobrazíme základní porovnání
                return Redirect("/");
            }

            SetViewbagSelectedYear(ref rok);

            var results = new List<SubjectWithKIndexAnnualData>();

            if (string.IsNullOrWhiteSpace(id))
                return View(results);

            
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
            

            return View(results);
        }
        public ActionResult Zebricek(string id, int? rok = null, string group = null, string kraj = null, string part = null)
        {
            if (!Framework.HtmlExtensions.ShowKIndex(this.User))
            {
                return Redirect("/");
            }

            SetViewbagSelectedYear(ref rok);
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

            SetViewbagSelectedYear(ref rok);
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
                            //.Where(s => s.KIndex > 0)
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
                            .SubjektOrderedListPartsCompanyAsc(selectedPart.Value);
                    else
                    {
                        result = Statistics.GetStatistics(rok.Value)
                            .SubjektOrderedListKIndexCompanyAsc()
                            .Select(subj => { 
                                subj.Kraj = Util.CZ_Nuts.Nace2Kraj(subj.KrajId, "(neznamý)");
                                return subj;
                            });
                    }
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

        public ActionResult RecalculateFeedback(string email, string txt, string url, string data)
        {
            // create a task, so user doesn't have to wait for anything
            System.Threading.Tasks.Task.Run(() =>
            {
                var f = Firmy.Get(Util.ParseTools.NormalizeIco(data));
                if (f.Valid)
                {

                    try
                    {
                        //string connectionString = Devmasters.Config.GetWebConfigValue("RabbitMqConnectionString");
                        //if (string.IsNullOrWhiteSpace(connectionString))
                        //    throw new Exception("Missing RabbitMqConnectionString");

                        //var message = new Q.Messages.RecalculateKindex()
                        //{
                        //    Comment = txt,
                        //    Created = DateTime.Now,
                        //    Ico = f.ICO,
                        //    User = this.User.Identity.Name
                        //};

                        //Q.Publisher.QuickPublisher.Publish(message, connectionString);
                        
                        string body = $@"
Žádost o rekalkulaci K-Indexu z hlidacstatu.cz.

Pro firmu:{f.ICO}
Od uzivatele:{email} [{this.User?.Identity?.Name}] (Emaily by se zde měli shodovat)
ke stránce:{url}

text zpravy: {txt}";
                        Util.SMTPTools.SendSimpleMailToPodpora("Žádost o rekalkulaci K-indexu", body, email);

                       

                    }
                    catch (Exception ex)
                    {
                        Util.Consts.Logger.Fatal($"Problem with SMTP. Message={ex}");
                    }
                }
            });
            return Content("");
        }

        // Used for searching
        public JsonResult FindCompany(string id)
        {
            Devmasters.Cache.LocalMemory.LocalMemoryCache<Index<SubjectNameCache>> FullTextSearchCache =
                new Devmasters.Cache.LocalMemory.LocalMemoryCache<Index<SubjectNameCache>>(TimeSpan.Zero,
                o =>
                {
                    return new Index<SubjectNameCache>(SubjectNameCache.GetCompanies().Values);
                });

            var searchResult = FullTextSearchCache.Get().Search(id, 10);

            return Json(searchResult.Select(r => r.Original), JsonRequestBehavior.AllowGet);
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
                            Label = KIndexData.KindexImageIcon(KIndexData.DetailInfo.KIndexLabelForPart(r.VelicinaPart, r.Hodnota),
                                "height: 25px",
                                showNone: true,
                                KIndexData.KIndexCommentForPart(r.VelicinaPart, kidx.ForYear(rok.Value))),
                            Value = r.Hodnota.ToString("F2")
                        }).ToList();

                    var result = new
                    {
                        UniqueId = Guid.NewGuid(),
                        Ico = kidx.Ico,
                        Jmeno = Devmasters.TextUtil.ShortenText(kidx.Jmeno, 55),
                        Kindex = KIndexData.KindexImageIcon(kidx.ForYear(rok.Value).KIndexLabel,
                                "height: 40px",
                                showNone: true),
                        Radky = radky,
                        KindexReady = kidx.ForYear(rok.Value).KIndexReady
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);

                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Feedback(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var feedback = KindexFeedback.GetById(id);

            if (feedback is null)
                return NotFound();
            
            return View(feedback);
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
#if (!DEBUG)
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
#endif
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
            var kidx = KIndex.Get(id);

            byte[] data = null;
            if (kidx != null)
            {
                KIndexData.KIndexLabelValues label;
                Util.InfoFact[] infoFacts;
                int year;
                if (rok is null)
                {
                    label = kidx.LastKIndexLabel(out int? y);
                    year = y.Value;
                    infoFacts = kidx.InfoFacts(year);
                }
                else
                {
                    year = Consts.FixKindexYear(rok);
                    label = kidx.ForYear(year)?.KIndexLabel ?? KIndexData.KIndexLabelValues.None;
                    infoFacts = kidx.InfoFacts(year);
                }


                KIndexGenerator.IndexLabel img = new KIndexGenerator.IndexLabel(Lib.StaticData.App_Data_Path);
                data = img.GenerateImageByteArray(kidx.Jmeno,
                    Util.InfoFact.RenderInfoFacts(infoFacts,
                        3,
                        takeSummary: (label == KIndexData.KIndexLabelValues.None),
                        shuffle: false,
                        " "),
                    label.ToString(),
                    year.ToString());
            }
            
            return File(data, "image/png");
        }

    }
}