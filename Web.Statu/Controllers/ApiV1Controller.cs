using FullTextSearch;
using HlidacStatu.Lib;
using HlidacStatu.Lib.Data;
using HlidacStatu.Util;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HlidacStatu.Web.Models.ApiV1Models;

namespace HlidacStatu.Web.Controllers
{
    public partial class ApiV1Controller : GenericAuthController
    {
        public ApiV1Controller()
        {
        }

        public ApiV1Controller(ApplicationUserManager userManager, ApplicationSignInManager signInManager) : base(userManager, signInManager)
        {
        }

        public ActionResult UX()
        {
            return View();
        }

        public ActionResult Doc()
        {
            return View();
        }


        public ActionResult OcrStat()
        {
            return View();
        }

        // GET: ApiV1
        public ActionResult Index()
        {
            if (Framework.ApiAuth.IsApiAuth(this).Authentificated)
            {

                //global::hlst
                ViewBag.Token = HlidacStatu.Lib.Data.AspNetUserToken.GetToken(this.User.Identity.Name).Token.ToString("N");

                if (!string.IsNullOrEmpty(Request.QueryString["getocr"]))
                {
                    using (Devmasters.Net.HttpClient.URLContent url = new Devmasters.Net.HttpClient.URLContent(
                        $"https://ocr.hlidacstatu.cz/AddApi.ashx?apikey={Devmasters.Config.GetWebConfigValue("OCRServerApiKey")}&email={this.User.Identity.Name}"
                        ))
                    {
                        var json = Newtonsoft.Json.Linq.JToken.Parse(url.GetContent().Text);

                        ViewBag.OcrToken = json.Value<string>("apikey");
                    }

                }
                return View();
            }
            else
            {
                return Redirect(MvcApplication.LoginRedirPath+"?returnUrl=" + System.Net.WebUtility.UrlEncode("/api/v1/Index"));
            }
        }

        public ActionResult ResendConfirmationMail(string _id)
        {
            string id = _id;

            if (Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("id", id)
                }).Authentificated)
            {

                var userEmail = Framework.ApiAuth.IsApiAuth(this).ApiCall.User;

                using (HlidacStatu.Lib.Data.DbEntities db = new DbEntities())
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        id = db.AspNetUsers.Where(m => m.Email == userEmail)
                            .FirstOrDefault()?.Id;
                    }

                    var users = db.AspNetUsers
                    .Where(m => m.EmailConfirmed == false);

                    if (!string.IsNullOrEmpty(id) && id != "*")
                        users = users.Where(m => m.Id == id);


                    foreach (var user in users)
                    {
                        var um = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                        string code = um.GenerateEmailConfirmationToken(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        //create email
                        var email = HlidacStatu.Lib.Emails.EmailMsg.CreateEmailMsgFromPostalTemplate("ConfirmEmail");
                        email.Model.CallbackUrl = callbackUrl;
                        email.To = user.Email;
                        email.SendMe();
                    }


                }

