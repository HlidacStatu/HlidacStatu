using HlidacStatu.Lib.Data.External.DataSets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace HlidacStatu.Web.Controllers
{
    public partial class DataController : GenericAuthController
    {

        static Devmasters.Cache.LocalMemory.LocalMemoryCache<Models.DatasetIndexStat[]> datasetIndexStatCache =
            new Devmasters.Cache.LocalMemory.LocalMemoryCache<Models.DatasetIndexStat[]>(TimeSpan.FromMinutes(15),
                (o) =>
                {
                    List<Models.DatasetIndexStat> ret = new List<Models.DatasetIndexStat>();
                    var datasets = DataSetDB.Instance.SearchDataRaw("*", 1, 200)
                        .Result
                        .Select(s => Newtonsoft.Json.JsonConvert.DeserializeObject<Registration>(s.Item2));

                    foreach (var ds in datasets)
                    {
                        var rec = new Models.DatasetIndexStat() { Ds = ds };
                        var dsContent = HlidacStatu.Lib.Data.External.DataSets.DataSet.CachedDatasets.Get(ds.id.ToString());
                        var allrec = dsContent.SearchData("", 1, 1, sort: "DbCreated desc", exactNumOfResults: true);
                        rec.RecordNum = allrec.Total;
                        if (rec.RecordNum > 0)
                        {
                            rec.LastRecord = (DateTime?)allrec.Result.First().DbCreated;
                        }

                        var recordWeek = dsContent.SearchData($"DbCreated:[{DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd")} TO *]", 1, 0, exactNumOfResults: true);
                        rec.RecordNumWeek = recordWeek.Total;
                        //string order = string.IsNullOrWhiteSpace(ds.defaultOrderBy) ? "DbCreated desc" : ds.defaultOrderBy;
                        //var data = dsContent.SearchDataRaw("*", 1, 1, order);

                        ret.Add(rec);
                    }
                    return ret.ToArray();
                }
                );




        public ActionResult Index(string id)
        {
            if (Request.QueryString["refresh"] == "1")
                datasetIndexStatCache.Invalidate();

            if (string.IsNullOrEmpty(id))
                return View(datasetIndexStatCache.Get());

            var ds = DataSet.CachedDatasets.Get(id);
            if (ds == null)
                return RedirectToAction("index", "Data", new { id = "" });

            return View("DatasetHomepage", ds);
        }

        public ActionResult TechnickeInfo(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            var ds = DataSet.CachedDatasets.Get(id);
            if (ds == null)
                return RedirectToAction("index");
            return View(ds);
        }

        public ActionResult Manage(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var ds = DataSet.CachedDatasets.Get(id);
                if (ds == null)
                    return Redirect("/data");

                if (ds.HasAdminAccess(Request?.RequestContext?.HttpContext?.User?.Identity?.Name) == true)
                    return View(ds?.Registration());
            }
            return View();
        }

        public ActionResult Delete(string id, string confirmation)
        {
            string email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;
            var ds = DataSet.CachedDatasets.Get(id);
            if (ds == null)
                return Redirect("/data");

            if (ds.HasAdminAccess(email) == false)
                return View("NoAccess");

            string[] neverDelete = new string[] { "veklep", "rozhodnuti-uohs", "centralniregistroznameni", "datasourcesdb" };
            if (neverDelete.Contains(ds.DatasetId.ToLower()))
                return View("NoAccess");

            if (confirmation == ds.DatasetId)
            {
                datasetIndexStatCache.Invalidate();

                DataSetDB.Instance.DeleteRegistration(ds.DatasetId, email);
                return RedirectToAction("Index");
            }
            return View(ds);
        }

        public ActionResult Backup(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            var ds = DataSet.CachedDatasets.Get(id);
            if (ds == null)
                return RedirectToAction("index");

            if (ds.HasAdminAccess(Request?.RequestContext?.HttpContext?.User?.Identity?.Name) == false)
            {
                ViewBag.DatasetId = id;
                return View("NoAccess");
            }
            return File(
                System.Text.UTF8Encoding.UTF8.GetBytes(
                    Newtonsoft.Json.JsonConvert.SerializeObject(ds.Registration(), Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = Serialization.PublicDatasetContractResolver.Instance })
                    ),
                "application/octet-streamSection", id + ".json");

        }




        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            var ds = DataSet.CachedDatasets.Get(id);
            if (ds == null)
                return RedirectToAction("index");

            if (ds.HasAdminAccess(Request?.RequestContext?.HttpContext?.User?.Identity?.Name) == false)
            {
                ViewBag.DatasetId = id;
                return View("NoAccess");
            }

            return View(ds.Registration());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(string id, Registration update, FormCollection form)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            datasetIndexStatCache.Invalidate();

            var ds = DataSet.CachedDatasets.Get(id);
            if (ds == null)
                return RedirectToAction("index");

            var logged = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;

            if (ds.HasAdminAccess(logged) == false)
            {
                ViewBag.DatasetId = id;
                return View("NoAccess");
            }

            //
            var newReg = Newtonsoft.Json.JsonConvert.DeserializeObject<Registration>(
                Newtonsoft.Json.JsonConvert.SerializeObject(update, DataSet.DefaultDeserializationSettings)
                , DataSet.DefaultDeserializationSettings);
            if (newReg.datasetId != id)
            {
                ViewBag.DatasetId = id;
                return View("NoAccess");
            }

            newReg = WebFormToRegistration(newReg, form);
            if (string.IsNullOrEmpty(newReg.createdBy))
                newReg.createdBy = logged;


            var res = DataSet.Api.Update(newReg, logged);
            if (res.valid)
                return RedirectToAction("Edit", "Data", new { id = ds.DatasetId });
            else
            {
                ViewBag.ApiResponseError = res;
                return View(newReg);
            }
        }

        private Registration WebFormToRegistration(Registration newReg, FormCollection form)
        {

            if (!string.IsNullOrEmpty(form["searchResultTemplate_body"]?.Trim()))
            {
                if (newReg.searchResultTemplate == null)
                    newReg.searchResultTemplate = new Registration.Template();
                newReg.searchResultTemplate.body = form["searchResultTemplate_body"];
            }
            if (!string.IsNullOrEmpty(form["detailTemplate_body"]?.Trim()))
            {
                if (newReg.detailTemplate == null)
                    newReg.detailTemplate = new Registration.Template();
                newReg.detailTemplate.body = form["detailTemplate_body"];
            }
            string[] orderlines = form["sorderList"]
                ?.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                ?.Where(m => !string.IsNullOrEmpty(m.Trim()))
                ?.ToArray() ?? new string[] { };

            string[,] orderList = null;
            if (orderlines.Count() > 0)
            {
                orderList = new string[orderlines.Count(), 2];
                for (int i = 0; i < orderlines.Count(); i++)
                {
                    var oo = orderlines[i].Split('|');
                    if (oo.Length == 2)
                    {
                        orderList[i, 0] = oo[0].Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();
                        orderList[i, 1] = oo[1].Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();
                    }
                }
            }
            else
                orderList = new string[,] { { Registration.DbCreatedLabel, "DbCreated" } };

            newReg.orderList = orderList;

            return newReg;
        }

        [HttpPost]
        public ActionResult DatasetTextJson(string id, FormCollection form)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("index");

            var ds = DataSet.CachedDatasets.Get(id);
            if (ds == null)
                return RedirectToAction("index");

            ViewBag.jsondata = form["jsondata"] ?? "";
            return View("DatasetTextJson", ds);
        }

        public ActionResult Napoveda(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("index");


            DataSet datasource = null;
            datasource = DataSet.CachedDatasets.Get(id);
            if (datasource == null)
                return RedirectToAction("index", new { id = id });

            return View(datasource);
        }
        public ActionResult Hledat(string id, DataSearchRawResult model)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("index");


            DataSet datasource = null;
            try
            {
                datasource = DataSet.CachedDatasets.Get(id);
                if (datasource == null)
                    return RedirectToAction("index", new { id = id });

                model = datasource.SearchDataRaw(model.Q, model.Page, model.PageSize, model.Order);
                Lib.Data.Audit.Add(
                    Lib.Data.Audit.Operations.UserSearch
                    , this.User?.Identity?.Name
                    , this.Request.UserHostAddress
                    , "Dataset." + datasource.DatasetId
                    , model.IsValid ? "valid" : "invalid"
                    , model.Q, model.OrigQuery);

                return View(model);
            }
            catch (DataSetException e)
            {
                if (e.APIResponse.error.number == ApiResponseStatus.InvalidSearchQuery.error.number)
                {
                    model.DataSet = datasource;
                    model.IsValid = false;
                    return View(model);
                }
                return RedirectToAction("index", new { id = id });

            }
            catch (Exception)
            {
                return RedirectToAction("index", new { id = id });
            }

        }

        public ActionResult Detail(string id, string dataid)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("index");

            try
            {
                var ds = DataSet.CachedDatasets.Get(id);
                if (ds == null)
                    return RedirectToAction("index");
                if (string.IsNullOrEmpty(dataid))
                    return RedirectToAction("index", new { id = id });

                var dataItem = ds.GetData(dataid);
                if (dataItem == null)
                    return RedirectToAction("index", new { id = id });

                if (!string.IsNullOrEmpty(this.Request.QueryString["qs"]))
                {
                    try
                    {
                        var findSm = ds.SearchDataRaw($"_id:\"{dataid}\" AND ({this.Request.QueryString["qs"]})", 1, 1,
                            null, withHighlighting: true);
                        if (findSm.Total > 0)
                            ViewBag.Highlighting = findSm.ElasticResultsRaw.Hits.First().Highlight;

                    }
                    catch (Exception e)
                    {

                        Util.Consts.Logger.Error("Dataset Detail Highligting query error ", e);
                    }

                }

                ViewBag.Id = id;
                return View(new Models.DataDetailModel() { Dataset = ds, Data = dataid });
            }
            catch (DataSetException ex)
            {
                Util.Consts.Logger.Error("Dataset Detail", ex);
                return RedirectToAction("index");
            }

        }

        public ActionResult DetailText(string id, string dataid)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("index");

            try
            {
                var ds = DataSet.CachedDatasets.Get(id);
                if (ds == null)
                    return RedirectToAction("index");
                if (string.IsNullOrEmpty(dataid))
                    return RedirectToAction("index", new { id = id });

                var dataItem = ds.GetData(dataid);
                if (dataItem == null)
                    return RedirectToAction("index", new { id = id });

                ViewBag.Id = id;
                return View(new Models.DataDetailModel() { Dataset = ds, Data = dataid });
            }
            catch (DataSetException)
            {
                return RedirectToAction("index");
            }

        }

        static string search_PropertiesTemplateFileName = "_data_hledat_properties";//"~/Views/Data/_data_hledat_properties.cshtml";
        static string detail_PropertiesTemplateFileName = "_data_detail_properties";//"~/Views/Data/_data_detail_properties.cshtml";
        static string noTeplate_PropertiesTemplateFileName = "_data_noTemplate";//"~/Views/Data/_data_noTemplate.cshtml";


        [ChildActionOnly]
        public ActionResult HledatProperties_CustomdataTemplate(DataSearchRawResult model)
        {
            return PartialView(search_PropertiesTemplateFileName, model);

        }


        [ChildActionOnly]
        public ActionResult Detail_CustomdataTemplate(Models.DataDetailModel model)
        {

            var dataId = model.Data;
            var dataset = model.Dataset;
            var itemInS = dataset.GetData(dataId);
            var newModel = new Models.DataDetailModel() { Dataset = dataset, Data = itemInS };
            Registration dsReg = DataSetDB.Instance.GetRegistration(model.Dataset.DatasetId);
            if (dsReg == null)
                return PartialView(noTeplate_PropertiesTemplateFileName, newModel);
            else
            {

                return PartialView(detail_PropertiesTemplateFileName, newModel);
            }
        }

    }
}

/*
 if (virtualPath.Contains(CustomDataSearchTemplatePostfix))
 content = dsReg.searchResultTemplate?.ToPageContent() ?? search_PropertiesTemplateFileName;
else if (virtualPath.Contains(CustomDataDetailTemplatePostfix))
 content = dsReg.searchResultTemplate?.ToPageContent() ?? detail_PropertiesTemplateFileName;
else
 content = "";
}
if (content == search_PropertiesTemplateFileName //use .properties[]
|| content == detail_PropertiesTemplateFileName //use .properties[]
|| content == noTeplate_PropertiesTemplateFileName //use file for noTemplate msg
)
{
return base.GetFile(content); //use template from disk
}
else
{
return new VirtualFileFromString(virtualPath, content);
}
*/
