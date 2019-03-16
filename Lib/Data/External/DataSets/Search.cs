using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HlidacStatu.Lib.Data.External.DataSets
{
    public static class Search
    {

        public static string GetSpecificQueriesForDataset(DataSet ds, string mappingProperty, string query)
        {
            var props = ds.GetMappingList(mappingProperty).ToArray();
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
        public static DataSearchResult SearchData(DataSet ds, string queryString, int page, int pageSize, string sort = null)
        {
            Devmasters.Core.StopWatchEx sw = new Devmasters.Core.StopWatchEx();

            sw.Start();
            var query = Lib.ES.SearchTools.FixInvalidQuery(queryString, queryShorcuts, queryOperators);

            var res = _searchData(ds, query, page, pageSize, sort);

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
                };
        }
        public static DataSearchRawResult SearchDataRaw(DataSet ds, string queryString, int page, int pageSize, string sort = null)
        {
            var query = Lib.ES.SearchTools.FixInvalidQuery(queryString, queryShorcuts, queryOperators);
            var res = _searchData(ds, queryString, page, pageSize, sort);
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
                    Order = sort ?? "0"
                };
            else
                return new DataSearchRawResult()
                {
                    Q = queryString,
                    IsValid = true,
                    Total = 0,
                    Result = new List<Tuple<string, string>>(),
                    Page = page,
                    PageSize = pageSize,
                    DataSet = ds,
                    Order = sort ?? "0"
                };
        }

        public static ISearchResponse<object> _searchData(DataSet ds, string queryString, int page, int pageSize, string sort = null)
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


            var res = client
                .Search<object>(s => s
                    .Type("data")
                    .Size(pageSize)
                    .From(page * pageSize)
                    .Query(q => qc)
                    .Sort(ss => sortD)
            );

            return res;
        }


        static RegexOptions regexQueryOption = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline;
        public static QueryContainer GetSimpleQuery(DataSet ds, string origQuery)
        {
            if (origQuery == null)
                return new QueryContainerDescriptor<object>().MatchNone();

            var query = origQuery.Trim();
            if (string.IsNullOrEmpty(query) || query == "*")
                return new QueryContainerDescriptor<object>().MatchAll();


            string regexPrefix = @"(^|\s|[(])";
            string regexTemplate = "{0}(?<q>(-|\\w)*)\\s*";
            //fix field prefixes
            //ds: -> 
            string[,] rules = new string[,] {
                    {regexPrefix + "ico:","${ico}" },
                    {@"osobaid:(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?)) (\s|$){1,}","${ico}" },
                    {@"holding:(?<q>(\d{1,8})) (\s|$){1,}","${ico}" },
                };


            string modifiedQ = query; // ES.SearchTools.FixInvalidQuery(query, queryShorcuts, queryOperators) ?? "";
                                      //check invalid query ( tag: missing value)



            for (int i = 0; i < rules.GetLength(0); i++)
            {
                string lookFor = rules[i, 0];
                string replaceWith = rules[i, 1];

                MatchEvaluator evalMatch = (m) =>
                {
                    var s = m.Value;
                    if (string.IsNullOrEmpty(s))
                        return string.Empty;
                    var newVal = replaceWith;
                    if (newVal.Contains("${q}"))
                        newVal = newVal.Replace("${q}", m.Groups["q"].Value);
                    if (s.StartsWith("("))
                        return " (" + newVal;
                    else
                        return " " + newVal;
                };

                //if (modifiedQ.ToLower().Contains(lookFor.ToLower()))
                if (Regex.IsMatch(modifiedQ, lookFor, regexQueryOption))
                {
                    if (replaceWith.Contains("${q}"))
                    {
                        modifiedQ = Regex.Replace(modifiedQ, string.Format(regexTemplate, lookFor), evalMatch, regexQueryOption);
                    } //do regex replace
                      //else if (replaceWith.Contains("${zahajeny}") && )
                      //{
                      //    modifiedQ = Regex.Replace(modifiedQ, lookFor, "stavVZ:<=100", regexQueryOption);
                      //} //do regex replace
                    else if (lookFor.Contains("holding:"))
                    {
                        //list of ICO connected to this person
                        Match m = Regex.Match(modifiedQ, lookFor, regexQueryOption);
                        string holdingIco = m.Groups["q"].Value;
                        HlidacStatu.Lib.Data.Relation.AktualnostType aktualnost = HlidacStatu.Lib.Data.Relation.AktualnostType.Nedavny;

                        var icoQuerypath = HlidacStatu.Lib.Data.External.DataSets.Search.GetSpecificQueriesForDataset(ds, "ICO", "{0}");
                        if (!string.IsNullOrEmpty(icoQuerypath))
                        {
                            Data.Firma f = Data.Firmy.Get(holdingIco);
                            if (f != null && f.Valid)
                            {
                                var icos = new string[] { f.ICO }
                                        .Union(
                                            f.AktualniVazby(aktualnost)
                                            .Select(s => s.To.Id)
                                        )
                                        .Distinct();
                                string icosQuery = "";
                                var icosPresLidi = f.AktualniVazby(aktualnost)
                                        .Where(o => o.To.Type == Graph.Node.NodeType.Person)
                                        .Select(o => Data.Osoby.GetById.Get(Convert.ToInt32(o.To.Id)))
                                        .Where(o => o != null)
                                        .SelectMany(o => o.AktualniVazby(aktualnost))
                                        .Select(v => v.To.Id)
                                        .Distinct();
                                icos = icos.Union(icosPresLidi).Distinct();


                                var icoquerypath = HlidacStatu.Lib.Data.External.DataSets.Search.GetSpecificQueriesForDataset(ds, "ICO", "{0}");

                                var templ = "(ico:{0})";
                                if (!string.IsNullOrEmpty(icoquerypath))
                                    templ = icoquerypath;

                                if (icos != null && icos.Count() > 0)
                                {
                                    icosQuery = "(" + icos
                                        .Select(t => string.Format(templ, t))
                                        .Aggregate((fi, s) => fi + " OR " + s) + ")";
                                }
                                else
                                {
                                    icosQuery = "(ico:noOne)";
                                }
                                modifiedQ = Regex.Replace(modifiedQ, lookFor, icosQuery, regexQueryOption);

                            }
                        }
                    } //do regex replace
                    else if (lookFor.Contains("osobaid:"))
                    {
                        //list of ICO connected to this person
                        Match m = Regex.Match(modifiedQ, lookFor, regexQueryOption);
                        string nameId = m.Groups[1].Value;


                        var osobaidQuerypath = HlidacStatu.Lib.Data.External.DataSets.Search.GetSpecificQueriesForDataset(ds, "OsobaId", "{0}");
                        var icoQuerypath = HlidacStatu.Lib.Data.External.DataSets.Search.GetSpecificQueriesForDataset(ds, "ICO", "{0}");
                        if (!string.IsNullOrEmpty(osobaidQuerypath) || !string.IsNullOrEmpty(icoQuerypath))
                        {
                            Data.Osoba p = Data.Osoby.GetByNameId.Get(nameId);
                            string icosQuery = "";
                            if (p != null)
                            {
                                var icos = p
                                            .AktualniVazby(Data.Relation.AktualnostType.Nedavny)
                                            .Where(w => !string.IsNullOrEmpty(w.To.Id))
                                            //.Where(w => Analysis.ACore.GetBasicStatisticForICO(w.To.Id).Summary.Pocet > 0)
                                            .Select(w => w.To.Id)
                                            .Distinct().ToArray();
                                if (icos != null && icos.Length > 0 && !string.IsNullOrEmpty(icoQuerypath))
                                {
                                    icosQuery = "(" + icos
                                        .Select(t => string.Format(icoQuerypath, t))
                                        .Aggregate((f, s) => f + " OR " + s) + ")";
                                }
                                if (!string.IsNullOrEmpty(osobaidQuerypath))
                                {
                                    if (icosQuery.Length > 0)
                                        icosQuery = icosQuery + " OR ";
                                    icosQuery = icosQuery + string.Format(osobaidQuerypath, nameId);
                                }
                                modifiedQ = Regex.Replace(modifiedQ, lookFor, icosQuery.Trim(), regexQueryOption).Trim();
                                if (modifiedQ == "")
                                    return new QueryContainerDescriptor<object>().MatchNone();

                            }
                        }
                    }
                    else if (lookFor.Contains("ico:"))
                    {
                        //list of ICO connected to this person
                        var ico = HlidacStatu.Util.ParseTools.GetRegexGroupValue(modifiedQ, @"ico:(?<ico>\d*)", "ico");


                        //string ico = m.Groups[1].Value;
                        string icosQuery = "";
                        var icoQuerypath = HlidacStatu.Lib.Data.External.DataSets.Search.GetSpecificQueriesForDataset(ds, "ICO", "{0}");
                        if (!string.IsNullOrEmpty(icoQuerypath))
                        {
                            var templ = "(ico:{0})";
                            if (!string.IsNullOrEmpty(icoQuerypath))
                                templ = icoQuerypath;

                            icosQuery = string.Format(templ, ico).Trim();
                            modifiedQ = Regex.Replace(modifiedQ, @"ico:(?<ico>\d*)", icosQuery, regexQueryOption);
                        }
                    }
                    else
                    {
                        modifiedQ = Regex.Replace(modifiedQ, lookFor, evalMatch, regexQueryOption);

                    }
                }
            }


            QueryContainer qc = null;
            if (modifiedQ == null)
                qc = new QueryContainerDescriptor<VZ.VerejnaZakazka>().MatchNone();
            else if (string.IsNullOrEmpty(modifiedQ))
                qc = new QueryContainerDescriptor<VZ.VerejnaZakazka>().MatchAll();
            else
            {
                modifiedQ = modifiedQ.Replace(" | ", " OR ").Trim();
                qc = new QueryContainerDescriptor<VZ.VerejnaZakazka>()
                            .QueryString(qs => qs
                                .Query(modifiedQ)
                                .DefaultOperator(Operator.And)
                            );
            }

            return qc;
        }

    }
}
