using Devmasters.Core;
using HlidacStatu.Lib.ES;
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
        static RegexOptions regexQueryOption = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline;

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
            string[,] rules = new string[,] {
                    {@"osobaid:(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?)) ","ico" },
                    {@"holding:(?<q>(\d{1,8})) (\s|$){1,}","ico" },

                    {@"osobaiddluznik:(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?)) ","icodluznik" },
                    {@"osobaidveritel:(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?)) ","icoveritel" },
                    {@"osobaidspravce:(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?)) ","icospravce" },

                    {"ico:","(dluznici.iCO:${q} OR veritele.iCO:${q} OR spravci.iCO:${q}) " },
                    {"icodluznik:","dluznici.iCO:" },
                    {"icoveritel:","veritele.iCO:" },
                    {"icospravce:","spravci.iCO:" },
                    {"jmeno:","(dluznici.plneJmeno:${q} OR veritele.plneJmeno:${q} OR spravci.plneJmeno:${q})" },
                    {"jmenodluznik:","dluznici.plneJmeno:" },
                    {"jmenoveritel:","veritele.plneJmeno:" },
                    {"jmenospravce:","spravci.plneJmeno:" },
                    {"spisovaznacka:","spisovaZnacka:" },
                    {"id:","spisovaZnacka:" },
                    {"zmeneno:\\[","posledniZmena:[" },
                    {"zmeneno:(?=[<>])","posledniZmena:${q}" },
                    {"zmeneno:(?=\\d)","posledniZmena:[${q} TO ${q}||+1d]" },
                    {"zahajeno:\\[","datumZalozeni:[" },
                    {"zahajeno:(?=[<>])","datumZalozeni:${q}" },
                    {"zahajeno:(?=\\d)","datumZalozeni:[${q} TO ${q}||+1d]" },
                    {"stav:","stav:" },
                    {"text:","dokumenty.plainText:" },
                    {"typdokumentu:","dokumenty.popis:" },
                    {"dokumenttyp:","dokumenty.popis:" },
                    {"oddil:","dokumenty.oddil:" },
            };

            string modifiedQ = query; // Search.Tools.FixInvalidQuery(query, queryShorcuts, queryOperators) ?? "";
                                      //check invalid query ( tag: missing value)

            if (searchdata.LimitedView)
                modifiedQ = Lib.ES.SearchTools.ModifyQuery(modifiedQ, "onRadar:true");

            var qc = Lib.Search.Tools.GetSimpleQuery<Lib.Data.Insolvence.Rizeni>(modifiedQ, rules); ;

            return qc;

        }


        public static InsolvenceSearchResult SimpleSearch(string query, int page, int pagesize, int order,
            bool withHighlighting = false,
            bool limitedView = true,
            AggregationContainerDescriptor<Lib.Data.Insolvence.Rizeni> anyAggregation = null)
        {
            return SimpleSearch(new InsolvenceSearchResult()
            {
                Q = query,
                Page = page,
                PageSize = pagesize,
                LimitedView = limitedView,
                Order = order.ToString()
            }, withHighlighting, anyAggregation); ;
        }
        public static InsolvenceSearchResult SimpleSearch(InsolvenceSearchResult search,
            bool withHighlighting = false,
            AggregationContainerDescriptor<Lib.Data.Insolvence.Rizeni> anyAggregation = null)
        {
            var client = Manager.GetESClient_Insolvence();
            var page = search.Page - 1 < 0 ? 0 : search.Page - 1;

            var sw = new StopWatchEx();
            sw.Start();
            search.OrigQuery = search.Q;
            search.Q = Lib.Search.Tools.FixInvalidQuery(search.Q ?? "", queryShorcuts, queryOperators);

            ISearchResponse<Rizeni> res = null;
            try
            {
                res = client
                        .Search<Rizeni>(s => s
                        .Size(search.PageSize)
                        .ExpandWildcards(Elasticsearch.Net.ExpandWildcards.All)
                        .From(page * search.PageSize)
                        .Source(sr => sr.Excludes(r => r.Fields("dokumenty.plainText")))
                        .Query(q => GetSimpleQuery(search))
                        //.Sort(ss => new SortDescriptor<Rizeni>().Field(m => m.Field(f => f.PosledniZmena).Descending()))
                        .Sort(ss => GetSort(Convert.ToInt32(search.Order)))
                        .Highlight(h => Lib.Search.Tools.GetHighlight<Rizeni>(withHighlighting))
                        .Aggregations(aggr => anyAggregation)
                );
            }
            catch (Exception e)
            {
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

        public static SortDescriptor<Data.Insolvence.Rizeni> GetSort(int iorder)
        {
            ES.InsolvenceSearchResult.InsolvenceOrderResult order = (ES.InsolvenceSearchResult.InsolvenceOrderResult)iorder;

            SortDescriptor<Data.Insolvence.Rizeni> s = new SortDescriptor<Data.Insolvence.Rizeni>().Field(f => f.Field("_score").Descending());
            switch (order)
            {
                case ES.InsolvenceSearchResult.InsolvenceOrderResult.DateAddedDesc:
                    s = new SortDescriptor<Data.Insolvence.Rizeni>().Field(m => m.Field(f => f.DatumZalozeni).Descending());
                    break;
                case ES.InsolvenceSearchResult.InsolvenceOrderResult.DateAddedAsc:
                    s = new SortDescriptor<Data.Insolvence.Rizeni>().Field(m => m.Field(f => f.DatumZalozeni).Ascending());
                    break;
                case ES.InsolvenceSearchResult.InsolvenceOrderResult.LatestUpdateDesc:
                    s = new SortDescriptor<Data.Insolvence.Rizeni>().Field(m => m.Field(f => f.PosledniZmena).Descending());
                    break;
                case ES.InsolvenceSearchResult.InsolvenceOrderResult.LatestUpdateAsc:
                    s = new SortDescriptor<Data.Insolvence.Rizeni>().Field(m => m.Field(f => f.PosledniZmena).Ascending());
                    break;
                case ES.InsolvenceSearchResult.InsolvenceOrderResult.FastestForScroll:
                    s = new SortDescriptor<Data.Insolvence.Rizeni>().Field(f => f.Field("_doc"));
                    break;
                case ES.InsolvenceSearchResult.InsolvenceOrderResult.Relevance:
                default:
                    break;
            }

            return s;

        }

    }
}
