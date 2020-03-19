using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using Swashbuckle.Swagger.Annotations;
using System.Web.Mvc;
using Newtonsoft.Json;
using HlidacStatu.Web.Models.apiv2;
using HlidacStatu.Lib.Data.External.DataSets;
using System;
using System.Web;
using System.IO;
using HlidacStatu.Web.Framework;
using System.Linq;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2/datasety")]
    public class ApiV2DatasetyController : GenericAuthController
    {
        // /api/v2/datasety/
        [AuthorizeAndAudit]
        [HttpGet, Route()]
        public ActionResult GetAll()
        {
            var result = DataSetDB.Instance.SearchData("*", 1, 100);
            return Content(new SearchResultDTO(result.Total, result.Page, result.Result).ToJson(), "application/json");
        }

        // /api/v2/datasety/{id}
        [AuthorizeAndAudit]
        [HttpGet, Route("{datasetId}")]
        public ActionResult Detail(string datasetId)
        {
            if (string.IsNullOrWhiteSpace(datasetId))
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Hodnota id chybí.").ToJson(), "application/json");
            }

            var ds = DataSet.CachedDatasets.Get(datasetId);
            if (ds == null)
            {
                Response.StatusCode = 404;
                return Content(new ErrorMessage($"Dataset {datasetId} nenalezen.").ToJson(), "application/json");
            }

            return Content(JsonConvert.SerializeObject(ds.Registration()), "application/json");

        }

        [AuthorizeAndAudit]
        [HttpGet, Route("{datasetId}/hledat")]
        public ActionResult DatasetSearch(string datasetId, string query, int? strana, string sort = null, string desc = "0")
        {
            if (strana is null || strana < 1)
                strana = 1;
            if (strana > 200)
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Hodnota page nemůže být větší než 200").ToJson(), "application/json");
            }
             
            try
            {
                var ds = DataSet.CachedDatasets.Get(datasetId?.ToLower());
                if (ds == null)
                {
                    Response.StatusCode = 400;
                    return Content(new ErrorMessage($"Dataset [{datasetId}] nenalezen.").ToJson(), "application/json");
                }

                bool bDesc = (desc == "1" || desc?.ToLower() == "true");
                var res = ds.SearchData(query, strana.Value, 50, sort + (bDesc ? " desc" : ""));
                res.Result = res.Result.Select(m => { m.DbCreatedBy = null; return m; });

                return Content(new SearchResultDTO(res.Total, res.Page, res.Result).ToJson(), "application/json");

            }
            catch (DataSetException dex)
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"{dex.APIResponse.error.description}").ToJson(), "application/json");
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Obecná chyba - {ex.Message}").ToJson(), "application/json");
            }
        }

        [AuthorizeAndAudit]
        [HttpPost, Route()]
        public ActionResult Create()
        {
            var data = ApiHelpers.ReadRequestBody(this.Request);

            try
            {
                var reg = JsonConvert.DeserializeObject<Registration>(data, DataSet.DefaultDeserializationSettings);
                var res = DataSet.Api.Create(reg, this.User.Identity.Name);

                if (res.valid)
                {
                    Response.StatusCode = 201;
                    return Content(JsonConvert.SerializeObject(new { datasetId = res.value }), "application/json");
                }
                else
                {
                    Response.StatusCode = 400;
                    return Content(new ErrorMessage($"{res.error.description}").ToJson(), "application/json");
                }
            }
            catch (JsonSerializationException jex)
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"{jex.Message}").ToJson(), "application/json");
            }
            catch (DataSetException dse)
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"{dse.APIResponse.error.description}").ToJson(), "application/json");
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Obecná chyba - {ex.Message}").ToJson(), "application/json");
            }
        }

        [AuthorizeAndAudit]
        [HttpDelete, Route("{datasetId}")]
        public ActionResult Delete(string datasetId)
        {
            try
            {
                if (string.IsNullOrEmpty(datasetId))
                {
                    Response.StatusCode = 400;
                    return Content(new ErrorMessage($"Hodnota id chybí.").ToJson(), "application/json");
                }

                datasetId = datasetId.ToLower();
                var r = DataSetDB.Instance.GetRegistration(datasetId);
                if (r == null)
                {
                    Response.StatusCode = 404;
                    return Content(new ErrorMessage($"Dataset nenalezen.").ToJson(), "application/json");
                }

                if (r.createdBy != null && this.User.Identity.Name.ToLower() != r.createdBy?.ToLower())
                {
                    Response.StatusCode = 403;
                    return Content(new ErrorMessage($"Nejste oprávněn mazat tento dataset.").ToJson(), "application/json");
                }

                var res = DataSetDB.Instance.DeleteRegistration(datasetId, this.User.Identity.Name);
                return Content(JsonConvert.SerializeObject(new { valid = res }),
                    "application/json");

            }
            catch (DataSetException dse)
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"{dse.APIResponse.error.description}").ToJson(), "application/json");
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Obecná chyba - {ex.Message}").ToJson(), "application/json");
            }
        }

        [AuthorizeAndAudit]
        [HttpPut, Route()]
        public ActionResult Update()
        {
            var data = ApiHelpers.ReadRequestBody(this.Request);

            try
            {
                var newReg = JsonConvert.DeserializeObject<Registration>(data, DataSet.DefaultDeserializationSettings);
                return Content(JsonConvert.SerializeObject(DataSet.Api.Update(newReg, this.User.Identity.Name)), "application/json");
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Obecná chyba - {ex.Message}").ToJson(), "application/json");
            }

        }

        #region items

        [AuthorizeAndAudit]
        [HttpGet, Route("{datasetId}/zaznamy/{itemId}")]
        public ActionResult DatasetItem_Get(string datasetId, string itemId)
        {
            
            try
            {
                var ds = DataSet.CachedDatasets.Get(datasetId.ToLower());
                var value = ds.GetDataObj(itemId);
                //remove from item
                if (value == null)
                {
                    Response.StatusCode = 404;
                    return Content(new ErrorMessage($"Zaznam nenalezen.").ToJson(), "application/json");
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
                Response.StatusCode = 404;
                return Content(new ErrorMessage($"Dataset nenalezen.").ToJson(), "application/json");
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Obecná chyba - {ex.Message}").ToJson(), "application/json");
            }
        }

        [AuthorizeAndAudit]
        [HttpPost, Route("{datasetId}/zaznamy/{itemId}")]
        public ActionResult DatasetItem_Update(string datasetId, string itemId, string mode = "", bool? rewrite = false) //rewrite for backwards compatibility
        {
            
            mode = mode.ToLower();
            if (string.IsNullOrEmpty(mode))
            {
                if (rewrite == true)
                    mode = "rewrite";
                else
                    mode = "skip";
            }

            var data = ApiHelpers.ReadRequestBody(this.Request);
            datasetId = datasetId.ToLower();
            try
            {
                var ds = DataSet.CachedDatasets.Get(datasetId);
                var newId = itemId;

                if (mode == "rewrite")
                {
                    newId = ds.AddData(data, itemId, this.User.Identity.Name, true);
                }
                else if (mode == "merge")
                {
                    if (ds.ItemExists(itemId))
                    {
                        //merge
                        var oldObj = Lib.Data.External.DataSets.Util.CleanHsProcessTypeValuesFromObject(ds.GetData(itemId));
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
                                });

                            newId = ds.AddData(oldObj.ToString(), itemId, this.User.Identity.Name, true);
                        }
                    }
                    else
                        newId = ds.AddData(data, itemId, this.User.Identity.Name, true);

                }
                else //skip 
                {
                    if (!ds.ItemExists(itemId))
                        newId = ds.AddData(data, itemId, this.User.Identity.Name, true);
                }
                return Content(JsonConvert.SerializeObject(new { id = newId }), "application/json");
            }
            catch (DataSetException dse)
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"{dse.APIResponse.error.description}").ToJson(), "application/json");
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Obecná chyba - {ex.Message}").ToJson(), "application/json");
            }
        }

        [AuthorizeAndAudit]
        [HttpGet, Route("{datasetId}/zaznamy/{itemId}/existuje")]
        public ActionResult DatasetItem_Exists(string datasetId, string itemId)
        {
            try
            {
                var ds = DataSet.CachedDatasets.Get(datasetId.ToLower());
                var value = ds.ItemExists(itemId);
                //remove from item
                return Content(JsonConvert.SerializeObject(new { itemExists = value }), "application/json");
            }
            catch (DataSetException)
            {
                Response.StatusCode = 404;
                return Content(new ErrorMessage($"Dataset {datasetId} nenalezen.").ToJson(), "application/json");
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Obecná chyba - {ex.Message}").ToJson(), "application/json");
            }
        }

        #endregion

    }

}
