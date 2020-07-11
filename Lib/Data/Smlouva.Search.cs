using Devmasters.Core.Batch;
using HlidacStatu.Lib.Searching;
using HlidacStatu.Lib.Searching.Rules;
using HlidacStatu.Util;
using HlidacStatu.Util.Cache;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class Smlouva
    {
        public static class Search
        {
            public const int DefaultPageSize = 40;


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


            public static IRule[] irules = new IRule[] {
               new OsobaId("osobaid:","ico:" ),
               new Holding("holdingprijemce:","icoprijemce:" ),
               new Holding("holdingplatce:","icoplatce:" ),
               new Holding("holdingdodavatel:","icoprijemce:" ),
               new Holding("holdingzadavatel:","icoplatce:" ),
               new Holding(null,"ico:" ),

               new TransformPrefixWithValue("ds:","(prijemce.datovaSchranka:${q} OR platce.datovaSchranka:${q}) ",null ),
               new TransformPrefix("dsprijemce:","prijemce.datovaSchranka:",null  ),
               new TransformPrefix("dsplatce:","platce.datovaSchranka:",null  ),
               new TransformPrefixWithValue("ico:","(prijemce.ico:${q} OR platce.ico:${q}) ",null ),
               new TransformPrefix("icoprijemce:","prijemce.ico:",null ),
               new TransformPrefix("icoplatce:","platce.ico:",null ),
               new TransformPrefix("jmenoprijemce:","prijemce.nazev:",null ),
               new TransformPrefix("jmenoplatce:","platce.nazev:",null ),
               new TransformPrefix("id:","id:",null ),
               new TransformPrefix("idverze:","id:",null ),
               new TransformPrefix("idsmlouvy:","identifikator.idSmlouvy:",null ),
               new TransformPrefix("predmet:","predmet:",null ),
               new TransformPrefix("cislosmlouvy:","cisloSmlouvy:",null ),
               new TransformPrefix("mena:","ciziMena.mena:",null ),
               new TransformPrefix("cenasdph:","hodnotaVcetneDph:",null ),
               new TransformPrefix("cenabezdph:","hodnotaBezDph:",null ),
               new TransformPrefix("cena:","calculatedPriceWithVATinCZK:",null ),
               new TransformPrefix("zverejneno:","casZverejneni:", "[<>]?[{\\[]+" ),
               new TransformPrefixWithValue("zverejneno:","casZverejneni:[${q} TO ${q}||+1d}", "\\d+" ),
               new TransformPrefix("podepsano:","datumUzavreni:", "[<>]?[{\\[]+" ),
               new TransformPrefixWithValue("podepsano:","datumUzavreni:[${q} TO ${q}||+1d}", "\\d+"  ),
               new TransformPrefix("schvalil:","schvalil:",null ),
               new TransformPrefix("textsmlouvy:","prilohy.plainTextContent:",null ),
               new Smlouva_Chyby(),
               new Smlouva_Oblast(),

            };


            public static QueryContainer GetSimpleQuery(string query)
            {
                //ds: -> 
                //var qc  = Lib.Search.Tools.GetSimpleQuery<Lib.Data.Smlouva>(query,rules);;
                var qc = Lib.Searching.SimpleQueryCreator.GetSimpleQuery<Lib.Data.Smlouva>(query, irules);
                return qc;
            }


            static string regex = "[^/]*\r\n/(?<regex>[^/]*)/\r\n[^/]*\r\n";
            static System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
                        | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            static System.Text.RegularExpressions.Regex regFindRegex = new System.Text.RegularExpressions.Regex(regex, options);


            public static SmlouvaSearchResult SearchRaw(QueryContainer query, int page, int pageSize, OrderResult order,
        AggregationContainerDescriptor<Lib.Data.Smlouva> anyAggregation = null,
        bool? platnyZaznam = null, bool includeNeplatne = false, bool logError = true, bool fixQuery = true,
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

                ISearchResponse<Lib.Data.Smlouva> res = _coreSearch(query, page, pageSize, order, anyAggregation, platnyZaznam,
                    includeNeplatne, logError, withHighlighting);


                if (res.IsValid == false && logError)
                    Lib.ES.Manager.LogQueryError<Lib.Data.Smlouva>(res, query.ToString());


                result.Total = res?.Total ?? 0;
                result.IsValid = res?.IsValid ?? false;
                result.ElasticResults = res;
                return result;
            }



            public static SmlouvaSearchResult SimpleSearch(string query, int page, int pageSize, string order,
AggregationContainerDescriptor<Lib.Data.Smlouva> anyAggregation = null,
bool? platnyZaznam = null, bool includeNeplatne = false, bool logError = true, bool fixQuery = true,
bool withHighlighting = false, bool exactNumOfResults = false)
            {
                order = Devmasters.Core.TextUtil.NormalizeToNumbersOnly(order);
                OrderResult eorder = OrderResult.Relevance;
                System.Enum.TryParse<OrderResult>(order, out eorder);

                return SimpleSearch(query, page, pageSize, eorder, anyAggregation,
                    platnyZaznam, includeNeplatne, logError, fixQuery,
                    withHighlighting, exactNumOfResults
                    );

            }
            public static SmlouvaSearchResult SimpleSearch(string query, int page, int pageSize, OrderResult order,
        AggregationContainerDescriptor<Lib.Data.Smlouva> anyAggregation = null,
        bool? platnyZaznam = null, bool includeNeplatne = false, bool logError = true, bool fixQuery = true,
        bool withHighlighting = false, bool exactNumOfResults = false)
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
                    result.ElasticResults = null;
                    result.IsValid = false;
                    result.Total = 0;
                    return result;
                }

                Devmasters.Core.StopWatchEx sw = new Devmasters.Core.StopWatchEx();
                sw.Start();

                if (fixQuery)
                {
                    query = Searching.Tools.FixInvalidQuery(query, irules, Searching.Tools.DefaultQueryOperators );
                    result.Q = query;
                }

                if (platnyZaznam.HasValue)
                    query = Lib.Searching.Tools.ModifyQueryAND(query, "platnyZaznam:" + platnyZaznam.Value.ToString().ToLower());


                ISearchResponse<Lib.Data.Smlouva> res =
                    _coreSearch(GetSimpleQuery(query), page, pageSize, order, anyAggregation, platnyZaznam,
                    includeNeplatne, logError, withHighlighting, exactNumOfResults);

                Data.Audit.Add(Data.Audit.Operations.Search, "", "", "Smlouva", res.IsValid ? "valid" : "invalid", query, null);

                if (res.IsValid == false && logError)
                    Lib.ES.Manager.LogQueryError<Lib.Data.Smlouva>(res, query);

                sw.Stop();

                result.ElapsedTime = sw.Elapsed;
                try
                {
                    result.Total = res?.Total ?? 0;

                }
                catch (Exception)
                {
                    result.Total = 0;
                }
                result.IsValid = res?.IsValid ?? false;
                result.ElasticResults = res;
                return result;
            }


            private static ISearchResponse<Lib.Data.Smlouva> _coreSearch(QueryContainer query, int page, int pageSize,
                OrderResult order,
                AggregationContainerDescriptor<Lib.Data.Smlouva> anyAggregation = null,
                bool? platnyZaznam = null, bool includeNeplatne = false, bool logError = true,
                bool withHighlighting = false, bool exactNumOfResults = false)
            {
                page = page - 1;
                if (page < 0)
                    page = 0;

                if (page * pageSize >= Searching.Tools.MaxResultWindow) //elastic limit
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
                    if (platnyZaznam.HasValue && platnyZaznam == false)
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
                            .Highlight(h => Lib.Searching.Tools.GetHighlight<Data.Smlouva>(withHighlighting))
                            .TrackTotalHits(exactNumOfResults || page * pageSize == 0 ? true : (bool?)null)
                    );
                    if (res.IsValid == false && res.ServerError.Status == 429)
                    {
                        System.Threading.Thread.Sleep(100);
                        res = client
                            .Search<Lib.Data.Smlouva>(s => s
                                .Index(indexes)
                                .Size(pageSize)
                                .From(page * pageSize)
                                .Query(q => query)
                                .Source(m => m.Excludes(e => e.Field(o => o.Prilohy)))
                                .Sort(ss => GetSort(order))
                                .Aggregations(aggrFunc)
                                .Highlight(h => Lib.Searching.Tools.GetHighlight<Data.Smlouva>(withHighlighting))
                                .TrackTotalHits(exactNumOfResults || page * pageSize == 0 ? true : (bool?)null)
                        );
                        if (res.IsValid == false && res.ServerError.Status == 429)
                        {
                            System.Threading.Thread.Sleep(200);
                            res = client
                                .Search<Lib.Data.Smlouva>(s => s
                                    .Index(indexes)
                                    .Size(pageSize)
                                    .From(page * pageSize)
                                    .Query(q => query)
                                    .Source(m => m.Excludes(e => e.Field(o => o.Prilohy)))
                                    .Sort(ss => GetSort(order))
                                    .Aggregations(aggrFunc)
                                    .Highlight(h => Lib.Searching.Tools.GetHighlight<Data.Smlouva>(withHighlighting))
                                    .TrackTotalHits(exactNumOfResults || page * pageSize == 0 ? true : (bool?)null)
                            );

                        }

                    }

                    if (withHighlighting && res.Shards != null && res.Shards.Failed > 0) //if some error, do it again without highlighting
                    {
                        res = client
                            .Search<Lib.Data.Smlouva>(s => s
                                .Index(indexes)
                                .Size(pageSize)
                                .From(page * pageSize)
                                .Query(q => query)
                                .Source(m => m.Excludes(e => e.Field(o => o.Prilohy)))
                                .Sort(ss => GetSort(order))
                                .Aggregations(aggrFunc)
                                .Highlight(h => Lib.Searching.Tools.GetHighlight<Data.Smlouva>(false))
                                .TrackTotalHits(exactNumOfResults || page * pageSize == 0 ? true : (bool?)null)
                        );
                    }
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
                AggregationContainerDescriptor<Lib.Data.Smlouva> anyAggregation = null, bool? platnyZaznam = null,
                bool includeNeplatne = false, bool exactNumOfResults = false
                )
            {
                return RawSearch(Searching.Tools.GetRawQuery(jsonQuery), page, pageSize, order, anyAggregation, platnyZaznam, includeNeplatne,
                    exactNumOfResults: exactNumOfResults);
            }
            public static Nest.ISearchResponse<Lib.Data.Smlouva> RawSearch(Nest.QueryContainer query, int page, int pageSize, OrderResult order = OrderResult.Relevance,
                AggregationContainerDescriptor<Lib.Data.Smlouva> anyAggregation = null, bool? platnyZaznam = null,
                bool includeNeplatne = false,
                bool withHighlighting = false, bool exactNumOfResults = false
                )
            {
                var res = _coreSearch(query, page, pageSize, order, anyAggregation, platnyZaznam: platnyZaznam, includeNeplatne: includeNeplatne, logError: true, withHighlighting: withHighlighting, exactNumOfResults: exactNumOfResults);
                return res;

            }
            public static SortDescriptor<Smlouva> GetSort(string sorder)
            {
                OrderResult order = OrderResult.Relevance;
                Enum.TryParse<OrderResult>(sorder, out order);
                return GetSort(order);
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
                bool? platnyZaznam = null, bool includeNeplatne = false,
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
                    q.query, q.page, q.pageSize, q.order, q.anyAggregation, q.platnyZaznam, q.includeNeplatne, q.logError, q.fixQuery, exactNumOfResults: q.exactNumOfResults
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
                public bool? platnyZaznam = null;
                public bool includeNeplatne = false;
                public bool logError = true;
                public bool fixQuery = true;
                public bool exactNumOfResults = false;
            }

        }
    }
}