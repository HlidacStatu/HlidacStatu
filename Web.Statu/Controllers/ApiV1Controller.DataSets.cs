using HlidacStatu.Lib.Data.External.DataSets;
using HlidacStatu.Util;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

namespace HlidacStatu.Web.Controllers
{
    public partial class ApiV1Controller : Controllers.GenericAuthController
    {


        [System.Web.Mvc.NonAction]
        public ActionResult _templateAction(string query, int? page, int? order)
        {
            page = page ?? 1;
            order = order ?? 0;

            if (!Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("query", query),
                    new Framework.ApiCall.CallParameter("page", page?.ToString()),
                    new Framework.ApiCall.CallParameter("order", order?.ToString())
                })
                .Authentificated)
            {
                //Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Ok = true }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetJmenoPrijmeniFromString(string text)
        {
            if (!Framework.ApiAuth.IsApiAuth(this,
                   parameters: new Framework.ApiCall.CallParameter[] {
                                new Framework.ApiCall.CallParameter("text", text),
                   })
                   .Authentificated)
            {
                //Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var o = HlidacStatu.Lib.Validators.JmenoInText(text);
                if (o == null)
                    return Json(new { }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new
                    {
                        titulPred = o.TitulPred,
                        jmeno = o.Jmeno,
                        prijmeni = o.Prijmeni,
                        titulPo = o.TitulPo
                    }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult FindOsobaId(string jmeno, string prijmeni, string celejmeno, string narozeni, string funkce)
        {

            if (!Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("jmeno", jmeno),
                    new Framework.ApiCall.CallParameter("prijmeni", prijmeni),
                    new Framework.ApiCall.CallParameter("celejmeno", celejmeno),
                    new Framework.ApiCall.CallParameter("narozeni", narozeni),
                    new Framework.ApiCall.CallParameter("funkce", funkce)
                })
                .Authentificated)
            {
                //Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {
                DateTime? dt = Devmasters.DT.Util.ToDateTime(narozeni, "yyyy-MM-dd");
                if (dt.HasValue == false && string.IsNullOrEmpty(funkce))
                {
                    var status = ApiResponseStatus.InvalidFormat;
                    status.error.errorDetail = "invalid date format for parameter 'narozeni'. Use yyyy-MM-dd format.";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(jmeno) && string.IsNullOrEmpty(prijmeni) && !string.IsNullOrEmpty(celejmeno))
                {
                    Lib.Data.Osoba osobaZeJmena = Lib.Validators.JmenoInText(celejmeno);
                    if (osobaZeJmena == null)
                    {
                        jmeno = "";
                        prijmeni = "";
                    }
                    else
                    {
                        jmeno = osobaZeJmena.Jmeno;
                        prijmeni = osobaZeJmena.Prijmeni;
                    }
                }
                if (string.IsNullOrEmpty(jmeno) || string.IsNullOrEmpty(prijmeni))
                {
                    var status = ApiResponseStatus.InvalidFormat;
                    status.error.errorDetail = "no data for parameter 'jmeno' or 'prijmeni' or 'celejmeno'.";
                    return Json(status, JsonRequestBehavior.AllowGet);

                }

                IEnumerable<Lib.Data.Osoba> found = null;
                if (dt.HasValue)
                    found = FindByDate(jmeno, prijmeni, dt);
                else
                    found = FindByFunkce(jmeno, prijmeni, funkce);

                if (found == null || found?.Count() == 0)
                    return Json(new { }, JsonRequestBehavior.AllowGet);
                else
                {
                    var f = found.First();
                    return Json(new
                    {
                        Jmeno = f.Jmeno,
                        Prijmeni = f.Prijmeni,
                        //Narozeni = found.Narozeni.Value.ToString("yyyy-MM-dd"),
                        OsobaId = f.NameId
                    }
                            , JsonRequestBehavior.AllowGet);
                }
            }
        }

        [System.Web.Mvc.NonAction]
        private IEnumerable<Lib.Data.Osoba> FindByFunkce(string jmeno, string prijmeni, string funkce)
        {
            if (string.IsNullOrEmpty(funkce))
            {
                return new Lib.Data.Osoba[] { };
            }

            var found = HlidacStatu.Lib.StaticData.PolitickyAktivni.Get()
                        .Where(o =>
                            string.Equals(o.Jmeno, jmeno, StringComparison.OrdinalIgnoreCase)
                            && string.Equals(o.Prijmeni, prijmeni, StringComparison.OrdinalIgnoreCase)
                            )
                        .ToArray();
            ;

            if (found?.Count() > 0)
            {

            }
            else
            {
                string jmenoasc = Devmasters.TextUtil.RemoveDiacritics(jmeno);
                string prijmeniasc = Devmasters.TextUtil.RemoveDiacritics(prijmeni);
                found = HlidacStatu.Lib.StaticData.PolitickyAktivni.Get()
                            .Where(o =>
                                string.Equals(o.JmenoAscii, jmenoasc, StringComparison.OrdinalIgnoreCase)
                                && string.Equals(o.PrijmeniAscii, prijmeniasc, StringComparison.OrdinalIgnoreCase)
                                )
                            .ToArray();
            }

            funkce = HlidacStatu.Util.ParseTools.NormalizePolitikFunkce(funkce);

            found = found
                .Where(m =>
                    m.Events().Any(e => HlidacStatu.Util.ParseTools.FindInStringSqlLike(e.AddInfo, funkce))
                    )
                .ToArray();

            return found ?? new Lib.Data.Osoba[] { };
        }


        [System.Web.Mvc.NonAction]
        private IEnumerable<Lib.Data.Osoba> FindByDate(string jmeno, string prijmeni, DateTime? dt)
        {
            if (dt.HasValue == false)
            {
                return new Lib.Data.Osoba[] { };
            }

            var found = HlidacStatu.Lib.StaticData.PolitickyAktivni.Get()
                        .Where(o =>
                            string.Equals(o.Jmeno, jmeno, StringComparison.OrdinalIgnoreCase)
                            && string.Equals(o.Prijmeni, prijmeni, StringComparison.OrdinalIgnoreCase)
                            && o.Narozeni == dt.Value);

            if (found?.Count() > 0)
                return found;

            string jmenoasc = Devmasters.TextUtil.RemoveDiacritics(jmeno);
            string prijmeniasc = Devmasters.TextUtil.RemoveDiacritics(prijmeni);
            found = HlidacStatu.Lib.StaticData.PolitickyAktivni.Get()
                        .Where(o =>
                            string.Equals(o.JmenoAscii, jmenoasc, StringComparison.OrdinalIgnoreCase)
                            && string.Equals(o.PrijmeniAscii, prijmeniasc, StringComparison.OrdinalIgnoreCase)
                            && o.Narozeni == dt.Value)
                        ;

            return found ?? new Lib.Data.Osoba[] { };
        }


        public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                filterContext.RequestContext.HttpContext.Response.Headers.Remove("Access-Control-Allow-Origin");
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
                base.OnActionExecuting(filterContext);
            }
        }

