using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace HlidacStatu.Lib.ES
{
    public class Manager
    {

        public static Devmasters.Core.Logging.Logger ESTraceLogger = new Devmasters.Core.Logging.Logger("HlidacStatu.Lib.ES.Trace");
        public static Devmasters.Core.Logging.Logger ESLogger = new Devmasters.Core.Logging.Logger("HlidacStatu.Lib.ES");
        public static bool ESTraceLoggerExists = log4net.LogManager.Exists("HlidacStatu.Lib.ES.Trace")?.Logger?.IsEnabledFor(log4net.Core.Level.Debug) == true;



        public enum IndexType
        {
            Smlouvy,
            BankovniUcet,
            BankovniPolozka,
            Firmy,
            VerejneZakazky,
            ProfilZadavatele,
            VerejneZakazkyRaw2006,
            VerejneZakazkyRaw,
            VerejneZakazkyNaProfiluRaw,
            Logs,
            //DataSourceDb,
            DataSource,
            Insolvence,
            Dotace
        }

        public static string defaultIndexName = "hlidacsmluv";
        public static string defaultIndexName_Sneplatne = "hlidacsmluvneplatne";
        public static string defaultIndexName_SAll = defaultIndexName + ", " + defaultIndexName_Sneplatne;
        public static string defaultIndexName_BankovniUcet = "bankovniucet";
        public static string defaultIndexName_BankovniPolozka = "bankovnipolozka";
        public static string defaultIndexName_VerejneZakazky = "verejnezakazky";
        public static string defaultIndexName_ProfilZadavatele = "profilzadavatele";
        public static string defaultIndexName_VerejneZakazkyRaw2006 = "verejnezakazkyraw2006";
        public static string defaultIndexName_VerejneZakazkyRaw = "verejnezakazkyraw";
        public static string defaultIndexName_VerejneZakazkyNaProfiluRaw = "verejnezakazkyprofilraw";
        public static string defaultIndexName_VerejneZakazkyNaProfiluConverted = "verejnezakazkyprofilconverted";
        public static string defaultIndexName_Firmy = "firmy";
        public static string defaultIndexName_Logs = "logs";
        //public static string defaultIndexName_DataSourceDb = "hlidacstatu_datasources";
        public static string defaultIndexName_Insolvence = "insolvencnirestrik";
        public static string defaultIndexName_Dotace = "dotace";


        private static object _clientLock = new object();
        private static Dictionary<string, ElasticClient> _clients = new Dictionary<string, ElasticClient>();

        private static object locker = new object();

        static Manager()
        {
            if (!string.IsNullOrEmpty(Devmasters.Core.Util.Config.GetConfigValue("DefaultIndexName")))
                defaultIndexName = Devmasters.Core.Util.Config.GetConfigValue("DefaultIndexName");
            System.Net.ServicePointManager.DefaultConnectionLimit = 1000;
        }

        //public static void InitElasticSearchIndex()
        //{
        //    InitElasticSearchIndex(defaultIndexName);
        //}
        public static void InitElasticSearchIndex(ElasticClient client, IndexType? idxType)
        {
            if (idxType == null)
                return;
            if (idxType.Value == IndexType.DataSource)
                return;
            var ret = client.Indices.Exists(client.ConnectionSettings.DefaultIndex);
            if (ret.Exists == false)
                CreateIndex(client, idxType.Value);

        }
        public static void DeleteIndex()
        {
            //GetESClient().DeleteIndex(defaultIndexName);
        }
        public static ElasticClient GetESClient(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName, timeOut, connectionLimit);
        }
        public static ElasticClient GetESClient_Sneplatne(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_Sneplatne, timeOut, connectionLimit);
        }

        public static ElasticClient GetESClient_BankovniUcty(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_BankovniUcet, timeOut, connectionLimit, IndexType.BankovniUcet);
        }
        public static ElasticClient GetESClient_BankovniPolozky(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_BankovniPolozka, timeOut, connectionLimit, IndexType.BankovniPolozka);
        }
        public static ElasticClient GetESClient_VZ(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_VerejneZakazky, timeOut, connectionLimit, IndexType.VerejneZakazky);
        }
        public static ElasticClient GetESClient_ProfilZadavatele(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_ProfilZadavatele, timeOut, connectionLimit, IndexType.ProfilZadavatele);
        }
        public static ElasticClient GetESClient_VerejneZakazkyRaw2006(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_VerejneZakazkyRaw2006, timeOut, connectionLimit, IndexType.VerejneZakazkyRaw2006);
        }
        public static ElasticClient GetESClient_VerejneZakazkyNaProfiluRaw(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_VerejneZakazkyNaProfiluRaw, timeOut, connectionLimit, IndexType.VerejneZakazkyNaProfiluRaw);
        }
        public static ElasticClient GetESClient_VerejneZakazkyNaProfiluConverted(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_VerejneZakazkyNaProfiluConverted, timeOut, connectionLimit, IndexType.VerejneZakazky);
        }
        public static ElasticClient GetESClient_VerejneZakazkyRaw(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_VerejneZakazkyRaw, timeOut, connectionLimit, IndexType.VerejneZakazkyRaw
                );
        }
        public static ElasticClient GetESClient_Logs(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_Logs, timeOut, connectionLimit, IndexType.Logs
                );
        }

        public static ElasticClient GetESClient_Insolvence(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_Insolvence, timeOut, connectionLimit, IndexType.Insolvence);
        }

        public static ElasticClient GetESClient_Dotace(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_Dotace, timeOut, connectionLimit, IndexType.Dotace);
        }

        public static ElasticClient GetESClient_Firmy(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_Firmy, timeOut, connectionLimit, IndexType.Firmy);
        }

        static string dataSourceIndexNamePrefix = "data_";
        public static ElasticClient GetESClient(string indexName, int timeOut = 60000, int connectionLimit = 80, IndexType? idxType = null)
        {
            lock (_clientLock)
            {
                if (idxType == IndexType.DataSource)
                    indexName = dataSourceIndexNamePrefix + indexName;

                string cnnset = string.Format("{0}|{1}|{2}", indexName, timeOut, connectionLimit);
                ConnectionSettings sett = GetElasticSearchConnectionSettings(indexName, timeOut, connectionLimit);
                if (!_clients.ContainsKey(cnnset))
                {
                    //if (idxType.HasValue == false)
                    //    idxType = GetIndexTypeForDefaultIndexName(indexName);
                    var _client = new ElasticClient(sett);
                    InitElasticSearchIndex(_client, idxType);

                    _clients.Add(cnnset, _client);
                }
                return _clients[cnnset];
            }

        }

        private static IndexType GetIndexTypeForDefaultIndexName(string indexName)
        {
            if (indexName == defaultIndexName
                || indexName == defaultIndexName_Sneplatne
                //|| indexName == defaultIndexName_SAll
                )
                return IndexType.Smlouvy;
            else if (indexName == defaultIndexName_Firmy)
                return IndexType.Firmy;
            else if (indexName == defaultIndexName_BankovniUcet)
                return IndexType.BankovniUcet;
            else if (indexName == defaultIndexName_BankovniPolozka)
                return IndexType.BankovniPolozka;
            else if (indexName == defaultIndexName_VerejneZakazky)
                return IndexType.VerejneZakazky;
            else if (indexName == defaultIndexName_ProfilZadavatele)
                return IndexType.ProfilZadavatele;
            else if (indexName == defaultIndexName_VerejneZakazkyRaw)
                return IndexType.VerejneZakazkyRaw;
            else if (indexName == defaultIndexName_VerejneZakazkyNaProfiluRaw)
                return IndexType.VerejneZakazkyNaProfiluRaw;
            else
                return IndexType.DataSource;

        }
        public static ConnectionSettings GetElasticSearchConnectionSettings(string indexName, int timeOut = 60000, int? connectionLimit = null)
        {

            string esUrl = Devmasters.Core.Util.Config.GetConfigValue("ESConnection");
            var settings = new ConnectionSettings(new Uri(esUrl))
                .DefaultIndex(indexName)
                .DisableAutomaticProxyDetection(false)
                .RequestTimeout(TimeSpan.FromMilliseconds(timeOut))
                .SniffLifeSpan(null)
                .OnRequestCompleted(call =>
                {
                    // log out the request and the request body, if one exists for the type of request
                    if (call.RequestBodyInBytes != null)
                    {
                        ESTraceLogger.Debug($"{call.HttpMethod}\t{call.Uri}\t" +
                            $"{Encoding.UTF8.GetString(call.RequestBodyInBytes)}");
                    }
                    else
                    {
                        ESTraceLogger.Debug($"{call.HttpMethod}\t{call.Uri}\t");
                    }

                })
                ;

            if (System.Diagnostics.Debugger.IsAttached || ESTraceLoggerExists || Devmasters.Core.Util.Config.GetConfigValue("ESDebugDataEnabled") == "true")
                settings = settings.DisableDirectStreaming();

            if (connectionLimit.HasValue)
                settings = settings.ConnectionLimit(connectionLimit.Value);

            //.ConnectionLimit(connectionLimit)
            //.MaximumRetries(2)

            //.SetProxy(new Uri("http://localhost.fiddler:8888"), "", "")


#if DEBUG
            //settings = settings.;
#endif
            return settings;


        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(TSource source,
    Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="props"></param>
        /// <returns>true if changed at least one property</returns>
        public static bool AddMissingPropertyValuesToFirst<T>(ref T first, ref T second, params Expression<Func<T, object>>[] props)
        {
            //http://stackoverflow.com/questions/671968/retrieving-property-name-from-lambda-expression
            //http://stackoverflow.com/questions/9516235/there-is-a-way-to-update-all-properties-of-an-object-changing-only-it-values

            bool changed = false;
            List<string> propNames = new List<string>();
            propNames = props
                .Select(p => (MemberExpression)(p.Body as MemberExpression))
                .Where(p => p != null)
                .Select(m => m.Member.Name)
                .ToList();

            foreach (var propName in propNames)
            {
                PropertyInfo propertyInfo = first.GetType().GetProperty(propName);
                object newPropVal = propertyInfo.GetValue(second);
                object oldPropVal = propertyInfo.GetValue(first);
                bool isNullable = (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null);

                if (isNullable)
                {
                    if (oldPropVal == null && newPropVal != null)
                    {
                        propertyInfo.SetValue(first, newPropVal);
                        changed = true;
                    }
                    else
                        goto compareValues;
                }

            compareValues:
                object defValue = GetDefault(propertyInfo.PropertyType);
                if (oldPropVal == defValue && newPropVal != defValue)
                {
                    propertyInfo.SetValue(first, newPropVal);
                    changed = true;
                }
            }
            return changed;
        }

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }



        //public static void CreateIndex()
        //{
        //    CreateIndex(defaultIndexName);
        //}

        public static void CreateIndex(ElasticClient client)
        {
            IndexSettings set = new IndexSettings();
            set.NumberOfReplicas = 2;
            set.NumberOfShards = 25;
            // Create a Custom Analyzer ...
            var an = new CustomAnalyzer();
            an.Tokenizer = "standard";
            // ... with Filters from the StandardAnalyzer
            var filter = new List<string>();
            filter.Add("lowercase");
            filter.Add("czech_stop");
            //an.Filter.Add("czech_keywords");
            filter.Add("czech_stemmer");
            filter.Add("asciifolding");
            an.Filter = filter;
            // Add the Analyzer with a name
            set.Analysis = new Nest.Analysis()
            {
                Analyzers = new Analyzers(),
                TokenFilters = new TokenFilters(),
            };

            set.Analysis.Analyzers.Add("default", an);
            set.Analysis.TokenFilters.Add("czech_stop", new StopTokenFilter() { StopWords = new string[] { "_czech_" } });
            set.Analysis.TokenFilters.Add("czech_stemmer", new StemmerTokenFilter() { Language = "czech" });
            IndexState idxSt = new IndexState();
            idxSt.Settings = set;

            var res = client.Indices
                .Create(client.ConnectionSettings.DefaultIndex, i => i  //todo: es7 check
                    .InitializeUsing(idxSt)
                    .Map(mm => mm
                    .Properties(ps => ps
                        .Date(psn => psn.Name("DbCreated"))
                        .Keyword(psn => psn.Name("DbCreatedBy"))
                        )
                    )
                    
                );

        }

        public static void CreateIndex(ElasticClient client, IndexType idxTyp)
        {
            IndexSettings set = new IndexSettings();
            set.NumberOfReplicas = 2;
            if (idxTyp == IndexType.DataSource)
                set.NumberOfShards = 4;
            else
                set.NumberOfShards = 8;
            // Create a Custom Analyzer ...
            var an = new CustomAnalyzer();
            an.Tokenizer = "standard";
            // ... with Filters from the StandardAnalyzer
            var filter = new List<string>();
            filter.Add("lowercase");
            filter.Add("czech_stop");
            //an.Filter.Add("czech_keywords");
            filter.Add("czech_stemmer"); //pouzit Hunspell
            filter.Add("asciifolding");
            an.Filter = filter;
            // Add the Analyzer with a name
            set.Analysis = new Nest.Analysis()
            {
                Analyzers = new Analyzers(),
                TokenFilters = new TokenFilters(),
            };

            set.Analysis.Analyzers.Add("default", an);
            set.Analysis.TokenFilters.Add("czech_stop", new StopTokenFilter() { StopWords = new string[] { "_czech_" } });
            set.Analysis.TokenFilters.Add("czech_stemmer", new StemmerTokenFilter() { Language = "czech" }); //Humspell
            IndexState idxSt = new IndexState();
            idxSt.Settings = set;

            CreateIndexResponse res = null;
            switch (idxTyp)
            {
                case IndexType.VerejneZakazky:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i
                           .InitializeUsing(idxSt)
                           .Map<Lib.Data.VZ.VerejnaZakazka>(map => map.AutoMap().DateDetection(false))
                       );
                    break;
                case IndexType.ProfilZadavatele:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i
                           .InitializeUsing(idxSt)
                           .Map<Lib.Data.VZ.ProfilZadavatele>(map => map.AutoMap().DateDetection(false))
                       );
                    break;
                case IndexType.Insolvence:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i //todo: es7 check
                           .InitializeUsing(idxSt)
                           .Map<Lib.Data.Insolvence.Rizeni>(map => map.AutoMap().DateDetection(false))
                       );
                    break;
                case IndexType.Dotace:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i //todo: es7 check
                           .InitializeUsing(idxSt)
                           .Map<Data.Dotace.Dotace>(map => map.AutoMap().DateDetection(false))
                       );
                    break;

                case IndexType.Smlouvy:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i //todo: es7 check
                           .InitializeUsing(idxSt)
                           .Map<Lib.Data.Smlouva>(map => map.AutoMap().DateDetection(false))
                       );
                    break;
                case IndexType.Firmy:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i //todo: es7 check
                           .InitializeUsing(idxSt)
                           .Map<Data.Firma.Search.FirmaInElastic>(map => map.AutoMap(maxRecursion: 1))
                       );
                    break;
                case IndexType.BankovniUcet:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i //todo: es7 check
                           .InitializeUsing(idxSt)
                               .Map<Lib.Data.TransparentniUcty.BankovniUcet>(map => map.AutoMap(maxRecursion: 1))
                       );
                    break;
                case IndexType.BankovniPolozka:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i //todo: es7 check
                           .InitializeUsing(idxSt)
                               .Map<Lib.Data.TransparentniUcty.BankovniPolozka>(map => map.AutoMap(maxRecursion: 1))
                       );
                    break;
                case IndexType.Logs:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i //todo: es7 check
                           .InitializeUsing(idxSt)
                           .Map<Lib.Data.Logs.ProfilZadavateleDownload>(map => map.AutoMap(maxRecursion: 1))
                       );
                    break;
                case IndexType.VerejneZakazkyNaProfiluRaw:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i //todo: es7 check
                            .InitializeUsing(idxSt)
                            .Map<Lib.Data.External.ProfilZadavatelu.ZakazkaRaw>(map => map
                                    .Properties(p => p
                                        .Keyword(k => k.Name(n => n.ZakazkaId))
                                        .Keyword(k => k.Name(n => n.Profil))
                                        .Date(k => k.Name(n => n.LastUpdate))
                                )
                            )
                       );
                    break;
            }

        }



        public static void LogQueryError<T>(Nest.ISearchResponse<T> esReq, string text = "", System.Web.HttpContextBase httpContext = null, Exception ex = null)
            where T : class
        {
            Elasticsearch.Net.ServerError serverErr = esReq.ServerError;
            ESLogger.Error(new Devmasters.Core.Logging.LogMessage()
                    .SetMessage("ES query error: " + text
                        + "\n\nCause:" + serverErr?.Error?.ToString()
                        + "\n\nDetail:" + esReq.DebugInformation
                        + "\n\n\n"
                        )
                    .SetException(ex)
                    .SetCustomKeyValue("URL", httpContext?.Request?.RawUrl)
                    .SetCustomKeyValue("Stack-trace", System.Environment.StackTrace)
                    .SetCustomKeyValue("Referer", httpContext?.Request?.UrlReferrer?.AbsoluteUri)
                    .SetCustomKeyValue("User-agent", httpContext?.Request?.Browser?.Browser)
                    .SetCustomKeyValue("IP", httpContext?.Request?.UserHostAddress + " " + System.Web.HttpContext.Current?.Request?.UserHostName)
                    );

        }

        public static Dictionary<string, ElasticClient> GetConnectionPool()
        {
            return _clients;
        }

    }
}
