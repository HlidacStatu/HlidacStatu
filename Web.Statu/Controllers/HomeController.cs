using Devmasters.Core;
using HlidacStatu.Lib;
using HlidacStatu.Lib.Data;
using HlidacStatu.Lib.Data.VZ;
using HlidacStatu.Lib.Render;
using HlidacStatu.Web.Framework;
using HlidacStatu.Web.Models;
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
    public partial class HomeController : Controller
    {
        public HomeController()
        {
        }

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ActionResult Analyza(string id, string p, string q, string title, string description, string moreUrl)
        {
            return View("Analyza");
        }

        [OutputCache(VaryByParam = "id;p;q;title;description;moreUrl;embed", Duration = 60 * 60 * 12)]
        [ChildActionOnly]
        public ActionResult Analyza_Child(string id, string p, string q, string title, string description, string moreUrl)
        {
            if (string.IsNullOrEmpty(id))
                id = "platcu";

            HlidacStatu.Lib.Analysis.TemplatedQuery model = null;
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

                var nameId = HlidacStatu.Util.ParseTools.GetRegexGroupValue(f, @"\d{2} \\ (?<nameid>\w* - \w* (-\d{1,3})?) - small\.jpg", "nameid");
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
                return File(o.GetPhotoPath(), "image/jpg");
            }

        }

        public ActionResult ProvozniPodminky()
        {
            return RedirectPermanent("/texty/provoznipodminky");
        }

        [OutputCache(VaryByParam ="*", Duration =60*60*1)]
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

        [OutputCache(VaryByParam = "*", Duration = 60 * 1)]

        public ActionResult Tmp()
        {
            int a = 22;
            int b = 2;
            //int c = (a / (b - 2));
            //Console.Write(c);
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NovyClenTeamu()
        {
            return View();
        }

        public ActionResult PridatSe()
        {
            return View(new Models.PridatSeModel());
        }

        Devmasters.Cache.V20.LocalMemory.LocalMemoryCache<Dictionary<string, string>> slackChannels = new Devmasters.Cache.V20.LocalMemory.LocalMemoryCache<Dictionary<string, string>>
            (TimeSpan.FromHours(12), (o) =>
            {
                var list = new Dictionary<string, string>();

                var slackRes = new System.Net.WebClient().DownloadString("https://slack.com/api/channels.list?token=xoxp-160733539584-162133403223-264615274039-935dab6d5515d60c2a1fa2929757bbd2");
                JToken slackJson = JToken.Parse(slackRes);
                if (((bool?)slackJson["ok"]) == true)
                {
                    foreach (JToken ch in slackJson["channels"])
                    {
                        if ((bool?)ch["is_channel"] == true)
                        {
                            list.Add(ch["name"].Value<string>(), ch["id"].Value<string>());
                        }
                    }
                }


                return list;
            });


        [HttpPost]
        public ActionResult PridatSe(Models.PridatSeModel model, FormCollection form)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                try
                {
                    using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient())
                    {
                        smtp.Send("info@hlidacstatu.cz", "michal@michalblaha.cz", "Nový spolupracovník Hlidac Statu",
                            "Email: " + model.Email + "\n"
                            + "Jmeno: " + model.Jmeno + "\n"
                            + "Prijmeni: " + model.Prijmeni + "\n"
                            + "Tel: " + model.Phone + "\n"
                            + "Vzkaz: " + model.Vzkaz + "\n"
                            + "Temata: " + string.Join(", ", model.TypPrace) + "\n"
                            );
                        smtp.Send("info@hlidacstatu.cz", "fipa@me.com", "Nový spolupracovník Hlidac Statu",
                            "Email: " + model.Email + "\n"
                            + "Jmeno: " + model.Jmeno + "\n"
                            + "Prijmeni: " + model.Prijmeni + "\n"
                            + "Tel: " + model.Phone + "\n"
                            + "Vzkaz: " + model.Vzkaz + "\n"
                            + "Temata: " + string.Join(", ", model.TypPrace) + "\n"
                            );
                        smtp.Send("info@hlidacstatu.cz", "jan.matosik@gmail.com", "Nový spolupracovník Hlidac Statu",
                            "Email: " + model.Email + "\n"
                            + "Jmeno: " + model.Jmeno + "\n"
                            + "Prijmeni: " + model.Prijmeni + "\n"
                            + "Tel: " + model.Phone + "\n"
                            + "Vzkaz: " + model.Vzkaz + "\n"
                            + "Temata: " + string.Join(", ", model.TypPrace) + "\n"
                            );

                        smtp.Send("info@hlidacstatu.cz", "platforma+public-support@hlidacstatu.cz", "Nový spolupracovník Hlidac Statu",
                            "Email: " + model.Email + "\n"
                            + "Jmeno: " + model.Jmeno + "\n"
                            + "Prijmeni: " + model.Prijmeni + "\n"
                            + "Tel: " + model.Phone + "\n"
                            + "Vzkaz: " + model.Vzkaz + "\n"
                            + "Temata: " + string.Join(", ", model.TypPrace) + "\n"
                            );
                    }

                    //register into HlidacSmluv
                    try
                    {

                        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                        var password = Devmasters.Core.TextUtil.GenRandomString(8);
                        var result = UserManager.Create(user, password);
                        result = UserManager.AddToRole(user.Id, "TeamMember");

                        using (Devmasters.Net.Web.URLContent net = new Devmasters.Net.Web.URLContent("https://www.hlidacstatu.cz/account/confirmemail"))
                        {
                            net.Method = Devmasters.Net.Web.MethodEnum.POST;
                            net.RequestParams.Form.Add("email", model.Email);
                            net.GetContent();
                        }
                        new HlidacStatu.Lib.Data.External.Discourse().InviteNewUser(model.Email);

                        if (true)
                        {
                            List<string> joinChannels = new List<string>();
                            joinChannels.Add(slackChannels.Get()["general"]);
                            foreach (var tp in model.TypPrace)
                            {
                                if (slackChannels.Get().ContainsKey(tp))
                                    joinChannels.Add(slackChannels.Get()[tp]);
                                else if (tp == "other")
                                    joinChannels.Add(slackChannels.Get()["komunita"]);
                                else if (tp == "writing" && !joinChannels.Contains(slackChannels.Get()["marketing"]))
                                    joinChannels.Add(slackChannels.Get()["marketing"]);
                            }
                            string channelsToJoin = "channels=" + joinChannels.Aggregate((f, s) => f + "," + s);

                            var slackRes = new System.Net.WebClient().DownloadString("https://slack.com/api/users.admin.invite?token=xoxp-160733539584-162133403223-264615274039-935dab6d5515d60c2a1fa2929757bbd2&email=" + user.Email + "&resend=true&" + channelsToJoin);
                            Newtonsoft.Json.Linq.JToken slackJson = Newtonsoft.Json.Linq.JToken.Parse(slackRes);
                            if (((bool?)slackJson["ok"]) != true)
                            {
                                HlidacStatu.Util.Consts.Logger.Error("Slack Join new user. Error: " + slackRes);
                            }
                        }
                    }
                    catch (Exception e)
                    {

                        HlidacStatu.Util.Consts.Logger.Error("PridatSe new user. Error: " + e);
                    }


                    return RedirectToAction("NovyClenTeamu");
                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Error("Join new user. ", e);
                    ModelState.AddModelError("", "Nastala neočekávaná chyba. Omlouváme se, vaši registraci vyřídíme ručně do nejdříve.");

                    return View(model);
                }
            }
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


        public ActionResult NotFound(string nextUrl = null, string nextUrlText = null)
        {
            ViewBag.NextText = nextUrl;
            ViewBag.NextUrlText = nextUrlText;
            Response.StatusCode = 404;


            HlidacStatu.Util.Consts.Logger.Warning(new Devmasters.Core.Logging.LogMessage()
                .SetMessage("Url not found")
                .SetCustomKeyValue("URL", Request.RawUrl)
                );

            return View("Error404");

        }

        [OutputCache(VaryByParam = "id;embed;nameofview", Duration = 60 * 60 * 1)]
        [ChildActionOnly()]
        public ActionResult Subjekt_child(string id, string NameOfView, HlidacStatu.Lib.Data.Firma firma)
        {
            return View(NameOfView, firma);
        }

        public ActionResult Subjekt(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");


            string ico = id.Trim();
            if (string.IsNullOrEmpty(ico))
                return RedirectToAction("Report", new { id = "1" });

            if (ico.Length < 8)
                ico = ico.PadLeft(8, '0');

            //if (!Devmasters.Core.TextUtil.IsNumeric(ico))
            //    ico = Devmasters.Core.TextUtil.NormalizeToNumbersOnly(ico);

            if (!HlidacStatu.Lib.Validators.CheckCZICO(ico))
            {

                return View("Subjekt_err_spatneICO");
            }

            HlidacStatu.Lib.Data.Firma firma = HlidacStatu.Lib.Data.Firmy.Get(ico);
            if (!HlidacStatu.Lib.Data.Firma.IsValid(firma))
            {
                return View("Subjekt_err_nezname");
            }
            //Framework.Visit.AddVisit("/subjekt/" + ico, Visit.VisitChannel.Web);
            return View(new Models.SubjektReportModel() { firma = firma, ICO = ico });
        }

        public ActionResult sendFeedbackMail(string typ, string email, string txt, string url, bool? auth, string data)
        {
            string to = "michal@michalblaha.cz";
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
                        subject = subject + $" ohledně datové sady {ds.DatasetId}";
                    }
                    catch (Exception)
                    {
                        return Content("");
                    }
                }
            }

            if (auth == false || (auth == true && this.User?.Identity?.IsAuthenticated == true))
            {
                if (!string.IsNullOrEmpty(email) && Devmasters.Core.TextUtil.IsValidEmail(email)
                    && !string.IsNullOrEmpty(to) && Devmasters.Core.TextUtil.IsValidEmail(to)
                    )
                {
                    try
                    {
                        using (var smtp = new System.Net.Mail.SmtpClient())
                        {
                            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage("info@hlidacstatu.cz", to);
                            if (to != "michal@michalblaha.cz")
                                msg.Bcc.Add("michal@michalblaha.cz");
                            msg.Subject = subject;
                            msg.IsBodyHtml = false;
                            msg.BodyEncoding = System.Text.Encoding.UTF8;
                            msg.SubjectEncoding = System.Text.Encoding.UTF8;
                            msg.Body = $@"
Zpráva z hlidacstatu.cz.

Typ zpravy:{typ}
Od uzivatele:{email} 
ke stránce:{url}

text zpravy: {txt}";

                            smtp.Send(msg);
                        }

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
        public ActionResult TextSmlouvy(string Id, string hash)
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(hash))
                return NotFound();

            var model = HlidacStatu.Lib.ES.Manager.Load(Id);
            if (model == null)
            {
                return NotFound();
            }
            ViewBag.hashValue = hash;
            return View(model);
        }
        public ActionResult KopiePrilohy(string Id, string hash)
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(hash))
                return NotFound();

            var model = HlidacStatu.Lib.ES.Manager.Load(Id);
            if (model == null)
            {
                return NotFound();
            }

            if (model.Prilohy != null && model.Prilohy.Count() > 0)
            {

                foreach (var priloha in model.Prilohy)
                {
                    if (priloha.hash.Value == hash)
                    {
                        return File(HlidacStatu.Lib.Init.PrilohaLocalCopy.GetFullPath(model, priloha),
                            string.IsNullOrWhiteSpace(priloha.ContentType) ? "application/octet-stream" : priloha.ContentType,
                            string.IsNullOrWhiteSpace(priloha.nazevSouboru) ? "priloha" : priloha.nazevSouboru);
                    }
                }
            }
            return NotFound();


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
            return RedirectPermanent("https://www.hlidacstatu.cz/texty/oregistru");
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


            if (!string.IsNullOrWhiteSpace(qs["cenaOd"]) && Devmasters.Core.TextUtil.IsNumeric(qs["cenaOd"]))
                query += " cena:>" + qs["cenaOd"];

            if (!string.IsNullOrWhiteSpace(qs["cenaDo"]) && Devmasters.Core.TextUtil.IsNumeric(qs["cenaDo"]))
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

        //[OutputCache(VaryByParam ="q;prefix", Duration =60*60)]
        public ActionResult Osoby(string prefix, string q, bool ftx = false)
        {
            ViewBag.SearchFtx = ftx;
            return View();
        }


        [ChildActionOnly]