        public ActionResult DatasetTemplatePreview(string _id, string type, string template)
        {
            string id = _id;
            if (string.IsNullOrEmpty(id))
                return Json(ApiResponseStatus.DatasetNotFound, JsonRequestBehavior.AllowGet);

            var reg = DataSetDB.Instance.GetRegistration(id);
            if (reg == null)
                return Json(ApiResponseStatus.DatasetNotFound, JsonRequestBehavior.AllowGet);

            try
            {

                var ds = DataSet.CachedDatasets.Get(id);
                if (type == "jsonschema")
                {
                    return Content(ds.Registration().jsonSchema);
                }
                else if (type == "search")
                {
                    var res = ds.SearchData("*", 1, 5, "DbCreated desc");
                    var temp = new Registration.Template() { body = template };

                    var html = temp.Render(ds, res, "*");

                    return Content(html);
                }
                else if (type == "detail")
                {
                    var res = ds.SearchData("*", 1, 1, "DbCreated desc")?.Result?.FirstOrDefault();
                    var temp = new Registration.Template() { body = template };

                    var html = temp.Render(ds, res);
                }
                else
                {
                    return Content("");
                }
            }
            catch (Exception e)
            {
                var msg = e?.InnerException?.Message ?? e.Message;
                msg = Devmasters.RegexUtil.ReplaceWithRegex(msg, "", @".*: \s* error \s* CS\d{1,8}:");

                return Content($"<h2>Chyba v template - zpráva pro autora této databáze</h2><pre>{msg}</pre>");
            }
            return Content("");

        }


