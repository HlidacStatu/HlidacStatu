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
                "jmenodluznik:",
                "jmenoveritel:",
                "jmenospravce:",
                "spisovaznacka:",
                "zmeneno",
                "zahajeno",
                "stav",
                "text",
                "typdokumentu","dokumenttyp"
            };
        static string[] queryOperators = new string[] { "AND", "OR" };


        public static QueryContainer GetSimpleQuery(string query)
        {
            return GetSimpleQuery(new InsolvenceSearchResult() { Q = query, Page = 1 });
        }
        public static QueryContainer GetSimpleQuery(InsolvenceSearchResult searchdata)
        {
            var query = searchdata.Q;


            string regexPrefix = @"(^|\s|[(])";
            string regexTemplate = "{0}(?<q>(-|\\w)*)\\s*";
            //fix field prefixes
            //ds: -> 
            string[,] rules = new string[,] {
                    {@"osobaid:(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?)) (\s|$){1,}","${ico}" },
                    {@"holding:(?<q>(\d{1,8})) (\s|$){1,}","${ico}" },
                    {regexPrefix + "ico:","(dluznici.iCO:${q} OR veritele.iCO:${q} OR spravci.iCO:${q}) " },
                    {regexPrefix + "icodluznik:","dluznici.iCO:" },
                    {regexPrefix + "icoveritel:","veritele.iCO:" },
                    {regexPrefix + "icospravce:","spravci.iCO:" },
                    {regexPrefix + "jmenodluznik:","dluznici.plneJmeno:" },
                    {regexPrefix + "jmenoveritel:","veritele.plneJmeno:" },
                    {regexPrefix + "jmenospravce:","spravci.plneJmeno:" },
                    {regexPrefix + "spisovaznacka:","spisovaZnacka:" },
                    {regexPrefix + "id:","spisovaZnacka:" },
                    {"zmeneno:\\[","posledniZmena:[" },
                    {"zmeneno:(?=[<>])","posledniZmena:${q}" },
                    {"zmeneno:(?=\\d)","posledniZmena:[${q} TO ${q}||+1d]" },
                    {"zahajeno:\\[","datumZalozeni:[" },
                    {"zahajeno:(?=[<>])","datumZalozeni:${q}" },
                    {"zahajeno:(?=\\d)","datumZalozeni:[${q} TO ${q}||+1d]" },
                    {regexPrefix + "stav:","stav:" },
                    {regexPrefix + "text:","dokumenty.plainText:" },
                    {regexPrefix + "typdokumentu:","dokumenty.popis:" },
                    {regexPrefix + "dokumenttyp:","dokumenty.popis:" },
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
                    } 
                    else if (lookFor.Contains("holding:"))
                    {
                        //list of ICO connected to this person
                        Match m = Regex.Match(modifiedQ, lookFor, regexQueryOption);
                        string holdingIco = m.Groups["q"].Value;
                        HlidacStatu.Lib.Data.Relation.AktualnostType aktualnost = HlidacStatu.Lib.Data.Relation.AktualnostType.Nedavny;
                        Data.Firma f = Data.Firmy.Get(holdingIco);
                        if (f != null && f.Valid)
                        {
                            var icos = new string[] { f.ICO }
                                    .Union(f.AktualniVazby(aktualnost)
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

                            var templ = "(ico:{0})";
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
                    } //do regex replace
                    else if (replaceWith.Contains("${ico}"))
                    {
                        //list of ICO connected to this person
                        Match m = Regex.Match(modifiedQ, lookFor, regexQueryOption);
                        string nameId = m.Groups["q"].Value;
                        Data.Osoba p = Data.Osoby.GetByNameId.Get(nameId);
                        string icosQuery = "";
                        if (p != null)
                        {
                            var icos = p
                                        .AktualniVazby(Data.Relation.AktualnostType.Nedavny)
                                        .Where(w => !string.IsNullOrEmpty(w.To.Id))
                                        .Where(w => Analysis.ACore.GetBasicStatisticForICO(w.To.Id).Summary.Pocet > 0)
                                        .Select(w => w.To.Id)
                                        .Distinct().ToArray();
                            var templ = "(ico:{0})";
                            if (icos != null && icos.Length > 0)
                            {
                                icosQuery = "(" + icos
                                    .Select(t => string.Format(templ, t))
                                    .Aggregate((f, s) => f + " OR " + s) + ")";
                            }
                            else
                            {
                                icosQuery = "(ico:noOne)";
                            }
                            modifiedQ = Regex.Replace(modifiedQ, lookFor, icosQuery, regexQueryOption);
                        }
                        else
                            modifiedQ = Regex.Replace(modifiedQ, lookFor, "(ico:noOne)", regexQueryOption);
                    }
                    else
                    {
                        modifiedQ = Regex.Replace(modifiedQ, lookFor, evalMatch, regexQueryOption);

                    }
                }
            }



            QueryContainer qc = null;
            if (modifiedQ == null)
                qc = new QueryContainerDescriptor<Rizeni>().MatchNone();
            else if (string.IsNullOrEmpty(modifiedQ))
                qc = new QueryContainerDescriptor<Rizeni>().MatchAll();
            else
            {
                modifiedQ = modifiedQ.Replace(" | ", " OR ").Trim();
                qc = new QueryContainerDescriptor<Rizeni>()
                    .QueryString(qs => qs
                        .Query(modifiedQ)
                        .DefaultOperator(Operator.And)
                    );
            }

            return qc;
        }


        public static InsolvenceSearchResult SimpleSearch(InsolvenceSearchResult search)
        {
            var client = Manager.GetESClient_Insolvence();
            var page = search.Page - 1 < 0 ? 0 : search.Page - 1;

            var sw = new StopWatchEx();
            sw.Start();

            search.Q = SearchTools.FixInvalidQuery(search.Q ?? "", queryShorcuts, queryOperators);

            ISearchResponse<Rizeni> res = null;
            try
            {
                res = client
                        .Search<Rizeni>(s => s
                        .Size(search.PageSize)
                        .ExpandWildcards(Elasticsearch.Net.ExpandWildcards.All)
                        .From(page * search.PageSize)
                        .Source(sr => sr.Excludes(r => r.Fields("dokumenty.*")))
                        .Query(q => GetSimpleQuery(search))
                        .Sort(ss => new SortDescriptor<Rizeni>().Field(m => m.Field(f => f.PosledniZmena).Descending()))
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

    }
}
