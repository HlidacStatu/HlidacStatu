using Devmasters.Core;
using HlidacStatu.Lib.Searching;
using HlidacStatu.Lib.Searching.Rules;
using Nest;
using System;
using System.Linq;

namespace HlidacStatu.Lib.Data.Dotace
{
    public partial class DotaceService
    {
        static string regex = "[^/]*\r\n/(?<regex>[^/]*)/\r\n[^/]*\r\n";
        static System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
                    | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        static System.Text.RegularExpressions.Regex regFindRegex = new System.Text.RegularExpressions.Regex(regex, options);


        static string[] queryOperators = new string[] { "AND", "OR" };


        static IRule[] irules = new IRule[] {
               new OsobaId("osobaid:","ico:" ),
               new Holding(null,"ico:" ),

               new TransformPrefix("ico:","prijemce.ico:",null ),
               new TransformPrefix("jmeno:","prijemce.jmeno:",null ),
               new TransformPrefix("projekt:","nazevProjektu:",null ),
                new TransformPrefix("castka:","dotaceCelkem:",null ),
                new TransformPrefixWithValue("cena:","dotaceCelkem:<=${q} ","<=\\d" ),
                new TransformPrefixWithValue("cena:","dotaceCelkem:>=${q} ",">=\\d" ),
                new TransformPrefixWithValue("cena:","dotaceCelkem:<${q} ","<\\d" ),
                new TransformPrefixWithValue("cena:","dotaceCelkem:>${q} ",">\\d" ),
                new TransformPrefixWithValue("cena:","dotaceCelkem:${q} ",null ),


            };

        private string[] queryShorcuts()
        {
            return irules.SelectMany(m => m.Prefixes).Distinct().ToArray();
        }

        public QueryContainer GetSimpleQuery(string query)
        {
            return GetSimpleQuery(new DotaceSearchResult() { Q = query, Page = 1 });
        }
        public QueryContainer GetSimpleQuery(DotaceSearchResult searchdata)
        {
            var query = searchdata.Q;

            string modifiedQ = query; // Search.Tools.FixInvalidQuery(query, queryShorcuts, queryOperators) ?? "";
                                      //check invalid query ( tag: missing value)

            //var qc  = Lib.Search.Tools.GetSimpleQuery<Lib.Data.Smlouva>(query,rules);;
            var qc = Lib.Searching.SimpleQueryCreator.GetSimpleQuery<Lib.Data.Dotace.Dotace>(query, irules);

            return qc;

        }

        public DotaceSearchResult SimpleSearch(string query, int page, int pagesize, int order,
            bool withHighlighting = false,
            AggregationContainerDescriptor<Dotace> anyAggregation = null, bool exactNumOfResults = false)
        {
            return SimpleSearch(new DotaceSearchResult()
            {
                Q = query,
                Page = page,
                PageSize = pagesize,
                Order = order.ToString(),
                ExactNumOfResults = exactNumOfResults
            }, withHighlighting, anyAggregation); ;
        }
        public DotaceSearchResult SimpleSearch(DotaceSearchResult search,
            bool withHighlighting = false,
            AggregationContainerDescriptor<Dotace> anyAggregation = null)
        {

            var page = search.Page - 1 < 0 ? 0 : search.Page - 1;

            var sw = new StopWatchEx();
            sw.Start();
            search.OrigQuery = search.Q;
            search.Q = Lib.Searching.Tools.FixInvalidQuery(search.Q ?? "", queryShorcuts(), queryOperators);

            ISearchResponse<Dotace> res = null;
            try
            {
                res = _esClient
                        .Search<Dotace>(s => s
                        .Size(search.PageSize)
                        .ExpandWildcards(Elasticsearch.Net.ExpandWildcards.All)
                        .From(page * search.PageSize)
                        .Query(q => GetSimpleQuery(search))
                        .Sort(ss => GetSort(Convert.ToInt32(search.Order)))
                        .Highlight(h => Lib.Searching.Tools.GetHighlight<Dotace>(withHighlighting))
                        .Aggregations(aggr => anyAggregation)
                        .TrackTotalHits(search.ExactNumOfResults ? true : (bool?)null)
                );
                if (res.IsValid && withHighlighting && res.Shards.Failed > 0) //if some error, do it again without highlighting
                {
                    res = _esClient
                            .Search<Dotace>(s => s
                            .Size(search.PageSize)
                            .ExpandWildcards(Elasticsearch.Net.ExpandWildcards.All)
                            .From(page * search.PageSize)
                            .Query(q => GetSimpleQuery(search))
                            .Sort(ss => GetSort(Convert.ToInt32(search.Order)))
                            .Highlight(h => Lib.Searching.Tools.GetHighlight<Dotace>(false))
                            .Aggregations(aggr => anyAggregation)
                            .TrackTotalHits(search.ExactNumOfResults ? true : (bool?)null)
                    );
                }

            }
            catch (Exception e)
            {
                Audit.Add(Audit.Operations.Search, "", "", "Dotace", "error", search.Q, null);
                if (res != null && res.ServerError != null)
                {
                    ES.Manager.LogQueryError<Dotace>(res, "Exception, Orig query:"
                        + search.OrigQuery + "   query:"
                        + search.Q
                        + "\n\n res:" + search.Result.ToString()
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
                ES.Manager.LogQueryError<Dotace>(res, "Exception, Orig query:"
                    + search.OrigQuery + "   query:"
                    + search.Q
                    + "\n\n res:" + search.Result?.ToString()
                    );
            }

            search.Total = res?.Total ?? 0;
            search.IsValid = res?.IsValid ?? false;
            search.ElasticResults = res;
            search.ElapsedTime = sw.Elapsed;
            return search;
        }

        public SortDescriptor<Dotace> GetSort(int iorder)
        {
            DotaceSearchResult.DotaceOrderResult order = (DotaceSearchResult.DotaceOrderResult)iorder;

            SortDescriptor<Dotace> s = new SortDescriptor<Dotace>().Field(f => f.Field("_score").Descending());
            switch (order)
            {
                case DotaceSearchResult.DotaceOrderResult.DateAddedDesc:
                    s = new SortDescriptor<Dotace>().Field(m => m.Field(f => f.DatumPodpisu).Descending());
                    break;
                case DotaceSearchResult.DotaceOrderResult.DateAddedAsc:
                    s = new SortDescriptor<Dotace>().Field(m => m.Field(f => f.DatumPodpisu).Ascending());
                    break;
                case DotaceSearchResult.DotaceOrderResult.LatestUpdateDesc:
                    s = new SortDescriptor<Dotace>().Field(m => m.Field(f => f.DotaceCelkem).Descending());
                    break;
                case DotaceSearchResult.DotaceOrderResult.LatestUpdateAsc:
                    s = new SortDescriptor<Dotace>().Field(m => m.Field(f => f.DotaceCelkem).Ascending());
                    break;
                case DotaceSearchResult.DotaceOrderResult.FastestForScroll:
                    s = new SortDescriptor<Dotace>().Field(f => f.Field("_doc"));
                    break;
                case DotaceSearchResult.DotaceOrderResult.ICODesc:
                    s = new SortDescriptor<Dotace>().Field(m => m.Field(f => f.Prijemce.Ico).Descending());
                    break;
                case DotaceSearchResult.DotaceOrderResult.ICOAsc:
                    s = new SortDescriptor<Dotace>().Field(m => m.Field(f => f.Prijemce.Ico).Ascending());
                    break;
                case DotaceSearchResult.DotaceOrderResult.Relevance:
                default:
                    break;
            }

            return s;

        }
    }
}
