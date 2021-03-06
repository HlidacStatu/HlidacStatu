﻿using Devmasters;
using Devmasters.Enums;

using HlidacStatu.Lib;
using HlidacStatu.Lib.Data;
using HlidacStatu.Lib.Data.VZ;
using HlidacStatu.Lib.Render;
using HlidacStatu.Web.Framework;
using HlidacStatu.Web.Models;

using Microsoft.ApplicationInsights;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace HlidacStatu.Web.Controllers
{
    [RobotFromIP]
    public partial class HomeController : GenericAuthController
    {

        public static string[] DontIndexICOS = null;
        public static string[] DontIndexOsoby = null;
        static HomeController()
        {
            DontIndexOsoby = Devmasters.Config
                 .GetWebConfigValue("DontIndexOsoby")
                 .Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries)
                 .Select(m => m.ToLower())
                 .ToArray();
            DontIndexICOS = Devmasters.Config
                 .GetWebConfigValue("DontIndexFirmy")
                 .Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries)
                 .Select(m => m.ToLower())
                 .ToArray();

        }

        public HomeController()
        {

        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            : base(userManager, signInManager)
        {
        }


        public ActionResult Analyza(string id, string p, string q, string title, string description, string moreUrl)
        {
            return View("Analyza");
        }

#if (!DEBUG)
        [OutputCache(VaryByParam = "id;p;q;title;description;moreUrl;embed", Duration = 60 * 60 * 12)]
#endif
        [ChildActionOnly]
        public ActionResult Analyza_Child(string id, string p, string q, string title, string description, string moreUrl)
        {
            var model = new HlidacStatu.Lib.Analysis.TemplatedQuery() { Query = q, Text = title, Description = description };

            if (string.IsNullOrEmpty(id))
                return View("AnalyzaStart", model);


            if (StaticData.Afery.ContainsKey(p?.ToLower() ?? ""))
                model = StaticData.Afery[p.ToLower()];
            else
            {
                model = new HlidacStatu.Lib.Analysis.TemplatedQuery() { Query = q, Text = title, Description = description };
                if (System.Uri.TryCreate(moreUrl, UriKind.Absolute, out var uri))
                    model.Links = new HlidacStatu.Lib.Analysis.TemplatedQuery.AHref[] {
                        new HlidacStatu.Lib.Analysis.TemplatedQuery.AHref(uri.AbsoluteUri,"více informací")
                    };
            }
            if (string.IsNullOrEmpty(id))
                id = "platcu";

            model.NameOfView = "Analyza" + id;
            return View(model.NameOfView, model);
        }



        public ActionResult Photo(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                string f = Request.QueryString["f"];
                if (string.IsNullOrEmpty(f) || f?.Contains("..") == true)
                    return File(HlidacStatu.Lib.Init.WebAppRoot + @"Content\Img\personNoPhoto.png", "image/png");

                var nameId = Devmasters.RegexUtil.GetRegexGroupValue(f, @"\d{2} \\ (?<nameid>\w* - \w* (-\d{1,3})?) - small\.jpg", "nameid");
                if (string.IsNullOrEmpty(nameId))
                    return File(HlidacStatu.Lib.Init.WebAppRoot + @"Content\Img\personNoPhoto.png", "image/png");
                else
                    return Redirect("/photo/" + nameId);
            }
            var o = HlidacStatu.Lib.Data.Osoby.GetByNameId.Get(id);
            if (o == null)
                return File(HlidacStatu.Lib.Init.WebAppRoot + @"Content\Img\personNoPhoto.png", "image/png");
            else
            {
                if (o.HasPhoto())
                    return File(o.GetPhotoPath(), "image/jpg");
                else
                    return File(HlidacStatu.Lib.Init.WebAppRoot + @"Content\Img\personNoPhoto.png", "image/png");
            }

        }

        public ActionResult ProvozniPodminky()
        {
            return RedirectPermanent("/texty/provoznipodminky");
        }

        public ActionResult Bot()
        {
            return View();
        }

#if (!DEBUG)
        [OutputCache(VaryByParam = "*", Duration = 60 * 60 * 1)]
