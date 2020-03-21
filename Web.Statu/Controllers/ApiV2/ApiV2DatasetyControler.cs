using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using Swashbuckle.Swagger.Annotations;
using Newtonsoft.Json;
using HlidacStatu.Web.Models.Apiv2;
using HlidacStatu.Lib.Data.External.DataSets;
using System;
using System.Web;
using System.IO;
using HlidacStatu.Web.Framework;
using System.Linq;
using System.Web.Http;
using System.Net.Http;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2/datasety")]
    public class ApiV2DatasetyController : ApiController
    {
        // /api/v2/datasety/
        [AuthorizeAndAudit]
        [HttpGet, Route()]
        public SearchResultDTO<Registration> GetAll()
        {
            var result = DataSetDB.AllDataSets.Get();
            return new SearchResultDTO<Registration>(result.Length, 1, result.Select(m=>m.Registration()));
        }

        // /api/v2/datasety/{id}
        [AuthorizeAndAudit]
        [HttpGet, Route("{datasetId}")]
        public Registration Detail(string datasetId)
        {
            if (string.IsNullOrWhiteSpace(datasetId))
            {
                //Response.StatusCode = 400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota id chybí."));
            }

            var ds = DataSet.CachedDatasets.Get(datasetId);
            if (ds == null)
            {
                //Response.StatusCode = 404;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Dataset {datasetId} nenalezen."));
            }

            return ds.Registration();

        }

        [AuthorizeAndAudit]
        [HttpGet, Route("{datasetId}/hledat")]
        public SearchResultDTO<object> DatasetSearch(string datasetId, string query, int? strana, string sort = null, string desc = "0")
        {
            if (strana is null || strana < 1)
                strana = 1;
            if (strana > 200)
            {
                //Response.StatusCode = 400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota page nemůže být větší než 200"));
            }

            try
            {
                var ds = DataSet.CachedDatasets.Get(datasetId?.ToLower());
                if (ds == null)
                {
                    //Response.StatusCode = 400;
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Dataset [{datasetId}] nenalezen."));
                }

                bool bDesc = (desc == "1" || desc?.ToLower() == "true");
                var res = ds.SearchData(query, strana.Value, 50, sort + (bDesc ? " desc" : ""));
                res.Result = res.Result.Select(m => { m.DbCreatedBy = null; return m; });

                return new SearchResultDTO<object>(res.Total, res.Page, res.Result);

            }
            catch (DataSetException dex)
            {
                //Response.StatusCode = 400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"{dex.APIResponse.error.description}"));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                //Response.StatusCode = 400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }
        }

        [AuthorizeAndAudit]
        [HttpPost, Route()]
        public Registration Create([FromBody] string data)
        {
            //var data = ApiHelpers.ReadRequestBody(this.Request);

            try
            {
                var reg = JsonConvert.DeserializeObject<Registration>(data, DataSet.DefaultDeserializationSettings);
                var res = DataSet.Api.Create(reg, this.User.Identity.Name);

                if (res.valid)
                {
                    //Response.StatusCode = 201;
                    return ((Registration)res.value);
                }
                else
                {
                    //Response.StatusCode = 400;
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"{res.error.description}"));
                }
            }
            catch (JsonSerializationException jex)
            {
                //Response.StatusCode = 400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"{jex.Message}"));
            }
            catch (DataSetException dse)
            {
                //Response.StatusCode = 400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"{dse.APIResponse.error.description}"));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                //Response.StatusCode = 400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }
        }

        [AuthorizeAndAudit]
        [HttpDelete, Route("{datasetId}")]
        public bool Delete(string datasetId)
        {
            try
            {
                if (string.IsNullOrEmpty(datasetId))
                {
                    //Response.StatusCode = 400;
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota id chybí."));
                }

                datasetId = datasetId.ToLower();
                var r = DataSetDB.Instance.GetRegistration(datasetId);
                if (r == null)
                {
                    //Response.StatusCode = 404;
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Dataset nenalezen."));
                }

                if (r.createdBy != null && this.User.Identity.Name.ToLower() != r.createdBy?.ToLower())
                {
                    //Response.StatusCode = 403;
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.Forbidden, $"Nejste oprávněn mazat tento dataset."));
                }

                var res = DataSetDB.Instance.DeleteRegistration(datasetId, this.User.Identity.Name);
                return res;

            }
            catch (DataSetException dse)
            {
                //Response.StatusCode = 400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"{dse.APIResponse.error.description}"));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                //Response.StatusCode = 400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }
        }

        [AuthorizeAndAudit]
        [HttpPut, Route()]
        public Registration Update([FromBody] string data)
        {
            //var data = ApiHelpers.ReadRequestBody(this.Request);

            try
            {
                var newReg = JsonConvert.DeserializeObject<Registration>(data, DataSet.DefaultDeserializationSettings);
                var res = DataSet.Api.Update(newReg, this.User.Identity.Name);
                if (res.valid)
                {
                    return (Registration)res.value;
                }
                throw new HttpResponseException(new ErrorMessage(res));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                //Response.StatusCode = 400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }

        }

        #region items

        [AuthorizeAndAudit]
        [HttpGet, Route("{datasetId}/zaznamy/{itemId}")]
        public HttpResponseMessage DatasetItem_Get(string datasetId, string itemId, string nice)
        {

            try
            {
                var ds = DataSet.CachedDatasets.Get(datasetId.ToLower());
                var value = ds.GetDataObj(itemId);
                //remove from item
                if (value == null)
                {
                    //Response.StatusCode = 404;
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Zaznam nenalezen."));
                }
                else
                {
                    value.DbCreatedBy = null;
                    string s = Newtonsoft.Json.JsonConvert.SerializeObject(value, (nice == "1" ? Formatting.Indented : Formatting.None));
                    
                    return Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, s);
                }
            }
            catch (DataSetException)
            {
                //Response.StatusCode = 404;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Dataset nenalezen."));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                //Response.StatusCode = 400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }
        }

        [AuthorizeAndAudit]
        [HttpPost, Route("{datasetId}/zaznamy/{itemId}")]
        public CreatedDatasetItemResponseDTO DatasetItem_Update(string datasetId, string itemId, 
            [FromBody]string data,
            string mode = "") 
        {

            mode = mode.ToLower();
            if (string.IsNullOrEmpty(mode))
            {
                mode = "skip";
            }

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
                return new CreatedDatasetItemResponseDTO() { id = newId };
            }
            catch (DataSetException dse)
            {
                //Response.StatusCode = 400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"{dse.APIResponse.error.description}"));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                //Response.StatusCode = 400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }
        }

        [AuthorizeAndAudit]
        [HttpGet, Route("{datasetId}/zaznamy/{itemId}/existuje")]
        public bool DatasetItem_Exists(string datasetId, string itemId)
        {
            try
            {
                var ds = DataSet.CachedDatasets.Get(datasetId.ToLower());
                var value = ds.ItemExists(itemId);
                //remove from item
                return value;
            }
            catch (DataSetException)
            {
                //Response.StatusCode = 404;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Dataset {datasetId} nenalezen."));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                //Response.StatusCode = 400;
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }
        }

        #endregion

    }

}