#if (!DEBUG)
        [OutputCache(Duration = 60 * 60 * 6, VaryByParam = "id;aktualnost;embed")]
#endif
        public ActionResult Osoba_Part_Vazby(string id, Osoba osoba, HlidacStatu.Lib.Data.Relation.AktualnostType aktualnost)
        {
            ViewBag.Aktualnost = aktualnost;
            return PartialView("Osoba_Part_Vazby", osoba);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 60 * 60 * 6, VaryByParam = "id;aktualnost;embed;NameOfView")]
        public ActionResult Osoba_child(string Id, Osoba model, HlidacStatu.Lib.Data.Relation.AktualnostType aktualnost, string NameOfView)
        {
            ViewBag.Aktualnost = aktualnost;

            return View(NameOfView, model);
        }
        public ActionResult Osoba(string Id, HlidacStatu.Lib.Data.Relation.AktualnostType? aktualnost)
        {

            Osoba o = null;
            if (string.IsNullOrWhiteSpace(Id))
                return NotFound("/Osoby", "Pokračovat na seznamu politiků");
            Guid politikId;
            if (Guid.TryParse(Id, out politikId))
            {
                o = HlidacStatu.Lib.Data.Osoba.GetByExternalID(politikId.ToString(), OsobaExternalId.Source.HlidacSmluvGuid);
                if (o == null)
                    return NotFound("/Osoby", "Pokračovat na seznamu politiků");
                else
                {
                    if (string.IsNullOrEmpty(o.NameId))
                        o.NameId = o.GetUniqueNamedId();
                    return RedirectPermanent("/osoba/" + o.NameId);
                }
            }

            HlidacStatu.Lib.Data.Osoba model = HlidacStatu.Lib.Data.Osoby.GetByNameId.Get(Id);

            if (aktualnost.HasValue == false)
                aktualnost = HlidacStatu.Lib.Data.Relation.AktualnostType.Nedavny;

            ViewBag.Aktualnost = aktualnost;

            if (model == null)
            {
                return NotFound("/Osoby", "Pokračovat na seznamu politiků");
            }
            else
            {
                //Framework.Visit.AddVisit("/osoba/" + model.NameId, Visit.VisitChannel.Web);
                return View(model);
            }
        }


        public ActionResult Sponzori(string strana, string typ, int? rok)
        {
            if (string.IsNullOrWhiteSpace(strana))
            {
                return Redirect("/osoby");
            }

            bool firmy = false;
            if (typ?.ToLower() == "firma")
                firmy = true;

            ViewBag.SponzoriFirmy = firmy;
            ViewBag.Strana = strana;
            ViewBag.Rok = rok;
            if (rok.HasValue == false)
            {
                var result = Sponsors.Strany.PerYears(strana);

                return View(Sponsors.Strany.RenderPerYearsTable(result));

            }
            else if (firmy)
            {

                Dictionary<HlidacStatu.Lib.Data.Firma, HlidacStatu.Lib.Data.FirmaEvent[]> result = null;
                using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
                {
                    result = db.FirmaEvent
                        .Where(m => m.AddInfo == strana && m.Type == (int)FirmaEvent.Types.Sponzor)
                        .ToArray()
                        .Where(m => m.DatumOd.HasValue && m.DatumOd.Value.Year == rok)
                        .Select(m => new { firma = Firmy.Get(m.ICO), oe = m })
                        .GroupBy(g => g.firma, oe => oe.oe, (f, oe) => new { firma = f, events = oe.ToArray() })
                        .ToDictionary(d => d.firma, o => o.events);
                }


                ReportDataSource sponzoriTable = new ReportDataSource(new ReportDataSource.Column[]
                {
                new ReportDataSource.Column() { Name="Sponzor",
                    HtmlRender = (s) => {
                            KeyValuePair<HlidacStatu.Lib.Data.Firma, HlidacStatu.Lib.Data.FirmaEvent[]> data = (KeyValuePair<HlidacStatu.Lib.Data.Firma, HlidacStatu.Lib.Data.FirmaEvent[]>)s;
                            return string.Format(@"<a href='{0}'>{1}</a> IC: {2}", data.Key.GetUrlOnWebsite(), data.Key.Jmeno, data.Key.ICO);
                                        },
                    OrderValueRender = (s) => {
                            KeyValuePair<HlidacStatu.Lib.Data.Firma, HlidacStatu.Lib.Data.FirmaEvent[]> data = (KeyValuePair<HlidacStatu.Lib.Data.Firma, HlidacStatu.Lib.Data.FirmaEvent[]>)s;
                return data.Key.Jmeno;
                            }
                        }
                ,
                new ReportDataSource.Column() { Name = "Naposledy darovala",
                    HtmlRender = (s) => {
                            KeyValuePair<HlidacStatu.Lib.Data.Firma, HlidacStatu.Lib.Data.FirmaEvent[]> data = (KeyValuePair<HlidacStatu.Lib.Data.Firma, HlidacStatu.Lib.Data.FirmaEvent[]>)s;
                            var rok2 = data.Value.OrderByDescending(o=>o.DatumDo.Value.Year).FirstOrDefault();
                            return rok2?.DatumDo?.Year.ToString() ?? "";
                                        },
                    OrderValueRender = (s) => {
                            KeyValuePair<HlidacStatu.Lib.Data.Firma, HlidacStatu.Lib.Data.FirmaEvent[]> data = (KeyValuePair<HlidacStatu.Lib.Data.Firma, HlidacStatu.Lib.Data.FirmaEvent[]>)s;
                            var rok2 = data.Value.OrderByDescending(o=>o.DatumDo.Value.Year).FirstOrDefault();
                            return rok2?.DatumDo?.Year.ToString() ?? "";
                            }
                },
                new ReportDataSource.Column() { Name = "Dary",
                    HtmlRender = (s) => {
                            KeyValuePair<HlidacStatu.Lib.Data.Firma, HlidacStatu.Lib.Data.FirmaEvent[]> data = (KeyValuePair<HlidacStatu.Lib.Data.Firma, HlidacStatu.Lib.Data.FirmaEvent[]>)s;
                            var celkem = data.Value.Sum(sum=>sum.AddInfoNum ?? 0);
                            return string.Format("<h4>{0}</h4><ul>{1}</ul>",
                                celkem == 0 ? "Výši darů neznáme" : "Celkem " + HlidacStatu.Lib.Data.Smlouva.NicePrice(celkem),
                                data.Value.OrderByDescending(o=>o.DatumDo.Value.Year).Select(v=>"<li>" + v.RenderText() + "</li>").Aggregate((f,sec) => f + sec)
                                );
                                        },
                    OrderValueRender = (s) => {
                            KeyValuePair<HlidacStatu.Lib.Data.Firma, HlidacStatu.Lib.Data.FirmaEvent[]> data = (KeyValuePair<HlidacStatu.Lib.Data.Firma, HlidacStatu.Lib.Data.FirmaEvent[]>)s;
                            return data.Value.Sum(sum=>sum.AddInfoNum ?? 0).ToString();
                            }
                },
                });


                foreach (var r in result)
                {
                    sponzoriTable.AddRow(r);
                }


                return View("SponzoriList", sponzoriTable);
            }
            else //osoby
            {
                Dictionary<HlidacStatu.Lib.Data.Osoba, HlidacStatu.Lib.Data.OsobaEvent[]> result = null;
                using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
                {
                    result = db.OsobaEvent
                        .Where(m => m.Type == (int)OsobaEvent.Types.Sponzor && m.AddInfo == strana && m.DatumOd.HasValue && m.DatumOd.Value.Year == rok)
                        .Join(db.Osoba, oe => oe.OsobaId, o => o.InternalId, (oe, o) => new { osoba = o, oe = oe })
                        .ToArray()
                        .GroupBy(g => g.osoba, oe => oe.oe, (o, oe) => new { osoba = o, events = oe.ToArray() })
                        .ToDictionary(o => o.osoba, o => o.events);
                }


                ReportDataSource sponzoriTable = new ReportDataSource(new ReportDataSource.Column[]
                {
                new ReportDataSource.Column() { Name="Sponzor",
                    HtmlRender = (s) => {
                            KeyValuePair<HlidacStatu.Lib.Data.Osoba, HlidacStatu.Lib.Data.OsobaEvent[]> data = (KeyValuePair<HlidacStatu.Lib.Data.Osoba, HlidacStatu.Lib.Data.OsobaEvent[]>)s;
                            return string.Format(@"<a href='{0}'>{1}</a> (*{2})", data.Key.GetUrlOnWebsite(), data.Key.FullName(), data.Key.Narozeni?.Year.ToString() ?? "?");
                                        },
                    OrderValueRender = (s) => {
                            KeyValuePair<HlidacStatu.Lib.Data.Osoba, HlidacStatu.Lib.Data.OsobaEvent[]> data = (KeyValuePair<HlidacStatu.Lib.Data.Osoba, HlidacStatu.Lib.Data.OsobaEvent[]>)s;
                return data.Key.Prijmeni + " " + data.Key.Jmeno;
                            }
                        }
                ,
                new ReportDataSource.Column() { Name = "Naposledy daroval",
                    HtmlRender = (s) => {
                            KeyValuePair<HlidacStatu.Lib.Data.Osoba, HlidacStatu.Lib.Data.OsobaEvent[]> data = (KeyValuePair<HlidacStatu.Lib.Data.Osoba, HlidacStatu.Lib.Data.OsobaEvent[]>)s;
                            var rok2 = data.Value.OrderByDescending(o=> o.DatumDo?.Year ?? (DateTime.Now.Year+1)).FirstOrDefault();
                            return rok2?.DatumDo?.Year.ToString() ?? "";
                                        },
                    OrderValueRender = (s) => {
                            KeyValuePair<HlidacStatu.Lib.Data.Osoba, HlidacStatu.Lib.Data.OsobaEvent[]> data = (KeyValuePair<HlidacStatu.Lib.Data.Osoba, HlidacStatu.Lib.Data.OsobaEvent[]>)s;
                            var rok2 = data.Value.OrderByDescending(o=>o.DatumDo?.Year ?? (DateTime.Now.Year+1)).FirstOrDefault();
                            return rok2?.DatumDo?.Year.ToString() ?? "";
                            }
                },
                new ReportDataSource.Column() { Name = "Dary",
                    HtmlRender = (s) => {
                            KeyValuePair<HlidacStatu.Lib.Data.Osoba, HlidacStatu.Lib.Data.OsobaEvent[]> data = (KeyValuePair<HlidacStatu.Lib.Data.Osoba, HlidacStatu.Lib.Data.OsobaEvent[]>)s;
                            var celkem = data.Value.Sum(sum=>sum.AddInfoNum ?? 0);
                            return string.Format("<h4>{0}</h4><ul>{1}</ul>",
                                celkem == 0 ? "Výši darů neznáme" : "Celkem " + HlidacStatu.Lib.Data.Smlouva.NicePrice(celkem),
                                data.Value.OrderByDescending(o=>o.DatumDo?.Year ?? (DateTime.Now.Year+1)).Select(v=>"<li>" + v.RenderHtml() + "</li>").Aggregate((f,sec) => f + sec)
                                );
                                        },
                    OrderValueRender = (s) => {
                            KeyValuePair<HlidacStatu.Lib.Data.Osoba, HlidacStatu.Lib.Data.OsobaEvent[]> data = (KeyValuePair<HlidacStatu.Lib.Data.Osoba, HlidacStatu.Lib.Data.OsobaEvent[]>)s;
                            return data.Value.Sum(sum=>sum.AddInfoNum ?? 0).ToString();
                            }
                },
                });


                foreach (var r in result)
                {
                    sponzoriTable.AddRow(r);
                }


                return View("SponzoriList", sponzoriTable);
            }
        }

        public ActionResult Politici(string prefix)
        {
            return RedirectPermanent(Url.Action("Osoby", new { prefix = prefix }));

            //List<Lib.Data.Osoba> model = HlidacStatu.Lib.StaticData.Politici.Get();
            //return View(model);

        }