#endif
        public ActionResult PorovnatSubjekty(string id, string ico, string ds, string title, int? width, string specialtype, string specialvalue, string part)
        {
            if (id == "special")
            {
                if (specialtype != null)
                {
                    var specval = specialvalue;
                    if (!string.IsNullOrEmpty(specval) && StaticData.MestaPodleKraju.ContainsKey(specval))
                    {
                        ds = StaticData.MestaPodleKraju[specval].Aggregate((f, s) => f + "," + s);
                    }
                }
            }


            if (!string.IsNullOrEmpty(ico) || !string.IsNullOrEmpty(ds))
            {
                List<string> icos = new List<string>();

                if (!string.IsNullOrEmpty(ico))
                {
                    foreach (var i in ico.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var f = Firmy.Get(i);
                        if (f.Valid)
                            icos.Add(f.ICO);
                    }
                }
                if (!string.IsNullOrEmpty(ds))
                {
                    foreach (var i in ds.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var f = Firmy.GetByDS(i);
                        if (f.Valid)
                            icos.Add(f.ICO);
                    }
                }
                return View(icos.Distinct().ToArray());
            }
            else
                return Content("<h3>Neplatné parametry stránky</h3>");
        }

        public ActionResult Porovnat(string id, string ico, string ds, string title, int? width, string specialtype, string specialvalue)
        {
            if (id == "special" || id == "subjekty")
            {
                ViewBag.NameOfView = "PorovnatSubjekty";
                return View("Porovnat_child");
            }


            return View("Porovnat");
        }


        public ActionResult Tmp()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        [ActionName("K-Index")]
        public ActionResult Kindex()
        {
            return RedirectPermanent("/kindex");
        }

        public ActionResult NovyClenTeamu()
        {
            return View();
        }

        public ActionResult PridatSe()
        {
            return RedirectPermanent("https://www.hlidacstatu.cz/texty/pridejte-se/");
            //return View(new Models.PridatSeModel());
        }


        [HttpPost]
        public ActionResult PridatSe(Models.PridatSeModel model, FormCollection form)
        {
            return RedirectPermanent("https://www.hlidacstatu.cz/texty/pridejte-se/");
            
        }

        public ActionResult VerejneZakazky(string q)
        {
            return Redirect("/VerejneZakazky");
        }



        public ActionResult Licence()
        {
            return RedirectPermanent("/texty/licence");
        }

        public ActionResult OServeru()
        {

            return RedirectPermanent("/texty/o-serveru");
        }

        public ActionResult Zatmivaci()
        {

            return View();
        }
        public ActionResult VisitImg(string id)
        {
            try
            {
                var path = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(id));
                Framework.Visit.AddVisit(path,
                    Framework.Visit.IsCrawler(Request.UserAgent) ?
                        Visit.VisitChannel.Crawler : Visit.VisitChannel.Web);
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Info("VisitImg base64 encoding error", e);
            }

            return File(HlidacStatu.Lib.Init.WebAppRoot + @"Content\Img\1x1.png", "image/png");
        }

        public ActionResult Kontakt()
        {
            return RedirectPermanent("/texty/kontakt");
        }


        public ActionResult sendFeedbackMail(string typ, string email, string txt, string url, bool? auth, string data)
        {
            string to = "podpora@hlidacstatu.cz";
            string subject = "Zprava z HlidacStatu.cz: " + typ;
            if (!string.IsNullOrEmpty(data))
            {
                if (data.StartsWith("dataset|"))
                {
                    data = data.Replace("dataset|", "");
                    try
                    {
                        HlidacStatu.Lib.Data.External.DataSets.DataSet ds = HlidacStatu.Lib.Data.External.DataSets.DataSet.CachedDatasets.Get(data);
                        to = ds.Registration().createdBy;
                        subject = subject + $" ohledně databáze {ds.DatasetId}";
                    }
                    catch (Exception)
                    {
                        return Content("");
                    }
                }
            }

            if (auth == false || (auth == true && this.User?.Identity?.IsAuthenticated == true))
            {
                if (!string.IsNullOrEmpty(email) && Devmasters.TextUtil.IsValidEmail(email)
                    && !string.IsNullOrEmpty(to) && Devmasters.TextUtil.IsValidEmail(to)
                    )
                {
                    try
                    {
                        string body = $@"
Zpráva z hlidacstatu.cz.

Typ zpravy:{typ}
Od uzivatele:{email} 
ke stránce:{url}

text zpravy: {txt}";
                        HlidacStatu.Util.SMTPTools.SendSimpleMailToPodpora(subject, body, email);

                    }
                    catch (Exception ex)
                    {

                        HlidacStatu.Util.Consts.Logger.Fatal(string.Format("{0}|{1}|{2}", email, url, txt, ex));
                        return Content("");
                    }
                }
            }
            return Content("");
        }

        public ActionResult ClassificationFeedback(string typ, string email, string txt, string url, string data)
        {
            // create a task, so user doesn't have to wait for anything
            System.Threading.Tasks.Task.Run(() =>
            {

                try
                {
                    string subject = "Zprava z HlidacStatu.cz: " + typ;
                    string body = $@"
Návrh na opravu klasifikace.

Od uzivatele:{email} 
ke stránce:{url}

text zpravy: {txt}

";
                    HlidacStatu.Util.SMTPTools.SendSimpleMailToPodpora(subject, body, email);

                    string classificationExplanation = Smlouva.SClassification.GetClassificationExplanation(data);

                    string explain = $"explain result: {classificationExplanation} ";

                    HlidacStatu.Util.SMTPTools.SendEmail(subject, "", body + explain, "michal@michalblaha.cz");
                    HlidacStatu.Util.SMTPTools.SendEmail(subject, "", body + explain, "petr@hlidacstatu.cz");
                    HlidacStatu.Util.SMTPTools.SendEmail(subject, "", body + explain, "lenka@hlidacstatu.cz");
                }
                catch (Exception ex)
                {
                    Util.Consts.Logger.Fatal(string.Format("{0}|{1}|{2}", email, url, txt, ex));
                }

                try
                {
                    string connectionString = Devmasters.Config.GetWebConfigValue("RabbitMqConnectionString");
                    if (string.IsNullOrWhiteSpace(connectionString))
                        throw new Exception("Missing RabbitMqConnectionString");

                    var message = new Q.Messages.ClassificationFeedback()
                    {
                        FeedbackEmail = email,
                        IdSmlouvy = data,
                        ProposedCategories = txt
                    };

                    Q.Publisher.QuickPublisher.Publish(message, connectionString);
                }
                catch (Exception ex)
                {
                    Util.Consts.Logger.Fatal($"Problem sending data to ClassificationFeedback queue. Message={ex}");
                }


            });

            return Content("");
        }




        public ActionResult TextSmlouvy(string Id, string hash, string secret)
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(hash))
                return NotFound();

            var model = HlidacStatu.Lib.Data.Smlouva.Load(Id);
            if (model == null)
            {
                return NotFound();
            }
            var priloha = model.Prilohy?.FirstOrDefault(m => m.hash.Value == hash);
            if (priloha == null)
            {
                return NotFound();
            }

            if (model.znepristupnenaSmlouva())
            {
                if (string.IsNullOrEmpty(secret)) //pokus jak se dostat k znepristupnene priloze
                    return Redirect(model.GetUrl(false)); //jdi na detail smlouvy
                else if (this.User?.Identity?.IsAuthenticated == false) //neni zalogovany
                    return Redirect(model.GetUrl(false)); //jdi na detail smlouvy
                else
                {
                    if (priloha.LimitedAccessSecret(this.User.Identity.GetUserName()) != secret)
                        return Redirect(model.GetUrl(false)); //jdi na detail smlouvy
                }
            }

            ViewBag.hashValue = hash;
            return View(model);
        }
        public ActionResult KopiePrilohy(string Id, string hash, string secret)
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(hash))
                return NotFound();

            var model = HlidacStatu.Lib.Data.Smlouva.Load(Id);
            if (model == null)
            {
                return NotFound();
            }

            var priloha = model.Prilohy?.FirstOrDefault(m => m.hash.Value == hash);
            if (priloha == null)
            {
                return NotFound();
            }

            if (model.znepristupnenaSmlouva())
            {
                if (this.AuthUser()?.IsInRole("Admin") == false)
                {
                    if (string.IsNullOrEmpty(secret)) //pokus jak se dostat k znepristupnene priloze
                        return Redirect(model.GetUrl(false)); //jdi na detail smlouvy
                    else if (this.User?.Identity?.IsAuthenticated == false) //neni zalogovany
                        return Redirect(model.GetUrl(false)); //jdi na detail smlouvy
                    else if (this.AuthUser()?.EmailConfirmed == false)
                    {
                        return Redirect(model.GetUrl(false)); //jdi na detail smlouvy
                    }
                    else
                    {
                        if (priloha.LimitedAccessSecret(this.User.Identity.GetUserName()) != secret)
                            return Redirect(model.GetUrl(false)); //jdi na detail smlouvy
                    }
                }
            }
            var fn = HlidacStatu.Lib.Init.PrilohaLocalCopy.GetFullPath(model, priloha);
            if (HlidacStatu.Lib.OCR.DocTools.HasPDFHeader(fn))
            {
                return File(fn, "application/pdf", string.IsNullOrWhiteSpace(priloha.nazevSouboru) ? $"{model.Id}_smlouva.pdf" : priloha.nazevSouboru);
            }
            else
                return File(fn,
                    string.IsNullOrWhiteSpace(priloha.ContentType) ? "application/octet-stream" : priloha.ContentType,
                    string.IsNullOrWhiteSpace(priloha.nazevSouboru) ? "priloha" : priloha.nazevSouboru);

        }

        public ActionResult ZobrazD()
        {
            var deb = Framework.WebContextInfo.GetFullInfoString(this.HttpContext);
            Lib.Watchdogs.Email.SendEmail("michal@michalblaha.cz", "ZobrazD", new Lib.Watchdogs.RenderedContent() { ContentText = deb });

            return View();
        }

        public ActionResult PoliticiChybejici()
        {

            return View();
        }

        public ActionResult Api(string id)
        {
            return RedirectToActionPermanent("Index", "ApiV1");
        }


        public ActionResult Tmp1()
        {
            using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
            {
                var f = db.Invoices.First();
                return File(f.PrintToPDF(), "application/pdf");
                //faktura = f.Print2Html();
            }

        }


        public ActionResult JsonDoc()
        {
            return View();
        }
        public ActionResult Smlouvy()
        {
            return View();
        }
        public ActionResult ORegistru()
        {
            return RedirectPermanent("https://www.hlidacstatu.cz/texty/o-registru");
        }

        public ActionResult Afery()
        {
            return View();
        }


        public ActionResult PridatPolitika(Models.NewPersonModel model)
        {
            return View(model);
        }
        [HttpPost]
        public ActionResult PridatPolitika(Models.NewPersonModel model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Thanks = true;
                //    <b>jmeno \t prijmeni \t datum narozeni</b> \t titul_pred_jmenem \t titul_po_jmene \t popisek \t zdroj \t moreInfoUrl

                //string content = model.jmeno + "\t"
                //    + model.prijmeni + "\t"
                //    + (model.narozeni.HasValue ? model.narozeni.Value.ToString("d.M.yyyy") : "") + "\t"
                //    + model.titulPred + "\t"
                //    + model.titulPo + "\t"
                //    + model.pozice + " " + model.strana + "\t"
                //    + model.zdroj + "\n\n\n";

                //DateTime? narozeni = null;
                //DateTime tmpNar = DateTime.MinValue;
                //if (model.narozeni.HasValue && DateTime.TryParseExact(model.narozeni.Value, "d.M.yyyy", HlidacStatu.Util.Consts.czCulture, System.Globalization.DateTimeStyles.AssumeLocal, out tmpNar))
                //{

                //}
                try
                {
                    if (this.User.IsInRole("Admin"))
                    {
                    }
                }
                catch (Exception)
                {

                    //HlidacStatu.Util.Consts.Logger.Fatal(string.Format("Pridat Politik : {0}", content), ex);
                }

                return View(new Models.NewPersonModel());
            }
            return View(model);
        }

        public ActionResult HledatVice()
        {
            return RedirectToActionPermanent("SnadneHledani");
        }
        public ActionResult SnadneHledani()
        {
            string[] splitChars = new string[] { " " };
            var qs = this.Request.QueryString;

            string query = "";


            if (!string.IsNullOrWhiteSpace(qs["alltxt"]))
            {
                query += " " + qs["alltxt"];
            }
            if (!string.IsNullOrWhiteSpace(qs["exacttxt"]))
            {
                query += " \"" + qs["exacttxt"] + "\"";
            }
            if (!string.IsNullOrWhiteSpace(qs["anytxt"]))
            {
                query += " ("
                    + qs["anytxt"].Split(splitChars, StringSplitOptions.RemoveEmptyEntries).Aggregate((f, s) => f + " OR " + s)
                    + ")";
            }
            if (!string.IsNullOrWhiteSpace(qs["nonetxt"]))
            {
                query += " " + qs["nonetxt"].Split(splitChars, StringSplitOptions.RemoveEmptyEntries).Select(s => s.StartsWith("-") ? s : "-" + s).Aggregate((f, s) => f + " " + s);
            }
            if (!string.IsNullOrWhiteSpace(qs["textsmlouvy"]))
            {
                query += " textSmlouvy:\"" + qs["textsmlouvy"].Trim() + "\"";
            }


            List<KeyValuePair<string, string>> platce = new List<KeyValuePair<string, string>>();
            if (qs["icoPlatce"] != null)
                foreach (var val in qs["icoPlatce"]
                    .Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    )
                { platce.Add(new KeyValuePair<string, string>("icoPlatce", val)); }


            if (qs["dsPlatce"] != null)
                foreach (var val in qs["dsPlatce"]
                        .Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    )
                { platce.Add(new KeyValuePair<string, string>("dsPlatce", val)); }


            platce.Add(new KeyValuePair<string, string>("jmenoPlatce", qs["jmenoPlatce"]));
            if (platce.Count(m => !string.IsNullOrWhiteSpace(m.Value)) > 1)
            { // into ()
                query += " ("
                        + platce.Where(m => !string.IsNullOrWhiteSpace(m.Value)).Select(m => m.Key + ":" + m.Value).Aggregate((f, s) => f + " OR " + s)
                        + ")";
            }
            else if (platce.Count(m => !string.IsNullOrWhiteSpace(m.Value)) == 1)
            {
                query += " " + platce.Where(m => !string.IsNullOrWhiteSpace(m.Value)).Select(m => m.Key + ":" + m.Value).First();
            }


            List<KeyValuePair<string, string>> prijemce = new List<KeyValuePair<string, string>>();
            if (qs["icoPrijemce"] != null)
                foreach (var val in qs["icoPrijemce"]
                    .Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    )
                { prijemce.Add(new KeyValuePair<string, string>("icoPrijemce", val)); }


            if (qs["dsPrijemce"] != null)
                foreach (var val in qs["dsPrijemce"]
                        .Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    )
                { prijemce.Add(new KeyValuePair<string, string>("dsPrijemce", val)); }



            prijemce.Add(new KeyValuePair<string, string>("jmenoprijemce", qs["jmenoprijemce"]));
            if (prijemce.Count(m => !string.IsNullOrWhiteSpace(m.Value)) > 1)
            { // into ()
                query += " ("
                        + prijemce.Where(m => !string.IsNullOrWhiteSpace(m.Value)).Select(m => m.Key + ":" + m.Value).Aggregate((f, s) => f + " OR " + s)
                        + ")";
            }
            else if (prijemce.Count(m => !string.IsNullOrWhiteSpace(m.Value)) == 1)
            {
                query += " " + prijemce.Where(m => !string.IsNullOrWhiteSpace(m.Value)).Select(m => m.Key + ":" + m.Value).First();
            }


            if (!string.IsNullOrWhiteSpace(qs["cenaOd"]) && Devmasters.TextUtil.IsNumeric(qs["cenaOd"]))
                query += " cena:>" + qs["cenaOd"];

            if (!string.IsNullOrWhiteSpace(qs["cenaDo"]) && Devmasters.TextUtil.IsNumeric(qs["cenaDo"]))
                query += " cena:<" + qs["cenaDo"];

            if (!string.IsNullOrWhiteSpace(qs["zverejnenoOd"]) && !string.IsNullOrWhiteSpace(qs["zverejnenoDo"]))
            {
                query += $" zverejneno:[{qs["zverejnenoOd"]} TO {qs["zverejnenoDo"]}]";
            }
            else if (!string.IsNullOrWhiteSpace(qs["zverejnenoOd"]))
            {
                query += $" zverejneno:[{qs["zverejnenoOd"]} TO *]";
            }
            else if (!string.IsNullOrWhiteSpace(qs["zverejnenoDo"]))
            {
                query += $" zverejneno:[* TO {qs["zverejnenoDo"]}]";
            }

            if (!string.IsNullOrWhiteSpace(qs["podepsanoOd"]) && !string.IsNullOrWhiteSpace(qs["podepsanoDo"]))
            {
                query += $" podepsano:[{qs["podepsanoOd"]} TO {qs["podepsanoDo"]}]";
            }
            else if (!string.IsNullOrWhiteSpace(qs["podepsanoOd"]))
            {
                query += $" podepsano:[{qs["podepsanoOd"]} TO *]";
            }
            else if (!string.IsNullOrWhiteSpace(qs["podepsanoDo"]))
            {
                query += $" podepsano:[* TO {qs["podepsanoDo"]}]";
            }


            if (!string.IsNullOrWhiteSpace(qs["osobaNamedId"]))
            {
                query += " osobaid:" + qs["osobaNamedId"];
            }
            if (!string.IsNullOrWhiteSpace(qs["holding"]))
            {
                query += " holding:" + qs["holding"];
            }
            query = query.Trim();

            if (!string.IsNullOrWhiteSpace(query))
            {
                if (!string.IsNullOrEmpty(qs["hledatvse"]))
                    return Redirect("/Hledat?q=" + System.Net.WebUtility.UrlEncode(query));

                if (!string.IsNullOrEmpty(qs["hledatsmlouvy"]))
                    return Redirect("/HledatSmlouvy?q=" + System.Net.WebUtility.UrlEncode(query));

                if (!string.IsNullOrEmpty(qs["hledatvz"]))
                    return Redirect("/VerejneZakazky/Hledat?q=" + System.Net.WebUtility.UrlEncode(query));
            }

            return View();
        }

        [OutputCache(Duration = 60 * 60 * 3)]
        [ChildActionOnly]
        public ActionResult Adresar_child()
        {
            return View();
        }

        public ActionResult Adresar()
        {
            return View();
        }

        [ChildActionOnly]
