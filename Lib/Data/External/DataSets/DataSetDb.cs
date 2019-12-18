using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data.External.DataSets
{
    public class DataSetDB : DataSet
    {
        public static string DataSourcesDbName = "datasourcesdb";

        public static DataSetDB Instance = new DataSetDB();


        public static Devmasters.Cache.V20.LocalMemory.AutoUpdatedLocalMemoryCache<DataSet[]> AllDataSets =
            new Devmasters.Cache.V20.LocalMemory.AutoUpdatedLocalMemoryCache<DataSet[]>(
                        TimeSpan.FromMinutes(5), (obj) =>
                        {

                            var datasets = HlidacStatu.Lib.Data.External.DataSets.DataSetDB.Instance.SearchDataRaw("*", 1, 100)
                            .Result
                            .Select(s => DataSet.CachedDatasets.Get(s.Item1))
                            .Where(d => d != null)
                            .ToArray();

                            return datasets;
                        }
                    );

        public static Devmasters.Cache.V20.LocalMemory.AutoUpdatedLocalMemoryCache<DataSet[]> ProductionDataSets =
            new Devmasters.Cache.V20.LocalMemory.AutoUpdatedLocalMemoryCache<DataSet[]>(
                        TimeSpan.FromMinutes(5), (obj) =>
                        {

                            var datasets = HlidacStatu.Lib.Data.External.DataSets.DataSetDB.Instance.SearchDataRaw("*", 1, 100)
                            .Result
                            .Select(s => DataSet.CachedDatasets.Get(s.Item1))
                            .Where(d => d != null)
                            .Where(d => d.Registration().betaversion == false)
                            .ToArray();

                            return datasets;
                        }
                    );

        //    var datasets = HlidacStatu.Lib.Data.External.DataSets.DataSetDB.Instance.SearchDataRaw("*", 1, 100)
        //.Result
        //.Select(s => Newtonsoft.Json.JsonConvert.DeserializeObject<HlidacStatu.Lib.Data.External.DataSets.Registration>(s.Item2));


        private DataSetDB() : base(DataSourcesDbName, false)
        {

            if (client == null)
            {
                this.client = Lib.ES.Manager.GetESClient(DataSourcesDbName, idxType: ES.Manager.IndexType.DataSource);
                var ret = client.Indices.Exists(client.ConnectionSettings.DefaultIndex); //todo: es7 check
                if (!ret.Exists)
                {
                    Newtonsoft.Json.Schema.Generation.JSchemaGenerator jsonG = new Newtonsoft.Json.Schema.Generation.JSchemaGenerator();
                    jsonG.DefaultRequired = Newtonsoft.Json.Required.Default;
                    Registration reg = new Registration()
                    {
                        datasetId = DataSourcesDbName,
                        jsonSchema = jsonG.Generate(typeof(Registration)).ToString()
                    };
                    Lib.ES.Manager.CreateIndex(client);

                    //add record
                    Elasticsearch.Net.PostData pd = Elasticsearch.Net.PostData.String(Newtonsoft.Json.JsonConvert.SerializeObject(reg));

                    var tres = client.LowLevel.Index<Elasticsearch.Net.StringResponse>(client.ConnectionSettings.DefaultIndex, DataSourcesDbName, pd);
                    if (tres.Success == false)
                        throw new ApplicationException(tres.DebugInformation);
                }
            }
        }

        public Registration GetRegistration(string datasetId)
        {
            datasetId = datasetId.ToLower();
            var s = this.GetData(datasetId);
            if (string.IsNullOrEmpty(s))
                return null;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Registration>(s, DefaultDeserializationSettings);
        }


        public virtual string AddData(Registration reg, string user)
        {
            if (reg.jsonSchema == null)
                throw new DataSetException(this.datasetId, ApiResponseStatus.DatasetJsonSchemaMissing);

            Registration oldReg = null;
            oldReg = DataSet.CachedDatasets.Get(reg.datasetId)?.Registration();
            if (oldReg == null)
                Audit.Add<Registration>(Audit.Operations.Create, user, reg, null);
            else
                Audit.Add<Registration>(Audit.Operations.Update, user, reg, oldReg);

            var addDataResult = base.AddData(reg, reg.datasetId, reg.createdBy);
            DataSet.CachedDatasets.Delete(reg.datasetId);

            //check orderList
            if (reg.orderList?.Length > 0)
            {

                //get mapping
                var ds = CachedDatasets.Get(addDataResult);
                var txtProps = ds.GetTextMappingList();
                var allProps = ds.GetMappingList();
                if (allProps.Where(m => !DataSet.DefaultDatasetProperties.Keys.Contains(m)).Count() > 0) //0=mapping not available , (no record in db)
                {

                    bool changedOrderList = false;

                    //find missing and remove it
                    List<int> toRemove = new List<int>();
                    for (int i = 0; i < reg.orderList.GetLength(0); i++)
                    {

                        string oProp = reg.orderList[i, 1]
                            .Replace(DataSearchResult.OrderAsc, "")
                            .Replace(DataSearchResult.OrderDesc, "")
                            .Replace(DataSearchResult.OrderAscUrl, "")
                            .Replace(DataSearchResult.OrderDescUrl, "")
                            .Trim();
                        if (oProp.EndsWith(".keyword"))
                            oProp = System.Text.RegularExpressions.Regex.Replace(oProp, "\\.keyword$", "");
                        if (allProps.Contains(oProp) == false)
                        {
                            //neni na seznamu properties, pridej do seznamu k smazani
                            toRemove.Add(i);
                        }
                    }
                    if (toRemove.Count > 0)
                    {
                        foreach (var i in toRemove.OrderByDescending(n => n))
                        {
                            reg.orderList = HlidacStatu.Util.ArrayTools.TrimArray<string>(i, null, reg.orderList);
                            changedOrderList = true;
                        }
                    }

                    for (int i = 0; i < reg.orderList.GetLength(0); i++)
                    {
                        string oProp = reg.orderList[i, 1]
                            .Replace(DataSearchResult.OrderAsc, "")
                            .Replace(DataSearchResult.OrderAscUrl, "")
                            .Replace(DataSearchResult.OrderDesc, "")
                            .Replace(DataSearchResult.OrderDescUrl, "")
                            .Trim();
                        if (txtProps.Contains(oProp))
                        {
                            //pridej keyword na konec
                            reg.orderList[i, 1] = reg.orderList[i, 1].Replace(oProp, oProp + ".keyword");
                            changedOrderList = true;
                        }
                    }

                    if (changedOrderList)
                        addDataResult = base.AddData(reg, reg.datasetId, reg.createdBy);
                }

            }
            DataSet.CachedDatasets.Set(reg.datasetId, null);

            return addDataResult;
        }


        public bool DeleteRegistration(string datasetId, string user)
        {
            Audit.Add<Registration>(Audit.Operations.Delete, user, this.Registration(), null);

            datasetId = datasetId.ToLower();
            var res = this.DeleteData(datasetId);
            var idxClient = Lib.ES.Manager.GetESClient(datasetId, idxType: ES.Manager.IndexType.DataSource);

            var delRes = idxClient.Indices.Delete(idxClient.ConnectionSettings.DefaultIndex);
            DataSet.CachedDatasets.Delete(datasetId);
            return res && delRes.IsValid;
        }


        public override DataSearchResult SearchData(string queryString, int page, int pageSize, string sort = null,
            bool excludeBigProperties = true, bool withHighlighting = false)
        {
            var resData = base.SearchData(queryString, page, pageSize, sort, excludeBigProperties, withHighlighting);
            if (resData == null || resData?.Result == null)
                return resData;

            resData.Result = resData.Result.Where(r => r.id.ToString() != DataSourcesDbName);

            return resData;

        }
        public override DataSearchRawResult SearchDataRaw(string queryString, int page, int pageSize, string sort = null,
            bool excludeBigProperties = true, bool withHighlighting = false)
        {
            var resData = base.SearchDataRaw($"NOT(id:{DataSourcesDbName}) AND ({queryString})", page, pageSize,
                sort, excludeBigProperties, withHighlighting);
            //var resData = base.SearchDataRaw($"({queryString})", page, pageSize, sort);
            if (resData == null || resData?.Result == null)
                return resData;

            return resData;

        }


    }
}
