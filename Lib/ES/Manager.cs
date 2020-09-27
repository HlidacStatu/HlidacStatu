using Nest;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace HlidacStatu.Lib.ES
{
    public class Manager
    {

        public static Devmasters.Logging.Logger ESTraceLogger = new Devmasters.Logging.Logger("HlidacStatu.Lib.ES.Trace");
        public static Devmasters.Logging.Logger ESLogger = new Devmasters.Logging.Logger("HlidacStatu.Lib.ES");
        public static bool ESTraceLoggerExists = log4net.LogManager.Exists("HlidacStatu.Lib.ES.Trace")?.Logger?.IsEnabledFor(log4net.Core.Level.Debug) == true;



        public enum IndexType
        {
            Smlouvy,
            Firmy,
            KIndex,
            VerejneZakazky,
            ProfilZadavatele,
            VerejneZakazkyRaw2006,
            VerejneZakazkyRaw,
            VerejneZakazkyNaProfiluRaw,
            Logs,
            //DataSourceDb,
            DataSource,
            Insolvence,
            Dotace,
            Osoby,
            Audit,
            RPP_Kategorie,
            RPP_OVM,
            RPP_ISVS
        }

        public static string defaultIndexName = "hlidacsmluv";
        public static string defaultIndexName_Sneplatne = "hlidacsmluvneplatne";
        public static string defaultIndexName_SAll = defaultIndexName + "," + defaultIndexName_Sneplatne;

        public static string defaultIndexName_VerejneZakazky = "verejnezakazky";
        public static string defaultIndexName_ProfilZadavatele = "profilzadavatele";
        public static string defaultIndexName_VerejneZakazkyRaw2006 = "verejnezakazkyraw2006";
        public static string defaultIndexName_VerejneZakazkyRaw = "verejnezakazkyraw";
        public static string defaultIndexName_VerejneZakazkyNaProfiluRaw = "verejnezakazkyprofilraw";
        public static string defaultIndexName_VerejneZakazkyNaProfiluConverted = "verejnezakazkyprofilconverted";
        public static string defaultIndexName_Firmy = "firmy";
        public static string defaultIndexName_KIndex = "kindex";
        public static string defaultIndexName_Logs = "logs";
        //public static string defaultIndexName_DataSourceDb = "hlidacstatu_datasources";
        public static string defaultIndexName_Insolvence = "insolvencnirestrik";
        public static string defaultIndexName_Dotace = "dotace";
        public static string defaultIndexName_Osoby = "osoby";
        public static string defaultIndexName_Audit = "audit";

        public static string defaultIndexName_RPP_Kategorie = "rpp_kategorie";
        public static string defaultIndexName_RPP_OVM = "rpp_ovm";
        public static string defaultIndexName_RPP_ISVS = "rpp_isvs";

        private static object _clientLock = new object();
        private static Dictionary<string, ElasticClient> _clients = new Dictionary<string, ElasticClient>();

        private static object locker = new object();

        static Manager()
        {
            if (!string.IsNullOrEmpty(Devmasters.Config.GetWebConfigValue("DefaultIndexName")))
                defaultIndexName = Devmasters.Config.GetWebConfigValue("DefaultIndexName");
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
        public static ElasticClient GetESClient_Audit(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_Audit, timeOut, connectionLimit, IndexType.Audit
                );
        }
        public static ElasticClient GetESClient_RPP_OVM(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_RPP_OVM, timeOut, connectionLimit, IndexType.RPP_OVM
                );
        }
        public static ElasticClient GetESClient_RPP_ISVS(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_RPP_ISVS, timeOut, connectionLimit, IndexType.RPP_ISVS
                );
        }
        public static ElasticClient GetESClient_RPP_Kategorie(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_RPP_Kategorie, timeOut, connectionLimit, IndexType.RPP_Kategorie
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

        public static ElasticClient GetESClient_Osoby(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_Osoby, timeOut, connectionLimit, IndexType.Osoby);
        }

        public static ElasticClient GetESClient_Firmy(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_Firmy, timeOut, connectionLimit, IndexType.Firmy);
        }
        public static ElasticClient GetESClient_KIndex(int timeOut = 60000, int connectionLimit = 80)
        {
            return GetESClient(defaultIndexName_KIndex, timeOut, connectionLimit, IndexType.Firmy);
        }

        static string dataSourceIndexNamePrefix = "data_";
        public static ElasticClient GetESClient(string indexName, int timeOut = 60000, int connectionLimit = 80, IndexType? idxType = null, bool init = true)
        {
            lock (_clientLock)
            {
                if (idxType == IndexType.DataSource)
                    indexName = dataSourceIndexNamePrefix + indexName;
                else if (indexName == defaultIndexName_Audit)
                {
                    //audit_Year-weekInYear
                    DateTime d = DateTime.Now;
                    indexName = $"{indexName}_{d.Year}-{System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}";
                }
                string cnnset = string.Format("{0}|{1}|{2}", indexName, timeOut, connectionLimit);
                ConnectionSettings sett = GetElasticSearchConnectionSettings(indexName, timeOut, connectionLimit);
                if (!_clients.ContainsKey(cnnset))
                {
                    //if (idxType.HasValue == false)
                    //    idxType = GetIndexTypeForDefaultIndexName(indexName);

                    var _client = new ElasticClient(sett);
                    if (init)
                        InitElasticSearchIndex(_client, idxType);

                    _clients.Add(cnnset, _client);
                }
                return _clients[cnnset];
            }

        }


        public static ConnectionSettings GetElasticSearchConnectionSettings(string indexName, int timeOut = 60000, int? connectionLimit = null)
        {

            string esUrl = Devmasters.Config.GetWebConfigValue("ESConnection");

            //var singlePool = new Elasticsearch.Net.SingleNodeConnectionPool(new Uri(esUrl));
            var pool = new Elasticsearch.Net.StaticConnectionPool(esUrl
                .Split(';')
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Select(u => new Uri(u))
                );

            //var pool = new Elasticsearch.Net.SniffingConnectionPool(esUrl
            //    .Split(';')
            //    .Where(m=>!string.IsNullOrWhiteSpace(m))
            //    .Select(u => new Uri(u)));
            var settings = new ConnectionSettings(pool)
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

            if (System.Diagnostics.Debugger.IsAttached || ESTraceLoggerExists || Devmasters.Config.GetWebConfigValue("ESDebugDataEnabled") == "true")
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

            // Add the Analyzer with a name
            set.Analysis = new Nest.Analysis()
            {
                Analyzers = new Analyzers(),
                TokenFilters = BasicTokenFilters(),
            };

            set.Analysis.Analyzers.Add("default", DefaultAnalyzer());

            IndexState idxSt = new IndexState();
            idxSt.Settings = set;

            var res = client.Indices
                .Create(client.ConnectionSettings.DefaultIndex, i => i
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

            // Add the Analyzer with a name
            set.Analysis = new Nest.Analysis()
            {
                Analyzers = new Analyzers(),
                TokenFilters = BasicTokenFilters(),
            };

            set.Analysis.Analyzers.Add("default", DefaultAnalyzer());

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
                       .Create(client.ConnectionSettings.DefaultIndex, i => i
                           .InitializeUsing(idxSt)
                           .Map<Lib.Data.Insolvence.Rizeni>(map => map.AutoMap().DateDetection(false))
                       );
                    break;
                case IndexType.Dotace:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i
                           .InitializeUsing(idxSt)
                           .Map<Data.Dotace.Dotace>(map => map.AutoMap().DateDetection(false))
                       );
                    break;
                case IndexType.Osoby:
                    res = client.Indices
                        .Create(client.ConnectionSettings.DefaultIndex, i => i
                            .InitializeUsing(new IndexState()
                            {
                                Settings = new IndexSettings()
                                {
                                    NumberOfReplicas = 2,
                                    NumberOfShards = 2,
                                    Analysis = new Nest.Analysis()
                                    {
                                        TokenFilters = BasicTokenFilters(),
                                        Analyzers = new Analyzers(new Dictionary<string, IAnalyzer>()
                                        {
                                            ["default"] = DefaultAnalyzer(),
                                            ["lowercase"] = LowerCaseOnlyAnalyzer(),
                                            ["lowercase_ascii"] = LowerCaseAsciiAnalyzer()
                                        })
                                    }
                                }
                            })
                            .Map<Data.OsobyES.OsobaES>(map => map
                                .AutoMap()
                                .Properties(p => p
                                    .Text(t => t
                                        .Name(n => n.FullName)
                                        .Fields(ff => ff
                                            .Text(tt => tt
                                                .Name("lower")
                                                .Analyzer("lowercase")
                                            )
                                            .Text(tt => tt
                                                .Name("lowerascii")
                                                .Analyzer("lowercase_ascii")
                                            )
                                        )
                                    )
                               )
                            )
                        );
                    break;

                case IndexType.Smlouvy:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i
                           .InitializeUsing(idxSt)
                           .Map<Lib.Data.Smlouva>(map => map.AutoMap().DateDetection(false))
                       );
                    break;
                case IndexType.Firmy:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i
                           .InitializeUsing(idxSt)
                           .Map<Data.Firma.Search.FirmaInElastic>(map => map.AutoMap(maxRecursion: 1))
                       );
                    break;
                case IndexType.KIndex:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i
                           .InitializeUsing(idxSt)
                           .Map<Analysis.KorupcniRiziko.KIndexData>(map => map.AutoMap(maxRecursion: 2))
                       );
                    break;
                case IndexType.Logs:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i
                           .InitializeUsing(idxSt)
                           .Map<Lib.Data.Logs.ProfilZadavateleDownload>(map => map.AutoMap(maxRecursion: 1))
                       );
                    break;
                case IndexType.Audit:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i
                           .InitializeUsing(new IndexState()
                           {
                               Settings = new IndexSettings()
                               {
                                   NumberOfReplicas = 1,
                                   NumberOfShards = 2
                               }
                           }
                           )
                           .Map<Lib.Data.Audit>(map => map.AutoMap(maxRecursion: 1))
                       );
                    break;
                case IndexType.VerejneZakazkyNaProfiluRaw:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i
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
                case IndexType.RPP_Kategorie:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i
                           .InitializeUsing(new IndexState()
                           {
                               Settings = new IndexSettings()
                               {
                                   NumberOfReplicas = 2,
                                   NumberOfShards = 2
                               }
                           }
                           )
                           .Map<Lib.Data.External.RPP.KategorieOVM>(map => map.AutoMap(maxRecursion: 1))
                       );
                    break;
                case IndexType.RPP_OVM:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i
                           .InitializeUsing(new IndexState()
                           {
                               Settings = new IndexSettings()
                               {
                                   NumberOfReplicas = 2,
                                   NumberOfShards = 2
                               }
                           }
                           )
                           .Map<Lib.Data.External.RPP.OVMFull>(map => map.AutoMap(maxRecursion: 1))
                       );
                    break;
                case IndexType.RPP_ISVS:
                    res = client.Indices
                       .Create(client.ConnectionSettings.DefaultIndex, i => i
                           .InitializeUsing(new IndexState()
                           {
                               Settings = new IndexSettings()
                               {
                                   NumberOfReplicas = 2,
                                   NumberOfShards = 2
                               }
                           }
                           )
                           .Map<Lib.Data.External.RPP.ISVS>(map => map.AutoMap(maxRecursion: 1))
                       );
                    break;
            }

        }

        private static ITokenFilters BasicTokenFilters()
        {
            var tokenFilters = new TokenFilters();
            tokenFilters.Add("czech_stop", new StopTokenFilter() { StopWords = new string[] { "_czech_" } });
            tokenFilters.Add("czech_stemmer", new StemmerTokenFilter() { Language = "czech" });
            return tokenFilters;
        }

        private static IAnalyzer DefaultAnalyzer()
        {
            var analyzer = new CustomAnalyzer();
            analyzer.Tokenizer = "standard";
            analyzer.Filter = new List<string>
            {
                "lowercase",
                "czech_stop",
                "czech_stemmer",
                "asciifolding"
            };
            return analyzer;
        }

        private static IAnalyzer LowerCaseOnlyAnalyzer()
        {
            var analyzer = new CustomAnalyzer();
            analyzer.Tokenizer = "whitespace";
            analyzer.Filter = new List<string>
            {
                "lowercase",
            };
            return analyzer;
        }

        private static IAnalyzer LowerCaseAsciiAnalyzer()
        {
            var analyzer = new CustomAnalyzer();
            analyzer.Tokenizer = "whitespace";
            analyzer.Filter = new List<string>
            {
                "lowercase",
                "asciifolding"
            };
            return analyzer;
        }

        public static void LogQueryError<T>(Nest.ISearchResponse<T> esReq, string text = "", System.Web.HttpContextBase httpContext = null, Exception ex = null)
            where T : class
        {
            Elasticsearch.Net.ServerError serverErr = esReq.ServerError;
            ESLogger.Error(new Devmasters.Logging.LogMessage()
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