#if (!DEBUG)
        [OutputCache(Duration = 60 * 60 * 6, VaryByParam = "id;aktualnost;embed;NameOfView;auth")]
#endif
        public ActionResult Osoba_child(string Id, Osoba model, HlidacStatu.Lib.Data.Relation.AktualnostType aktualnost, string NameOfView)
        {
            ViewBag.Aktualnost = aktualnost;

            return View(NameOfView, model);
        }

        

        public ActionResult Politici(string prefix)
        {
            return RedirectPermanent(Url.Action("Index", "Osoby", new { prefix = prefix }));

            //List<Lib.Data.Osoba> model = HlidacStatu.Lib.StaticData.PolitickyAktivni.Get();
            //return View(model);

        }

#if (!DEBUG)
        //[OutputCache(VaryByParam ="id;aktualnost;embed", Duration =60*6)]
#endif
        public ActionResult Politik(string Id, HlidacStatu.Lib.Data.Relation.AktualnostType? aktualnost)
        {
            return RedirectPermanent(Url.Action("Index","Osoba" , new { Id = Id, aktualnost = aktualnost }));
        }

        public ActionResult PolitikVazby(string Id, HlidacStatu.Lib.Data.Relation.AktualnostType? aktualnost)
        {
            return RedirectPermanent(Url.Action("Vazby","Osoba", new { Id = Id, aktualnost = aktualnost }));
        }



