using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using Newtonsoft.Json;
using HlidacStatu.Lib.Data.External.DataSets;
using System;
using System.Linq;
using System.Web.Http;
using System.Net.Http;
using System.Collections.Generic;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2/datasety")]
    public class ApiV2DatasetyController : ApiController
    {
        /// <summary>
        /// Načte seznam datasetů
        /// </summary>
        /// <returns>Seznam datastů</returns>
        [AuthorizeAndAudit]
        [HttpGet, Route()]
        public SearchResultDTO<Registration> GetAll()
        {
            var result = DataSetDB.AllDataSets.Get();
            return new SearchResultDTO<Registration>(result.Length, 1, result.Select(m=>m.Registration()));
        }

        /// <summary>
        /// Detail konkrétního datasetu
        /// </summary>
        /// <param name="datasetId">Id datasetu (můžeme ho získat ze seznamu datasetů)</param>
        /// <returns>Detail datasetu</returns>
        [AuthorizeAndAudit]
        [HttpGet, Route("{datasetId}")]
        public Registration Detail(string datasetId)
        {
            if (string.IsNullOrWhiteSpace(datasetId))
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota id chybí."));
            }

            var ds = DataSet.CachedDatasets.Get(datasetId);
            if (ds == null)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Dataset {datasetId} nenalezen."));
            }

            return ds.Registration();
        }

        /// <summary>
        /// Vyhledávání v položkách datasetu
        /// </summary>
        /// <param name="datasetId">Id datasetu (můžeme ho získat ze seznamu datasetů)</param>
        /// <param name="dotaz">Hledaný výraz</param>
        /// <param name="strana">Stránkování</param>
        /// <param name="sort">Název pole pro řazení</param>
        /// <param name="desc">Řazení: 0 - Vzestupně; 1 - Sestupně</param>
        /// <returns></returns>
        [AuthorizeAndAudit]
        [HttpGet, Route("{datasetId}/hledat")]
        public SearchResultDTO<object> DatasetSearch(string datasetId, [FromUri]string dotaz = null, [FromUri]int? strana = null, [FromUri]string sort = null, [FromUri]string desc = "0")
        {
            if (strana is null || strana < 1)
                strana = 1;
            if (strana > 200)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota page nemůže být větší než 200"));
            }

            try
            {
                var ds = DataSet.CachedDatasets.Get(datasetId?.ToLower());
                if (ds == null)
                {
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Dataset [{datasetId}] nenalezen."));
                }

                bool bDesc = (desc == "1" || desc?.ToLower() == "true");
                var res = ds.SearchData(dotaz, strana.Value, 50, sort + (bDesc ? " desc" : ""));
                res.Result = res.Result.Select(m => { m.DbCreatedBy = null; return m; });

                return new SearchResultDTO<object>(res.Total, res.Page, res.Result);
            }
            catch (DataSetException dex)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"{dex.APIResponse.error.description}"));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }
        }

        /// <summary>
        /// Vytvoří nový dataset
        /// </summary>
        /// <param name="data">Objekt typu Registration - asi example by to chtělo</param>
        /// <returns> vrací id vytvořeného datasetu </returns>
        [AuthorizeAndAudit]
        [HttpPost, Route()]
        public DSCreatedDTO Create([FromBody] Registration data)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(Environment.NewLine, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.Exception.Message));
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, message));
            }

            try
            {
                var reg = data;
                //var reg = JsonConvert.DeserializeObject<Registration>(data, DataSet.DefaultDeserializationSettings);
                var res = DataSet.Api.Create(reg, this.User.Identity.Name);

                if (res.valid)
                {
                    var regval = (DataSet)res.value;
                    return new DSCreatedDTO(regval.DatasetId);  
                }
                else
                {
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"{res.error.description}"));
                }
            }
            catch (JsonSerializationException jex)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"{jex.Message}"));
            }
            catch (DataSetException dse)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"{dse.APIResponse.error.description}"));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }
        }

        /// <summary>
        /// Smazání datasetu
        /// </summary>
        /// <param name="datasetId">Id datasetu (můžeme ho získat ze seznamu datasetů)</param>
        /// <returns>True/False</returns>
        [AuthorizeAndAudit]
        [HttpDelete, Route("{datasetId}")]
        public bool Delete(string datasetId)
        {
            try
            {
                if (string.IsNullOrEmpty(datasetId))
                {
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota id chybí."));
                }

                datasetId = datasetId.ToLower();
                var r = DataSetDB.Instance.GetRegistration(datasetId);
                if (r == null)
                {
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Dataset nenalezen."));
                }

                if (r.createdBy != null && this.User.Identity.Name.ToLower() != r.createdBy?.ToLower())
                {
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.Forbidden, $"Nejste oprávněn mazat tento dataset."));
                }

                var res = DataSetDB.Instance.DeleteRegistration(datasetId, this.User.Identity.Name);
                return res;

            }
            catch (DataSetException dse)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"{dse.APIResponse.error.description}"));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }
        }

        /// <summary>
        /// Update datasetu.
        /// Není možné změnit hodnoty jsonSchema a datasetId. Pokud je potřebuješ změnit, musíš datovou sadu smazat a zaregistrovat znovu.
        /// </summary>
        /// <param name="data">Objekt typu Registration - asi example by to chtělo</param>
        /// <returns></returns>
        [AuthorizeAndAudit]
        [HttpPut, Route()]
        public Registration Update([FromBody] Registration data)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(Environment.NewLine, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.Exception.Message));
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, message));
            }

            try
            {
                var newReg = data;
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
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }

        }

        #region items
        /// <summary>
        /// Načtení položky z datasetu
        /// </summary>
        /// <param name="datasetId">Id datasetu (můžeme ho získat ze seznamu datasetů)</param>
        /// <param name="itemId">Id položky v datasetu, kterou chceme načíst</param>
        /// <returns>Vrací detail položky</returns>
        [AuthorizeAndAudit]
        [HttpGet, Route("{datasetId}/zaznamy/{itemId}")]
        public object DatasetItem_Get(string datasetId, string itemId)
        {

            try
            {
                var ds = DataSet.CachedDatasets.Get(datasetId.ToLower());
                var value = ds.GetDataObj(itemId);
                //remove from item
                if (value == null)
                {
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Zaznam nenalezen."));
                }
                else
                {
                    value.DbCreatedBy = null;
                    return value;
                    
                }
            }
            catch (DataSetException)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Dataset nenalezen."));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }
        }

        [AuthorizeAndAudit]
        [HttpPost, Route("{datasetId}/zaznamy/{itemId}")]
        public DSItemResponseDTO DatasetItem_Update(string datasetId, string itemId, 
            [FromBody]object data,
            string mode = "") 
        {

            if (!ModelState.IsValid)
            {
                var message = string.Join(Environment.NewLine, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.Exception.Message));
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, message));
            }

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
                    newId = ds.AddData(data.ToString(), itemId, this.User.Identity.Name, true);
                }
                else if (mode == "merge")
                {
                    if (ds.ItemExists(itemId))
                    {
                        var oldObj = Lib.Data.External.DataSets.Util.CleanHsProcessTypeValuesFromObject(ds.GetData(itemId));
                        var newObj = Lib.Data.External.DataSets.Util.CleanHsProcessTypeValuesFromObject(data.ToString());

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
                        newId = ds.AddData(data.ToString(), itemId, this.User.Identity.Name, true);

                }
                else //skip 
                {
                    if (!ds.ItemExists(itemId))
                        newId = ds.AddData(data.ToString(), itemId, this.User.Identity.Name, true);
                }
                return new DSItemResponseDTO() { id = newId };
            }
            catch (DataSetException dse)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"{dse.APIResponse.error.description}"));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }
        }

        [AuthorizeAndAudit]
        [HttpPost, Route("{datasetId}/zaznamy/")]
        public List<DSItemResponseDTO> DatasetItem_BulkInsert(string datasetId, [FromBody]object data)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(Environment.NewLine, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.Exception.Message));
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, message));
            }

            var ds = DataSet.CachedDatasets.Get(datasetId);
            string json = data.ToString();
            List<string> results = new List<string>();
            try
            {
                results = ds.AddDataBulk(json, "usr");
            }
            catch (DataSetException dse)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"{dse.APIResponse.error.description}"));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }
            return results.Select(i => new DSItemResponseDTO() { id = i }).ToList();
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
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Dataset {datasetId} nenalezen."));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }
        }

        #endregion

    }

}