#if (!DEBUG)
        //[OutputCache(VaryByParam ="id;aktualnost;embed", Duration =60*6)]
#endif
        public ActionResult Politik(string Id, HlidacStatu.Lib.Data.Relation.AktualnostType? aktualnost)
        {
            return RedirectPermanent(Url.Action("Osoba", new { Id = Id, aktualnost = aktualnost }));
        }

        public ActionResult PolitikVazby(string Id, HlidacStatu.Lib.Data.Relation.AktualnostType? aktualnost)
        {
            return RedirectPermanent(Url.Action("OsobaVazby", new { Id = Id, aktualnost = aktualnost }));
        }

        //[OutputCache(Duration = 43200, VaryByParam = "id;embed")] //12 hours 60*6*12
        public ActionResult OsobaVazby(string Id, HlidacStatu.Lib.Data.Relation.AktualnostType? aktualnost)
        {
            HlidacStatu.Lib.Data.Osoba o = null;
            if (string.IsNullOrWhiteSpace(Id))
                return NotFound("/Politici", "Pokračovat na seznamu politiků");
            Guid politikId;
            if (Guid.TryParse(Id, out politikId))
            {
                o = HlidacStatu.Lib.Data.Osoba.GetByExternalID(politikId.ToString(), OsobaExternalId.Source.HlidacSmluvGuid);
                if (o == null)
                    return NotFound("/Politici", "Pokračovat na seznamu politiků");
            }

            HlidacStatu.Lib.Data.Osoba model = HlidacStatu.Lib.StaticData.Politici.Get().Where(m => m.NameId == Id).FirstOrDefault();

            if (aktualnost.HasValue == false)
                aktualnost = HlidacStatu.Lib.Data.Relation.AktualnostType.Nedavny;

            ViewBag.Aktualnost = aktualnost;

            if (model == null)
            {
                return NotFound("/Politici", "Pokračovat na seznamu politiků");
            }
            else
                return View(model);

        }
        public ActionResult SubjektVazby(string Id, HlidacStatu.Lib.Data.Relation.AktualnostType? aktualnost)
        {
            if (string.IsNullOrWhiteSpace(Id))
                return NotFound("/", "Pokračovat na titulní straně");


            HlidacStatu.Lib.Data.Firma model = HlidacStatu.Lib.Data.Firmy.Get(Id);

            if (aktualnost.HasValue == false)
                aktualnost = HlidacStatu.Lib.Data.Relation.AktualnostType.Nedavny;

            ViewBag.Aktualnost = aktualnost;

            if (model == null || !Firma.IsValid(model))
            {
                return NotFound("/", "Pokračovat na titulní straně");
            }
            else
                return View(model);

        }

        [OutputCache(Duration = 60 * 60 * 6, VaryByParam = "id;embed;nameOfView")]
        [ChildActionOnly()]
        public ActionResult Detail_Child(string Id, Smlouva model, string nameOfView)
        {
            return View(nameOfView, model);
        }

        public ActionResult Detail(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
                return NotFound();

            var model = HlidacStatu.Lib.ES.Manager.Load(Id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        public ActionResult HledatOld(HlidacStatu.Lib.ES.SmlouvaSearchResult model)
        {
            return HledatSmlouvy(model);
        }

        public ActionResult HledatSmlouvy(HlidacStatu.Lib.ES.SmlouvaSearchResult model)
        {
            if (model == null || ModelState.IsValid == false)
                return View(new HlidacStatu.Lib.ES.SmlouvaSearchResult());



            var sres = HlidacStatu.Lib.ES.SearchTools.SimpleSearch(model.Q, model.Page,
                HlidacStatu.Lib.ES.SearchTools.DefaultPageSize,
                (HlidacStatu.Lib.ES.SearchTools.OrderResult)(Convert.ToInt32(model.Order)),
                includeNeplatne: model.IncludeNeplatne,
                anyAggregation: new Nest.AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>().Sum("sumKc", m => m.Field(f => f.CalculatedPriceWithVATinCZK)),
                logError: false);

            if (sres.IsValid == false && !string.IsNullOrEmpty(sres.Q))
            {
                HlidacStatu.Lib.ES.Manager.LogQueryError<Smlouva>(sres.ElasticResults, "/hledej", this.HttpContext);
            }



            return View(sres);
        }

        public ActionResult Hledat(string q)
        {
            var res = HlidacStatu.Lib.Data.Search.GeneralSearch(q);
            HlidacStatu.Util.Consts.Logger.Info($"Search times: {q}\n" + res.SearchTimesReport());
            return View(res);
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



        public ActionResult Widget(string id, string width)
        {
            string path = Server.MapPath("/scripts/widget.js");
            //if (!page.EndsWith("/"))
            //    page += "/";

            string widgetjs = System.IO.File.ReadAllText(path)
                .Replace("#RND#", id ?? Devmasters.Core.TextUtil.GenRandomString(4))
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
                    new HlidacStatu.Lib.ES.VerejnaZakazkaSearchData()
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
                            vz.Result.Hits.Select(h => new
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
        }


        [ValidateInput(false)]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "embed")]
        public ActionResult ImageBannerCore(string id, string title, string subtitle, string body, string footer, string img, string ratio = "16x9")
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

            return View(viewName, new ImageBannerCoreData() { title = title, subtitle = subtitle, body = body, footer = footer, img = img });
        }

        //#if (!DEBUG)
        //        [OutputCache(VaryByParam = "id;v;t;st;b;f;img;rat;res;d;embed", Duration = 60 * 60 * 2)]
        //#endif
        [ValidateInput(false)]
        public ActionResult SocialBanner(string id, string v, string t, string st, string b, string f, string img, string rat = "16x9", string res = "1920x1080")
        {
            string mainUrl = this.Request.Url.Scheme + "://" + this.Request.Url.Host;

            //twitter Recommended size: 1024 x 512 pixels
            //fb Recommended size: 1200 pixels by 630 pixels

            string url = null;

            if (id?.ToLower() == "subjekt")
            {
                Firma fi = Firmy.Get(v);
                if (fi.Valid)
                {
                    url = mainUrl + HlidacStatu.Util.RenderData.GetSocialBannerUrl(fi, rat == "1x1", true);
                }
            }
            else if (id?.ToLower() == "zakazka")
            {
                VerejnaZakazka vz = VerejnaZakazka.LoadFromES(v);
                if (vz != null)
                {
                    url = mainUrl + HlidacStatu.Util.RenderData.GetSocialBannerUrl(vz, rat == "1x1", true);
                }

            }
            else if (id?.ToLower() == "osoba")
            {
                Osoba o = HlidacStatu.Lib.Data.Osoby.GetByNameId.Get(v);
                if (o != null)
                {
                    url = mainUrl + HlidacStatu.Util.RenderData.GetSocialBannerUrl(o, rat == "1x1", true);
                }

            }
            else if (id?.ToLower() == "smlouva")
            {
                Smlouva s = HlidacStatu.Lib.ES.Manager.Load(v);
                if (s != null)
                {
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
                    + "&ratio=" + rat;
                v = url;
            }
            else if (id?.ToLower() == "page")
            {
                var pageUrl = v;
                string socialHtml = "";
                string socialFooter = "";
                string socialSubFooter = "";
                string socialFooterImg = "";
                using (Devmasters.Net.Web.URLContent net = new Devmasters.Net.Web.URLContent(pageUrl))
                {
                    net.Timeout = 40000;
                    var cont = net.GetContent().Text;
                    socialHtml = System.Net.WebUtility.HtmlDecode(HlidacStatu.Util.ParseTools.GetRegexGroupValue(cont, @"<meta \s*  property=\""og:hlidac_html\"" \s*  content=\""(?<v>.*)\"" \s* />", "v"));
                    socialFooter = System.Net.WebUtility.HtmlDecode(HlidacStatu.Util.ParseTools.GetRegexGroupValue(cont, @"<meta \s*  property=\""og:hlidac_footer\"" \s*  content=\""(?<v>.*)\"" \s* />", "v"));
                    socialSubFooter = System.Net.WebUtility.HtmlDecode(HlidacStatu.Util.ParseTools.GetRegexGroupValue(cont, @"<meta \s*  property=\""og:hlidac_subfooter\"" \s*  content=\""(?<v>.*)\"" \s* />", "v"));
                    socialFooterImg = System.Net.WebUtility.HtmlDecode(HlidacStatu.Util.ParseTools.GetRegexGroupValue(cont, @"<meta \s*  property=\""og:hlidac_footerimg\"" \s*  content=\""(?<v>.*)\"" \s* />", "v"));
                }
                if (string.IsNullOrEmpty(socialHtml))
                    return File(HlidacStatu.Lib.Init.WebAppRoot + @"content\icons\largetile.png", "image/png");
                else
                    url = mainUrl + "/imagebannercore/quote"
                        + "?title="
                        + "&subtitle=" + System.Net.WebUtility.UrlEncode(socialSubFooter)
                        + "&body=" + System.Net.WebUtility.UrlEncode(socialHtml)
                        + "&footer=" + System.Net.WebUtility.UrlEncode(socialFooter)
                        + "&img=" + System.Net.WebUtility.UrlEncode(socialFooterImg)
                        + "&ratio=" + rat;
            }


            byte[] data = null;
            try
            {


                if (!string.IsNullOrEmpty(url))
                {
                    string scr = "http://127.0.0.1:9090/png?ratio=" + rat + "&url=" + System.Net.WebUtility.UrlEncode(url);
                    data = Framework.FileFromWebCache.Manager.Get(new Util.Cache.KeyAndId()
                    {
                        ValueForData = scr,
                        CacheNameOnDisk = (id?.ToLower() ?? "null") + "-" + rat + "-" + v
                    });
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

        protected override void HandleUnknownAction(string actionName)
        {
            HlidacStatu.Util.Consts.Logger.Warning(new Devmasters.Core.Logging.LogMessage()
                .SetMessage("Url not found")
                .SetCustomKeyValue("URL", Request.RawUrl)
                );


            RedirectToAction("Error404").ExecuteResult(this.ControllerContext);

        }



    }
}