#if (!DEBUG)
        [OutputCache(Duration = 60 * 60 * 1, VaryByParam = "id;embed;nameOfView")]
#endif
        [ChildActionOnly()]
        public ActionResult Detail_Child(string Id, Smlouva model, string nameOfView)
        {
            return View(nameOfView, model);
        }

        public ActionResult Detail(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
                return NotFound();

            var model = HlidacStatu.Lib.Data.Smlouva.Load(Id);
            if (model == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(this.Request.QueryString["qs"]))
            {
                var findSm = Lib.Data.Smlouva.Search.SimpleSearch($"_id:\"{model.Id}\" AND ({this.Request.QueryString["qs"]})", 1, 1,
                    Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, withHighlighting: true);
                if (findSm.Total > 0)
                    ViewBag.Highlighting = findSm.ElasticResults.Hits.First().Highlight;

            }
            return View(model);
        }

        public ActionResult HledatFirmy(string q, int? page = 1)
        {
            var model = Firma.Search.SimpleSearch(q,page.Value,50);
            return View(model);
        }

        public ActionResult HledatSmlouvy(Lib.Searching.SmlouvaSearchResult model)
        {
            if (model == null || ModelState.IsValid == false)
                return View(new Lib.Searching.SmlouvaSearchResult());



            var sres = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch(model.Q, model.Page,
                HlidacStatu.Lib.Data.Smlouva.Search.DefaultPageSize,
                (HlidacStatu.Lib.Data.Smlouva.Search.OrderResult)(Convert.ToInt32(model.Order)),
                includeNeplatne: model.IncludeNeplatne,
                anyAggregation: new Nest.AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>().Sum("sumKc", m => m.Field(f => f.CalculatedPriceWithVATinCZK)),
                logError: false);

            Lib.Data.Audit.Add(
                    Lib.Data.Audit.Operations.UserSearch
                    , this.User?.Identity?.Name
                    , this.Request.UserHostAddress
                    , "Smlouva"
                    , sres.IsValid ? "valid" : "invalid"
                    , sres.Q, sres.OrigQuery);

            if (sres.IsValid == false && !string.IsNullOrEmpty(sres.Q))
            {
                HlidacStatu.Lib.ES.Manager.LogQueryError<Smlouva>(sres.ElasticResults, "/hledat", this.HttpContext);
            }



            return View(sres);
        }

        public ActionResult Hledat(string q, string order)
        {
            bool showBeta = false;
            if (Request.IsAuthenticated && UserManager.IsInRole(Request?.RequestContext?.HttpContext?.User?.Identity.GetUserId(), "BetaTester") == true)
                showBeta = true;

            var res = HlidacStatu.Lib.Data.Search.GeneralSearch(q, 1, Lib.Searching.SearchDataResult<object>.DefaultPageSizeGlobal, showBeta, order);
            Lib.Data.Audit.Add(
                    Lib.Data.Audit.Operations.UserSearch
                    , this.User?.Identity?.Name
                    , this.Request.UserHostAddress
                    , "General"
                    , res.IsValid ? "valid" : "invalid"
                    , q, null);

            if (System.Diagnostics.Debugger.IsAttached ||
                Devmasters.Config.GetWebConfigValue("LogSearchTimes") == "true")
            {
                HlidacStatu.Util.Consts.Logger.Info($"Search times: {q}\n" + res.SearchTimesReport());

                var data = res.SearchTimes();
                TelemetryClient tm = new TelemetryClient();

                // Set up some properties:

                foreach (var kv in data)
                {
                    var metrics = new Dictionary<string, double> { { "web-search-" + kv.Key, kv.Value.TotalMilliseconds } };
                    var props = new Dictionary<string, string> { { "query", q }, { "database", kv.Key } };

                    Metric elaps = tm.GetMetric("web-GlobalSearch_Elapsed", "Database");
                    tm.TrackEvent("web-GlobalSearch_Elapsed", props, metrics);
                    var ok = elaps.TrackValue(kv.Value.TotalMilliseconds, kv.Key);
                }
            }
            string viewName = "Hledat";
            return View(viewName, res);
        }

        public ActionResult Novinky()
        {
            return RedirectPermanent("https://www.hlidacstatu.cz/texty");
        }
        public ActionResult Napoveda()
        {

            return View();
        }
        public ActionResult Cenik()
        {

            return View();
        }

        public ActionResult PravniPomoc()
        {

            return View();
        }



        public ActionResult Error404(string nextUrl = null, string nextUrlText = null)
        {
            return NotFound();
        }


        public enum ErrorPages
        { 
            Ok = 0,
            NotFound = 404,
            Error = 500,
            ErrorHack = 5666
        }

        public ActionResult Error(string id, string nextUrl = null, string nextUrlText = null)
        {
            ViewBag.NextText = nextUrl;
            ViewBag.NextUrlText = nextUrlText;
            ViewBag.InvokeErrorAction = true;
            //Response.StatusCode = 404;
            ErrorPages errp = (ErrorPages)Devmasters.Enums.EnumTools.GetValueOrDefaultValue(id, typeof(ErrorPages));

            switch (errp)
            {
                case ErrorPages.Ok:
                    return Redirect("/");
                case ErrorPages.NotFound:
                    return View("err404");
                case ErrorPages.Error:
                    return View("err500");
                case ErrorPages.ErrorHack:
                    return View("err500hack");
                default:
                    return Redirect("/");
            }



        }



        public ActionResult Widget(string id, string width)
        {
            string path = Server.MapPath("/scripts/widget.js");
            //if (!page.EndsWith("/"))
            //    page += "/";

            string widgetjs = System.IO.File.ReadAllText(path)
                .Replace("#RND#", id ?? Devmasters.TextUtil.GenRandomString(4))
                .Replace("#MAXWIDTH#", width != null ? ",maxWidth:" + width : "")
                .Replace("#WEBROOT#", this.Request.Url.Scheme + "://" + this.Request.Url.Host)
                ;
            return Content(widgetjs, "text/javascript");
        }

        [OutputCache(Duration = 60 * 60, VaryByParam = "id")]
        public ActionResult Export(string id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(2048);

            if (id == "uohs-ed")
            {
                HlidacStatu.Lib.Data.External.DataSets.DataSet ds = HlidacStatu.Lib.Data.External.DataSets.DataSet.CachedDatasets.Get("rozhodnuti-uohs");
                var res = ds.SearchData("*", 0, 30, "PravniMoc desc");
                if (res.Total > 0)
                {
                    sb.Append(
                        Newtonsoft.Json.JsonConvert.SerializeObject(
                            res.Result
                            .Select(m =>
                            {
                                m.DetailUrl = "https://www.hlidacstatu.cz/data/Detail/rozhodnuti-uohs/" + m.Id;
                                m.DbCreatedBy = null; m.Rozhodnuti = null;
                                return m;
                            })
                            )
                        );
                }
                else
                {
                    sb.Append("[]");
                }
            }
            else if (id == "vz-ed")
            {
                string[] icos = StaticData.MinisterstvaCache.Get().Select(s => s.ICO).ToArray();

                var vz = HlidacStatu.Lib.Data.VZ.VerejnaZakazka.Searching.CachedSimpleSearch(TimeSpan.FromHours(6),
                    new HlidacStatu.Lib.Searching.VerejnaZakazkaSearchData()
                    {
                        Q = icos.Select(i => "ico:" + i).Aggregate((f, s) => f + " OR " + s),
                        Page = 0,
                        PageSize = 30,
                        Order = "1"
                    }
                    );
                if (vz.Total > 0)
                {
                    sb.Append(
                        Newtonsoft.Json.JsonConvert.SerializeObject(
                            vz.ElasticResults.Hits.Select(h => new
                            {
                                Id = h.Id,
                                DetailUrl = h.Source.GetUrl(false),
                                Zadavatel = h.Source.Zadavatel,
                                Dodavatele = h.Source.Dodavatele,
                                NazevZakazky = h.Source.NazevZakazky,
                                Cena = h.Source.FormattedCena(false),
                                CPVkody = h.Source.CPV.Count() == 0
                                        ? "" :
                                        h.Source.CPV.Select(c => h.Source.CPVText(c)).Aggregate((f, s) => f + ", " + s),
                                Stav = h.Source.StavZakazky.ToNiceDisplayName(),
                                DatumUverejneni = h.Source.DatumUverejneni
                            })
                            )
                        );
                }
                else
                {
                    sb.Append("[]");
                }
            }
            return Content(sb.ToString(), "application/json");
        }


        public class ImageBannerCoreData
        {
            public string title { get; set; }
            public string subtitle { get; set; }
            public string body { get; set; }
            public string footer { get; set; }
            public string img { get; set; }
            public string color { get; set; }
        }


        [ValidateInput(false)]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult ImageBannerCore(string id, string title, string subtitle, string body, string footer, string img, string ratio = "16x9", string color = "blue-dark")
        {
            id = id ?? "social";

            string viewName = "ImageBannerCore16x9_social";
            if (id.ToLower() == "quote")
            {
                if (ratio == "1x1")
                    viewName = "ImageBannerCore1x1_quote";
                else
                    viewName = "ImageBannerCore16x9_quote";
            }
            else
            {
                if (ratio == "1x1")
                    viewName = "ImageBannerCore1x1_social";
                else
                    viewName = "ImageBannerCore16x9_social";
            }

            return View(viewName, new ImageBannerCoreData() { title = title, subtitle = subtitle, body = body, footer = footer, img = img, color = color });
        }


        //#if (!DEBUG)
        //        [OutputCache(VaryByParam = "id;v;t;st;b;f;img;rat;res;d;embed", Duration = 60 * 60 * 2)]
        //#endif
        [ValidateInput(false)]
        public ActionResult SocialBanner(string id, string v, string t, string st, string b, string f, string img, string rat = "16x9", string res = "1200x628", string col = "")
        {
            string mainUrl = this.Request.Url.Scheme + "://" + this.Request.Url.Host;


#if (DEBUG)
            if (System.Diagnostics.Debugger.IsAttached)
                mainUrl = "http://local.hlidacstatu.cz";
            //mainUrl = "https://www.hlidacstatu.cz";
#endif
            //twitter Recommended size: 1024 x 512 pixels
            //fb Recommended size: 1200 pixels by 630 pixels

            string url = null;

            byte[] data = null;
            if (id?.ToLower() == "subjekt")
            {
                Firma fi = Firmy.Get(v);
                if (fi.Valid)
                {
                    if (!fi.NotInterestingToShow())
                        url = mainUrl + HlidacStatu.Util.RenderData.GetSocialBannerUrl(fi, rat == "1x1", true);
                }
            }
            else if (id?.ToLower() == "zakazka")
            {
                VerejnaZakazka vz = VerejnaZakazka.LoadFromES(v);
                if (vz != null)
                    try
                    {
                        var path = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(id));
                        Framework.Visit.AddVisit(path,
                            Framework.Visit.IsCrawler(Request.UserAgent) ?
                                Visit.VisitChannel.Crawler : Visit.VisitChannel.Web);
                    }
                    catch (Exception e)
                    {
                        HlidacStatu.Util.Consts.Logger.Info("VisitImg base64 encoding error", e);
                    }

                if (!vz.NotInterestingToShow())
                {
                    url = mainUrl + HlidacStatu.Util.RenderData.GetSocialBannerUrl(vz, rat == "1x1", true);
                }

            }
            else if (id?.ToLower() == "osoba")
            {
                Osoba o = HlidacStatu.Lib.Data.Osoby.GetByNameId.Get(v);
                if (o != null)
                {
                    if (!o.NotInterestingToShow())
                        url = mainUrl + HlidacStatu.Util.RenderData.GetSocialBannerUrl(o, rat == "1x1", true);
                }

            }
            else if (id?.ToLower() == "smlouva")
            {
                Smlouva s = HlidacStatu.Lib.Data.Smlouva.Load(v);
                if (s != null)
                {
                    if (!s.NotInterestingToShow())
                        url = mainUrl + HlidacStatu.Util.RenderData.GetSocialBannerUrl(s, rat == "1x1", true);
                }

            }
            else if (id?.ToLower() == "insolvence")
            {
                var s = HlidacStatu.Lib.Data.Insolvence.Insolvence.LoadFromES(v, false, true)?.Rizeni;
                if (s != null)
                {
                    if (!s.NotInterestingToShow())
                        url = mainUrl + HlidacStatu.Util.RenderData.GetSocialBannerUrl(s, rat == "1x1", true);
                }

            }
            else if (id?.ToLower() == "dataset")
            {
                Lib.Data.External.DataSets.DataSet s = Lib.Data.External.DataSets.DataSet.CachedDatasets.Get(v);
                if (s != null)
                {
                    if (!s.NotInterestingToShow())
                        url = mainUrl + HlidacStatu.Util.RenderData.GetSocialBannerUrl(s, rat == "1x1", true);
                }

            }
            else if (id?.ToLower() == "quote")
            {
                url = mainUrl + "/imagebannercore/quote"
                    + "?title=" + System.Net.WebUtility.UrlEncode(t)
                    + "&subtitle=" + System.Net.WebUtility.UrlEncode(st)
                    + "&body=" + System.Net.WebUtility.UrlEncode(b)
                    + "&footer=" + System.Net.WebUtility.UrlEncode(f)
                    + "&img=" + System.Net.WebUtility.UrlEncode(img)
                    + "&color=" + col
                    + "&ratio=" + rat;
                v = Devmasters.Crypto.Hash.ComputeHashToHex(url);
            }
            else if (id?.ToLower() == "kindex")
            {
                data = Framework.RemoteUrlFromWebCache.GetBinary(mainUrl+"/kindex/banner/"+v,"kindex-banner-"+v, Request.QueryString["refresh"] == "1");
            }
            else if (id?.ToLower() == "page")
            {
                var pageUrl = v;
                string socialTitle = "";
                string socialHtml = "";
                string socialFooter = "";
                string socialSubFooter = "";
                string socialFooterImg = "";
                using (Devmasters.Net.HttpClient.URLContent net = new Devmasters.Net.HttpClient.URLContent(pageUrl))
                {
                    net.Timeout = 40000;
                    var cont = net.GetContent().Text;
                    socialTitle = System.Net.WebUtility.HtmlDecode(
                        Devmasters.RegexUtil
                        .GetRegexGroupValues(cont, @"<meta \s*  property=\""og:hlidac_title\"" \s*  content=\""(?<v>.*)\"" \s* />", "v")
                        .OrderByDescending(o => o.Length).FirstOrDefault()
                        );
                    socialHtml = System.Net.WebUtility.HtmlDecode(
                        Devmasters.RegexUtil
                        .GetRegexGroupValues(cont, @"<meta \s*  property=\""og:hlidac_html\"" \s*  content=\""(?<v>.*)\"" \s* />", "v")
                        .OrderByDescending(o => o.Length).FirstOrDefault()
                        );
                    socialFooter = System.Net.WebUtility.HtmlDecode(
                        Devmasters.RegexUtil.GetRegexGroupValues(cont, @"<meta \s*  property=\""og:hlidac_footer\"" \s*  content=\""(?<v>.*)\"" \s* />", "v")
                        .OrderByDescending(o => o.Length).FirstOrDefault()
                        );
                    socialSubFooter = System.Net.WebUtility.HtmlDecode(
                        Devmasters.RegexUtil.GetRegexGroupValues(cont, @"<meta \s*  property=\""og:hlidac_subfooter\"" \s*  content=\""(?<v>.*)\"" \s* />", "v")
                        .OrderByDescending(o => o.Length).FirstOrDefault()
                        );
                    socialFooterImg = System.Net.WebUtility.HtmlDecode(
                        Devmasters.RegexUtil.GetRegexGroupValues(cont, @"<meta \s*  property=\""og:hlidac_footerimg\"" \s*  content=\""(?<v>.*)\"" \s* />", "v")
                        .OrderByDescending(o => o.Length).FirstOrDefault()
                        );
                }
                if (string.IsNullOrEmpty(socialHtml))
                    return File(HlidacStatu.Lib.Init.WebAppRoot + @"content\icons\largetile.png", "image/png");
                else
                    url = mainUrl + "/imagebannercore/quote"
                        + "?title="
                        + "&subtitle=" + System.Net.WebUtility.UrlEncode(System.Net.WebUtility.HtmlDecode(socialSubFooter))
                        + "&body=" + System.Net.WebUtility.UrlEncode(System.Net.WebUtility.HtmlDecode(socialHtml))
                        + "&footer=" + System.Net.WebUtility.UrlEncode(System.Net.WebUtility.HtmlDecode(socialFooter))
                        + "&img=" + System.Net.WebUtility.UrlEncode(System.Net.WebUtility.HtmlDecode(socialFooterImg))
                        + "&color=" + col
                        + "&ratio=" + rat;
            }


            try
            {


                if (data == null && !string.IsNullOrEmpty(url))
                {

                    data = Framework.RemoteUrlFromWebCache.GetScreenshot(url, (id?.ToLower() ?? "null") + "-" + rat + "-" + v, Request.QueryString["refresh"] == "1");
                }
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("Manager Save", e);
            }
            if (data == null || data.Length == 0)
                return File(HlidacStatu.Lib.Init.WebAppRoot + @"content\icons\largetile.png", "image/png");
            else
                return File(data, "image/png");

        }




        public ActionResult Tip(string id)
        {
            string url = "";
            using (DbEntities db = new DbEntities())
            {
                url = db.TipUrl.Where(m => m.Name == id).Select(m => m.Url).FirstOrDefault();
                url = url ?? "/";
            }

            try
            {
                var path = "/tip/" + id;
                Framework.Visit.AddVisit(path,
                    Framework.Visit.IsCrawler(Request.UserAgent) ?
                        Visit.VisitChannel.Crawler : Visit.VisitChannel.Web);
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Info("VisitImg base64 encoding error", e);
            }

            return Redirect(url);
        }





    }
}