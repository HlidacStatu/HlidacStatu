using HlidacStatu.Lib.Data.External.DataSets;
using System;
using System.Web.Mvc;
using System.Linq;


namespace HlidacStatu.Web.Controllers
{
    public class DataController : Controller
    {
        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
                return View();

            var ds = DataSet.CachedDatasets.Get(id);
            if (ds == null)
                return RedirectToAction("index");

            return View("Dataset", ds);
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



        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            var ds = DataSet.CachedDatasets.Get(id);
            if (ds == null)
                return RedirectToAction("index");

            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;

            if (!
                    (email == "michal@michalblaha.cz"
                    || email == ds.Registration().createdBy
                    )
                )
                return View("EditNoAccess");

            return View(ds.Registration());
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(string id, Registration update, FormCollection form)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            var ds = DataSet.CachedDatasets.Get(id);
            if (ds == null)
                return RedirectToAction("index");

            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;

            if (!
                    (email == "michal@michalblaha.cz"
                    || email == ds.Registration().createdBy
                    )
                )
                return View("EditNoAccess");

            var newReg = Newtonsoft.Json.JsonConvert.DeserializeObject<Registration>(Newtonsoft.Json.JsonConvert.SerializeObject(update, DataSet.DefaultDeserializationSettings), DataSet.DefaultDeserializationSettings);
            //use everything from newReg, instead of jsonSchema, datasetId
            //update object
            newReg.jsonSchema = ds.Registration().jsonSchema;
            newReg.datasetId = ds.Registration().datasetId;
            newReg.created = DateTime.Now;
            newReg.createdBy = ds.Registration().createdBy;

            newReg.searchResultTemplate = ds.Registration().searchResultTemplate;
            newReg.detailTemplate = ds.Registration().detailTemplate;

            if (!string.IsNullOrEmpty(form["searchResultTemplate_body"]?.Trim()))
            {
                if (newReg.searchResultTemplate == null)
                    newReg.searchResultTemplate = new Registration.Template();
                newReg.searchResultTemplate.body = form["searchResultTemplate_body"] ;
            }
            if (!string.IsNullOrEmpty(form["detailTemplate_body"]?.Trim()))
            {
                if (newReg.detailTemplate == null)
                    newReg.detailTemplate = new Registration.Template();
                newReg.detailTemplate.body = form["detailTemplate_body"] ;
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
                        orderList[i, 0] = oo[0];
                        orderList[i, 1] = oo[1];
                    }
                }
            }
            else
                orderList = new string[,] { { Registration.DbCreatedLabel, "DbCreated" } };

            newReg.orderList = orderList;
            DataSetDB.Instance.AddData(newReg);


            return RedirectToAction("Edit", "Data", new { id = ds.DatasetId });
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


        public ActionResult Hledat(string id, DataSearchRawResult model)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("index");

            DataSet datasource = null;
            try
            {
                datasource = DataSet.CachedDatasets.Get(id);
                if (model == null)
                    return RedirectToAction("index", new { id = id });

                model = datasource.SearchDataRaw(model.Q, model.Page, model.PageSize, model.Order);

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

                ViewBag.Id = id;
                return View(new Models.DataDetailModel() { Dataset = ds, Data = dataid });
            }
            catch (DataSetException)
            {
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
