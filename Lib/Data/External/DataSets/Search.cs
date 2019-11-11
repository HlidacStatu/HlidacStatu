using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HlidacStatu.Lib.Data.External.DataSets
{
    public static class Search
    {

        public static string GetSpecificQueriesForDataset(DataSet ds, string mappingProperty, string query, string attrNameModif="")
        {
            var props = ds.GetMappingList(mappingProperty, attrNameModif).ToArray();
            var qSearch = SearchDataQuery(props, query);
            return qSearch;
        }

        public static Dictionary<DataSet, string> GetSpecificQueriesForDatasets(string mappingProperty, string query)
        {
            Dictionary<DataSet, string> queries = new Dictionary<DataSet, string>();
            foreach (var ds in DataSetDB.ProductionDataSets.Get())
            {
                var qSearch = GetSpecificQueriesForDataset(ds, mappingProperty, query);
                if (!string.IsNullOrEmpty(qSearch))
                {
                    queries.Add(ds, qSearch);
                }
            }

            return queries;
        }

        static string[] queryOperators = new string[] {
            "AND","OR"
        };
        static string[] queryShorcuts = new string[] {
                "ico:",
                "osobaid:",
                "holding:",
                "gps_lat:",
                "gps_lng:",
                "id:",
        };

        public static string SearchDataQuery(string[] properties, string value)
        {
            if (properties == null)
                return string.Empty;
            if (properties.Count() == 0)
                return string.Empty;
            //create query
            string q = properties
                .Select(p => $"{p}:{value}")
                .Aggregate((f, s) => f + " OR " + s);
            return $"( {q} )";
        }
        public static DataSearchResult SearchData(DataSet ds, string queryString, int page, int pageSize, string sort = null, bool excludeBigProperties = true, bool withHighlighting = false)
        {
            Devmasters.Core.StopWatchEx sw = new Devmasters.Core.StopWatchEx();

            sw.Start();
            var query = Lib.Search.Tools.FixInvalidQuery(queryString, queryShorcuts, queryOperators);

            var res = _searchData(ds, query, page, pageSize, sort, excludeBigProperties, withHighlighting);

            sw.Stop();
            if (!res.IsValid)
            {
                throw DataSetException.GetExc(
                    ds.DatasetId,
                    ApiResponseStatus.InvalidSearchQuery.error.number,
                    ApiResponseStatus.InvalidSearchQuery.error.description,
                    queryString
                    );
            }

            if (res.Total > 0)
                return new DataSearchResult()
                {
                    ElapsedTime = sw.Elapsed,
                    Q = queryString,
                    IsValid = true,
                    Total = res.Total,
                    Result = res.Hits
                            .Select(m => m.Source.ToString())
                            .Select(s => (dynamic)Newtonsoft.Json.Linq.JObject.Parse(s)),                    

                    Page = page,
                    PageSize = pageSize,
                    DataSet = ds,
                    ElasticResultsRaw = res,
                };
            else
                return new DataSearchResult()
                {
                    ElapsedTime = sw.Elapsed,
                    Q = queryString,
                    IsValid = true,
                    Total = 0,
                    Result = new dynamic[] { },
                    Page = page,
                    PageSize = pageSize,
                    DataSet = ds,
                    ElasticResultsRaw = res,

                };
        }
        public static DataSearchRawResult SearchDataRaw(DataSet ds, string queryString, int page, int pageSize, string sort = null, bool excludeBigProperties = true, bool withHighlighting = false)
        {
            var query = Lib.Search.Tools.FixInvalidQuery(queryString, queryShorcuts, queryOperators);
            var res = _searchData(ds, query , page, pageSize, sort, excludeBigProperties, withHighlighting);
            if (!res.IsValid)
            {
                throw DataSetException.GetExc(ds.DatasetId,
                    ApiResponseStatus.InvalidSearchQuery.error.number,
                    ApiResponseStatus.InvalidSearchQuery.error.description,
                    queryString
                    );
            }

            if (res.Total > 0)
                return new DataSearchRawResult()
                {
                    Q = queryString,
                    IsValid = true,
                    Total = res.Total,
                    Result = res.Hits.Select(m => new Tuple<string, string>(m.Id, m.Source.ToString())),
                    Page = page,
                    PageSize = pageSize,
                    DataSet = ds,
                    ElasticResultsRaw = res,
                    Order = sort ?? "0"
                };
            else
                return new DataSearchRawResult()
                {
                    Q = queryString,
                    IsValid = true,
                    Total = 0,
                    Result = new List<Tuple<string, string>>(),
                    ElasticResultsRaw = res,
                    Page = page,
                    PageSize = pageSize,
                    DataSet = ds,
                    Order = sort ?? "0"
                };
        }

        public static ISearchResponse<object> _searchData(DataSet ds, string queryString, int page, int pageSize, string sort = null, bool excludeBigProperties = true, bool withHighlighting = false)
        {
            SortDescriptor<object> sortD = new SortDescriptor<object>();
            if (sort == "0")
                sort = null;

            if (!string.IsNullOrEmpty(sort))
            {
                if (sort.EndsWith(DataSearchResult.OrderDesc) || sort.ToLower().EndsWith(DataSearchResult.OrderDescUrl))
                {
                    sort = sort.Replace(DataSearchResult.OrderDesc, "").Replace(DataSearchResult.OrderDescUrl, "").Trim();
                    sortD = sortD.Field(sort, SortOrder.Descending);
                }
                else
                {
                    sort = sort.Replace(DataSearchResult.OrderAsc, "").Replace(DataSearchResult.OrderAscUrl, "").Trim();
                    sortD = sortD.Field(sort, SortOrder.Ascending);
                }
            }


            Nest.ElasticClient client = Lib.ES.Manager.GetESClient(ds.DatasetId, idxType: ES.Manager.IndexType.DataSource);

            QueryContainer qc = GetSimpleQuery(ds, queryString);

            //QueryContainer qc = null;
            //if (queryString == null)
            //    qc = new QueryContainerDescriptor<object>().MatchNone();
            //else if (string.IsNullOrEmpty(queryString))
            //    qc = new QueryContainerDescriptor<object>().MatchAll();
            //else
            //{
            //    qc = new QueryContainerDescriptor<object>()
            //        .QueryString(qs => qs
            //            .Query(queryString)
            //            .DefaultOperator(Operator.And)
            //        );
            //}

            page = page - 1;
            if (page < 0)
                page = 0;
            if (page * pageSize > Lib.ES.SearchTools.MaxResultWindow)
            {
                page = (Lib.ES.SearchTools.MaxResultWindow / pageSize) - 1;
            }

            //exclude big properties from result
            var maps = excludeBigProperties ? ds.GetMappingList("DocumentPlainText").ToArray() : new string[] { };



            var res = client
                .Search<object>(s => s
                    .Type("data")
                    .Size(pageSize)
                    .Source(ss => ss.Excludes(ex => ex.Fields(maps)))
                    .From(page * pageSize)
                    .Query(q => qc)
                    .Sort(ss => sortD)
                    .Highlight(h => Lib.Search.Tools.GetHighlight<Object>(withHighlighting))
            );

            return res;
        }


        static string simpleQueryOsobaPrefix = "___";
        //static RegexOptions regexQueryOption = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline;
        public static QueryContainer GetSimpleQuery(DataSet ds, string query)
        {

            var icoQuerypath = HlidacStatu.Lib.Data.External.DataSets.Search.GetSpecificQueriesForDataset(ds, "ICO", "${q}");
            var osobaIdQuerypathToIco = HlidacStatu.Lib.Data.External.DataSets.Search
                            .GetSpecificQueriesForDataset(ds, "OsobaId", "${q}")
                            + " OR ( " + simpleQueryOsobaPrefix + "osobaid" + simpleQueryOsobaPrefix + ":${v} )";

            var osobaIdQuerypath = HlidacStatu.Lib.Data.External.DataSets.Search
                .GetSpecificQueriesForDataset(ds, "OsobaId", "${q}");

            var osobaQP = "";
            if (!string.IsNullOrEmpty(icoQuerypath) && !string.IsNullOrEmpty(osobaIdQuerypathToIco))
                osobaQP = $"({icoQuerypath} OR {osobaIdQuerypathToIco})";
            else if (!string.IsNullOrEmpty(icoQuerypath))
                osobaQP = icoQuerypath;
            else if (!string.IsNullOrEmpty(osobaIdQuerypathToIco))
                osobaQP = osobaIdQuerypathToIco;

            Lib.Search.Rule[] rules = new Lib.Search.Rule[] {
                    new Lib.Search.Rule(@"osobaid:(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?)) ", "ico"){ 
                        AddLastCondition = simpleQueryOsobaPrefix + "osobaid" + simpleQueryOsobaPrefix + ":${q}" 
                    },
                    new Lib.Search.Rule(@"holding:(?<q>(\d{1,8})) ",icoQuerypath ),
                    new Lib.Search.Rule("ico:",icoQuerypath ),
                    new Lib.Search.Rule(simpleQueryOsobaPrefix+@"osobaid" + simpleQueryOsobaPrefix + @":(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?)) ", osobaIdQuerypath,false),
                };


            var qc = Lib.Search.Tools.GetSimpleQuery<object>(query, rules);

            return qc;
        }

    }
}
