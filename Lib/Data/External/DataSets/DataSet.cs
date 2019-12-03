using Elasticsearch.Net;
using HlidacStatu.Util.Cache;
using Nest;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace HlidacStatu.Lib.Data.External.DataSets
{

    public partial class DataSet
    {
        public static volatile MemoryCacheManager<DataSet, string> CachedDatasets
                = MemoryCacheManager<DataSet, string>.GetSafeInstance("Datasets",
                    datasetId =>
                    {
                        return new DataSet(datasetId);
                    },
                    TimeSpan.FromMinutes(120));


        public static Newtonsoft.Json.JsonSerializerSettings DefaultDeserializationSettings = new Newtonsoft.Json.JsonSerializerSettings()
        {
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
        };

        public static DataSet RegisterNew(Registration reg, string user)
        {
            if (reg == null)
                throw new DataSetException(reg.datasetId, ApiResponseStatus.DatasetNotFound);

            if (reg.jsonSchema == null)
                throw new DataSetException(reg.datasetId, ApiResponseStatus.DatasetJsonSchemaMissing);



            reg.NormalizeShortName();
            var client = Lib.ES.Manager.GetESClient(reg.datasetId, idxType: ES.Manager.IndexType.DataSource);

            if (reg.searchResultTemplate != null && !string.IsNullOrEmpty(reg.searchResultTemplate?.body))
            {
                var errors = reg.searchResultTemplate.GetTemplateErrors();
                if (errors.Count > 0)
                {
                    var err = ApiResponseStatus.DatasetSearchTemplateError;
                    err.error.errorDetail = errors.Aggregate((f, s) => f + "\n" + s);
                    throw new DataSetException(reg.datasetId, err);
                }
            }

            if (reg.detailTemplate != null && !string.IsNullOrEmpty(reg.detailTemplate?.body))
            {
                var errors = reg.detailTemplate.GetTemplateErrors();
                if (errors.Count > 0)
                {
                    var err = ApiResponseStatus.DatasetDetailTemplateError;
                    err.error.errorDetail = errors.Aggregate((f, s) => f + "\n" + s);
                    throw new DataSetException(reg.datasetId, err);
                }
            }

            var ret = client.IndexExists(client.ConnectionSettings.DefaultIndex);
            if (ret.Exists)
            {
                throw new DataSetException(reg.datasetId, ApiResponseStatus.DatasetRegistered);
            }
            else
            {
                Lib.ES.Manager.CreateIndex(client);
                DataSetDB.Instance.AddData(reg,user);
            }


            return new DataSet(reg.datasetId);
        }


        protected Nest.ElasticClient client = null;
        protected Newtonsoft.Json.Schema.JSchema schema = null;

        protected string datasetId = null;

        protected DataSet(string datasourceName, bool fireException)
        {
            this.datasetId = datasourceName.ToLower();
            this.client = Lib.ES.Manager.GetESClient(datasetId, idxType: ES.Manager.IndexType.DataSource);


            var ret = client.IndexExists(client.ConnectionSettings.DefaultIndex);
            if (ret.Exists == false)
            {
                if (fireException)
                    throw new DataSetException(this.datasetId, ApiResponseStatus.DatasetNotFound);
                else
                    this.client = null;
            }
        }

        public Nest.ElasticClient ESClient { get { return client; } }

        IEnumerable<Nest.CorePropertyBase> _mapping = null;
        protected IEnumerable<Nest.CorePropertyBase> GetElasticMapping()
        {
            if (_mapping == null)
            {
                var getIndexResponse = this.client.GetIndex(this.client.ConnectionSettings.DefaultIndex);
                IIndexState remote = getIndexResponse.Indices[this.client.ConnectionSettings.DefaultIndex];
                var dataMapping = remote.Mappings
                    .Where(m => m.Key.Name == "data")
                    .FirstOrDefault(); //type data
                if (dataMapping.Value.Properties == null)
                    return new Nest.CorePropertyBase[] { };
                _mapping = dataMapping
                    .Value
                    .Properties
                    .Select(m => (Nest.CorePropertyBase)m.Value)
                    ;
            }
            return _mapping;
        }

        public bool HasAdminAccess(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            email = email.ToLower();

            if (HlidacStatu.Lib.Data.External.DataSets.DataSet.Api.SuperUsers.Contains(email))
                return true;

            if (string.IsNullOrEmpty(this.Registration().createdBy))
                return false; //only superadmins have access

            return this.Registration().createdBy.ToLower() == email;
        }

        public virtual DataSearchResult SearchData(string queryString, int page, int pageSize, string sort = null, bool excludeBigProperties = true, bool withHighlighting = false)
        {
            return Search.SearchData(this, queryString, page, pageSize, sort, excludeBigProperties, withHighlighting);
        }
        public virtual DataSearchRawResult SearchDataRaw(string queryString, int page, int pageSize, string sort = null, 
            bool excludeBigProperties = true, bool withHighlighting = false)
        {
            return Search.SearchDataRaw(this, queryString, page, pageSize, sort,excludeBigProperties, withHighlighting);
        }
        public IEnumerable<string> GetMappingList(string specificMapName = null, string attrNameModif = "")
        {
            List<string> properties = new List<string>();
            var mapping = GetElasticMapping();
            properties.AddRange(getMappingType("", null, mapping, specificMapName, attrNameModif));
            return properties;
        }
        public IEnumerable<string> GetTextMappingList()
        {
            List<string> properties = new List<string>();
            var mapping = GetElasticMapping();
            properties.AddRange(getMappingType("", typeof(Nest.TextProperty), mapping));
            return properties;
        }
        protected IEnumerable<string> getMappingType(string prefix, Type mappingType, IEnumerable<Nest.CorePropertyBase> props, string specName = null, string attrNameModif = "")
        {
            List<string> _props = new List<string>();

            foreach (var p in props)
            {
                if (mappingType == null || p.GetType() == mappingType)
                {
                    if (specName == null || p.Name.Name.ToLower() == specName.ToLower())
                        _props.Add(prefix + attrNameModif + p.Name.Name + attrNameModif);
                }

                if (p.GetType() == typeof(Nest.ObjectProperty))
                {
                    Nest.ObjectProperty pObj = (Nest.ObjectProperty)p;
                    if (pObj.Properties != null)
                    {
                        _props.AddRange(getMappingType(prefix + p.Name.Name + ".", mappingType, pObj.Properties.Select(m => (Nest.CorePropertyBase)m.Value), specName, attrNameModif));
                    }
                }
            }

            return _props;
        }

        public bool IsFlatStructure()
        {
            //important from import data from CSV
            var props = GetPropertyNamesFromSchema();
            return props.Any(p => p.Contains(".")) == false; //. is delimiter for inner objects
        }
        public string[] GetPropertyNameFromSchema(string name)
        {
            Dictionary<string, Type> names = new Dictionary<string, Type>();
            var sch = this.Schema;
            getPropertyNameTypeFromSchemaInternal(new JSchema[] { sch }, "", name, ref names);
            return names.Keys.ToArray();
        }
        public string[] GetPropertyNamesFromSchema()
        {
            return GetPropertyNameFromSchema("");
        }

        public static Dictionary<string, Type> DefaultDatasetProperties = new Dictionary<string, Type>()
        {
            { "hidden", typeof(bool) },
            { "DbCreated", typeof(DateTime) },
            { "DbCreatedBy", typeof(string) }
        };

    public Dictionary<string, Type> GetPropertyNamesTypesFromSchema(bool addDefaultDatasetProperties = false)
        {
            var properties = GetPropertyNameTypeFromSchema("");
            if (addDefaultDatasetProperties)
            {
                foreach (var pp in DefaultDatasetProperties)
                    if (!properties.ContainsKey(pp.Key))
                        properties.Add(pp.Key,pp.Value);
            }
            return properties;
        }
        public Dictionary<string, Type> GetPropertyNameTypeFromSchema(string name)
        {
            Dictionary<string, Type> names = new Dictionary<string, Type>();
            var sch = this.Schema;
            getPropertyNameTypeFromSchemaInternal(new JSchema[] { sch }, "", name, ref names);
            return names;
        }
        private void getPropertyNameTypeFromSchemaInternal(IEnumerable<JSchema> subschema, string prefix, string name, ref Dictionary<string, Type> names)
        {
            foreach (var ss in subschema)
            {
                foreach (var prop in ss.Properties)
                {
                    if (string.IsNullOrEmpty(name)
                        || (!string.IsNullOrEmpty(name) && prop.Key == name)
                        )
                    {
                        names.Add(prefix + prop.Key, JSchemaType2Type(prop.Value));
                    }
                    if (prop.Value.Items.Count > 0)
                        getPropertyNameTypeFromSchemaInternal(prop.Value.Items, prefix + prop.Key + ".", name, ref names);
                    else if (prop.Value.Properties?.Count > 0)
                    {
                        getPropertyNameTypeFromSchemaInternal(
                            new JSchema[] { prop.Value }
                            , prefix + prop.Key + ".", name, ref names);
                    }
                }
            }

        }

        private Type JSchemaType2Type(JSchema schema)
        {
            if (schema?.Type == null)
                return null;

            JSchemaType s = schema.Type.Value;

            if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.Null))
            {
                //nullable types
                if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.String))
                {
                    if (schema.Format == "date-time")
                        return typeof(Nullable<DateTime>);
                    else if (schema.Format == "date")
                        return typeof(Nullable<HlidacStatu.Util.Date>);
                    else
                        return typeof(string);
                }
                else if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.Number))
                    return typeof(Nullable<decimal>);
                else if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.Integer))
                    return typeof(Nullable<long>);
                else if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.Boolean))
                    return typeof(Nullable<bool>);
                else if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.Object))
                    return typeof(object);
                else if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.Array))
                    return typeof(object[]);
                else if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.None))
                    return null;
                else
                    return null;
            }
            else
            {
                if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.String))
                {
                    if (schema.Format == "date" || schema.Format == "date-time")
                        return typeof(DateTime);
                    else if (schema.Format == "date")
                        return typeof(HlidacStatu.Util.Date);
                    else
                        return typeof(string);
                }
                else if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.Number))
                    return typeof(decimal);
                else if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.Integer))
                    return typeof(long);
                else if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.Boolean))
                    return typeof(bool);
                else if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.Object))
                    return typeof(object);
                else if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.Array))
                    return typeof(object[]);
                else if (HlidacStatu.Lib.Helper.IsSet(s, JSchemaType.None))
                    return null;
                else
                    return null;
            }

        }

        public void SendErrorMsgToAuthor(string url, string errMsg)
        {
            if (Devmasters.Core.TextUtil.IsValidEmail(this.Registration().createdBy ?? ""))
            {
                try
                {
                    using (MailMessage msg = new MailMessage("podpora@hlidacstatu.cz", this.Registration().createdBy))
                    {
                        msg.Bcc.Add("michal@michalblaha.cz");
                        msg.Subject = "Chyba v template vasi databáze " + this.Registration().name;
                        msg.IsBodyHtml = false;
                        msg.Body = $"Upozornění!V template vaší databáze {this.Registration().datasetId} na adrese {url} došlo k chybě:\n\n{errMsg}\n\nProsíme opravte ji co nejdříve.\nDíky\n\nTeam Hlídače státu.";
                        msg.BodyEncoding = System.Text.Encoding.UTF8;
                        msg.SubjectEncoding = System.Text.Encoding.UTF8;
                        using (SmtpClient smtp = new SmtpClient())
                        {
                            HlidacStatu.Util.Consts.Logger.Info("Sending email to " + msg.To);
                            smtp.Send(msg);
                        }
                    }

                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Error("Send email", e);
#if DEBUG
                    throw e;
#endif
                }

            }
        }

        private Registration _registration = null;
        public Registration Registration()
        {
            if (_registration == null)
                _registration = DataSetDB.Instance.GetRegistration(datasetId);

            return _registration;
        }

        public string DatasetUrl(bool local = true)
        {
            var url = $"/data/Index/{this.DatasetId}";
            if (local)
                return url;
            else
                return "https://www.hlidacstatu.cz" + url;
        }
        public string DatasetSearchUrl(string query, bool local = true)
        {
            var url = $"/data/Hledat/{this.DatasetId}?q={System.Net.WebUtility.UrlEncode(query)}";
            if (local)
                return url;
            else
                return "https://www.hlidacstatu.cz" + url;
        }

        public string DatasetItemUrl(string dataId, bool local = true)
        {
            if (string.IsNullOrEmpty(dataId))
                return string.Empty;

            var url = $"/data/Detail/{this.DatasetId}/{dataId}";
            if (local)
                return url;
            else
                return "https://www.hlidacstatu.cz" + url;
        }

        protected DataSet(string datasourceName) : this(datasourceName, true)
        {
        }

        public string DatasetId
        {
            get
            {
                return datasetId;
            }
        }


        protected Newtonsoft.Json.Schema.JSchema Schema
        {
            get
            {
                if (this.schema == null)
                {
                    schema = DataSetDB.Instance.GetRegistration(this.DatasetId)
                        ?.GetSchema();
                }

                return schema;
            }
        }
        public virtual string AddData(object data, string id, string createdBy, bool validateSchema = true)
        {
            return AddData(Newtonsoft.Json.JsonConvert.SerializeObject(data), id, createdBy, validateSchema);
        }

        public virtual string AddData(string data, string id, string createdBy, bool validateSchema = true, bool skipOCR = false)
        {
            Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(data);
            dynamic objDyn = Newtonsoft.Json.Linq.JObject.Parse(data);
            if (validateSchema)
            {
                Newtonsoft.Json.Schema.JSchema schema = DataSetDB.Instance.GetRegistration(this.datasetId).GetSchema();

                if (schema != null)
                {
                    IList<string> errors;
                    if (!obj.IsValid(schema, out errors))
                    {
                        if (errors == null || errors?.Count == 0)
                            errors = new string[] { "", "" };

                        throw DataSetException.GetExc(this.datasetId,
                            ApiResponseStatus.DatasetItemInvalidFormat.error.number,
                            ApiResponseStatus.DatasetItemInvalidFormat.error.description,
                            errors.Aggregate((f, s) => f + ";" + s)
                            );
                    }
                }

            }
            if (string.IsNullOrEmpty(id))
                throw new DataSetException(this.datasetId, ApiResponseStatus.DatasetItemNoSetID);

            if (objDyn.Id == null
                &&
                objDyn.id == null)
                throw new DataSetException(this.datasetId, ApiResponseStatus.DatasetItemNoSetID);
            else
                id = objDyn.Id == null ? (string)objDyn.id : (string)objDyn.Id;

            objDyn.DbCreated = DateTime.UtcNow;
            objDyn.DbCreatedBy = createdBy;

            

            //check special HsProcessType
            var jobj = (Newtonsoft.Json.Linq.JObject)objDyn;
            var jpaths = jobj
                .SelectTokens("$..HsProcessType")
                .ToArray();
            var jpathObjs = jpaths.Select(j => j.Parent.Parent).ToArray();
            if (this.DatasetId == DataSetDB.DataSourcesDbName) //don't analyze for registration of new dataset
                jpathObjs = new JContainer[] { };

            foreach (var jo in jpathObjs)
            {
                if (jo["HsProcessType"].Value<string>() == "person")
                {
                    var jmenoAttrName = jo.Children()
                        .Select(c => c as JProperty)
                        .Where(c => c != null)
                        .Where(c => c.Name.ToLower() == "jmeno"
                            || c.Name.ToLower() == "name")
                        .FirstOrDefault()?.Name;
                    var prijmeniAttrName = jo.Children()
                        .Select(c => c as JProperty)
                        .Where(c => c != null)
                        .Where(c => c.Name.ToLower() == "prijmeni"
                            || c.Name.ToLower() == "surname")
                        .FirstOrDefault()?.Name;
                    var narozeniAttrName = jo.Children()
                        .Select(c => c as JProperty)
                        .Where(c => c != null)
                        .Where(c => c.Name.ToLower() == "narozeni"
                            || c.Name.ToLower() == "birthdate")
                        .FirstOrDefault()?.Name;
                    var osobaIdAttrName = jo.Children()
                        .Select(c => c as JProperty)
                        .Where(c => c != null)
                        .Where(c => c.Name.ToLower() == "osobaid")
                        .FirstOrDefault()?.Name ?? "OsobaId";

                    var celejmenoAttrName = jo.Children()
                        .Select(c => c as JProperty)
                        .Where(c => c != null)
                        .Where(c => c.Name.ToLower() == "celejmeno"
                            || c.Name.ToLower() == "fullname")
                        .FirstOrDefault()?.Name;


                    #region FindOsobaId
                    if (jmenoAttrName != null && prijmeniAttrName != null && narozeniAttrName != null)
                    {
                        if (string.IsNullOrEmpty(jo["OsobaId"]?.Value<string>())
                            && jo[narozeniAttrName] != null && jo[narozeniAttrName].Value<DateTime?>().HasValue
                            ) //pokud OsobaId je vyplnena, nehledej jinou
                        {
                            string osobaId = null;
                            var osobaInDb = Osoba.GetByName(
                                jo[jmenoAttrName].Value<string>(),
                                jo[prijmeniAttrName].Value<string>(),
                                jo[narozeniAttrName].Value<DateTime>()
                                );
                            if (osobaInDb == null)
                                osobaInDb = Osoba.GetByNameAscii(
                                    jo[jmenoAttrName].Value<string>(),
                                    jo[prijmeniAttrName].Value<string>(),
                                    jo[narozeniAttrName].Value<DateTime>()
                                    );

                            if (osobaInDb != null && string.IsNullOrEmpty(osobaInDb.NameId))
                            {
                                osobaInDb.NameId = osobaInDb.GetUniqueNamedId();
                                osobaInDb.Save();
                            }
                            osobaId = osobaInDb?.NameId;
                            jo["OsobaId"] = osobaId;
                        }
                    }
                    else if (celejmenoAttrName != null && narozeniAttrName != null)
                    {
                        if (string.IsNullOrEmpty(jo["OsobaId"]?.Value<string>())
                            && jo[narozeniAttrName].Value<DateTime?>().HasValue
                            ) //pokud OsobaId je vyplnena, nehledej jinou
                        {
                            string osobaId = null;
                            Lib.Data.Osoba osobaZeJmena = Lib.Validators.OsobaInText(jo[celejmenoAttrName].Value<string>());
                            if (osobaZeJmena != null)
                            {
                                var osobaInDb = Osoba.GetByName(
                                    osobaZeJmena.Jmeno,
                                    osobaZeJmena.Prijmeni,
                                    jo[narozeniAttrName].Value<DateTime>()
                                    );

                                if (osobaInDb == null)
                                    osobaInDb = Osoba.GetByNameAscii(
                                           osobaZeJmena.Jmeno,
                                           osobaZeJmena.Prijmeni,
                                           jo[narozeniAttrName].Value<DateTime>()
                                               );

                                if (osobaInDb != null && string.IsNullOrEmpty(osobaInDb.NameId))
                                {
                                    osobaInDb.NameId = osobaInDb.GetUniqueNamedId();
                                    osobaInDb.Save();
                                }
                                osobaId = osobaInDb?.NameId;
                            }
                            jo["OsobaId"] = osobaId;

                        }

                    }

                    #endregion
                }
            }


            string updatedData = Newtonsoft.Json.JsonConvert.SerializeObject(objDyn);
            PostData pd = PostData.String(updatedData);

            var tres = client.LowLevel.Index<StringResponse>(client.ConnectionSettings.DefaultIndex, "data", id, pd);

            if (tres.Success)
            {
                Newtonsoft.Json.Linq.JObject jobject = Newtonsoft.Json.Linq.JObject.Parse(tres.Body);

                string finalId = jobject.Value<string>("_id");

                //do DocumentMining after successfull save
                //record must exists before document mining
                bool needsOCR = false;
                if (skipOCR == false)
                {
                    foreach (var jo in jpathObjs)
                    {
                        if (jo["HsProcessType"].Value<string>() == "document")
                        {
                            if (jo["DocumentUrl"] != null && string.IsNullOrEmpty(jo["DocumentPlainText"].Value<string>()))
                            {
                                if (Uri.TryCreate(jo["DocumentUrl"].Value<string>(), UriKind.Absolute, out var uri2Ocr))
                                {
                                    //get text from document
                                    //var url = Devmasters.Core.Util.Config.GetConfigValue("ESConnection");
                                    //url = url + $"/{client.ConnectionSettings.DefaultIndex}/data/{finalId}/_update";
                                    //string callback = HlidacStatu.Lib.OCR.Api.CallbackData.PrepareElasticCallbackDataForOCRReq($"{jo.Path}.DocumentPlainText", false);
                                    //var ocrCallBack = new HlidacStatu.Lib.OCR.Api.CallbackData(new Uri(url), callback, HlidacStatu.Lib.OCR.Api.CallbackData.CallbackType.LocalElastic);
                                    //HlidacStatu.Lib.OCR.Api.Client.TextFromUrl(
                                    //    Devmasters.Core.Util.Config.GetConfigValue("OCRServerApiKey"),
                                    //    uri2Ocr, "Dataset+" + createdBy,
                                    //    HlidacStatu.Lib.OCR.Api.Client.TaskPriority.Standard, HlidacStatu.Lib.OCR.Api.Client.MiningIntensity.Maximum
                                    //    ); //TODOcallBackData: ocrCallBack);

                                    needsOCR = true;

                                }
                            }
                        }
                    }
                }
                if (needsOCR)
                    Lib.Data.ItemToOcrQueue.AddNewTask(ItemToOcrQueue.ItemToOcrType.Dataset, finalId, this.datasetId, OCR.Api.Client.TaskPriority.Standard);

                return finalId;
            }
            else
            {
                var status = ApiResponseStatus.DatasetItemSaveError;
                if (tres.TryGetServerError(out var servererr))
                {
                    status.error.errorDetail = servererr.Error.ToString();
                }
                throw new DataSetException(this.datasetId, status);
            }

            //return res.ToString();
            //ElasticsearchResponse<string> result = this.client.Raw.Index(document.Index, document.Type, document.Id, documentJson);

        }
        public bool ItemExists(string Id)
        {
            //GetRequest req = new GetRequest(client.ConnectionSettings.DefaultIndex, "data", Id);
            var res = this.client.LowLevel.Exists<ExistsResponse>(client.ConnectionSettings.DefaultIndex, "data", Id);
            return res.Exists;
        }
        public dynamic GetDataObj(string Id)
        {

            var data = GetData(Id);
            if (string.IsNullOrEmpty(data))
                return (dynamic)null;
            else
                return Newtonsoft.Json.Linq.JObject.Parse(data);

        }

        public string GetData(string Id)
        {
            GetRequest req = new GetRequest(client.ConnectionSettings.DefaultIndex, "data", Id);
            var res = this.client.Get<object>(req);
            if (res.Found)
                return res.Source.ToString();
            else
                return (string)null;
        }

        public bool DeleteData(string Id)
        {
            //DeleteRequest req = new DeleteRequest(client.ConnectionSettings.DefaultIndex, "data", Id);
            var res = this.client.LowLevel.Delete<StringResponse>(client.ConnectionSettings.DefaultIndex, "data", Id);
            return res.Success;
        }

        public static bool ExistsDataset(string datasetId)
        {
            DataSet ds = new DataSet(datasetId, false);
            return ds.client != null;
        }

        public static string NormalizeValueForId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            id = System.Text.RegularExpressions.Regex.Replace(id, "\\s", "_");
            id = id.Replace("/", "-");
            return id;
        }

    }
}