                return Content("ok");
            }
            else
                return new HttpStatusCodeResult(401);

        }


        [Obsolete()]
        public ActionResult Dumps()
        {
            if (Framework.ApiAuth.IsApiAuth(this, parameters: new Framework.ApiCall.CallParameter[] { new Framework.ApiCall.CallParameter("Dumps", "") }).Authentificated)
            {
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(Framework.Api.Dumps.GetDumps(),Newtonsoft.Json.Formatting.Indented), "application/json");
            }
            else
                return new HttpStatusCodeResult(401);

        }

        public ActionResult OCRStats(string type = "")
        {

            string cnnStr = Devmasters.Config.GetWebConfigValue("CnnString");
            string sql = @"select 'Celkem' as 'type',
		                        (select count(*) from ItemToOcrQueue with (nolock) where started is null) as waiting,
		                        (select count(*) from ItemToOcrQueue with (nolock) where started is not null and done is null) as running,
		                        (select count(*) from ItemToOcrQueue with (nolock) where started is not null and done is not null and done > DATEADD(dy,-1,getdate())) as doneIn24H,
		                        (select count(*) from ItemToOcrQueue with (nolock) where started is not null and done is null and started< dateadd(hh,-24,getdate())) as errors
                        union
	                        select distinct t.itemtype as 'type',
		                        (select count(*) from ItemToOcrQueue with (nolock) where started is null and itemtype = t.itemtype) as waiting,
		                        (select count(*) from ItemToOcrQueue with (nolock) where started is not null and done is null and itemtype = t.itemtype) as running,
		                        (select count(*) from ItemToOcrQueue with (nolock) where started is not null and done is not null 
		                        and done > DATEADD(dy,-1,getdate()) and itemtype = t.itemtype) as doneIn24H,
		                        (select count(*) from ItemToOcrQueue with (nolock) where started is not null and done is null 
		                        and started< dateadd(hh,-24,getdate()) and itemtype = t.itemtype) as errors
		                        from ItemToOcrQueue t with (nolock)
		                        order by type";
            using (var p = new Devmasters.PersistLib())
            {
                var ds = p.ExecuteDataset(cnnStr, System.Data.CommandType.Text, sql, null);
                System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);
                sb.AppendLine("Typ\tVe frontě\tZpracovavane\tHotovo za 24hod\tChyby pri zpracovani");
                foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                {
                    sb.Append((string)dr[0]);
                    sb.Append("\t");
                    sb.Append((int)dr[1]);
                    sb.Append("\t");
                    sb.Append((int)dr[2]);
                    sb.Append("\t");
                    sb.Append((int)dr[3]);
                    sb.Append("\t");
                    sb.Append((int)dr[4]);
                    sb.AppendLine();
                }
                return Content(sb.ToString());
            }

        }

        [Obsolete()]
        public ActionResult Dump(string date, string datatype = "smlouvy")
        {
            if (Framework.ApiAuth.IsApiAuth(this, parameters: new Framework.ApiCall.CallParameter[] { new Framework.ApiCall.CallParameter("Dump", date) }).Authentificated)
            {
                HlidacStatu.Util.Consts.Logger.Info(new Devmasters.Logging.LogMessage()
                    .SetMessage("Downloading smlouvy.dump.zip")
                    .SetCustomKeyValue("UserId", this.User.Identity.Name)
                            );

                DateTime? specificDate = Devmasters.DT.Util.ToDateTime(date, "yyyy-MM-dd");
                string onlyfile = $"{datatype}.dump" + (specificDate.HasValue ? "-" + specificDate.Value.ToString("yyyy-MM-dd") : "");
                string fn = HlidacStatu.Lib.StaticData.Dumps_Path + $"{onlyfile}" + ".zip";

                if (System.IO.File.Exists(fn))
                {
                    long FileL = (new FileInfo(fn)).Length;
                    byte[] bytes = new byte[1024 * 1024];
                    Response.Clear();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("content-disposition", "attachment; filename=" + System.IO.Path.GetFileName(fn));
                    //Response.AddHeader("Content-Length", FileL.ToString());
                    try
                    {

                    using (FileStream FS = System.IO.File.OpenRead(fn))
                    {
                        int bytesRead = 0;
                        while ((bytesRead = FS.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            Response.OutputStream.Write(bytes, 0, bytesRead);
                            Response.Flush();
                        };

                        Response.Flush();
                    }
                        //Response.Close();
                        Response.Flush();
                        System.Threading.Thread.Sleep(1000);
                        //Response.End();
                    }
                    catch (System.Web.HttpException wex)
                    {
                        if (wex.Message.StartsWith("The remote host closed the connection"))
                        {
                            //ignore
                        }
                        else
                            HlidacStatu.Util.Consts.Logger.Error("DUMP?" + date, wex);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    return new EmptyResult();
                    //return File(fn, "application/zip");
                }
                else
                {
                    Util.Consts.Logger.Error("API DUMP : not found file " + fn);
                    return new HttpStatusCodeResult(404);
                }
            }
            else
                return new HttpStatusCodeResult(401);

        }

        class VZProfilesListRes { public string profileId { get; set; } public string url { get; set; } public long? count { get; set; } }
        public ActionResult VZProfilesList()
        {
            if (!(Framework.ApiAuth.IsApiAuth(this).Authentificated)
                )
                return new HttpStatusCodeResult(401);
            else
            {
                List<VZProfilesListRes> list = new List<VZProfilesListRes>();
                var res = HlidacStatu.Lib.ES.Manager.GetESClient_VerejneZakazkyNaProfiluRaw()
                    .Search<HlidacStatu.Lib.Data.External.ProfilZadavatelu.ZakazkaRaw>(s => s
                        .Query(q => q.Bool(b => b.MustNot(mn => mn.Term(t => t.Field(f => f.Converted).Value(1)))))
                        .Size(0)
                        .Aggregations(agg => agg
                            .Terms("profiles", t => t.Field("profil")
                            .Size(250)
                            .Order(o => o.CountDescending())
                            )
                        )
                    );

                if (res.IsValid)
                {
                    foreach (Nest.KeyedBucket<object> val in ((BucketAggregate)res.Aggregations["profiles"]).Items)
                    {
                        var resProf = HlidacStatu.Lib.ES.Manager.GetESClient_VZ().Get<HlidacStatu.Lib.Data.VZ.ProfilZadavatele>((string)val.Key);
                        list.Add(new VZProfilesListRes() { profileId = (string)val.Key, url = resProf?.Source?.Url, count = val.DocCount });
                    }
                }
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(list.ToArray()), "application/json");
            }


        }

        public ActionResult VZList(string _id)
        {
            string id = _id;

            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(404);

            if (!System.Diagnostics.Debugger.IsAttached &&
                !(Framework.ApiAuth.IsApiAuth(this, parameters: new Framework.ApiCall.CallParameter[] { new Framework.ApiCall.CallParameter("id", id) }).Authentificated)
                )
                return new HttpStatusCodeResult(401);
            else
            {
                var res = HlidacStatu.Lib.ES.Manager.GetESClient_VerejneZakazkyNaProfiluRaw()
                    .Search<HlidacStatu.Lib.Data.External.ProfilZadavatelu.ZakazkaRaw>(s => s
                        .Query(q => q
                                .QueryString(qs => qs.DefaultOperator(Operator.And).Query("NOT(converted:1) AND profil:\"" + id + "\""))
                            )
                        .Size(50)
                        );

                if (res.IsValid)
                {
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                        res.Hits.Select(m => m.Source).ToArray()
                        ), "application/json");
                }
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { error = res.ServerError.ToString() }), "application/json");
            }


        }

        [HttpGet()]
        public ActionResult VZDetail(string _id)
        {
            string id = _id;

            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(404);

            if (!System.Diagnostics.Debugger.IsAttached &&
                !(Framework.ApiAuth.IsApiAuth(this, parameters: new Framework.ApiCall.CallParameter[] { new Framework.ApiCall.CallParameter("id", id) }).Authentificated)
                )
                return new HttpStatusCodeResult(401);
            else
            {

                var res = HlidacStatu.Lib.ES.Manager.GetESClient_VerejneZakazkyNaProfiluRaw()
                    .Search<HlidacStatu.Lib.Data.External.ProfilZadavatelu.ZakazkaRaw>(s => s
                        .Query(q => q
                                .QueryString(qs => qs.DefaultOperator(Operator.And).Query("zakazkaId:\"" + id + "\""))
                            )
                        .Size(50)
                        );

                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                    res.Hits.Select(m => m.Source).FirstOrDefault()
                    ), "application/json");


            }

        }

        static string ReadRequestBody(HttpRequestBase req)
        {
            string ret = "";
            using (var stream = new MemoryStream())
            {
                req.InputStream.Seek(0, SeekOrigin.Begin);
                req.InputStream.CopyTo(stream);
                ret = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }
            return ret;
        }

        [HttpPost()]
        public ActionResult VZDetail(string _id, FormCollection form)
        {
            string id = _id;

            var content = ReadRequestBody(this.Request);
            //return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { debug=content, error = "Not implemented yet. Come back tomorrow." }), "application/json");

            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(404);

            if (!System.Diagnostics.Debugger.IsAttached &&
                !(Framework.ApiAuth.IsApiAuth(this, parameters: new Framework.ApiCall.CallParameter[] { new Framework.ApiCall.CallParameter("id", id) }).Authentificated)
                )
                return new HttpStatusCodeResult(401);
            else
            {
                var authId = Framework.ApiAuth.IsApiAuth(this);
                var idxConn = HlidacStatu.Lib.ES.Manager.GetESClient_VerejneZakazkyNaProfiluConverted();
                HlidacStatu.Lib.Data.VZ.VerejnaZakazka vz = null;
                try
                {
                    vz = Newtonsoft.Json.JsonConvert.DeserializeObject<Lib.Data.VZ.VerejnaZakazka>(content);
                    if (vz == null)
                    {
                        ErrorEnvelope ee = new ErrorEnvelope() { Data = content, Error = "null data", UserId = authId.ApiCall.User, apiCallJson = Newtonsoft.Json.JsonConvert.SerializeObject(authId) ?? null };
                        ee.Save(idxConn);
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                            new { error = "data is empty" }
                            ), "application/json");
                    }
                    vz.Save(idxConn);
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                        new { result = "ok" }
                        ), "application/json");


                }
                catch (Exception e)
                {
                    ErrorEnvelope ee = new ErrorEnvelope() { Data = content, Error = e.ToString(), UserId = authId.ApiCall.User, apiCallJson = Newtonsoft.Json.JsonConvert.SerializeObject(authId) ?? null };
                    ee.Save(idxConn);

                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                        new { error = "deserialization error", descr = e.ToString() }
                        ), "application/json");
                }


            }

        }

        
        public ActionResult Status()
        {
            ClusterHealthResponse res = null;
            int num = 0;
            string status = "unknown";
            string nodes = "-------------------------\n";
            try
            {
                res = HlidacStatu.Lib.ES.Manager.GetESClient().Cluster.Health(); 
                num = res?.NumberOfNodes ?? 0;
                status = res?.Status.ToString() ?? "unknown";

                //GET /_cat/nodes?v&h=m,name,ip,u&s=name
                var catr = Lib.ES.Manager.GetESClient()
                     .LowLevel.Cat.Nodes<Elasticsearch.Net.StringResponse>(
                             new Elasticsearch.Net.Specification.CatApi.CatNodesRequestParameters()
                             {
                                 Headers = new[] { "m", "name", "ip", "u" },
                                 SortByColumns = new[] { "name" },
                                 Verbose = true
                             }
                     ).Body
                     ?.Replace("10.10.", "");
                     
                     ;
                nodes = nodes + catr;
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("Status page error", e);
                ViewBag.Error = e;
            }

            return Content(string.Format("{0}-{1}\n\n" + nodes, num, status), "text/plain");
        }


        //public ActionResult Politici(string q)
        //{
        //    PolitikTypeAhead[] res = new PolitikTypeAhead[] { };
        //    if (string.IsNullOrEmpty(q))
        //        return Json(res);
        //    string nquery = q.NormalizeToPureTextLower();


        //    res = HlidacStatu.Lib.StaticData.PolitickyAktivni.Get()
        //        .Where(m => m.Prijmeni.StartsWith(q) || m.Jmeno.StartsWith(q))
        //        .OrderBy(m => m.Prijmeni)
        //        .Take(15)
        //        .Select(m => new PolitikTypeAhead() { nameId = m.NameId, name = m.FullName() })
        //        .ToArray();

        //    return Json(res);

        //}

        public ActionResult Persons(string q)
        {
            PolitikTypeAhead[] res = new PolitikTypeAhead[] { };

            if (string.IsNullOrEmpty(q))
                return Json(res, JsonRequestBehavior.AllowGet);

            if (q.Length < 2)
                return Json(res, JsonRequestBehavior.AllowGet);


            res = HlidacStatu.Lib.Data.Osoba.Searching.GetPolitikByNameFtx(q, 15)
                .Select(m => new PolitikTypeAhead() { name = m.FullNameWithYear(), nameId = m.NameId })
                .ToArray();

            if (!string.IsNullOrEmpty(Request.Headers["Origin"]))
            {
                if (Request.Headers["Origin"].Contains(".hlidacstatu.cz")
                    //|| Request.Headers["Origin"].Contains(".hlidacstatu.cz")
                    )
                    Response.AddHeader("Access-Control-Allow-Origin", Request.Headers["Origin"]);
            }
            return Json(res, JsonRequestBehavior.AllowGet);

        }

        [Authorize(Roles = "NasiPoliticiAdmin")]
        public ActionResult AddInfo(string q, int? t)
        {
            if (!t.HasValue)
                return Json("", JsonRequestBehavior.AllowGet);
            List<string> result = new List<string>();

            //if (string.IsNullOrEmpty(q) || q.Length <2)
            //    return Json(result, JsonRequestBehavior.AllowGet);

            result = OsobaEvent.GetAddInfos(q, t, 200).ToList();


            if (!string.IsNullOrEmpty(Request.Headers["Origin"]))
            {
                if (Request.Headers["Origin"].Contains(".hlidacstatu.cz")
                    //|| Request.Headers["Origin"].Contains(".hlidacstatu.cz")
                    )
                    Response.AddHeader("Access-Control-Allow-Origin", Request.Headers["Origin"]);
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [Authorize(Roles = "NasiPoliticiAdmin")]
        public ActionResult Organisations(string q, int? t)
        {
            if (!t.HasValue)
                return Json("", JsonRequestBehavior.AllowGet);
            List<string> result = new List<string>();

            //if (string.IsNullOrEmpty(q) || q.Length <2)
            //    return Json(result, JsonRequestBehavior.AllowGet);

            result = OsobaEvent.GetOrganisations(q, t, 200).ToList();


            if (!string.IsNullOrEmpty(Request.Headers["Origin"]))
            {
                if (Request.Headers["Origin"].Contains(".hlidacstatu.cz")
                    //|| Request.Headers["Origin"].Contains(".hlidacstatu.cz")
                    )
                    Response.AddHeader("Access-Control-Allow-Origin", Request.Headers["Origin"]);
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [Authorize(Roles = "NasiPoliticiAdmin")]
        public ActionResult Companies(string q)
        {
            Devmasters.Cache.LocalMemory.LocalMemoryCache<Index<Autocomplete>> FullTextSearchCache =
                new Devmasters.Cache.LocalMemory.LocalMemoryCache<Index<Autocomplete>>(TimeSpan.FromDays(30), "nasipolitici_firmy_autocomplete",
                o =>
                {
                    return BuildNPFirmySearchIndex();
                });

            var searchCache = FullTextSearchCache.Get();

            var searchResult = searchCache.Search(q, 8);

            if (!string.IsNullOrEmpty(Request.Headers["Origin"]))
            {
                if (Request.Headers["Origin"].Contains(".hlidacstatu.cz"))
                    Response.AddHeader("Access-Control-Allow-Origin", Request.Headers["Origin"]);
            }
            return Json(searchResult.Select(r => r.Original), JsonRequestBehavior.AllowGet);

        }

        private Index<Autocomplete> BuildNPFirmySearchIndex()
        {
            var results = StaticData.Autocomplete_Firmy_Cache.Get();

            var index = new Index<Autocomplete>(results);

            return index;
        }

        public ActionResult Search(string query, int? page, int? order)
        {
            page = page ?? 1;
            order = order ?? 0;
            Lib.Searching.SmlouvaSearchResult res = null;
            var apires = Framework.ApiAuth.IsApiAuth(this, 
                parameters: new Framework.ApiCall.CallParameter[] { 
                    new Framework.ApiCall.CallParameter("query", query), 
                    new Framework.ApiCall.CallParameter("page", page?.ToString()), 
                    new Framework.ApiCall.CallParameter("order", order?.ToString()) 
                }
                );

            if (apires.Authentificated)
            {
                if (string.IsNullOrWhiteSpace(query))
                    return View("Error404");

                bool? platnyzaznam = null; //1 - nic defaultne
                if (
                    System.Text.RegularExpressions.Regex.IsMatch(query.ToLower(), "(^|\\s)id:")
                    ||
                    query.ToLower().Contains("idverze:")
                    ||
                    query.ToLower().Contains("idsmlouvy:")
                    ||
                    query.ToLower().Contains("platnyzaznam:")
                    )
                    platnyzaznam = null;

                res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch(query, page.Value,
                    HlidacStatu.Lib.Data.Smlouva.Search.DefaultPageSize,
                    (HlidacStatu.Lib.Data.Smlouva.Search.OrderResult)order.Value,
                    platnyZaznam: platnyzaznam);


                if (res.IsValid == false)
                {
                    Response.StatusCode = 500;
                    return Json(new { error = "Bad query", reason = res.ElasticResults.ServerError }, JsonRequestBehavior.AllowGet);

                }
                else
                {

                    var filtered = res.Results
                                    .Select(m =>  HlidacStatu.Lib.Data.Smlouva.Export(m,apires.ApiCall.UserRoles.Contains("Admin"),false))
                                    .ToArray();

                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { total = res.Total, items = filtered }, Newtonsoft.Json.Formatting.None), "application/json");
                }
            }
            else
            {
                Response.StatusCode = 401;
                return Json(new { error = "Unauthorized" }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Detail(string _id)
        {
            string Id = _id;

            var apires = Framework.ApiAuth.IsApiAuth(this, parameters: new Framework.ApiCall.CallParameter[] { new Framework.ApiCall.CallParameter("Detail", Id) });
            if (apires.Authentificated)
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return View("Error404");

                var model = HlidacStatu.Lib.Data.Smlouva.Load(Id);
                if (model == null)
                {
                    return View("Error404");
                }
                var smodel = Smlouva.Export(model, 
                    allData: apires.ApiCall.UserRoles.Contains("Admin"),
                    docsContent: true
                    );
                var s = Newtonsoft.Json.JsonConvert.SerializeObject(
                                    smodel,
                                    new Newtonsoft.Json.JsonSerializerSettings()
                                    {
                                        Formatting = (Request.QueryString["nice"]=="1" ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None),
                                        //NullValueHandling = NullValueHandling.Ignore,         
                                        ContractResolver = new HlidacStatu.Util.FirstCaseLowercaseContractResolver()
                                    }
                                );

                return Content(s, "application/json");
            }
            else
            {
                Response.StatusCode = 401;
                return Json(new { error = "Unauthorized" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ClassificationList(int pageSize = 200)
        {
            if (Framework.ApiAuth.IsApiAuth(this).Authentificated)
            {
                //string urlListTemplate = "https://www.hlidacstatu.cz/Api/V1/ClassificationList?date={0}&page={1}&pagesize={2}";
                string urlItemTemplate = "https://www.hlidacstatu.cz/api/v1/GetForClassification/{0}";


                if (pageSize > 500)
                    pageSize = 500;

                var items = Smlouva.Search.SimpleSearch("NOT(_exists_:prilohy.datlClassification)", 0, pageSize,
                    Smlouva.Search.OrderResult.DateAddedDesc, platnyZaznam: true);

                if (!items.IsValid)
                {
                    Response.StatusCode = 400;
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                var result = new Models.ApiV1Models.ClassificatioListItemModel();

                var contracts = items.ElasticResults.Hits
                            .Select(m => new Models.ApiV1Models.ClassificatioListItemModel.Contract()
                            {
                                contractId = m.Source.Id,
                                url = string.Format(urlItemTemplate, m.Source.Id)
                            }
                            )
                            .ToArray();
                result.contracts = contracts;

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = 401;
                return Json(new { error = "Unauthorized" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Obsolete]
        public ActionResult GetForClassification(string _id)
        {
            string Id = _id;

            if (Framework.ApiAuth.IsApiAuth(this).Authentificated)
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return View("Error404");

                var model = HlidacStatu.Lib.Data.Smlouva.Load(Id);
                if (model == null)
                {
                    return View("Error404");
                }
                if (model.znepristupnenaSmlouva() && model.Prilohy != null)
                {
                    foreach (var p in model.Prilohy)
                    {
                        p.PlainTextContent = "-- anonymizovano serverem hlidacstatu.cz --";
                        p.odkaz = "";
                    }
                }

                if (model.Prilohy != null)
                {
                    model.Save();
                }


                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.None), "application/json");
            }
            else
                return View("Error401");
        }

        class osobaResult
        {
            public osobaResult(Osoba o)
            {
                this.TitulPred = o.TitulPred;
                this.Jmeno = o.Jmeno;
                this.Prijmeni = o.Prijmeni;
                this.TitulPo = o.TitulPo;
                this.Narozeni = o.Narozeni;
                this.NameId = o.NameId;
                this.Profile = o.GetUrl();
            }
            public string TitulPred { get; set; }
            public string Jmeno { get; set; }
            public string Prijmeni { get; set; }
            public string TitulPo { get; set; }

            public DateTime? Narozeni { get; set; }
            public string NameId { get; set; }
            public string Profile { get; set; }
        }
        public ActionResult OsobaSmazat(string nameId)
        {
            var auth = Framework.ApiAuth.IsApiAuth(this, "TeamMember");
            if (auth.Authentificated)
            {
                HlidacStatu.Lib.Data.Osoba o = HlidacStatu.Lib.Data.Osoba.GetByNameId(nameId);
                if (o == null)
                {
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { valid = false, error = "Not found." }), "application/json");
                }

                o.Delete(auth.ApiCall.User);
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                    new { valid = true })
                    , "application/json");

            }
            else
            {
                return View("Error401");
            }
        }

        public ActionResult OsobaPridat(string jmeno, string prijmeni, string narozeni, string titulPred, string titulPo, int typOsoby)
        {
            var auth = Framework.ApiAuth.IsApiAuth(this, "TeamMember");
            if (auth.Authentificated)
            {
                DateTime? nar = Devmasters.DT.Util.ToDateTimeFromCode(narozeni);
                if (nar.HasValue == false)
                {
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { valid = false, error = "Invalid date format. Use yyyy-MM-dd format." }), "application/json");
                }
                if (typOsoby < 0 || typOsoby > 3)
                {
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { valid = false, error = "Invalid typOsoby. Use 0 = NeniPolitik , 1 = ByvalyPolitik , 2 = VazbyNaPolitiky , 3 = Politik." }));
                }

                var no = Osoba.GetOrCreateNew(titulPred, jmeno, prijmeni, titulPo, nar, (Osoba.StatusOsobyEnum)typOsoby, auth.ApiCall.User);
                no.Vazby(true);

                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                    new { valid = true, nameId = no.NameId })
                    , "application/json");

            }
            else
            {
                return View("Error401");
            }
        }


        public ActionResult PolitikFromText(string text)
        {
            if (Framework.ApiAuth.IsApiAuth(this, "TeamMember").Authentificated)
            {
                var oo = Lib.Data.Osoba.Searching.GetFirstPolitikFromText(text);

                if (oo != null)
                {
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                        new { osobaid = oo.NameId, jmeno = oo.Jmeno, prijmeni = oo.Prijmeni }
                        ), "application/json");
                }
                else
                {
                    return Content("{}", "application/json");
                }
            }
            else
            {
                return View("Error401");
            }
        }
        public ActionResult PoliticiFromText(string text)
        {
            if (Framework.ApiAuth.IsApiAuth(this, "TeamMember").Authentificated)
            {
                var oo = Lib.Data.Osoba.Searching.GetBestPoliticiFromText(text);

                if (oo != null)
                {
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                        oo
                            .Select(o=> new { osobaid = o.NameId, jmeno = o.Jmeno, prijmeni = o.Prijmeni })
                            .ToArray()
                        ), "application/json");
                }
                else
                {
                    return Content("[]", "application/json");
                }
            }
            else
            {
                return View("Error401");
            }
        }
        public ActionResult OsobaHledat(string jmeno, string prijmeni, string narozen)
        {
            if (Framework.ApiAuth.IsApiAuth(this, "TeamMember").Authentificated)
            {
                DateTime? dt = Devmasters.DT.Util.ToDateTime(narozen
                    , "yyyy-MM-dd");
                if (dt.HasValue == false)
                {
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                        new { error = "invalid date format. Use yyyy-MM-dd format." }
                        ), "application/json");
                }
                var found = Osoba.Searching.GetAllByNameAscii(jmeno, prijmeni, dt.Value)
                    .Select(o => new osobaResult(o))
                    .ToArray();

                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                    new { Total = found.Count(), Result = found }
                    ), "application/json");
            }
            else
            {
                return View("Error401");
            }
        }

        public ActionResult CheckText(string smlouvaid)
        {
            HlidacStatu.Lib.Issues.IIssueAnalyzer textCheck = new HlidacStatu.Plugin.IssueAnalyzers.Text();
            Smlouva s = HlidacStatu.Lib.Data.Smlouva.Load(smlouvaid);
            if (s != null)
            {
                if (s.Prilohy != null && s.Prilohy.Count() > 0)
                {
                    var newIss = s.Issues.Where(m => m.IssueTypeId != 200).ToList();
                    newIss.AddRange(textCheck.FindIssues(s));
                    s.Issues = newIss.ToArray();
                    s.Save();
                }
            }
            return Content("OK");
        }


    }
}