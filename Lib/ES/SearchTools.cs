using Devmasters.Core.Batch;
using HlidacStatu.Util;
using HlidacStatu.Util.Cache;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.ES
{


    public static class SearchTools
    {
        public const int DefaultPageSize = 40;
        public const int MaxResultWindow = 10000;


        [Devmasters.Core.ShowNiceDisplayName()]
        [Devmasters.Core.Sortable(Devmasters.Core.SortableAttribute.SortAlgorithm.BySortValue)]
        public enum OrderResult
        {
            [Devmasters.Core.SortValue(0)]
            [Devmasters.Core.NiceDisplayName("podle relevance")]
            Relevance = 0,

            [Devmasters.Core.SortValue(5)]
            [Devmasters.Core.NiceDisplayName("nově zveřejněné první")]
            DateAddedDesc = 1,

            [Devmasters.Core.NiceDisplayName("nově zveřejněné poslední")]
            [Devmasters.Core.SortValue(6)]
            DateAddedAsc = 2,

            [Devmasters.Core.SortValue(1)]
            [Devmasters.Core.NiceDisplayName("nejlevnější první")]
            PriceAsc = 3,

            [Devmasters.Core.SortValue(2)]
            [Devmasters.Core.NiceDisplayName("nejdražší první")]
            PriceDesc = 4,

            [Devmasters.Core.SortValue(7)]
            [Devmasters.Core.NiceDisplayName("nově uzavřené první")]
            DateSignedDesc = 5,

            [Devmasters.Core.NiceDisplayName("nově uzavřené poslední")]
            [Devmasters.Core.SortValue(8)]
            DateSignedAsc = 6,

            [Devmasters.Core.NiceDisplayName("nejvíce chybové první")]
            [Devmasters.Core.SortValue(10)]
            ConfidenceDesc = 7,

            [Devmasters.Core.NiceDisplayName("podle odběratele")]
            [Devmasters.Core.SortValue(98)]
            CustomerAsc = 8,

            [Devmasters.Core.NiceDisplayName("podle dodavatele")]
            [Devmasters.Core.SortValue(99)]
            ContractorAsc = 9,

            [Devmasters.Core.Disabled]
            ClassificationRelevance = 665,

            [Devmasters.Core.Disabled]
            FastestForScroll = 666,
            [Devmasters.Core.Disabled]
            LastUpdate = 667,

        }


        public static QueryContainer GetRawQuery(string jsonQuery)
        {
            QueryContainer qc = null;
            if (string.IsNullOrEmpty(jsonQuery))
                qc = new QueryContainerDescriptor<Lib.Data.Smlouva>().MatchAll();
            else
            {
                qc = new QueryContainerDescriptor<Lib.Data.Smlouva>().Raw(jsonQuery);
            }

            return qc;

        }


        static string[] queryShorcuts = new string[] {
                "ico:",
                "osobaid:",
                "ds:",
                "dsprijemce:",
                "dsplatce:",
                "icoprijemce:",
                "icoplatce:",
                "jmenoprijemce:",
                "jmenoplatce:",
                "id:",
                "idverze:",
                "idsmlouvy:",
                "predmet:",
                "cislosmlouvy:",
                "mena:",
                "cenasdph:",
                "cenabezdph:",
                "cena:",
                "zverejneno:",
                "podepsano:",
                "schvalil:",
                "textsmlouvy:",
                "holding:",
                "holdingprijemce:",
                "holdingplatce:",
                "holdingdodavatel:",
                "holdingzadavatel:",
            };

        public static string FixInvalidQuery(string query)
        {
            return Lib.Search.Tools.FixInvalidQuery(query, queryShorcuts, Lib.Search.Tools.defaultQueryOperators);
        }

        public static string ToElasticDate(DateTime date)
        {
            switch (date.Kind)
            {
                case DateTimeKind.Unspecified:
                case DateTimeKind.Local:
                    return date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                case DateTimeKind.Utc:
                    return date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                default:
                    return date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            }
        }


        public static QueryContainer GetSimpleQuery(string query)
        {
            //ds: -> 
            Lib.Search.Rule[] rules = new Lib.Search.Rule[] {
                new Lib.Search.Rule(@"osobaid:(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?)) ","ico" ),

               new Lib.Search.Rule(@"holding:(?<q>(\d{1,8})) ","ico" ),
               new Lib.Search.Rule(@"holdingprijemce:(?<q>(\d{1,8})) (\s|$){1,}","icoprijemce" ),
               new Lib.Search.Rule(@"holdingplatce:(?<q>(\d{1,8})) (\s|$){1,}","icoplatce" ),
               new Lib.Search.Rule(@"holdingdodavatel:(?<q>(\d{1,8})) (\s|$){1,}","icoprijemce" ),
               new Lib.Search.Rule(@"holdingzadavatel:(?<q>(\d{1,8})) (\s|$){1,}","icoplatce" ),

               new Lib.Search.Rule("ds:","(prijemce.datovaSchranka:${q} OR platce.datovaSchranka:${q}) " ),
               new Lib.Search.Rule("dsprijemce:","prijemce.datovaSchranka:" ),
               new Lib.Search.Rule("dsplatce:","platce.datovaSchranka:" ),
               new Lib.Search.Rule("ico:","(prijemce.ico:${q} OR platce.ico:${q}) " ),
               new Lib.Search.Rule("icoprijemce:","prijemce.ico:" ),
               new Lib.Search.Rule("icoplatce:","platce.ico:" ),
               new Lib.Search.Rule("jmenoprijemce:","prijemce.nazev:" ),
               new Lib.Search.Rule("jmenoplatce:","platce.nazev:" ),
               new Lib.Search.Rule("id:","id:" ),
               new Lib.Search.Rule("idverze:","id:" ),
               new Lib.Search.Rule("idsmlouvy:","identifikator.idSmlouvy:" ),
               new Lib.Search.Rule("predmet:","predmet:" ),
               new Lib.Search.Rule("cislosmlouvy:","cisloSmlouvy:" ),
               new Lib.Search.Rule("mena:","ciziMena.mena:" ),
               new Lib.Search.Rule("cenasdph:","hodnotaVcetneDph:" ),
               new Lib.Search.Rule("cenabezdph:","hodnotaBezDph:" ),
               new Lib.Search.Rule("cena:","calculatedPriceWithVATinCZK:" ),
               new Lib.Search.Rule("zverejneno:\\[","casZverejneni:[" ),
               new Lib.Search.Rule("zverejneno:(?=[<>])","casZverejneni:${q}" ),
               new Lib.Search.Rule("zverejneno:(?=\\d)","casZverejneni:[${q} TO ${q}||+1d]" ),
               new Lib.Search.Rule("podepsano:\\[","datumUzavreni:[" ),
               new Lib.Search.Rule("podepsano:(?=[<>])","datumUzavreni:${q}" ),
               new Lib.Search.Rule("podepsano:(?=\\d)","datumUzavreni:[${q} TO ${q}||+1d]" ),
               new Lib.Search.Rule("schvalil:","schvalil:" ),
               new Lib.Search.Rule("textsmlouvy:","prilohy.plainTextContent:" ),
               new Lib.Search.Rule("chyby:","${level}" ),

            };

            var qc  = Lib.Search.Tools.GetSimpleQuery<Lib.Data.Smlouva>(query,rules);;

            return qc;
        }

        private static string paramReplaceEval(Match m)
        {
            return m.Value;
        }

        static string regex = "[^/]*\r\n/(?<regex>[^/]*)/\r\n[^/]*\r\n";
        static System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
                    | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        static System.Text.RegularExpressions.Regex regFindRegex = new System.Text.RegularExpressions.Regex(regex, options);


        public static SmlouvaSearchResult Search(QueryContainer query, int page, int pageSize, OrderResult order,
    AggregationContainerDescriptor<Lib.Data.Smlouva> anyAggregation = null,
    int? platnyZaznam = null, bool includeNeplatne = false, bool logError = true, bool fixQuery = true,
    bool withHighlighting = false)
        {

            var result = new SmlouvaSearchResult()
            {
                Page = page,
                PageSize = pageSize,
                OrigQuery = "",
                Q = "",
                Order = ((int)order).ToString()
            };

            ISearchResponse<Lib.Data.Smlouva> res =
                
                _coreSearch(query, page, pageSize, order, anyAggregation, platnyZaznam,
                includeNeplatne, logError, withHighlighting);

            if (res.IsValid == false && logError)
                Lib.ES.Manager.LogQueryError<Lib.Data.Smlouva>(res, query.ToString());


            result.Total = res?.Total ?? 0;
            result.IsValid = res?.IsValid ?? false;
            result.ElasticResults = res;
            return result;
        }



        public static SmlouvaSearchResult SimpleSearch(string query, int page, int pageSize, OrderResult order,
    AggregationContainerDescriptor<Lib.Data.Smlouva> anyAggregation = null,
    int? platnyZaznam = null, bool includeNeplatne = false, bool logError = true, bool fixQuery = true,
    bool withHighlighting = false)
        {

            var result = new SmlouvaSearchResult()
            {
                Page = page,
                PageSize = pageSize,
                OrigQuery = query,
                Q = query,
                Order = ((int)order).ToString()
            };

            if (string.IsNullOrEmpty(query))
            {
                result.Result = null;
                result.IsValid = false;
                result.Total = 0;
                return result;
            }

            Devmasters.Core.StopWatchEx sw = new Devmasters.Core.StopWatchEx();
            sw.Start();

            if (fixQuery)
            {
                query = FixInvalidQuery(query);
                result.Q = query;
            }
            if (logError && result.Q != result.OrigQuery)
            {
                HlidacStatu.Util.Consts.Logger.Debug(new Devmasters.Core.Logging.LogMessage()
                    .SetMessage("Fixed query")
                    .SetCustomKeyValue("runningQuery", result.Q)
                    .SetCustomKeyValue("origQuery", result.OrigQuery)
                    );
            }

            if (platnyZaznam.HasValue)
                query = Lib.Search.Tools.ModifyQueryAND(query, "platnyZaznam:" + platnyZaznam.Value);


            ISearchResponse<Lib.Data.Smlouva> res =
                _coreSearch(GetSimpleQuery(query), page, pageSize, order, anyAggregation, platnyZaznam,
                includeNeplatne, logError, withHighlighting);

            if (res.IsValid == false && logError)
                Lib.ES.Manager.LogQueryError<Lib.Data.Smlouva>(res, query);

            sw.Stop();

            result.ElapsedTime = sw.Elapsed;

            result.Total = res?.Total ?? 0;
            result.IsValid = res?.IsValid ?? false;
            result.ElasticResults = res;
            return result;
        }


        private static ISearchResponse<Lib.Data.Smlouva> _coreSearch(QueryContainer query, int page, int pageSize,
            OrderResult order,
            AggregationContainerDescriptor<Lib.Data.Smlouva> anyAggregation = null,
            int? platnyZaznam = null, bool includeNeplatne = false, bool logError = true,
            bool withHighlighting = false)
        {
            page = page - 1;
            if (page < 0)
                page = 0;

            if (page * pageSize >= MaxResultWindow) //elastic limit
            {
                page = 0; pageSize = 0; //return nothing
            }

            AggregationContainerDescriptor<Lib.Data.Smlouva> baseAggrDesc = null;
            baseAggrDesc = anyAggregation == null ?
                    null //new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>().Sum("sumKc", m => m.Field(f => f.CalculatedPriceWithVATinCZK))
                    : anyAggregation;

            Func<AggregationContainerDescriptor<Lib.Data.Smlouva>, AggregationContainerDescriptor<Lib.Data.Smlouva>> aggrFunc
                = (aggr) => { return baseAggrDesc; };

            ISearchResponse<Lib.Data.Smlouva> res = null;
            try
            {
                var client = Lib.ES.Manager.GetESClient();
                if (platnyZaznam == 0)
                    client = Lib.ES.Manager.GetESClient_Sneplatne();
                Indices indexes = client.ConnectionSettings.DefaultIndex;
                if (includeNeplatne)
                {
                    indexes = ES.Manager.defaultIndexName_SAll;
                }

                res = client
                    .Search<Lib.Data.Smlouva>(s => s
                        .Index(indexes)
                        .Size(pageSize)
                        .From(page * pageSize)
                        .Query(q => query)
                        .Source(m => m.Excludes(e => e.Field(o => o.Prilohy)))
                        .Sort(ss => GetSort(order))
                        .Aggregations(aggrFunc)
                        .Highlight(h => Lib.Search.Tools.GetHighlight<Data.Smlouva>(withHighlighting))

                );

            }
            catch (Exception e)
            {
                if (res != null && res.ServerError != null)
                    Lib.ES.Manager.LogQueryError<Lib.Data.Smlouva>(res);
                else
                    HlidacStatu.Util.Consts.Logger.Error("", e);
                throw;
            }

            if (res.IsValid == false && logError)
                Lib.ES.Manager.LogQueryError<Lib.Data.Smlouva>(res);

            return res;
        }


        public static Nest.ISearchResponse<Lib.Data.Smlouva> RawSearch(string jsonQuery, int page, int pageSize, OrderResult order = OrderResult.Relevance,
            AggregationContainerDescriptor<Lib.Data.Smlouva> anyAggregation = null, int? platnyZaznam = null, bool includeNeplatne = false
            )
        {
            return RawSearch(GetRawQuery(jsonQuery), page, pageSize, order, anyAggregation, platnyZaznam, includeNeplatne);
        }
        public static Nest.ISearchResponse<Lib.Data.Smlouva> RawSearch(Nest.QueryContainer query, int page, int pageSize, OrderResult order = OrderResult.Relevance,
            AggregationContainerDescriptor<Lib.Data.Smlouva> anyAggregation = null, int? platnyZaznam = null, bool includeNeplatne = false,
            bool withHighlighting = false
            )
        {
            var res = _coreSearch(query, page, pageSize, order, anyAggregation, platnyZaznam: platnyZaznam, includeNeplatne: includeNeplatne, logError: true, withHighlighting: withHighlighting);
            return res;

        }

        public static SortDescriptor<Data.Smlouva> GetSort(OrderResult order)
        {
            SortDescriptor<Data.Smlouva> s = new SortDescriptor<Data.Smlouva>().Field(f => f.Field("_score").Descending());
            switch (order)
            {
                case OrderResult.DateAddedDesc:
                    s = new SortDescriptor<Data.Smlouva>().Field(m => m.Field(f => f.casZverejneni).Descending());
                    break;
                case OrderResult.DateAddedAsc:
                    s = new SortDescriptor<Data.Smlouva>().Field(m => m.Field(f => f.casZverejneni).Ascending());
                    break;
                case OrderResult.DateSignedDesc:
                    s = new SortDescriptor<Data.Smlouva>().Field(m => m.Field(f => f.datumUzavreni).Descending());
                    break;
                case OrderResult.DateSignedAsc:
                    s = new SortDescriptor<Data.Smlouva>().Field(m => m.Field(f => f.datumUzavreni).Ascending());
                    break;
                case OrderResult.PriceAsc:
                    s = new SortDescriptor<Data.Smlouva>().Field(m => m.Field(f => f.CalculatedPriceWithVATinCZK).Ascending());
                    break;
                case OrderResult.PriceDesc:
                    s = new SortDescriptor<Data.Smlouva>().Field(m => m.Field(f => f.CalculatedPriceWithVATinCZK).Descending());
                    break;
                case OrderResult.FastestForScroll:
                    s = new SortDescriptor<Data.Smlouva>().Field(f => f.Field("_doc"));
                    break;
                case OrderResult.ConfidenceDesc:
                    s = new SortDescriptor<Data.Smlouva>().Field(f => f.Field(ff => ff.ConfidenceValue).Descending());
                    break;
                case OrderResult.CustomerAsc:
                    s = new SortDescriptor<Data.Smlouva>().Field(f => f.Field(ff => ff.Platce.ico).Ascending());
                    break;
                case OrderResult.ContractorAsc:
                    s = new SortDescriptor<Data.Smlouva>().Field(f => f.Field("prijemce.ico").Ascending());
                    break;
                case OrderResult.LastUpdate:
                    s = new SortDescriptor<Data.Smlouva>().Field(f => f.Field("lastUpdate").Descending());
                    break;
                case OrderResult.ClassificationRelevance:
                    s = new SortDescriptor<Data.Smlouva>().Field(f => f.Field("classification.types.classifProbability").Descending());
                    break;
                case OrderResult.Relevance:
                default:
                    break;
            }

            return s;

        }






        static string ScrollLifeTime = "15m";
        public static void DoActionForAll<T>(
            System.Func<IHit<T>, object, ActionOutputData> action,
            object actionParameters,
            System.Action<string> logOutputFunc,
            System.Action<Devmasters.Core.Batch.ActionProgressData> progressOutputFunc,
            bool parallel,
            int blockSize = 500, int? maxDegreeOfParallelism = null,
            bool IdOnly = false,
            ElasticClient elasticClient = null,
            string query = null, OrderResult order = OrderResult.FastestForScroll,
            Indices indexes = null, string prefix = ""

            )
            where T : class
        {
            var client = elasticClient ?? Lib.ES.Manager.GetESClient();

            Func<int, int, ISearchResponse<T>> searchFunc = null;
            if (IdOnly)
                searchFunc = (size, page) =>
                {
                    return client.Search<T>(a => a
                                .Index(indexes ?? client.ConnectionSettings.DefaultIndex)
                                .Source(ss => ss.ExcludeAll())
                                //.Fields(f => f.Field("Id"))
                                .Size(size)
                                .From(page * size)
                                .Query(q => GetSimpleQuery(query))
                                .Sort(ss => GetSort(order))
                                .Scroll(ScrollLifeTime)
                                );
                };
            else
                searchFunc = (size, page) =>
                {
                    return client.Search<T>(a => a
                            .Index(indexes ?? client.ConnectionSettings.DefaultIndex)
                            .Size(size)
                            .From(page * size)
                            .Query(q => GetSimpleQuery(query))
                            .Sort(ss => GetSort(order))
                            .Scroll(ScrollLifeTime)
                        );
                };

            DoActionForQuery<T>(client,
                    searchFunc,
                    action, actionParameters,
                    logOutputFunc,
                    progressOutputFunc,
                    parallel,
                    blockSize, maxDegreeOfParallelism, prefix
                    );

        }

        public static void DoActionForQuery<T>(ElasticClient client,
            Func<int, int, ISearchResponse<T>> searchFunc,
            System.Func<IHit<T>, object, Devmasters.Core.Batch.ActionOutputData> action, object actionParameters,
            System.Action<string> logOutputFunc,
            System.Action<Devmasters.Core.Batch.ActionProgressData> progressOutputFunc,
            bool parallel,
            int blockSize = 500, int? maxDegreeOfParallelism = null, string prefix = ""
            )
            where T : class
        {
            DateTime started = DateTime.Now;
            long total = 0;
            int currIteration = 0;

            int processedCount = 0;
            ISearchResponse<T> result = default(ISearchResponse<T>);
            //var total = NoveInzeraty.Lib.ES.SearchTools.NumberOfDocs(null, null);
            string scrollId = null;

            //create scroll search context
            bool firstResult = true;

            if (maxDegreeOfParallelism <= 1)
                parallel = false;
            try
            {
                result = searchFunc(blockSize, currIteration);
                if (result.IsValid == false)
                    Lib.ES.Manager.LogQueryError<T>(result);
            }
            catch (Exception e1)
            {
                System.Threading.Thread.Sleep(10000);
                try
                {
                    result = searchFunc(blockSize, currIteration);

                }
                catch (Exception e2)
                {
                    System.Threading.Thread.Sleep(20000);
                    try
                    {
                        result = searchFunc(blockSize, currIteration);

                    }
                    catch (Exception ex)
                    {
                        HlidacStatu.Util.Consts.Logger.Error("Cannot read data from Elastic, skipping iteration" + currIteration, ex);
                        return;
                    }
                }
            }
            scrollId = result.ScrollId;

            do
            {
                DateTime iterationStart = DateTime.Now;
                if (firstResult)
                {
                    firstResult = false;
                }
                else
                {
                    result = client.Scroll<T>(ScrollLifeTime, scrollId);
                    scrollId = result.ScrollId;
                }
                currIteration++;

                if (result.Hits.Count() == 0)
                    break;
                total = result.Total;


                bool canceled = false;

                if (parallel)
                {

                    CancellationTokenSource cts = new CancellationTokenSource();
                    try
                    {
                        ParallelOptions pOptions = new ParallelOptions();
                        if (maxDegreeOfParallelism.HasValue)
                            pOptions.MaxDegreeOfParallelism = maxDegreeOfParallelism.Value;
                        pOptions.CancellationToken = cts.Token;
                        Parallel.ForEach(result.Hits, (hit) =>
                        {
                            if (action != null)
                            {
                                ActionOutputData cancel = null;
                                try
                                {
                                    cancel = action(hit, actionParameters);
                                    System.Threading.Interlocked.Increment(ref processedCount);
                                    if (logOutputFunc != null && !string.IsNullOrEmpty(cancel.Log))
                                        logOutputFunc(cancel.Log);

                                    if (cancel.CancelRunning)
                                        cts.Cancel();
                                }
                                catch (Exception e)
                                {
                                    HlidacStatu.Util.Consts.Logger.Error("DoActionForAll action error", e);
                                    cts.Cancel();
                                }

                            }
                            if (progressOutputFunc != null)
                            {

                                ActionProgressData apd = new ActionProgressData(total, processedCount, started, prefix);
                                progressOutputFunc(apd);
                            }
                        });
                    }
                    catch (OperationCanceledException e)
                    {
                        //Catestrophic Failure
                        canceled = true;
                    }


                }
                else
                    foreach (var hit in result.Hits)
                    {
                        if (action != null)
                        {
                            ActionOutputData cancel = action(hit, actionParameters);
                            System.Threading.Interlocked.Increment(ref processedCount);
                            if (logOutputFunc != null && !string.IsNullOrEmpty(cancel.Log))
                                logOutputFunc(cancel.Log);

                            if (cancel.CancelRunning)
                            {
                                canceled = true;
                                break;
                            }
                        }
                        if (progressOutputFunc != null)
                        {
                            ActionProgressData apd = new ActionProgressData(total, processedCount, started, prefix);
                            progressOutputFunc(apd);
                        }
                    }


                if (canceled)
                    break;
            } while (result.Hits.Count() > 0);

            if (logOutputFunc != null)
                logOutputFunc("Done");

        }


        public static string QueryHash(string typ, string q)
        {
            return Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(typ.ToLower() + "|" + q.ToLower() + "|" + q.Reverse());
        }

        public static bool IsQueryHashCorrect(string typ, string q, string h)
        {
            return h == QueryHash(typ, q);
        }

        public static MemoryCacheManager<SmlouvaSearchResult, string>
            cachedSearches = new MemoryCacheManager<SmlouvaSearchResult, string>("SMLOUVYsearch", funcSimpleSearch, TimeSpan.FromHours(24));


        public static SmlouvaSearchResult CachedSimpleSearch(TimeSpan expiration,
            string query, int page, int pageSize, OrderResult order,
            int? platnyZaznam = null, bool includeNeplatne = false,
            bool logError = true, bool fixQuery = true
            )
        {
            FullSearchQuery q = new FullSearchQuery()
            {
                query = query,
                page = page,
                pageSize = pageSize,
                order = order,
                platnyZaznam = platnyZaznam,
                includeNeplatne = includeNeplatne,
                logError = logError,
                fixQuery = fixQuery

            };
            return cachedSearches.Get(Newtonsoft.Json.JsonConvert.SerializeObject(q), expiration);
        }
        private static SmlouvaSearchResult funcSimpleSearch(string jsonFullSearchQuery)
        {
            var q = Newtonsoft.Json.JsonConvert.DeserializeObject<FullSearchQuery>(jsonFullSearchQuery);
            var ret = SimpleSearch(
                q.query, q.page, q.pageSize, q.order, q.anyAggregation, q.platnyZaznam, q.includeNeplatne, q.logError, q.fixQuery
                );
            //remove debug & more
            return ret;
        }

        private class FullSearchQuery
        {
            public string query;
            public int page;
            public int pageSize;
            public OrderResult order;

            public AggregationContainerDescriptor<Lib.Data.Smlouva> anyAggregation = null;
            public int? platnyZaznam = null;
            public bool includeNeplatne = false;
            public bool logError = true;
            public bool fixQuery = true;
        }

    }
}
