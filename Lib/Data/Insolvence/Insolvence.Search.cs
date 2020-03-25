using Devmasters.Core;
using HlidacStatu.Lib.ES;
using HlidacStatu.Lib.Searching;
using HlidacStatu.Lib.Searching.Rules;
using Nest;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace HlidacStatu.Lib.Data.Insolvence
{
    public partial class Insolvence
    {
        static string regex = "[^/]*\r\n/(?<regex>[^/]*)/\r\n[^/]*\r\n";
        static System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
                    | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        static System.Text.RegularExpressions.Regex regFindRegex = new System.Text.RegularExpressions.Regex(regex, options);

        static string[] queryShorcuts = new string[] {
                "ico:",
                "icodluznik:",
                "icoveritel:",
                "icospravce:",
                "jmeno:",
                "jmenodluznik:",
                "jmenoveritel:",
                "jmenospravce:",
                "spisovaznacka:",
                "zmeneno:",
                "zahajeno:",
                "stav:",
                "text:",
                "typdokumentu:","dokumenttyp:",
                "osobaid:","holding:",
                "osobaiddluznik:","holdingdluznik:",
                "osobaidveritel:","holdingveritel:",
                "osobaidspravce:","holdingspravce:",
                "id:"
            };
        static string[] queryOperators = new string[] { "AND", "OR" };


        public static QueryContainer GetSimpleQuery(string query)
        {
            return GetSimpleQuery(new InsolvenceSearchResult() { Q = query, Page = 1 });
        }
        public static QueryContainer GetSimpleQuery(InsolvenceSearchResult searchdata)
        {
            var query = searchdata.Q;


            //fix field prefixes
            //ds: -> 
            Lib.Searching.Rule[] rules = new Lib.Searching.Rule[] {
                   new Lib.Searching.Rule(@"osobaid:(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?)) ","ico" ),
                   new Lib.Searching.Rule(@"osobaiddluznik:(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?)) ","icodluznik" ),
                   new Lib.Searching.Rule(@"osobaidveritel:(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?)) ","icoveritel" ),
                   new Lib.Searching.Rule(@"osobaidspravce:(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?)) ","icospravce" ),

                   new Lib.Searching.Rule(@"holding:(?<q>(\d{1,8})) (\s|$){1,}","ico" ),
                   new Lib.Searching.Rule(@"holdindluznik:(?<q>(\d{1,8})) (\s|$){1,}","icodluznik" ),
                   new Lib.Searching.Rule(@"holdingveritel:(?<q>(\d{1,8})) (\s|$){1,}","icoveritel" ),
                   new Lib.Searching.Rule(@"holdingspravce:(?<q>(\d{1,8})) (\s|$){1,}","icospravce" ),

                   new Lib.Searching.Rule("ico:","(dluznici.iCO:${q} OR veritele.iCO:${q} OR spravci.iCO:${q}) " ),
                   new Lib.Searching.Rule("icodluznik:","dluznici.iCO:" ),
                   new Lib.Searching.Rule("icoveritel:","veritele.iCO:" ),
                   new Lib.Searching.Rule("icospravce:","spravci.iCO:" ),
                   new Lib.Searching.Rule("jmeno:","(dluznici.plneJmeno:${q} OR veritele.plneJmeno:${q} OR spravci.plneJmeno:${q})" ),
                   new Lib.Searching.Rule("jmenodluznik:","dluznici.plneJmeno:" ),
                   new Lib.Searching.Rule("jmenoveritel:","veritele.plneJmeno:" ),
                   new Lib.Searching.Rule("jmenospravce:","spravci.plneJmeno:" ),
                   new Lib.Searching.Rule("spisovaznacka:","spisovaZnacka:" ),
                   new Lib.Searching.Rule("id:","spisovaZnacka:" ),
                   new Lib.Searching.Rule("zmeneno:\\[","posledniZmena:[" ),
                   new Lib.Searching.Rule("zmeneno:(?=[<>])","posledniZmena:${q}" ),
                   new Lib.Searching.Rule("zmeneno:(?=\\d)","posledniZmena:[${q} TO ${q}||+1d}" ),
                   new Lib.Searching.Rule("zahajeno:\\[","datumZalozeni:[" ),
                   new Lib.Searching.Rule("zahajeno:(?=[<>])","datumZalozeni:${q}" ),
                   new Lib.Searching.Rule("zahajeno:(?=\\d)","datumZalozeni:[${q} TO ${q}||+1d}" ),
                   new Lib.Searching.Rule("stav:","stav:" ),
                   new Lib.Searching.Rule("text:","dokumenty.plainText:" ),
                   new Lib.Searching.Rule("texttypdokumentu:","dokumenty.popis:" ),
                   new Lib.Searching.Rule("typdokumentu:","dokumenty.typUdalosti:" ),
                   new Lib.Searching.Rule("oddil:","dokumenty.oddil:" ),
            };

            IRule[] irules = new IRule[] {
                    new OsobaId("osobaid:","ico:" ),
                    new OsobaId("osobaiddluznik:","icodluznik:" ),
                    new OsobaId("osobaidveritel:","icoveritel:" ),
                    new OsobaId("osobaidspravce:","icospravce:" ),

                    new Holding("holding:","ico:" ),
                    new Holding("holdindluznik:","icoplatce:" ),
                    new Holding("holdingveritel:","icoveritel:" ),
                    new Holding("holdingspravce:","icospravce:" ),

                    new TransformPrefixWithValue("ico:","(dluznici.iCO:${q} OR veritele.iCO:${q} OR spravci.iCO:${q}) ",null ),
                    new TransformPrefixWithValue("jmeno:","(dluznici.plneJmeno:${q} OR veritele.plneJmeno:${q} OR spravci.plneJmeno:${q})",null ),

                    new TransformPrefix("icodluznik:","dluznici.iCO:",null ),
                    new TransformPrefix("icoveritel:","veritele.iCO:",null ),
                    new TransformPrefix("icospravce:","spravci.iCO:" ,null),
                    new TransformPrefix("jmenodluznik:","dluznici.plneJmeno:",null ),
                    new TransformPrefix("jmenoveritel:","veritele.plneJmeno:" ,null),
                    new TransformPrefix("jmenospravce:","spravci.plneJmeno:" ,null),
                    new TransformPrefix("spisovaznacka:","spisovaZnacka:" ,null),
                    new TransformPrefix("id:","spisovaZnacka:" ,null),
                
                    new TransformPrefix("zmeneno:","posledniZmena:", "[<>]?[{\\[]+" ),
                    new TransformPrefixWithValue("zmeneno:","posledniZmena:[${q} TO ${q}||+1d}", "\\d+" ),
                    new TransformPrefix("zahajeno:","datumZalozeni:", "[<>]?[{\\[]+" ),
                    new TransformPrefixWithValue("zahajeno:","datumZalozeni:[${q} TO ${q}||+1d}", "\\d+" ),

                    new TransformPrefix("stav:","stav:"  ,null),
                    new TransformPrefix("text:","dokumenty.plainText:"  ,null),
                    new TransformPrefix("texttypdokumentu:","dokumenty.popis:" ,null ),
                    new TransformPrefix("typdokumentu:","dokumenty.typUdalosti:"  ,null),
                    new TransformPrefix("oddil:","dokumenty.oddil:"  ,null),


            };


            string modifiedQ = query; // Search.Tools.FixInvalidQuery(query, queryShorcuts, queryOperators) ?? "";
                                      //check invalid query ( tag: missing value)

            if (searchdata.LimitedView)
                modifiedQ = Lib.Searching.Tools.ModifyQueryAND(modifiedQ, "onRadar:true");

            //var qc = Lib.Search.Tools.GetSimpleQuery<Lib.Data.Insolvence.Rizeni>(modifiedQ, rules); ;
            var qc = Lib.Searching.SimpleQueryCreator.GetSimpleQuery<Lib.Data.Insolvence.Rizeni>(modifiedQ, irules);

            return qc;

        }


        public static InsolvenceSearchResult SimpleSearch(string query, int page, int pagesize, int order,
            bool withHighlighting = false,
            bool limitedView = true,
            AggregationContainerDescriptor<Lib.Data.Insolvence.Rizeni> anyAggregation = null, bool exactNumOfResults = false)
        {
            return SimpleSearch(new InsolvenceSearchResult()
            {
                Q = query,
                Page = page,
                PageSize = pagesize,
                LimitedView = limitedView,
                Order = order.ToString(),
                ExactNumOfResults = exactNumOfResults
            }, withHighlighting, anyAggregation); ;
        }
        public static InsolvenceSearchResult SimpleSearch(InsolvenceSearchResult search,
            bool withHighlighting = false,
            AggregationContainerDescriptor<Lib.Data.Insolvence.Rizeni> anyAggregation = null, bool exactNumOfResults = false)
        {
            var client = Manager.GetESClient_Insolvence();
            var page = search.Page - 1 < 0 ? 0 : search.Page - 1;

            var sw = new StopWatchEx();
            sw.Start();
            search.OrigQuery = search.Q;
            search.Q = Lib.Searching.Tools.FixInvalidQuery(search.Q ?? "", queryShorcuts, queryOperators);
            var sq = GetSimpleQuery(search);
            
            ISearchResponse<Rizeni> res = null;
            try
            {
                res = client
                        .Search<Rizeni>(s => s
                        .Size(search.PageSize)
                        .ExpandWildcards(Elasticsearch.Net.ExpandWildcards.All)
                        .From(page * search.PageSize)
                        .Source(sr => sr.Excludes(r => r.Fields("dokumenty.plainText")))
                        .Query(q => sq)
                        //.Sort(ss => new SortDescriptor<Rizeni>().Field(m => m.Field(f => f.PosledniZmena).Descending()))
                        .Sort(ss => GetSort(search.Order))
                        .Highlight(h => Lib.Searching.Tools.GetHighlight<Rizeni>(withHighlighting))
                        .Aggregations(aggr => anyAggregation)
                        .TrackTotalHits(search.ExactNumOfResults ? true : (bool?)null)
                );
                if (withHighlighting && res.Shards != null && res.Shards.Failed > 0) //if some error, do it again without highlighting
                {
                    res = client
                        .Search<Rizeni>(s => s
                        .Size(search.PageSize)
                        .ExpandWildcards(Elasticsearch.Net.ExpandWildcards.All)
                        .From(page * search.PageSize)
                        .Source(sr => sr.Excludes(r => r.Fields("dokumenty.plainText")))
                        .Query(q => sq)
                        //.Sort(ss => new SortDescriptor<Rizeni>().Field(m => m.Field(f => f.PosledniZmena).Descending()))
                        .Sort(ss => GetSort(search.Order))
                        .Highlight(h => Lib.Searching.Tools.GetHighlight<Rizeni>(false))
                        .Aggregations(aggr => anyAggregation)
                        .TrackTotalHits(search.ExactNumOfResults ? true : (bool?)null)
                );
                }
                }
            catch (Exception e)
            {
                Audit.Add(Audit.Operations.Search, "", "", "Insolvence", "error", search.Q, null);
                if (res != null && res.ServerError != null)
                {
                    Manager.LogQueryError<Rizeni>(res, "Exception, Orig query:"
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
            Audit.Add(Audit.Operations.Search, "", "", "Insolvence", res.IsValid ? "valid" : "invalid", search.Q, null);

            if (res.IsValid == false)
            {
                Manager.LogQueryError<Rizeni>(res, "Exception, Orig query:"
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

        public static SortDescriptor<Rizeni> GetSort(string sorder)
        {
            Searching.InsolvenceSearchResult.InsolvenceOrderResult order = Searching.InsolvenceSearchResult.InsolvenceOrderResult.Relevance;
            Enum.TryParse<InsolvenceSearchResult.InsolvenceOrderResult>(sorder, out order);
            return GetSort(order);
        }
        public static SortDescriptor<Rizeni> GetSort(InsolvenceSearchResult.InsolvenceOrderResult order)
        {

            SortDescriptor<Data.Insolvence.Rizeni> s = new SortDescriptor<Data.Insolvence.Rizeni>().Field(f => f.Field("_score").Descending());
            switch (order)
            {
                case Searching.InsolvenceSearchResult.InsolvenceOrderResult.DateAddedDesc:
                    s = new SortDescriptor<Data.Insolvence.Rizeni>().Field(m => m.Field(f => f.DatumZalozeni).Descending());
                    break;
                case Searching.InsolvenceSearchResult.InsolvenceOrderResult.DateAddedAsc:
                    s = new SortDescriptor<Data.Insolvence.Rizeni>().Field(m => m.Field(f => f.DatumZalozeni).Ascending());
                    break;
                case Searching.InsolvenceSearchResult.InsolvenceOrderResult.LatestUpdateDesc:
                    s = new SortDescriptor<Data.Insolvence.Rizeni>().Field(m => m.Field(f => f.PosledniZmena).Descending());
                    break;
                case Searching.InsolvenceSearchResult.InsolvenceOrderResult.LatestUpdateAsc:
                    s = new SortDescriptor<Data.Insolvence.Rizeni>().Field(m => m.Field(f => f.PosledniZmena).Ascending());
                    break;
                case Searching.InsolvenceSearchResult.InsolvenceOrderResult.FastestForScroll:
                    s = new SortDescriptor<Data.Insolvence.Rizeni>().Field(f => f.Field("_doc"));
                    break;
                case Searching.InsolvenceSearchResult.InsolvenceOrderResult.Relevance:
                default:
                    break;
            }

            return s;

        }

    }
}
