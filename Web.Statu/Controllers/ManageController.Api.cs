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

        public ActionResult ExportResult(string id, string q, string h, string o, string ct, int num = 1000)
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



            int numOfRecords = 1000;
            if (string.IsNullOrEmpty(q) || q?.Contains("*") == true)
                numOfRecords = 100;

            if (this.User.IsInRole("Admin"))
            {
                numOfRecords = num;
            }

            byte[] rawData = null;
            string contentType = "";
            string filename = "";
            List<dynamic> data = new List<dynamic>();


            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(q) || string.IsNullOrEmpty(h))
            {
                rawData = System.Text.Encoding.UTF8.GetBytes("žádná data nejsou k dispozici");
                contentType = "text/plain";
                filename = "chyba.txt";
                return File(rawData,contentType,filename);
            }
            else if (id == "smlouvy" && HlidacStatu.Lib.Data.Smlouva.Search.IsQueryHashCorrect(id, q, h))
            {
                var sres = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch(q, 0, num, o, logError: false);
                foreach (var m in sres.Results)
                {
                    dynamic v = new System.Dynamic.ExpandoObject();
                    v.url = m.GetUrl(false);
                    v.id = m.Id;
                    v.datumUzavreni = m.datumUzavreni;
                    v.casZverejneni = m.casZverejneni;
                    v.hodnotaSmlouvy_sDPH = m.CalculatedPriceWithVATinCZK;
                    v.platceJmeno = m.Platce.nazev;
                    v.platceIco = m.Platce.ico;
                    for (int i = 0; i < m.Prijemce.Count(); i++)
                    {
                        ((IDictionary<String, Object>)v).Add($"prijemceJmeno_{i}", m.Prijemce[i].nazev);
                        ((IDictionary<String, Object>)v).Add($"prijemceIco_{i}", m.Prijemce[i].ico);
                    }
                    data.Add(v);
                }

                if (sres.IsValid == false && !string.IsNullOrEmpty(sres.Q))
                {
                    HlidacStatu.Lib.ES.Manager.LogQueryError<Smlouva>(sres.ElasticResults, "/hledej", this.HttpContext);
                    rawData = System.Text.Encoding.UTF8.GetBytes("chyba při přípravě dat. Omlouváme se a řešíme to");
                    contentType = "text/plain";
                    filename = "export.txt";
                    return File(rawData, contentType, filename);
                }
            } //smlouvy
            else if (id == "zakazky" && HlidacStatu.Lib.Data.Smlouva.Search.IsQueryHashCorrect(id, q, h))
            {

                string[] cpvs = (Request.QueryString["cpv"] ?? "").Split(',');
                var sres = VerejnaZakazka.Searching.SimpleSearch(q, cpvs, 1, numOfRecords,
                    (Util.ParseTools.ToInt(o) ?? 0).ToString(), (Request.QueryString["zahajeny"] == "1")
                    );

                if (sres.IsValid == false && !string.IsNullOrEmpty(sres.Q))
                {
                    rawData = System.Text.Encoding.UTF8.GetBytes("chyba při přípravě dat. Omlouváme se a řešíme to");
                    contentType = "text/plain";
                    filename = "export.txt";
                    return File(rawData, contentType, filename);
                }
                else
                {

                    foreach (var s in sres.Results)
                    {
                        dynamic v = new System.Dynamic.ExpandoObject();
                        v.Url = s.GetUrl(false);
                        v.CisloZakazky = s.EvidencniCisloZakazky;

                        v.PosledniZmena = s.PosledniZmena;
                        v.LhutaDoruceni = s.LhutaDoruceni;
                        v.KonecnaHodnotaBezDPH = s.KonecnaHodnotaBezDPH;
                        v.OdhadovanaHodnotaBezDPH = s.OdhadovanaHodnotaBezDPH;
                        v.NazevZakazky = Devmasters.Core.TextUtil.NormalizeToBlockText(s.NazevZakazky);
                        v.PopisZakazky = Devmasters.Core.TextUtil.NormalizeToBlockText(s.PopisZakazky);
                        v.ZadavatelJmeno = s.Zadavatel?.Jmeno;
                        v.ZadavatelIco = s.Zadavatel?.ICO;

                        for (int i = 0; i < s.Dodavatele?.Count(); i++)
                        {
                            ((IDictionary<String, Object>)v).Add($"DodavatelJmeno_{i}", s.Dodavatele[i].Jmeno);
                            ((IDictionary<String, Object>)v).Add($"DodavatelIco_{i}", s.Dodavatele[i].ICO);
                        }
                        data.Add(v);

                    }
                }
            }
            if (data.Count == 0)
            {
                rawData = System.Text.Encoding.UTF8.GetBytes("žádná data nejsou k dispozici");
                contentType = "text/plain";
                filename = "chyba.txt";
            }
            else
            {
                if (ct == "tab")
                {
                    rawData = new HlidacStatu.ExportData.TabDelimited().ExportData(new ExportData.Data(data));
                    contentType = "text/tab-separated-values";
                    filename = "smlouvy-export.tabdelimited.txt";
                }
                else if (ct == "csv")
                {
                    rawData = new HlidacStatu.ExportData.Csv().ExportData(new ExportData.Data(data));
                    contentType = "text/csv";
                    filename = "smlouvy-export.csv";
                }
                else if (ct == "numbers")
                {
                    rawData = new HlidacStatu.ExportData.Excel().ExportData(new ExportData.Data(data));
                    contentType = "application/vnd.apple.numbers";
                    filename = "smlouvy-export.numbers";
                }
                else
                {
                    rawData = new HlidacStatu.ExportData.Excel().ExportData(new ExportData.Data(data));
                    contentType = "application/vnd.ms-excel";
                    filename = "smlouvy-export.xlsx";

                }

            }
            return File(rawData, contentType, filename);
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