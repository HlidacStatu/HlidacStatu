using HlidacStatu.Lib.Data;
using HlidacStatu.Lib.Data.VZ;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    [Authorize]
    public partial class ManageController : GenericAuthController
    {

        [AllowAnonymous]
        public ActionResult AddReview()
        {
            try
            {
                IDictionary<string, object> eo = new ExpandoObject() as IDictionary<string, object>;
                Review r = new Review();
                r.created = DateTime.Now;
                r.createdBy = (User?.Identity?.Name ?? Request.QueryString["email"]) ?? "";
                r.newValue = Request.QueryString["newV"] ?? "";
                r.itemType = Request.QueryString["t"] ?? "";
                string[] keys = new string[] { "t", "email", "oldV", "newV" };

                foreach (string key in Request.QueryString.AllKeys)
                {
                    if (key != null && !keys.Contains(key.ToLower()))
                        eo.Add(new KeyValuePair<string, object>(key, Request.QueryString[key]));
                }
                var oldValue = Request.QueryString["oldV"] ?? "";

                dynamic data = eo;
                data.HtmlOldValue = oldValue;

                r.oldValue = Newtonsoft.Json.JsonConvert.SerializeObject(data);

                r.Save();

                return Json("1", JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("addreview API error", e);
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExportResult(string id, string q, string h, string o, string ct, int num = 1000 )
        {
            var apiAuth = Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                                new Framework.ApiCall.CallParameter("id", id),
                                new Framework.ApiCall.CallParameter("q", q),
                                new Framework.ApiCall.CallParameter("o", o),
                                new Framework.ApiCall.CallParameter("ct", ct)
                });

            if (!apiAuth.Authentificated)
            {
                //Response.StatusCode = 401;
                return Redirect("/");
            }


            string contentType = "application/vnd.ms-excel";
            if (ct == "numbers")
                contentType = "application/vnd.apple.numbers";
            if (ct == "txt")
                contentType = "text/tab-separated-values";

            int numOfRecords = 1000;
            if (string.IsNullOrEmpty(q) || q?.Contains("*") == true)
                numOfRecords = 100;

            if (this.User.IsInRole("Admin"))
            {
                numOfRecords = num;
            }
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(q) || string.IsNullOrEmpty(h))
                return File(System.Text.Encoding.UTF8.GetBytes("žádná data nejsou k dispozici"), "text/plain", "export.txt");
            if (id == "smlouvy" && HlidacStatu.Lib.Data.Smlouva.Search.IsQueryHashCorrect(id, q, h))
            {
                var sres = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch(q, 0, num,
              (HlidacStatu.Lib.Data.Smlouva.Search.OrderResult)(Util.ParseTools.ToInt(o) ?? 0),
              logError: false);

                if (sres.IsValid == false && !string.IsNullOrEmpty(sres.Q))
                {
                    HlidacStatu.Lib.ES.Manager.LogQueryError<Smlouva>(sres.ElasticResults, "/hledej", this.HttpContext);
                    return File(System.Text.Encoding.UTF8.GetBytes("chyba při přípravě dat. Omlouváme se a řešíme to"), "text/plain", "export.txt");
                }
                else
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    //header
                    sb.AppendLine("URL\tID smlouvy\tPodepsána\tZveřejněna\tHodnota smlouvy\tPředmět smlouvy\tPlátce\tPlatce IC\tDodavatele a jejich ICO");
                    foreach (var item in sres.Result.Hits)
                    {
                        var s = item.Source;
                        sb.AppendLine(
                            s.GetUrl(false) + "\t"
                            + s.Id + "\t"
                            + s.datumUzavreni.ToString("dd.MM.yyyy") + "\t"
                            + s.casZverejneni.ToString("dd.MM.yyyy") + "\t"
                            + s.CalculatedPriceWithVATinCZK.ToString(Util.Consts.czCulture) + "\t"
                            + Devmasters.Core.TextUtil.NormalizeToBlockText(s.predmet) + "\t"
                            + s.Platce.nazev + "\t"
                            + s.Platce.ico + "\t"
                            + ((s.Prijemce?.Count() > 0) ?
                                s.Prijemce.Select(p => p.nazev + "\t" + p.ico).Aggregate((f, sec) => f + "\t" + sec)
                                : "")
                            );
                    }
                    return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()), contentType, "smlouvy-export.tabdelimited.txt");

                }
            }
            else if (id == "zakazky" && HlidacStatu.Lib.Data.Smlouva.Search.IsQueryHashCorrect(id, q, h))
            {

                string[] cpvs = (Request.QueryString["cpv"] ?? "").Split(',');
                var sres = VerejnaZakazka.Searching.SimpleSearch(q, cpvs, 1, numOfRecords,
                    (Util.ParseTools.ToInt(o) ?? 0).ToString(), (Request.QueryString["zahajeny"] == "1")
                    );

                if (sres.IsValid == false && !string.IsNullOrEmpty(sres.Q))
                {
                    return File(System.Text.Encoding.UTF8.GetBytes("chyba při přípravě dat. Omlouváme se a řešíme to"), "text/plain", "export.txt");
                }
                else
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    //header
                    sb.AppendLine("URL\tČíslo zakázky\tPoslední změna\tLhůta pro nabídky\tKonečná cena bez DPH"
                        + "\tOdhadovaná cena bez DPH\tNázev zakázky\tPopis\tZadavatel\tZadavatel IC\tDodavatele a jejich ICO");
                    foreach (var item in sres.Result.Hits)
                    {
                        var s = item.Source;
                        sb.AppendLine(
                            s.GetUrl(false) + "\t"
                            + s.EvidencniCisloZakazky + "\t"
                            + s.PosledniZmena?.ToString("dd.MM.yyyy") + "\t"
                            + (s.LhutaDoruceni?.ToString("dd.MM.yyyy") ?? "neuvedena") + "\t"
                            + (s.KonecnaHodnotaBezDPH?.ToString(Util.Consts.czCulture) ?? "") + "\t"
                            + (s.OdhadovanaHodnotaBezDPH?.ToString(Util.Consts.czCulture) ?? "") + "\t"
                            + Devmasters.Core.TextUtil.NormalizeToBlockText(s.NazevZakazky) + "\t"
                            + Devmasters.Core.TextUtil.NormalizeToBlockText(s.PopisZakazky) + "\t"
                            + s.Zadavatel?.Jmeno + "\t"
                            + s.Zadavatel?.ICO + "\t"
                            + ((s.Dodavatele?.Count() > 0) ?
                                s.Dodavatele.Select(p => p.Jmeno + "\t" + p.ICO).Aggregate((f, sec) => f + "\t" + sec)
                                : "")
                            );
                    }
                    return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()), contentType, "zakazky-export.tabdelimited.txt");
                }
            }

            return File(System.Text.Encoding.UTF8.GetBytes("žádná data"), "text/plain", "export.txt");
        }

        public ActionResult RemoveBookmark(string id, int type)
        {
            try
            {
                Bookmark.DeleteBookmark((Bookmark.ItemTypes)type, id, this.User.Identity.Name);
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("Manage.RemoveBookmark", e);
                return Json("-1", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SetBookmark(string name, string url, string id, int type)
        {
            try
            {
                Bookmark.SetBookmark(name, url, (Bookmark.ItemTypes)type, id, this.User.Identity.Name);
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("Manage.RemoveBookmark", e);
                return Json("-1", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddWd(string query, string dataType, string name, int period, int focus)
        {
            using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
            {

                if (string.IsNullOrEmpty(query))
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                string id = this.User.Identity.GetUserId();

                var dt = dataType;

                HlidacStatu.Lib.Data.WatchDog wd = new HlidacStatu.Lib.Data.WatchDog();
                wd.Created = DateTime.Now;
                wd.UserId = id;
                wd.StatusId = 1;
                wd.SearchTerm = query;
                wd.PeriodId = period;
                wd.FocusId = focus;
                wd.Name = Devmasters.Core.TextUtil.ShortenText(name, 50);
                if (dt.ToLower() == typeof(Smlouva).Name.ToLower())
                    wd.dataType = typeof(Smlouva).Name;
                else if (dt.ToLower() == typeof(VerejnaZakazka).Name.ToLower())
                    wd.dataType = typeof(VerejnaZakazka).Name;
                else if (dt.ToLower() == typeof(Lib.Data.Insolvence.Rizeni).Name.ToLower())
                    wd.dataType = typeof(Lib.Data.Insolvence.Rizeni).Name;
                else if (dt.ToLower().StartsWith(typeof(HlidacStatu.Lib.Data.External.DataSets.DataSet).Name.ToLower()))
                {
                    var dataSetId = dt.Replace("DataSet.", "");
                    if (HlidacStatu.Lib.Data.External.DataSets.DataSet.ExistsDataset(dataSetId) == false)
                    {
                        HlidacStatu.Util.Consts.Logger.Error("AddWd - try to hack, wrong dataType = " + dataType + "." + dataSetId);
                        throw new ArgumentOutOfRangeException("AddWd - try to hack, wrong dataType = " + dataType + "." + dataSetId);
                    }
                    wd.dataType = typeof(HlidacStatu.Lib.Data.External.DataSets.DataSet).Name + "." + dataSetId;
                }
                else if (dt == WatchDog.AllDbDataType)
                {
                    wd.dataType = dt;
                }
                else
                {
                    HlidacStatu.Util.Consts.Logger.Error("AddWd - try to hack, wrong dataType = " + dataType);
                    throw new ArgumentOutOfRangeException("AddWd - try to hack, wrong dataType = " + dataType);
                }
                if (!db.WatchDogs
                     .Any(m => m.dataType == wd.dataType && m.UserId == id && m.SearchTerm == query))
                {
                    wd.Save();
                }


                //var lastOrder = HlidacStatu.Lib.Data.Invoices.GetCurrentOrderForUser(this.User.Identity.GetUserId());
                //if (lastOrder.HasValue == false && focus == 1)
                //    return Json("2", JsonRequestBehavior.AllowGet);

                return Json("1", JsonRequestBehavior.AllowGet);
            }
        }


    }
}