        [HttpGet]
        public ActionResult FindCompanyID(string companyName)
        {
            return CompanyID(companyName);
        }
        [HttpGet]
        public ActionResult CompanyID(string companyName)
        {
            if (!Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("companyName", companyName)
                })
                .Authentificated)
            {
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(companyName))
                        return Json(new { }, JsonRequestBehavior.AllowGet);
                    else
                    {
                        //HlidacStatu.Lib.Data.Firma f = HlidacStatu.Lib.Validators.FirmaInText(companyName);
                        var name = HlidacStatu.Lib.Data.Firma.JmenoBezKoncovky(companyName);
                        var found = HlidacStatu.Lib.Data.Firma.Search.FindAll(name, 1).FirstOrDefault();
                        if (found == null)
                            return Json(new { }, JsonRequestBehavior.AllowGet);
                        else
                            return Json(new { ICO = found.ICO, Jmeno = found.Jmeno, DatovaSchranka = found.DatovaSchranka }, JsonRequestBehavior.AllowGet);
                    }


                }
                catch (DataSetException dse)
                {
                    return Json(dse.APIResponse, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    HlidacStatu.Util.Consts.Logger.Error("Dataset API", ex);
                    return Json(ApiResponseStatus.GeneralExceptionError(ex), JsonRequestBehavior.AllowGet);
                }

            }
        }

        [HttpPost, ActionName("Datasets")]
        [ValidateInput(false)]
        public ActionResult Datasets_Create()
        {
            var data = ReadRequestBody(this.Request);
            var apiAuth = Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("data", data)
                });
            if (!apiAuth.Authentificated)
            {
                //Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess);
            }
            else
            {
                try
                {
                    var reg = Newtonsoft.Json.JsonConvert.DeserializeObject<Registration>(data, DataSet.DefaultDeserializationSettings);
                    var res = DataSet.Api.Create(reg, apiAuth.ApiCall.User);

                    if (res.valid)
                        return Json(new { datasetId = ((DataSet)res.value).DatasetId });
                    else
                        return Json(res);
                }
                catch (Newtonsoft.Json.JsonSerializationException jex)
                {
                    var status = ApiResponseStatus.DatasetItemInvalidFormat;
                    status.error.errorDetail = jex.Message;
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                catch (DataSetException dse)
                {
                    return Json(dse.APIResponse, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    HlidacStatu.Util.Consts.Logger.Error("Dataset API", ex);
                    return Json(ApiResponseStatus.GeneralExceptionError(ex), JsonRequestBehavior.AllowGet);

                }


            }
        }


        [HttpPut, ActionName("Datasets")]
        [ValidateInput(false)]
        public ActionResult Datasets_Update(string _id)
        {
            string id = _id;

            var data = ReadRequestBody(this.Request);
            var apiAuth = Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("id", id)
                });
            if (!apiAuth.Authentificated)
            {
                //Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess);
            }
            else
            {

                try
                {
                    var newReg = Newtonsoft.Json.JsonConvert.DeserializeObject<Registration>(data, DataSet.DefaultDeserializationSettings);
                    return Json(DataSet.Api.Update(newReg, apiAuth.ApiCall?.User), JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    HlidacStatu.Util.Consts.Logger.Error("Dataset API", ex);
                    return Json(ApiResponseStatus.GeneralExceptionError(ex), JsonRequestBehavior.AllowGet);
                }

            }
        }

        [HttpPut, ActionName("DatasetsPart")]
        [ValidateInput(false)]
        public ActionResult DatasetsPart_Update(string _id, string atribut)
        {
            string id = _id;

            if (string.IsNullOrEmpty(atribut))
                return Json(ApiResponseStatus.InvalidFormat, JsonRequestBehavior.AllowGet);

            var data = ReadRequestBody(this.Request);
            var apiAuth = Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("id", id),
                    new Framework.ApiCall.CallParameter("atribut", atribut)
                });
            if (!apiAuth.Authentificated)
            {
                //Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess);
            }
            else
            {
                try
                {
                    var newReg = Newtonsoft.Json.JsonConvert.DeserializeObject<Registration>(data, DataSet.DefaultDeserializationSettings);

                    return Json(DataSet.Api.Update(newReg, apiAuth.ApiCall?.User?.ToLower()), JsonRequestBehavior.AllowGet);
                }
                catch (DataSetException dse)
                {
                    return Json(dse.APIResponse, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    HlidacStatu.Util.Consts.Logger.Error("Dataset API", ex);
                    return Json(ApiResponseStatus.GeneralExceptionError(ex), JsonRequestBehavior.AllowGet);
                }


            }
        }


        [HttpPatch, ActionName("Datasets")]
        public ActionResult Datasets_Patch(string _id)
        {
            string id = _id;

            return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
        }
        [HttpOptions, ActionName("Datasets")]
        public ActionResult Datasets_Options(string _id)
        {
            string id = _id;

            return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
        }
        [HttpGet, ActionName("Datasets")]
        public ActionResult Datasets_GET(string _id)
        {
            string id = _id;

            if (!Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("id", id)
                })
                .Authentificated)
            {
                //Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(id))
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(DataSetDB.Instance.SearchData("*", 1, 100).Result,
                                Newtonsoft.Json.Formatting.None,
                                new JsonSerializerSettings() { ContractResolver = Serialization.PublicDatasetContractResolver.Instance })
                            , "application/json");
                    else
                    {
                        var ds = DataSet.CachedDatasets.Get(id);
                        if (ds == null)
                            return Json(ApiResponseStatus.DatasetNotFound, JsonRequestBehavior.AllowGet);
                        else
                            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Registration()), "application/json");
                    }

                }
                catch (DataSetException dse)
                {
                    return Json(dse.APIResponse, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    HlidacStatu.Util.Consts.Logger.Error("Dataset API", ex);
                    return Json(ApiResponseStatus.GeneralExceptionError(ex), JsonRequestBehavior.AllowGet);

                }

            }
        }

        [HttpDelete, ActionName("Datasets")]
        public ActionResult Datasets_Delete(string _id)
        {
            string id = _id;

            var apiAuth = Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("id", id)
                });
            if (!apiAuth.Authentificated)
            {
                //Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(id))
                        return Json(ApiResponseStatus.DatasetNotFound, JsonRequestBehavior.AllowGet);

                    id = id.ToLower();
                    var r = DataSetDB.Instance.GetRegistration(id);
                    if (r == null)
                        return Json(ApiResponseStatus.DatasetNotFound, JsonRequestBehavior.AllowGet);

                    if (r.createdBy != null && apiAuth.ApiCall.User.ToLower() != r.createdBy?.ToLower())
                    {
                        return Json(ApiResponseStatus.DatasetNoPermision, JsonRequestBehavior.AllowGet);
                    }

                    var res = DataSetDB.Instance.DeleteRegistration(id, apiAuth.ApiCall.User);
                    return Json(new ApiResponseStatus() { valid = res }, JsonRequestBehavior.AllowGet);

                }
                catch (DataSetException dse)
                {
                    return Json(dse.APIResponse, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    HlidacStatu.Util.Consts.Logger.Error("Dataset API", ex);
                    return Json(ApiResponseStatus.GeneralExceptionError(ex), JsonRequestBehavior.AllowGet);

                }

            }
        }


        [HttpGet, ActionName("DatasetSearch")]
        [ValidateInput(false)]
        public ActionResult DatasetSearch(string _id, string q, int? page, string sort = null, string desc = "0")
        {
            string id = _id;

            page = page ?? 1;
            if (page < 1)
                page = 1;
            if (page > 200)
                return Content(
                    Newtonsoft.Json.JsonConvert.SerializeObject(
                    new { total = 0, page = 201, results = Array.Empty<dynamic>() }
                )
                , "application/json");

            if (!Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("id", id),
                    new Framework.ApiCall.CallParameter("q", q),
                    new Framework.ApiCall.CallParameter("page", page?.ToString()),
                    new Framework.ApiCall.CallParameter("sort", sort)
                })
                .Authentificated)
            {
                //Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var ds = DataSet.CachedDatasets.Get(id?.ToLower());
                    if (ds == null)
                        return Json(ApiResponseStatus.DatasetNotFound, JsonRequestBehavior.AllowGet);

                    if (false)
                    {
                        var res = ds.SearchDataRaw(q, page.Value, 50, null);

                        System.Text.StringBuilder sb = new System.Text.StringBuilder(512 * (int)res.Total);
                        sb.Append($"{{ \"total\": {res.Total}, \"page\": {page}, \"results\" : [ ");
                        foreach (var item in res.Result)
                        {
                            sb.Append(item.Item2 + ", ");
                        }
                        sb.Remove(sb.Length - 2, 2);
                        sb.Append($"]}}");

                        return Content(sb.ToString(), "application/json");
                    }
                    else
                    {
                        bool bDesc = (desc == "1" || desc?.ToLower() == "true");
                        var res = ds.SearchData(q, page.Value, 50, sort + (bDesc ? " desc" : ""));
                        res.Result = res.Result.Select(m => { m.DbCreatedBy = null; return m; });


                        return Content(
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                            new { total = res.Total, page = res.Page, results = res.Result }
                        )
                        , "application/json");

                    }

                }
                catch (DataSetException dex)
                {
                    return Json(dex.APIResponse, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(ApiResponseStatus.GeneralExceptionError(ex), JsonRequestBehavior.AllowGet);
                }

            }
        }

        [HttpGet, ActionName("DatasetItem")]
        public ActionResult DatasetItem_Get(string _id, string _dataid)
        {
            string id = _id;
            string dataid = _dataid;

            if (!Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("id", id),
                    new Framework.ApiCall.CallParameter("dataid", dataid)
                })
                .Authentificated)
            {
                //Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var ds = DataSet.CachedDatasets.Get(id.ToLower());
                    var value = ds.GetDataObj(dataid);
                    //remove from item
                    if (value == null)
                    {
                        return Content("null", "application/json");
                    }
                    else
                    {
                        value.DbCreatedBy = null;
                        return Content(
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                value, Request.QueryString["nice"] == "1" ? Formatting.Indented : Formatting.None
                                ) ?? "null", "application/json");
                    }
                }
                catch (DataSetException)
                {
                    return Json(ApiResponseStatus.DatasetNotFound, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(ApiResponseStatus.GeneralExceptionError(ex), JsonRequestBehavior.AllowGet);
                }

            }
        }



        [HttpGet, ActionName("DatasetItem_Exists")]
        public ActionResult DatasetItem_Exists(string _id, string _dataid)
        {
            string id = _id;
            string dataid = _dataid;

            if (!Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("id", id),
                    new Framework.ApiCall.CallParameter("dataid", dataid)
                })
                .Authentificated)
            {
                //Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var ds = DataSet.CachedDatasets.Get(id.ToLower());
                    var value = ds.ItemExists(dataid);
                    //remove from item
                    if (value == null)
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(false), "application/json");
                    else
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(true), "application/json");
                }
                catch (DataSetException)
                {
                    return Json(ApiResponseStatus.DatasetNotFound, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(ApiResponseStatus.GeneralExceptionError(ex), JsonRequestBehavior.AllowGet);
                }

            }
        }


        [HttpPost, ActionName("DatasetItem")]
        [ValidateInput(false)]
        public ActionResult DatasetItem_Post(string _id, string _dataid, string mode = "", bool? rewrite = false) //rewrite for backwards compatibility
        {
            string id = _id;
            string dataid = _dataid;

            var apiAuth = Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("id", id),
                    new Framework.ApiCall.CallParameter("dataid", dataid),
                    new Framework.ApiCall.CallParameter("mode", mode)
                });
            if (!apiAuth.Authentificated)
            {
                //Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess);
            }
            else
            {

                mode = mode.ToLower();
                if (string.IsNullOrEmpty(mode))
                {
                    if (rewrite == true)
                        mode = "rewrite";
                    else
                        mode = "skip";
                }

                var data = ReadRequestBody(this.Request);
                id = id.ToLower();
                try
                {
                    var ds = DataSet.CachedDatasets.Get(id);
                    var newId = dataid;

                    if (mode == "rewrite")
                    {
                        newId = ds.AddData(data, dataid, apiAuth.ApiCall.User, true);
                    }
                    else if (mode == "merge")
                    {
                        if (ds.ItemExists(dataid))
                        {
                            //merge
                            var oldObj = Lib.Data.External.DataSets.Util.CleanHsProcessTypeValuesFromObject(ds.GetData(dataid));
                            var newObj = Lib.Data.External.DataSets.Util.CleanHsProcessTypeValuesFromObject(data);

                            newObj["DbCreated"] = oldObj["DbCreated"];
                            newObj["DbCreatedBy"] = oldObj["DbCreatedBy"];

                            var diffs = Lib.Data.External.DataSets.Util.CompareObjects(oldObj, newObj);
                            if (diffs.Count > 0)
                            {
                                oldObj.Merge(newObj,
                                new Newtonsoft.Json.Linq.JsonMergeSettings()
                                {
                                    MergeArrayHandling = Newtonsoft.Json.Linq.MergeArrayHandling.Union,
                                    MergeNullValueHandling = Newtonsoft.Json.Linq.MergeNullValueHandling.Ignore
                                }
                                );
                                newId = ds.AddData(oldObj.ToString(), dataid, apiAuth.ApiCall.User, true);
                            }
                        }
                        else
                            newId = ds.AddData(data, dataid, apiAuth.ApiCall.User, true);


                    }
                    else //skip 
                    {
                        if (!ds.ItemExists(dataid))
                            newId = ds.AddData(data, dataid, apiAuth.ApiCall.User, true);
                    }
                    return Json(new { id = newId }, JsonRequestBehavior.AllowGet);
                }
                catch (DataSetException dse)
                {
                    return Json(dse.APIResponse, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    HlidacStatu.Util.Consts.Logger.Error("Dataset API", ex);
                    return Json(ApiResponseStatus.GeneralExceptionError(ex), JsonRequestBehavior.AllowGet);

                }


            }
        }


    }
}


