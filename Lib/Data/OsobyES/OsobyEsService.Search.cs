﻿using Devmasters.Core;
using HlidacStatu.Lib.Searching;
using HlidacStatu.Lib.Searching.Rules;
using Nest;
using System;
using System.Linq;

namespace HlidacStatu.Lib.Data.OsobyES
{
    public static partial class OsobyEsService
    {
        static string regex = "[^/]*\r\n/(?<regex>[^/]*)/\r\n[^/]*\r\n";
        static System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
                    | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        static System.Text.RegularExpressions.Regex regFindRegex = new System.Text.RegularExpressions.Regex(regex, options);


        static string[] queryOperators = new string[] { "AND", "OR" };


        static IRule[] irules = new IRule[] {
               new TransformPrefix("osobaid:","osobaId:",null ),
               new TransformPrefixWithValue("status:","(statusText:${q} OR status:${q})",null ),
               new TransformPrefix("roknarozeni:","birthYear:",null ),


            };

        private static string[] queryShorcuts()
        {
            return irules.SelectMany(m => m.Prefixes).Distinct().ToArray();
        }

        public static QueryContainer GetSimpleQuery(string query)
        {
            return GetSimpleQuery(new OsobaEsSearchResult() { Q = query, Page = 1 });
        }
        public static QueryContainer GetSimpleQuery(OsobaEsSearchResult searchdata)
        {
            var query = searchdata.Q;

            string modifiedQ = query; // Search.Tools.FixInvalidQuery(query, queryShorcuts, queryOperators) ?? "";
                                      //check invalid query ( tag: missing value)

            //var qc  = Lib.Search.Tools.GetSimpleQuery<Lib.Data.Smlouva>(query,rules);;
            var qc = Lib.Searching.SimpleQueryCreator.GetSimpleQuery<Lib.Data.Dotace.Dotace>(query, irules);

            return qc;

        }
        public static OsobaEsSearchResult SimpleSearch(string query, int page, int pagesize, OsobaEsSearchResult.OsobaEsOrderResult order,
            bool withHighlighting = false,
            AggregationContainerDescriptor<OsobaES> anyAggregation = null, bool exactNumOfResults = false)
        {
            return SimpleSearch(query, page, pagesize, ((int)order).ToString(),
                withHighlighting,
                anyAggregation, exactNumOfResults);
        }
        public static OsobaEsSearchResult SimpleSearch(string query, int page, int pagesize, string order,
        bool withHighlighting = false,
        AggregationContainerDescriptor<OsobaES> anyAggregation = null, bool exactNumOfResults = false)
        {
            return SimpleSearch(new OsobaEsSearchResult()
            {
                Q = query,
                Page = page,
                PageSize = pagesize,
                Order = Devmasters.Core.TextUtil.NormalizeToNumbersOnly(order),
                ExactNumOfResults = exactNumOfResults
            }, withHighlighting, anyAggregation); ;
        }
        public static OsobaEsSearchResult SimpleSearch(OsobaEsSearchResult search,
            bool withHighlighting = false,
            AggregationContainerDescriptor<OsobaES> anyAggregation = null)
        {

            var page = search.Page - 1 < 0 ? 0 : search.Page - 1;

            var sw = new StopWatchEx();
            sw.Start();
            search.OrigQuery = search.Q;
            search.Q = Lib.Searching.Tools.FixInvalidQuery(search.Q ?? "", queryShorcuts(), queryOperators);

            ISearchResponse<OsobaES> res = null;
            try
            {
                res = _esClient
                        .Search<OsobaES>(s => s
                        .Size(search.PageSize)
                        .ExpandWildcards(Elasticsearch.Net.ExpandWildcards.All)
                        .From(page * search.PageSize)
                        .Query(q => GetSimpleQuery(search))
                        .Sort(ss => GetSort(search.Order))
                        .Highlight(h => Lib.Searching.Tools.GetHighlight<OsobaES>(withHighlighting))
                        .Aggregations(aggr => anyAggregation)
                        .TrackTotalHits((search.ExactNumOfResults || page * search.PageSize == 0) ? true : (bool?)null)
                );
                if (res.IsValid && withHighlighting && res.Shards.Failed > 0) //if some error, do it again without highlighting
                {
                    res = _esClient
                            .Search<OsobaES>(s => s
                            .Size(search.PageSize)
                            .ExpandWildcards(Elasticsearch.Net.ExpandWildcards.All)
                            .From(page * search.PageSize)
                            .Query(q => GetSimpleQuery(search))
                            .Sort(ss => GetSort(search.Order))
                            .Highlight(h => Lib.Searching.Tools.GetHighlight<OsobaES>(false))
                            .Aggregations(aggr => anyAggregation)
                            .TrackTotalHits(search.ExactNumOfResults || page * search.PageSize == 0 ? true : (bool?)null)
                    );
                }

            }
            catch (Exception e)
            {
                Audit.Add(Audit.Operations.Search, "", "", "Dotace", "error", search.Q, null);
                if (res != null && res.ServerError != null)
                {
                    ES.Manager.LogQueryError<OsobaES>(res, "Exception, Orig query:"
                        + search.OrigQuery + "   query:"
                        + search.Q
                        + "\n\n res:" + search.ElasticResults.ToString()
                        , ex: e);
                }
                else
                {
                    HlidacStatu.Util.Consts.Logger.Error("", e);
                }
                throw;
            }
            sw.Stop();

            Audit.Add(Audit.Operations.Search, "", "", "Dotace", res.IsValid ? "valid" : "invalid", search.Q, null);

            if (res.IsValid == false)
            {
                ES.Manager.LogQueryError<OsobaES>(res, "Exception, Orig query:"
                    + search.OrigQuery + "   query:"
                    + search.Q
                    + "\n\n res:" + search.ElasticResults?.ToString()
                    );
            }

            search.Total = res?.Total ?? 0;
            search.IsValid = res?.IsValid ?? false;
            search.ElasticResults = res;
            search.ElapsedTime = sw.Elapsed;
            return search;
        }

        public static SortDescriptor<OsobaES> GetSort(string sorder)
        {
            OsobaEsSearchResult.OsobaEsOrderResult order = OsobaEsSearchResult.OsobaEsOrderResult.Relevance;
            Enum.TryParse<OsobaEsSearchResult.OsobaEsOrderResult>(sorder, out order);
            return GetSort(order);
        }
        public static SortDescriptor<OsobaES> GetSort(OsobaEsSearchResult.OsobaEsOrderResult order)
        {

            SortDescriptor<OsobaES> s = new SortDescriptor<OsobaES>().Field(f => f.Field("_score").Descending());
            switch (order)
            {
                case OsobaEsSearchResult.OsobaEsOrderResult.NameDesc:
                    s = new SortDescriptor<OsobaES>().Field(m => m.Field(f => f.FullName).Descending());
                    break;
                case OsobaEsSearchResult.OsobaEsOrderResult.NameAsc:
                    s = new SortDescriptor<OsobaES>().Field(m => m.Field(f => f.FullName).Ascending());
                    break;
                case OsobaEsSearchResult.OsobaEsOrderResult.FastestForScroll:
                    s = new SortDescriptor<OsobaES>().Field(f => f.Field("_doc"));
                    break;
                case OsobaEsSearchResult.OsobaEsOrderResult.Relevance:
                default:
                    break;
            }

            return s;

        }


    }
}