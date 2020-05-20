using Devmasters.Core.Batch;
using HlidacStatu.Lib.Searching;
using HlidacStatu.Lib.Searching.Rules;
using HlidacStatu.Util;
using HlidacStatu.Util.Cache;
using HlidacStatu.Util;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data.Insolvence;

namespace HlidacStatu.Lib.Data
{
    public partial class Osoba
    {
        public static class Search
        {
            public const int DefaultPageSize = 40;
            public const int MaxResultWindow = 200;


            [Devmasters.Core.ShowNiceDisplayName()]
            [Devmasters.Core.Sortable(Devmasters.Core.SortableAttribute.SortAlgorithm.BySortValue)]
            public enum OrderResult
            {
                [Devmasters.Core.SortValue(0)]
                [Devmasters.Core.NiceDisplayName("podle relevance")]
                Relevance = 0,


                [Devmasters.Core.SortValue(1)]
                [Devmasters.Core.NiceDisplayName("podle abecedy")]
                NameAsc = 1,


                [Devmasters.Core.Disabled]
                FastestForScroll = 666

            }

            static string[] queryShorcuts = new string[] {
                "ico:",
                "osobaid:",
            };


            public static IRule[] irules = new IRule[] {
               new TransformPrefix("osobaid:","osobaid:",null ),

            };


            public static QueryContainer GetSimpleQuery(string query)
            {
                var qc = Lib.Searching.SimpleQueryCreator.GetSimpleQuery<Lib.Data.Smlouva>(query, irules);
                return qc;
            }


            static string regex = "[^/]*\r\n/(?<regex>[^/]*)/\r\n[^/]*\r\n";
            static System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
                        | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            static System.Text.RegularExpressions.Regex regFindRegex = new System.Text.RegularExpressions.Regex(regex, options);


            public static OsobaSearchResult SimpleSearch(string query, int page, int pageSize, string order,
                bool exactNumOfResults = false)
            {
                order = Devmasters.Core.TextUtil.NormalizeToNumbersOnly(order);
                OrderResult eorder = OrderResult.Relevance;
                System.Enum.TryParse<OrderResult>(order, out eorder);

                return SimpleSearch(query, page, pageSize, eorder,exactNumOfResults);

            }
            public static OsobaSearchResult SimpleSearch(string query, int page, int pageSize, OrderResult order
                , bool exactNumOfResults = false)
            {

                //fix without elastic
                if (page < 1)
                    page = 1;
                var takeNum = page * pageSize;
                if (takeNum > 100)
                    takeNum = 100;
                //elastik hyr
                
                List<Osoba> foundPepole = new List<Osoba>();

                string regex = @"osoba\w{0,13}:\s?(?<osoba>[\w-]{3,20})";
                List<string> peopleIds = Util.ParseTools.GetRegexGroupValues(query, regex, "osoba").ToList();
                if (peopleIds is null || peopleIds.Count == 0)
                {
                    var people = OsobyES.OsobyEsService.SimpleSearch(query, page, pageSize, ((int)order).ToString());
                    peopleIds = people.Results.Select(r => r.NameId).ToList();
                }
                
                foreach (var id in peopleIds)
                {
                    var foundPerson = Osoba.GetByNameId(id);
                    if (foundPerson != null)
                    {
                        foundPepole.Add(foundPerson);
                    }
                }

                var result = new OsobaSearchResult();
                result.Total = foundPepole.Count();
                result.Q = query;
                result.ElasticResults = null; //TODO
                result.Results = foundPepole;
                result.Page = page;
                result.PageSize = pageSize;
                result.Order = order.ToString();
                result.IsValid = true;
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
                        Lib.ES.Manager.LogQueryError<Lib.Data.Smlouva>(res,query.ToString());
                    else
                        HlidacStatu.Util.Consts.Logger.Error("", e);
                    throw;
                }

                if (res.IsValid == false && logError)
                    Lib.ES.Manager.LogQueryError<Lib.Data.Smlouva>(res, query.ToString());

                return res;
            }

            public static SortDescriptor<Osoba> GetSort(string sorder)
            {
                OrderResult order = OrderResult.Relevance;
                Enum.TryParse<OrderResult>(sorder, out order);
                return GetSort(order);
            }

            public static SortDescriptor<Data.Osoba> GetSort(OrderResult order)
            {
                SortDescriptor<Data.Osoba> s = new SortDescriptor<Data.Osoba>().Field(f => f.Field("_score").Descending());
                switch (order)
                {
                    case OrderResult.FastestForScroll:
                        s = new SortDescriptor<Data.Osoba>().Field(f => f.Field("_doc"));
                        break;
                    case OrderResult.NameAsc:
                        s = new SortDescriptor<Data.Osoba>().Field(f => f.Field(ff=>ff.Prijmeni).Ascending());
                        break;
                    case OrderResult.Relevance:
                    default:
                        break;
                }

                return s;

            }
        }
    }
}