using HlidacStatu.Lib.Data;
using HlidacStatu.Util;
using HlidacStatu.Web.Framework;
using HlidacStatu.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    [Authorize]
    public partial class ManageController : GenericAuthController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
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





        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            if (!string.IsNullOrEmpty(Request.QueryString["msg"]))
            {
                ViewBag.StatusMessage = Request.QueryString["msg"];
            }
            var userId = User.Identity.GetUserId();
            IndexViewModel model = null;
            using (var db = new DbEntities())
            {
                model = new IndexViewModel
                {
                    HasPassword = HasPassword(),
                    PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                    TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                    Logins = await UserManager.GetLoginsAsync(userId),
                    BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                    HasWatchdogs = db.WatchDogs.AsNoTracking().Any(m => m.UserId == userId)
                };
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ReviewForm(string url)
        {
            string content = DateTime.Now.ToString();
            //return Content(string.Format(template, content), "text/html");
            return View();
        }

        public ActionResult AnalyzeSubjekt()
        {
            return View();
        }

        public ActionResult OsobaMerge(string osoba1, string osoba2)
        {
            if (ParseTools.ToInt(osoba1).HasValue && ParseTools.ToInt(osoba2).HasValue)
            {
                Osoba o1 = Osoba.GetByInternalId(ParseTools.ToInt(osoba1).Value);
                Osoba o2 = Osoba.GetByInternalId(ParseTools.ToInt(osoba2).Value);
                if (o1 != null && o2 != null)
                {
                    o1.MergeWith(o2, this.User.Identity.Name);
                    return Redirect(o1.GetUrl(true));
                }

            }
            else
            {
                Osoba o1 = Osoba.GetByNameId(osoba1.Trim());
                Osoba o2 = Osoba.GetByNameId(osoba2.Trim());
                if (o1 != null && o2 != null)
                {
                    o1.MergeWith(o2, this.User.Identity.Name);
                    return Redirect(o1.GetUrl(true));
                }
            }
            return View("index");
        }

        public ActionResult SubjektHlidac(string Id)
        {


            if (string.IsNullOrWhiteSpace(Id))
                return new HomeController().NotFound("/", "Pokračovat na titulní straně");
            HlidacStatu.Lib.Data.Firma model = HlidacStatu.Lib.Data.Firmy.Get(Id);
            if (model == null || !Firma.IsValid(model))
            {
                return new HomeController().NotFound("/", "Pokračovat na titulní straně");
            }
            else
            {
                var aktualnost = HlidacStatu.Lib.Data.Relation.AktualnostType.Nedavny;
                ViewBag.Aktualnost = aktualnost;
                return View(model);
            }
        }

        [HttpPost()]
        public ActionResult SubjektHlidac(string Id, FormCollection form)
        {
            bool existing = false;
            string wd_id = form["wd_id"];
            int wdId;
            string subject = form["subjekt"];
            using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
            {

                string id = this.User.Identity.GetUserId();
                HlidacStatu.Lib.Data.WatchDog wd = null;
                if (!string.IsNullOrEmpty(wd_id) && int.TryParse(wd_id, out wdId))
                {
                    wd = WatchDog.Load(wdId);
                    existing = wd != null;
                }
                string query = "";
                if (string.IsNullOrEmpty(form["ico"]) && wd != null)
                {
                    wd.Delete();
                    return RedirectToAction("Watchdogs", "Manage");
                }
                else if (string.IsNullOrEmpty(form["ico"]))
                    return Redirect("/Subjekt/" + id);

                query = form["ico"].Split(',').Select(m => "ico:" + m).Aggregate((f, s) => f + " OR " + s);

                if (wd == null)
                    wd = new HlidacStatu.Lib.Data.WatchDog();

                wd.Created = DateTime.Now;
                wd.UserId = id;
                wd.StatusId = 1;
                wd.SearchTerm = query;
                wd.SearchRawQuery = "icoVazby:" + form["ico"];
                wd.PeriodId = Convert.ToInt32(form["period"]);
                wd.FocusId = Convert.ToInt32(form["focus"]);
                wd.Name = Devmasters.Core.TextUtil.ShortenText(form["wdname"], 50);
                wd.Save();
                return RedirectToAction("Watchdogs", "Manage");
            }


        }

        [Authorize(Roles = "canEditData")]
        public ActionResult ShowAccountConfirmationCode(string email, string userId, string code, string action)
        {
            if (action == "test")
            {
                var result = UserManager.ConfirmEmail(userId, code);
                //todo
            }
            else
            {
                var usr = UserManager.FindByEmail(email);
                if (usr != null)
                {
                    ViewBag.Code = UserManager.GenerateEmailConfirmationToken(usr.Id);
                    ViewBag.UserId = usr.Id;
                }
            }
            return View();
        }

        [Authorize(Roles = "canEditData")]
        public ActionResult Debug()
        {
            return View();
        }


        [Authorize(Roles = "canEditData")]
        public ActionResult WatchdogsAdminList()
        {
            return View();
        }
        [Authorize(Roles = "canEditData")]
        public ActionResult EditSmlouva(string Id, string type)
        {
            object item = HlidacStatu.Lib.Data.Smlouva.Load(Id);
            if (item != null)
            {
                ViewBag.objectType = type;
                ViewBag.objectId = Id;
                return View(item);
            }
            else
                return View("~/Views/Shared/404");
        }

        [Authorize(Roles = "canEditData")]
        public ActionResult EditObject(string Id, string type)
        {
            if (type?.ToLower() == "bankovniucet")
            {
                if (string.IsNullOrEmpty(Id))
                {
                    ViewBag.objectType = type;
                    ViewBag.objectId = Id;
                    return View(new HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcet() { CisloUctu = "", Mena = "Kč", Subjekt = "", Url = "" });

                }
                var item =
                    HlidacStatu.Lib.ES.Manager.GetESClient().Get<HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcet>(Id);
                if (item.Found)
                {
                    ViewBag.objectType = type;
                    ViewBag.objectId = Id;
                    return View(item);
                }
            }
            return View("~/Views/Shared/404");
        }

        [ValidateInput(false)]
        [Authorize(Roles = "canEditData")]
        [HttpPost]
        public ActionResult EditObject(string Id, FormCollection form)
        {
            string oldJson = form["oldJson"];
            string newJson = form["jsonRaw"];
            string type = form["type"];

            if (type?.ToLower() == "bankovniucet")
            {
                HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcet obj = Newtonsoft.Json.JsonConvert.DeserializeObject<HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcet>(newJson);
                var ret = HlidacStatu.Lib.ES.Manager.GetESClient().IndexDocument<HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcet>(obj);
                return Redirect("Index?ret=" + ret.IsValid);

            }

            return Redirect("Index");
        }

        [Authorize(Roles = "canEditData")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditSmlouva(string Id, FormCollection form)
        {
            string oldJson = form["oldJson"];
            string newJson = form["jsonRaw"];
            Smlouva s = Newtonsoft.Json.JsonConvert.DeserializeObject<Smlouva>(newJson);
            s.Save();

            return Redirect("Index");
        }

        [Authorize(Roles = "canEditData")]
        public ActionResult EditOsoba(int Id)
        {
            Osoba item = HlidacStatu.Lib.Data.Osoby.GetById.Get(Id);
            if (item != null)
            {
                if (Request.QueryString["del"] == "1")
                {
                    item.Delete(this.User.Identity.Name);
                    return Redirect("/manage/index?msg=Smazano");
                }


                ViewBag.objectId = Id;
                return View(item);
            }
            else
                ViewBag.objectId = 0;
            return View(item);
        }

        [Authorize(Roles = "canEditData")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditOsoba(string Id, FormCollection form)
        {

            var jsonOsoba = Newtonsoft.Json.JsonConvert.DeserializeObject<HlidacStatu.Lib.Data.Osoba.JSON>(form["json"]);
            jsonOsoba.Id = Convert.ToInt32(Id);
            jsonOsoba.Save(this.User.Identity.Name);
            return Redirect("Index");
        }

        [Authorize(Roles = "canEditData")]
        public ActionResult HledejOsoby(string jmeno, string prijmeni, string narozeni)
        {
            DateTime? nar = ParseTools.ToDateTime(narozeni, "yyyy-MM-dd");
            var osoby = Osoba.GetAllByName(jmeno, prijmeni, nar);
            if (osoby.Count() == 0)
                osoby = Osoba.GetAllByNameAscii(jmeno, prijmeni, nar);

            return View(osoby);
        }

        [Authorize(Roles = "canEditData")]
        public ActionResult AddPersons()
        {
            return View();
        }

        [Authorize(Roles = "canEditData")]
        [HttpPost]
        public ActionResult AddPersons(FormCollection form)
        {
            this.Server.ScriptTimeout = 600;

            List<string> newIds = new List<string>();
            string tabdelimited = form["data"];
            foreach (var line in tabdelimited.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] cols = line.Split(new string[] { "\t","|" }, StringSplitOptions.None);

                // Vždy musí být řádek o 13 sloupcích. Povinné položky jsou:
                // varianta a) jmeno, prijmeni, narozeni
                // varianta b) fullname, narozeni

                if (cols.Count() != 13)
                    continue;
                string fullName = cols[0];
                string jmeno = cols[1];
                string prijmeni = cols[2];
                string titulPred = cols[3];
                string titulPo = cols[4];
                DateTime? narozeni = ParseTools.ToDate(cols[5]);

                Osoba.StatusOsobyEnum status = GetStatusFromText(cols[6]);

                string clenstviStrana = cols[7];
                DateTime? clenstviVznik = ParseTools.ToDate(cols[8]);

                string eventOrganizace = cols[9];
                string eventRole = cols[10];
                DateTime? eventVznik = ParseTools.ToDate(cols[11]);
                string eventTyp = cols[12];

                // set person from fulltext when not properly defined
                if (string.IsNullOrWhiteSpace(jmeno) || string.IsNullOrWhiteSpace(prijmeni))
                {
                    if (string.IsNullOrWhiteSpace(fullName))
                        continue;

                    var osoba = Lib.Validators.OsobaInText(fullName);

                    if (osoba is null)
                        continue;
                    if (string.IsNullOrWhiteSpace(jmeno))
                        jmeno = osoba.Jmeno;
                    if (string.IsNullOrWhiteSpace(prijmeni))
                        prijmeni = osoba.Prijmeni;
                    if (string.IsNullOrWhiteSpace(titulPred))
                        titulPred = osoba.TitulPred;
                    if (string.IsNullOrWhiteSpace(titulPo))
                        titulPo = osoba.TitulPo;
                }

                // when there is no narozeni Date, then we are not going to save person...
                if (!narozeni.HasValue)
                    continue;

                Osoba p = Osoba.GetOrCreateNew(titulPred, jmeno, prijmeni, titulPo, narozeni, status,
                    this.User.Identity.Name);

                if (!string.IsNullOrWhiteSpace(clenstviStrana))
                {
                    OsobaEvent clenStrany = new OsobaEvent
                    {
                        OsobaId = p.InternalId,
                        DatumOd = clenstviVznik,
                        Type = 7,
                        AddInfo = "člen",
                        Organizace = clenstviStrana,
                        Title = $"člen v {clenstviStrana}"
                    };

                    OsobaEvent.CreateOrUpdate(clenStrany, this.User.Identity.Name);
                }


                if (int.TryParse(eventTyp, out int typ)
                    && !string.IsNullOrWhiteSpace(eventRole)
                    && !string.IsNullOrWhiteSpace(eventOrganizace))
                {
                    OsobaEvent dalsiEvent = new OsobaEvent
                    {
                        OsobaId = p.InternalId,
                        DatumOd = eventVznik,
                        Type = typ,
                        AddInfo = eventRole,
                        Organizace = eventOrganizace,
                        Title = $"{eventRole} v {eventOrganizace}"
                    };

                    OsobaEvent.CreateOrUpdate(dalsiEvent, this.User.Identity.Name);
                }

                


                //Guid? foundId;
                //if (Osoba.GetByName(p.Jmeno, p.Prijmeni, p.Narozeni.Value) == null)
                //{
                //    if (cols.Count() > 5)
                //        p.Description = cols[5];
                //    if (cols.Count() > 6)
                //        p.PersonStatus = string.IsNullOrEmpty(cols[6]) ? 0 : int.Parse(cols[6]);
                //    if (cols.Count() > 7)
                //        p.Zdroj = cols[7];
                //    if (cols.Count() > 8)
                //        p.MoreInfoUrl = cols[8];


                //    var res = Person.Import(p, true);
                //    if (res.IsValid)
                //        newIds.Add(res.Id + " " + p.FullName());

                //}



            }
            return View(newIds);

        }

        /// <summary>
        /// Assigns StatusOsoby based on text. 
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Returns StatusOsobyEnum.Politik if string is empty or invalid (not a number)</returns>
        private Osoba.StatusOsobyEnum GetStatusFromText(string text)
        {
            Osoba.StatusOsobyEnum statusOsoby = Osoba.StatusOsobyEnum.Politik;

            try
            {
                if (int.TryParse(text, out int statx))
                {
                    statusOsoby = (Osoba.StatusOsobyEnum)statx;
                }
            }
            catch {}

            return statusOsoby;
        }

        [Authorize(Roles = "canEditData")]
        public ActionResult ShowClassification(string id, bool force = false)
        {

            return View(new Tuple<string,bool>(id, force));
        }

        [Authorize(Roles = "canEditData")]
        public ActionResult Reviews(string id, string a, string reason)
        {
            var model = new List<Review>();
            //show list
            using (HlidacStatu.Lib.Data.DbEntities db = new DbEntities())
            {
                if (!string.IsNullOrEmpty(id)
                    && !string.IsNullOrEmpty(a)
                    && Devmasters.Core.TextUtil.IsNumeric(id)
                    )
                {
                    var iId = Convert.ToInt32(id);
                    var item = db.Review.FirstOrDefault(m => m.Id == iId);
                    if (item != null)
                    {
                        switch (a.ToLower())
                        {
                            case "accepted":
                                item.Accepted(this.User.Identity.Name);
                                break;
                            case "denied":
                                item.Denied(this.User.Identity.Name, reason);
                                break;
                            default:
                                break;
                        }
                    }
                    return RedirectToAction("Reviews", "Manage");
                }
                else
                {
                    model = db.Review.AsNoTracking()
                            .Where(m => m.reviewed == null)
                            .ToList();
                }
            }

            return View(model);
        }


        static string[] WatchdogTypes = new string[]{
                    WatchDog.AllDbDataType,
                    typeof(Smlouva).Name,
                    typeof(HlidacStatu.Lib.Data.VZ.VerejnaZakazka).Name,
                    typeof(HlidacStatu.Lib.Data.External.DataSets.DataSet).Name,
                };

        public ActionResult Watchdogs(string id, string wid, string disable, string enable, string delete)
        {
            string currWDtype = null;
            if (!string.IsNullOrEmpty(id))
            {
                if (WatchdogTypes.Any(w => w.ToLower() == id.ToLower()))
                    currWDtype =  id ;
            }
            //if (currWDtypes == null)
            //    currWDtypes = WatchdogTypes;

            List<WatchDog> wds = new List<WatchDog>();
            using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
            {
                string userid = this.User.Identity.GetUserId();

                if (!string.IsNullOrEmpty(wid))
                {
                    int watchdogid = Convert.ToInt32(wid);
                    HlidacStatu.Lib.Data.WatchDog wd;
                    wd = db.WatchDogs
                        .Where(m => m.Id == watchdogid && m.UserId == userid)
                        .FirstOrDefault();

                    if (wd == null)
                    {
                        return View("Error404");
                    }
                    else
                    {
                        if (disable == "1")
                        {
                            wd.StatusId = 0;
                            wd.Save();
                        }
                        if (enable == "1")
                        {
                            wd.StatusId = 1;
                            wd.Save();
                        }
                        if (delete == "1")
                            wd.Delete();

                        return RedirectToAction("Watchdogs");
                    }
                }
                else
                {
                        wds.AddRange(
                            db.WatchDogs.AsNoTracking()
                                .Where(m => 
                                (m.dataType == currWDtype || currWDtype == null) 
                                && m.UserId == userid)
                            );
                }

            }
            return View(wds);

        }


        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Objednavka(int Id)
        {
            if (!Enum.IsDefined(typeof(HlidacStatu.Lib.Data.InvoiceItems.ShopItem), Id))
                return Redirect("/Cenik");
            return View(Id);
        }
        [HttpGet]
        public ActionResult Objednavka2()
        {
            return Redirect("/Cenik");
        }

        public ActionResult OutOfWatchdogs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Objednavka2(FormCollection form)
        {
            var _sluzba = form["sluzba"];
            if (Devmasters.Core.TextUtil.IsNumeric(_sluzba)
                &&
                !Enum.IsDefined(typeof(HlidacStatu.Lib.Data.InvoiceItems.ShopItem), Convert.ToInt32(_sluzba))
                )
                return Redirect("/Cenik");

            int sluzba = Convert.ToInt32(form["sluzba"]);
            HlidacStatu.Lib.Data.InvoiceItems.ShopItem sluzbaN = (HlidacStatu.Lib.Data.InvoiceItems.ShopItem)sluzba;

            ViewBag.Sluzba = sluzba;
            ViewBag.ICO = form["ICO"];
            HlidacStatu.Lib.Data.External.Merk.CoreCompanyStructure f = null;
            if (!string.IsNullOrEmpty(form["ICO"]))
                f = HlidacStatu.Lib.Data.External.Merk.FromIcoFull(form["ICO"]);

            var obj = new ObjednavkaViewModel();



            if (f != null)
            {
                obj.ICO = HlidacStatu.Util.ParseTools.MerkIcoToICO(f.regno.ToString());
                obj.JmenoFirmy = f.name;
                obj.DIC = f.vatno;
                obj.Adresa = f.address.street + " " + f.address.number_descriptive;
                obj.Mesto = f.address.municipality;
                obj.PSC = f.address.postal_code.ToString();

            }

            return View(obj);
        }
        [HttpPost]
        public ActionResult ObjednavkaPotvrzeni(ObjednavkaViewModel data, FormCollection form)
        {
            var _sluzba = form["sluzba"];
            if (Devmasters.Core.TextUtil.IsNumeric(_sluzba)
                &&
                !Enum.IsDefined(typeof(HlidacStatu.Lib.Data.InvoiceItems.ShopItem), Convert.ToInt32(_sluzba))
                )
                return Redirect("/Cenik");

            int sluzba = Convert.ToInt32(_sluzba);

            var inv = HlidacStatu.Lib.Data.Invoices.CreateNew(
                (HlidacStatu.Lib.Data.InvoiceItems.ShopItem)sluzba,
                data.Adresa,
                data.Mesto, data.JmenoFirmy, data.ICO, data.JmenoOsoby, data.DIC, data.PSC,
                this.User.Identity.GetUserId(), this.User.Identity.GetUserName(),
                true

                );

            return View(sluzba);
        }


        [AllowAnonymous]
        public ActionResult ChangePhoto(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Osoby");

            var o = HlidacStatu.Lib.Data.Osoby.GetByNameId.Get(id);
            if (o == null)
                return new HomeController().NotFound("/", "Pokračovat na titulní straně");

            ViewBag.Phase = "start";
            ViewBag.Osoba = o;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ChangePhoto(string id, FormCollection form, HttpPostedFileBase file1)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return RedirectToAction("Osoby");

                var o = HlidacStatu.Lib.Data.Osoby.GetByNameId.Get(id);
                if (o == null)
                    return new HomeController().NotFound("/", "Pokračovat na titulní straně");
                ViewBag.Osoba = o;

                if (form["phase"] == "start") //upload
                {
                    byte[] data = null;
                    var path = HlidacStatu.Lib.Init.OsobaFotky.GetFullPath(o, "original.uploaded.jpg");
                    var pathTxt = HlidacStatu.Lib.Init.OsobaFotky.GetFullPath(o, "source.txt");
                    var source = form["source"] ?? "";
                    string[] facesFiles = new string[] { };
                    if (!string.IsNullOrEmpty(form["url"]))
                    {
                        try
                        {
                            data = new System.Net.WebClient().DownloadData(form["url"]);
                            source = form["url"];
                        }
                        catch (Exception)
                        {
                            data = null;
                        }
                    }
                    else if (file1.ContentLength > 0)
                    {

                        using (var binaryReader = new BinaryReader(file1.InputStream))
                        {
                            data = binaryReader.ReadBytes(file1.ContentLength);
                        }
                    }
                    try
                    {
                        facesFiles = HlidacStatu.Lib.RenderTools.DetectAndParseFacesIntoFiles(data, 150, 40).ToArray();
                        if (data != null && facesFiles.Length > 0)
                        {
                            System.IO.File.WriteAllText(pathTxt, source);
                            System.IO.File.WriteAllBytes(path, data);
                        }
                    }
                    catch (Exception e)
                    {
                        HlidacStatu.Util.Consts.Logger.Error("PhotoChange processing", e);
                    }

                    ViewBag.Osoba = o;
                    ViewBag.Phase = "choose";
                    ViewBag.Email = form["email"] ?? "";
                    return View("ChangePhoto_Choose", facesFiles);

                } //upload
                else if (form["phase"] == "choose") //upload
                {
                    string fn = form["fn"];

                    if (!string.IsNullOrEmpty(fn) && fn.Contains(System.IO.Path.GetTempPath()))
                    {
                        if (System.IO.File.Exists(fn))
                        {
                            //C:\Windows\TEMP\tmp3EB0.tmp.0.faces.jpg
                            var rootfn = HlidacStatu.Util.ParseTools.GetRegexGroupValue(fn, @"(?<tempfn>.*)\.\d{1,2}\.faces\.jpg$", "tempfn");
                            var target = HlidacStatu.Lib.Init.OsobaFotky.GetFullPath(o, "small.uploaded.jpg");
                            try
                            {
                                using (Devmasters.Imaging.InMemoryImage imi = new Devmasters.Imaging.InMemoryImage(fn))
                                {
                                    HlidacStatu.Util.IOTools.DeleteFile(fn);
                                    if (this.User?.IsInRole("Admin") == true)
                                    {
                                        //HlidacStatu.Util.IOTools.MoveFile(fn, HlidacStatu.Lib.Init.OsobaFotky.GetFullPath(o, "small.jpg"));
                                        imi.Resize(new System.Drawing.Size(300, 300), true, Devmasters.Imaging.InMemoryImage.InterpolationsQuality.High, true);
                                        imi.SaveAsJPEG(HlidacStatu.Lib.Init.OsobaFotky.GetFullPath(o, "small.jpg"), 80);
                                        return Redirect("/Osoba/" + o.NameId);
                                    }
                                    else
                                    {
                                        imi.SaveAsJPEG(target, 80);
                                        //HlidacStatu.Util.IOTools.MoveFile(fn, HlidacStatu.Lib.Init.OsobaFotky.GetFullPath(o, "small.review.jpg"));
                                        //HlidacStatu.Util.IOTools.MoveFile(HlidacStatu.Lib.Init.OsobaFotky.GetFullPath(o, "uploaded.jpg"), HlidacStatu.Lib.Init.OsobaFotky.GetFullPath(o, "original.uploaded.jpg"));
                                        using (HlidacStatu.Lib.Data.DbEntities db = new DbEntities())
                                        {
                                            var r = new Review()
                                            {
                                                created = DateTime.Now,
                                                createdBy = form["email"] ?? "",
                                                itemType = "osobaPhoto",
                                                newValue = Newtonsoft.Json.JsonConvert.SerializeObject(new { nameId = o.NameId, file = target }),
                                            };

                                            db.Review.Add(r);
                                            using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient())
                                            {
                                                smtp.Send("info@hlidacstatu.cz", "michal@michalblaha.cz", "Photo review",
                                                    Newtonsoft.Json.JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.Indented)
                                                    );
                                            }
                                            db.SaveChanges();
                                        }
                                        return View("ChangePhoto_finish", o);
                                    }
                                }
                            }
                            finally
                            {
                                try
                                {
                                    foreach (var f in System.IO.Directory.EnumerateFiles(System.IO.Path.GetDirectoryName(rootfn), System.IO.Path.GetFileName(rootfn) + ".*"))
                                    {
                                        HlidacStatu.Util.IOTools.DeleteFile(f);
                                    }
                                }
                                catch
                                {
                                }
                            }



                        }

                    }
                }
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("PhotoChange", e);
                return View();
            }
            return View();
        }

        public ActionResult Zalozky()
        {
            using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
            {
                var bookmarks = db.Bookmark
                    .AsNoTracking()
                    .Where(m => m.UserId == User.Identity.Name)
                    .OrderByDescending(m => m.Created)
                    .ToArray();
                return View(bookmarks);
            }

        }

        #region Disabled Controllers


        //
        // POST: /Manage/RemoveLogin
        [DisableController]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        [DisableController]
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [DisableController]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [DisableController]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [DisableController]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        [DisableController]
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [DisableController]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [DisableController]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }


        //
        // GET: /Manage/ManageLogins
        [DisableController]
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [DisableController]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        [DisableController]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        #endregion Controllers


        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}