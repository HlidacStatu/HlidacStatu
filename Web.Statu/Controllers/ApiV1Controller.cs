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

        // GET: ApiV1
        [Authorize]
        public ActionResult Index()
        {
            //global::hlst
            ViewBag.Token = HlidacStatu.Lib.Data.AspNetUserToken.GetToken(this.User.Identity.Name).Token.ToString("N");

            return View();
        }

        public ActionResult ResendConfirmationMail(string id)
        {

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


        public ActionResult Dumps()
        {
            if (Framework.ApiAuth.IsApiAuth(this, parameters: new Framework.ApiCall.CallParameter[] { new Framework.ApiCall.CallParameter("Dumps", "") }).Authentificated)
            {
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(GetDumps()), "application/json");
            }
            else
                return new HttpStatusCodeResult(401);

        }

        public ActionResult OCRStats(string type="")
        {

                string cnnStr = Devmasters.Core.Util.Config.GetConfigValue("CnnString");
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
                using (var p = new Devmasters.Core.PersistLib())
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

        private List<Models.ApiV1Models.DumpInfoModel> GetDumps()
        {

            string baseUrl = "https://www.hlidacstatu.cz/api/v1/";
            List<DumpInfoModel> data = new List<DumpInfoModel>();
            if (System.IO.File.Exists(HlidacStatu.Lib.StaticData.App_Data_Path + "dump\\smlouvy.dump.zip"))
            {
                System.IO.FileInfo fi = new FileInfo(HlidacStatu.Lib.StaticData.App_Data_Path + "dump\\smlouvy.dump.zip");
                data.Add(
                    new DumpInfoModel() { url = baseUrl + "Dump", created = fi.LastWriteTimeUtc, date = fi.LastWriteTimeUtc, fulldump = true, size = fi.Length }
                    );
            }
            for (int i = 1; i < 31; i++)
            {
                DateTime date = DateTime.Now.Date.AddDays(-1 * i);
                string fn = HlidacStatu.Lib.StaticData.App_Data_Path + "dump\\smlouvy.dump-" + date.ToString("yyyy-MM-dd") + ".zip";
                if (System.IO.File.Exists(fn))
                {
                    System.IO.FileInfo fil = new FileInfo(fn);
                    data.Add(
                        new DumpInfoModel()
                        {
                            url = baseUrl + "dump?date=" + date.ToString("yyyy-MM-dd"),
                            created = fil.LastWriteTimeUtc,
                            date = date,
                            fulldump = false,
                            size = fil.Length
                        }
                        );

                }
            }

            return data;
        }


        public ActionResult Dump(string date)
        {
            if (Framework.ApiAuth.IsApiAuth(this, parameters: new Framework.ApiCall.CallParameter[] { new Framework.ApiCall.CallParameter("Dump", date) }).Authentificated)
            {
                HlidacStatu.Util.Consts.Logger.Info(new Devmasters.Core.Logging.LogMessage()
                    .SetMessage("Downloading smlouvy.dump.zip")
                    .SetCustomKeyValue("UserId", this.User.Identity.Name)
                            );

                DateTime? specificDate = ParseTools.ToDateTime(date, "yyyy-MM-dd");
                string onlyfile = "smlouvy.dump" + (specificDate.HasValue ? "-" + specificDate.Value.ToString("yyyy-MM-dd") : "");
                string fn = HlidacStatu.Lib.StaticData.App_Data_Path + $"dump\\{onlyfile}" + ".zip";
                if (System.IO.File.Exists(fn))
                {
                    long FileL = (new FileInfo(fn)).Length;
                    byte[] bytes = new byte[1024 * 1024];
                    Response.Clear();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("content-disposition", "attachment; filename=" + System.IO.Path.GetFileName(fn));
                    Response.AddHeader("Content-Length", FileL.ToString());
                    try
                    {

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
                    Response.End();
                    return null;
                    //return File(fn, "application/zip");
                }
                else
                    return new HttpStatusCodeResult(404);
            }
            else
                return new HttpStatusCodeResult(401);

        }


        public ActionResult AddPersonSponzoring(
            string titulpred, string jmeno, string prijmeni, string titulpo, string datumNarozeni,
            string strana, string rok, string castka
            )
        {
            DateTime? dat = HlidacStatu.Util.ParseTools.ToDateTimeFromCode(datumNarozeni);
            if (dat.HasValue == false)
            {
                return new HttpStatusCodeResult(500, "Use date format yyyy-MM-dd");
            }
            var apires = Framework.ApiAuth.IsApiAuth(this);
            if (apires.Authentificated &&
                    (apires.ApiCall.User == "petr@stastny.eu"
                    || apires.ApiCall.User == "michal@michalblaha.cz"
                    )
                )
            {
                Osoba o = Osoba.GetByName(jmeno, prijmeni, dat.Value);
                if (o == null)
                {
                    o = new Osoba() { TitulPred = titulpred, Jmeno = jmeno, Prijmeni = prijmeni, TitulPo = titulpo, Narozeni = dat.Value };
                    o.Status = (int)Osoba.StatusOsobyEnum.VazbyNaPolitiky;
                    o.Save();
                }

                var oe = o.AddSponsoring(strana, Convert.ToInt32(rok), ParseTools.ToDecimal(castka).Value, "https://udhpsh.cz/5290-2/", this.User.Identity.Name, checkDuplicates: false);
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { ok = true }), "application/json");

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

        public ActionResult VZList(string id)
        {
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
        public ActionResult VZDetail(string id)
        {
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
        public ActionResult VZDetail(string id, FormCollection form)
        {
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

        public ActionResult AddCompanySponzoring(
            string ico,
            string strana, string rok, string castka, string udalost
            )
        {
            var apires = Framework.ApiAuth.IsApiAuth(this);
            if (apires.Authentificated &&
                    (apires.ApiCall.User == "petr@stastny.eu"
                    || apires.ApiCall.User == "michal@michalblaha.cz"
                    )
                )
            {
                Firma f = Firma.FromIco(ico, true);
                if (f == null)
                {
                    HlidacStatu.Util.Consts.Logger.Error("API AddCompanySponzoring: ICO " + ico + " not found");
                    return new HttpStatusCodeResult(500, "ICO not found.");
                }

                f.AddSponsoring(strana, Convert.ToInt32(rok), ParseTools.ToDecimal(castka).Value, "https://udhpsh.cz/5290-2/", this.User.Identity.Name, checkDuplicates: false);
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { ok = true }), "application/json");

            }
            else
                return new HttpStatusCodeResult(401);

        }
        public ActionResult Status()
        {
            IClusterHealthResponse res = null;
            int num = 0;
            string status = "unknown";
            try
            {
                res = HlidacStatu.Lib.ES.Manager.GetESClient().ClusterHealth();
                num = res?.NumberOfNodes ?? 0;
                status = res?.Status.ToString() ?? "unknown";
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("Status page error", e);
                ViewBag.Error = e;
            }

            return Content(string.Format("{0}-{1}", num, status));
        }


        //public ActionResult Politici(string q)
        //{
        //    PolitikTypeAhead[] res = new PolitikTypeAhead[] { };
        //    if (string.IsNullOrEmpty(q))
        //        return Json(res);
        //    string nquery = q.NormalizeToPureTextLower();


        //    res = HlidacStatu.Lib.StaticData.Politici.Get()
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


            res = HlidacStatu.Lib.Data.Osoba.GetPolitikByNameFtx(q, 15)
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

        public ActionResult AddInfo(string q)
        {
            List<string> result = new List<string>();

            if (string.IsNullOrEmpty(q) || q.Length <2)
                return Json(result, JsonRequestBehavior.AllowGet);

            result = OsobaEvent.GetAddInfos(q, null, 200).ToList();
                

            if (!string.IsNullOrEmpty(Request.Headers["Origin"]))
            {
                if (Request.Headers["Origin"].Contains(".hlidacstatu.cz")
                    //|| Request.Headers["Origin"].Contains(".hlidacstatu.cz")
                    )
                    Response.AddHeader("Access-Control-Allow-Origin", Request.Headers["Origin"]);
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult TransparentniUctyExport()
        {
            if (Framework.ApiAuth.IsApiAuth(this, parameters: new Framework.ApiCall.CallParameter[] { new Framework.ApiCall.CallParameter("TransparentniUctyExport", "") }).Authentificated)
            {
                var resBU = HlidacStatu.Lib.ES.Manager.GetESClient_Ucty()
                    .Search<HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcet>(m => m
                        .Query(q => q.MatchAll())
                        .Size(1000)
                    );

                if (resBU.Total == 0)
                    return View("Error404");


                var ret = new { upozorneni = "Bez zaruky, zkusebni provoz", transparentniUcty = resBU.Hits.Select(m => m.Source).ToArray() };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ret), "application/json");
                //Json(ret, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = 401;
                return Json(new { error = "Unauthorized" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UcetExport(string id)
        {
            if (Framework.ApiAuth.IsApiAuth(this, parameters: new Framework.ApiCall.CallParameter[] { new Framework.ApiCall.CallParameter("UcetExport", id) }).Authentificated)
            {

                if (string.IsNullOrEmpty(id))
                    return View("Error404");

                var resBU = HlidacStatu.Lib.ES.Manager.GetESClient_Ucty()
                    .Search<HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcet>(m => m
                        .Query(q => q
                            .Term(t => t.Field(ff => ff.CisloUctu).Value(id))
                            )
                    );

                if (resBU.Total == 0)
                    return View("Error404");

                HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcet bu = resBU.Hits.First().Source;

                List<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka> polozky = new List<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>();
                Func<int, int, Nest.ISearchResponse<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>> searchFunc = (size, page) =>
                {
                    return HlidacStatu.Lib.ES.Manager.GetESClient_Ucty()
                            .Search<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>(a => a
                                .Size(size)
                                .From(page * size)
                                .Query(q => q.Term(t => t.Field(f => f.CisloUctu).Value(id)))
                                .Scroll("2m")
                                );
                };

                HlidacStatu.Lib.ES.SearchTools.DoActionForQuery<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>(
                    HlidacStatu.Lib.ES.Manager.GetESClient_Ucty(),
                    searchFunc,
                    (p, o) =>
                    {
                        polozky.Add(p.Source);
                        return new Devmasters.Core.Batch.ActionOutputData();
                    }, null, null, null, false, blockSize: 500

                    );


                var ret = new { upozorneni = "Bez zaruky, zkusebni provoz", bankovniUcet = bu, polozky = polozky };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ret), "application/json");
                //Json(ret, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = 401;
                return Json(new { error = "Unauthorized" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Search(string query, int? page, int? order)
        {
            page = page ?? 1;
            order = order ?? 0;
            HlidacStatu.Lib.ES.SmlouvaSearchResult res = null;

            if (Framework.ApiAuth.IsApiAuth(this, parameters: new Framework.ApiCall.CallParameter[] { new Framework.ApiCall.CallParameter("query", query), new Framework.ApiCall.CallParameter("page", page?.ToString()), new Framework.ApiCall.CallParameter("order", order?.ToString()) }).Authentificated)
            {
                if (string.IsNullOrWhiteSpace(query))
                    return View("Error404");

                int? platnyzaznam = null; //1 - nic defaultne
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

                res = HlidacStatu.Lib.ES.SearchTools.SimpleSearch(query, page.Value,
                    HlidacStatu.Lib.ES.SearchTools.DefaultPageSize,
                    (HlidacStatu.Lib.ES.SearchTools.OrderResult)order.Value,
                    platnyZaznam: platnyzaznam);


                if (res.IsValid == false)
                {
                    Response.StatusCode = 500;
                    return Json(new { error = "Bad query", reason = res.Result.ServerError }, JsonRequestBehavior.AllowGet);

                }
                else
                {

                    var filtered = res.Result.Hits
                                    .Select(m => HlidacStatu.Lib.Data.Smlouva.PrepareForDump(m.Source))
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
        public ActionResult Detail(string Id)
        {
            if (Framework.ApiAuth.IsApiAuth(this, parameters: new Framework.ApiCall.CallParameter[] { new Framework.ApiCall.CallParameter("Detail", Id) }).Authentificated)
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return View("Error404");

                var model = HlidacStatu.Lib.ES.Manager.Load(Id);
                if (model == null)
                {
                    return View("Error404");
                }
                model = Smlouva.PrepareForDump(model);

                if (Request.QueryString["nice"] != null && (Request.QueryString["nice"].ToLower() == "true" || Request.QueryString["nice"] == "1"))
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.Indented), "application/json");
                else
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.None), "application/json");
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
                string urlListTemplate = "https://www.hlidacstatu.cz/Api/V1/ClassificationList?date={0}&page={1}&pagesize={2}";
                string urlItemTemplate = "https://www.hlidacstatu.cz/api/v1/GetForClassification/{0}";


                if (pageSize > 500)
                    pageSize = 500;

                var items = HlidacStatu.Lib.ES.SearchTools.SimpleSearch("NOT(_exists_:prilohy.datlClassification)", 0, pageSize,
                    HlidacStatu.Lib.ES.SearchTools.OrderResult.DateAddedDesc, platnyZaznam: 1);

                if (!items.IsValid)
                {
                    Response.StatusCode = 400;
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                var result = new Models.ApiV1Models.ClassificatioListItemModel();

                var contracts = items.Result.Hits
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

        public ActionResult GetForClassification(string Id)
        {
            if (Framework.ApiAuth.IsApiAuth(this).Authentificated)
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return View("Error404");

                var model = HlidacStatu.Lib.ES.Manager.Load(Id);
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
                    foreach (var p in model.Prilohy)
                    {
                        p.DatlClassification = new Smlouva.Priloha.Classification() { Created = DateTime.Now };

                    }
                    HlidacStatu.Lib.ES.Manager.Save(model);
                }


                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.None), "application/json");
            }
            else
                return View("Error401");
        }


        private class BankovniPolozkaForExport
        {
            public class xOsoba
            {
                public string TitulPred { get; set; }
                public string Jmeno { get; set; }
                public string Prijmeni { get; set; }
                public string TitulPo { get; set; }
                public string NameId { get; set; }
                public System.DateTime? Narozeni { get; set; }
            }
            public class xFirma
            {
                public string Jmeno { get; set; }
                public string ICO { get; set; }
            }

            public HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka Transaction { get; set; }

            public xOsoba FoundPerson { get; set; }
            public xFirma FoundCompany { get; set; }
        }
        public ActionResult TransparentniUctyFullExport()
        {
            string login = "";
            List<BankovniPolozkaForExport> items = new List<BankovniPolozkaForExport>();
            if (Framework.ApiAuth.IsApiAuth(this, parameters: new Framework.ApiCall.CallParameter[] { new Framework.ApiCall.CallParameter("TransparentniUctyFullExport", "") }).Authentificated)
            {
                if (!string.IsNullOrEmpty(this.User?.Identity?.Name))
                    login = this.User?.Identity?.Name;
                if (login == "jskuhrovec@gmail.com" || login == "titl.vitezslav@gmail.com" || login == "michal@michalblaha.cz")
                {

                    HlidacStatu.Lib.ES.SearchTools.DoActionForAll<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>(
                        (t, obj) =>
                        {

                            var bp = t.Source;

                            Osoba o = null;
                            Firma f = null;
                            if (bp.Comments.Any(m => m.Type == HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka.Comment.Types.VazbaNaOsobu))
                            {
                                o = Osoby.GetById.Get(
                                    bp.Comments
                                    .Where(m => m.Type == HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka.Comment.Types.VazbaNaOsobu)
                                    .First()
                                    .ValueInt);
                            }
                            if (bp.Comments.Any(m => m.Type == HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka.Comment.Types.VazbaNaFirmu))
                            {
                                f = Firmy.Get(
                                    bp.Comments
                                    .Where(m => m.Type == HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka.Comment.Types.VazbaNaFirmu)
                                    .First()
                                    .ValueInt);
                                if (f.Valid == false)
                                    f = null;
                            }


                            BankovniPolozkaForExport bpe = new Controllers.ApiV1Controller.BankovniPolozkaForExport();
                            bpe.Transaction = bp;
                            bpe.Transaction.Comments = null;
                            if (o != null)
                                bpe.FoundPerson = new BankovniPolozkaForExport.xOsoba()
                                {
                                    Jmeno = o.Jmeno,
                                    NameId = o.NameId,
                                    Narozeni = o.Narozeni,
                                    Prijmeni = o.Prijmeni,
                                    TitulPred = o.TitulPred,
                                    TitulPo = o.TitulPo
                                };

                            if (f != null)
                                bpe.FoundCompany = new BankovniPolozkaForExport.xFirma()
                                {
                                    ICO = f.ICO,
                                    Jmeno = f.Jmeno
                                };

                            items.Add(bpe);

                            return new Devmasters.Core.Batch.ActionOutputData();
                        }, null,
                    null, new Devmasters.Core.Batch.ActionProgressWriter(1f).Write,
                    false,
                    elasticClient: HlidacStatu.Lib.ES.Manager.GetESClient_Ucty(),
                    blockSize: 50)
                    ;

                }


            }
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.None), "application/json");

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
                DateTime? nar = ParseTools.ToDateTimeFromCode(narozeni);
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

       
        public ActionResult OsobaHledat(string jmeno, string prijmeni, string narozen)
        {
            if (Framework.ApiAuth.IsApiAuth(this, "TeamMember").Authentificated)
            {
                DateTime? dt = ParseTools.ToDateTime(narozen
                    , "yyyy-MM-dd");
                if (dt.HasValue == false)
                {
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                        new { error = "invalid date format. Use yyyy-MM-dd format." }
                        ), "application/json");
                }
                var found = Osoba.GetAllByNameAscii(jmeno, prijmeni, dt.Value)
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
            Smlouva s = HlidacStatu.Lib.ES.Manager.Load(smlouvaid);
            if (s != null)
            {
                if (s.Prilohy != null && s.Prilohy.Count() > 0)
                {
                    var newIss = s.Issues.Where(m => m.IssueTypeId != 200).ToList();
                    newIss.AddRange(textCheck.FindIssues(s));
                    s.Issues = newIss.ToArray();
                    HlidacStatu.Lib.ES.Manager.Save(s);
                }
            }
            return Content("OK");
        }


    }
}