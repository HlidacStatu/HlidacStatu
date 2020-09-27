using System;
using HlidacStatu.Util;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nest;
using Devmasters.Batch;
using System.Threading;

namespace HlidacStatu.Lib.Searching
{
    public static class Tools
    {
        public const int MaxResultWindow = 10000;

        static string regexInvalidQueryTemplate = @"(^|\s|[(])(?<q>$operator$\s{1} (?<v>(\w{1,})) )($|\s|[)])";
        static RegexOptions regexQueryOption = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline;

        public static readonly string[] DefaultQueryOperators = new string[] { "AND", "OR" };


        public static HighlightDescriptor<T> GetHighlight<T>(bool enable)
            where T : class
        {
            HighlightDescriptor<T> hh = new HighlightDescriptor<T>();
            if (enable)
                hh = hh.Order(HighlighterOrder.Score)
                        .PreTags("<highl>")
                        .PostTags("</highl>")
                        .Fields(ff => ff
                                    .Field("*")
                                    .RequireFieldMatch(false)
                                    .Type(HighlighterType.Unified)
                                    .FragmentSize(100)
                                    .NumberOfFragments(3)
                                    .Fragmenter(HighlighterFragmenter.Span)
                                    .BoundaryScanner(BoundaryScanner.Sentence)
                                    .BoundaryScannerLocale("cs_CZ")
                        );
            return hh;
        }
        public static string FixInvalidQuery(string query, Rules.IRule[] rules, string[] operators = null)
        {
            return FixInvalidQuery(query, rules.SelectMany(m => m.Prefixes).ToArray(), operators);
        }
        public static string FixInvalidQuery(string query, string[] shortcuts, string[] operators = null)
        {
            if (operators == null)
                operators = DefaultQueryOperators;

            if (string.IsNullOrEmpty(query))
                return query;

            string newquery = query;

            //fix query ala (issues.issueTypeId:18+OR+issues.issueTypeId:12)+ico:00283924
            if (!string.IsNullOrEmpty(query))
            {
                query = query.Trim();
                if (query.Contains("+") && !query.Contains(" "))
                    newquery = System.Net.WebUtility.UrlDecode(query);
            }

            MatchEvaluator evalDelimiterMatch = (m) =>
            {
                var s = m.Value;
                if (string.IsNullOrEmpty(s))
                    return string.Empty;
                if (m.Groups["v"].Success)
                {
                    var v = m.Groups["v"]?.Value?.ToUpper()?.Trim() ?? "";
                    if (operators.Contains(v))
                        return s;
                }
                var newVal = s.Replace(": ", ":");
                return newVal;
            };

            foreach (var qo in shortcuts)
            {
                string lookFor = regexInvalidQueryTemplate.Replace("$operator$", qo);
                //if (modifiedQ.ToLower().Contains(lookFor.ToLower()))
                if (Regex.IsMatch(newquery, lookFor, regexQueryOption))
                {
                    newquery = Regex.Replace(newquery, lookFor, evalDelimiterMatch, regexQueryOption);
                }
            }


            string invalidFormatRegex = @"(AND \s+)? ([^""]\w +\.?\w +) :($|[^\""=><[{A-Za-z0-9_)]) (AND )?";
            Match mIsInvalid = Regex.Match(query, invalidFormatRegex, (RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase));
            if (mIsInvalid.Success)
            {
                newquery = Regex.Replace(newquery, invalidFormatRegex, " ", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase).Trim();
            }

            var textParts = Devmasters.TextUtil.SplitStringToPartsWithQuotes(newquery, '\"');
            //make operator UpperCase and space around '(' and ')'
            if (textParts.Count > 0)
            {
                string fixedOperator = "";
                foreach (var tp in textParts)
                {
                    if (tp.Item2 == true)
                        fixedOperator = fixedOperator + tp.Item1;
                    else
                    {
                        var fixPart = tp.Item1;
                        fixPart = System.Net.WebUtility.UrlDecode(fixPart);
                        fixPart = System.Net.WebUtility.HtmlDecode(fixPart);
                        Regex opReg = new Regex(string.Format(@"(^|\s)({0})(\s|$)", operators.Aggregate((f, s) => f + "|" + s)), regexQueryOption);

                        //UPPER Operator
                        fixPart = opReg.Replace(fixPart, (me) => { return me.Value.ToUpper(); });

                        //Space around '(' and ')'
                        fixPart = fixPart.Replace("(", "( ").Replace(")", " )");
                        fixedOperator = fixedOperator + fixPart;
                    }
                }
                newquery = fixedOperator;
            }

            //fix DÚK/Sou/059/2009  -> DÚK\\/Sou\\/059\\/2009
            //
            // but support regex name:/joh?n(ath[oa]n)/
            //
            //if (newquery.Contains("/")) //&& regFindRegex.Match(query).Success == false)
            //{
            //    newquery = newquery.Replace("/", "\\/");
            //}

            //fix with small "to" in zverejneno:[2018-12-13 to *]
            var dateintervalRegex = @"(podepsano|zverejneno):({|\[)\d{4}\-\d{2}\-\d{2}(?<to> \s*to\s*)(\d{4}-\d{2}-\d{2}|\*)(}|\])";
            if (Regex.IsMatch(newquery, dateintervalRegex, regexQueryOption))
            {
                newquery = newquery.ReplaceGroupMatchNameWithRegex(dateintervalRegex, "to", " TO ");
            }


            if (newquery != query)
                return newquery;
            else
                return query;
        }

        public static bool ValidateQuery<T>(Nest.ElasticClient client, QueryContainer qc)
       where T : class
        {
            return ValidateSpecificQueryRaw<T>(client, qc)?.Valid ?? false;
        }

        public static bool ValidateQuery(string query)
        {
            return ValidateQueryRaw(query)?.Valid ?? false;
        }
        public static ValidateQueryResponse ValidateQueryRaw(string query)
        {
            return ValidateSpecificQueryRaw<Lib.Data.Smlouva>(Lib.ES.Manager.GetESClient(),
                Data.Smlouva.Search.GetSimpleQuery(query));
        }


        public static ValidateQueryResponse ValidateSpecificQueryRaw<T>(Nest.ElasticClient client, QueryContainer qc)
        where T : class
        {
            var res = client.Indices
                .ValidateQuery<T>(v => v
                    .Query(q => qc)
                );

            return res;
        }

        private static string GetSimpleQueryCore<T>(string query, Rule[] rules)
            where T : class
        {
            query = query?.Trim();
            if (query == null)
                return null;
            else if (string.IsNullOrEmpty(query) || query == "*")
                return "";

            string regexPrefix = @"(^|\s|[(])";
            string regexTemplate = "{0}(?<q>(-|\\w)*)\\s*";

            string modifiedQ = query; //FixInvalidQuery(query) ?? "";
            //check invalid query ( tag: missing value)

            for (int i = 0; i < rules.Length; i++)
            {
                string lookFor = regexPrefix + rules[i].LookFor;
                string replaceWith = rules[i].ReplaceWith;
                bool doFullReplace = rules[i].FullReplace;




                MatchEvaluator evalMatch = (m) =>
                {
                    var s = m.Value;
                    if (string.IsNullOrEmpty(s))
                        return string.Empty;
                    var newVal = replaceWith;
                    if (newVal.Contains("${q}"))
                    {
                        var capt = m.Groups["q"].Captures;
                        var captVal = "";
                        foreach (Capture c in capt)
                            if (c.Value.Length > captVal.Length)
                                captVal = c.Value;

                        newVal = newVal.Replace("${q}", captVal);
                    }
                    if (s.StartsWith("("))
                        return " (" + newVal;
                    else
                        return " " + newVal;
                };

                //if (modifiedQ.ToLower().Contains(lookFor.ToLower()))
                if (Regex.IsMatch(modifiedQ, lookFor, regexQueryOption))
                {
                    Match mFirst = Regex.Match(modifiedQ, lookFor, regexQueryOption);
                    string foundValue = mFirst.Groups["q"].Value;


                    if (doFullReplace
                        && !string.IsNullOrEmpty(replaceWith)
                        && (
                            lookFor.Contains("holding:")
                            //RS
                            || lookFor.Contains("holdingprijemce:")
                            || lookFor.Contains("holdingplatce:")
                            //insolvence
                            || lookFor.Contains("holdingdluznik:")
                            || lookFor.Contains("holdingveritel:")
                            || lookFor.Contains("holdingspravce:")
                            //VZ
                            || lookFor.Contains("holdingdodavatel:")
                            || lookFor.Contains("holdingzadavatel:")
                        )
                        )
                    {
                        //list of ICO connected to this holding
                        Match m = Regex.Match(modifiedQ, lookFor, regexQueryOption);
                        string holdingIco = m.Groups["q"].Value;
                        HlidacStatu.Lib.Data.Relation.AktualnostType aktualnost = HlidacStatu.Lib.Data.Relation.AktualnostType.Nedavny;
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
                                    .Where(o => o.To.Type == Data.Graph.Node.NodeType.Person)
                                    .Select(o => Data.Osoby.GetById.Get(Convert.ToInt32(o.To.Id)))
                                    .Where(o => o != null)
                                    .SelectMany(o => o.AktualniVazby(aktualnost))
                                    .Select(v => v.To.Id)
                                    .Distinct();
                            icos = icos.Union(icosPresLidi).Distinct();

                            var templ = $" ( {replaceWith}:{{0}} ) ";
                            if (replaceWith.Contains("${q}"))
                                templ = $" ( {replaceWith.Replace("${q}", "{0}")} )";

                            if (icos != null && icos.Count() > 0)
                            {
                                icosQuery = " ( " + icos
                                    .Select(t => string.Format(templ, t))
                                    .Aggregate((fi, s) => fi + " OR " + s) + " ) ";
                            }
                            else
                            {
                                icosQuery = string.Format(templ, "noOne"); //$" ( {icoprefix}:noOne ) ";
                            }
                            if (!string.IsNullOrEmpty(rules[i].AddLastCondition))
                            {
                                if (rules[i].AddLastCondition.Contains("${q}"))
                                {
                                    rules[i].AddLastCondition = rules[i].AddLastCondition.Replace("${q}", foundValue);
                                }

                                icosQuery = ModifyQueryOR(icosQuery, rules[i].AddLastCondition);

                                rules[i].AddLastCondition = null; //done, don't do it anywhere
                            }

                            modifiedQ = Regex.Replace(modifiedQ, lookFor, " (" + icosQuery + ") ", regexQueryOption);

                        }
                    } //do regex replace
                    else if (doFullReplace
                                && !string.IsNullOrEmpty(replaceWith)
                                && (
                                    lookFor.Contains("osobaid:")
                                    || lookFor.Contains("osobaiddluznik:")
                                    || lookFor.Contains("osobaidveritel:")
                                    || lookFor.Contains("osobaidspravce:")
                                    || lookFor.Contains("osobaidzadavatel:")
                                    || lookFor.Contains("osobaiddodavatel:")
                                    )
                        )//(replaceWith.Contains("${ico}"))
                    {
                        //list of ICO connected to this person
                        Match m = Regex.Match(modifiedQ, lookFor, regexQueryOption);
                        string nameId = m.Groups["q"].Value;
                        Data.Osoba p = Data.Osoby.GetByNameId.Get(nameId);
                        string icosQuery = "";

                        //string icoprefix = replaceWith;
                        var templ = $" ( {replaceWith}:{{0}} ) ";
                        if (replaceWith.Contains("${q}"))
                            templ = $" ( {replaceWith.Replace("${q}", "{0}")} )";



                        if (p != null)
                        {
                            var icos = p
                                        .AktualniVazby(Data.Relation.AktualnostType.Nedavny)
                                        .Where(w => !string.IsNullOrEmpty(w.To.Id))
                                        //.Where(w => Analysis.ACore.GetBasicStatisticForICO(w.To.Id).Summary.Pocet > 0)
                                        .Select(w => w.To.Id)
                                        .Distinct().ToArray();


                            if (icos != null && icos.Length > 0)
                            {
                                icosQuery = " ( " + icos
                                    .Select(t => string.Format(templ, t))
                                    .Aggregate((f, s) => f + " OR " + s) + " ) ";
                            }
                            else
                            {
                                icosQuery = string.Format(templ, "noOne"); //$" ( {icoprefix}:noOne ) ";
                            }
                            if (!string.IsNullOrEmpty(rules[i].AddLastCondition))
                            {
                                if (rules[i].AddLastCondition.Contains("${q}"))
                                {
                                    rules[i].AddLastCondition = rules[i].AddLastCondition.Replace("${q}", foundValue);
                                }

                                icosQuery = ModifyQueryOR(icosQuery, rules[i].AddLastCondition);

                                rules[i].AddLastCondition = null; //done, don't do it anywhere
                            }
                            modifiedQ = Regex.Replace(modifiedQ, lookFor, " (" + icosQuery + ") ", regexQueryOption);
                        }
                        else
                            modifiedQ = Regex.Replace(modifiedQ, lookFor, " (" + string.Format(templ, "noOne") + ") ", regexQueryOption);
                    }

                    //VZ
                    else if (doFullReplace && replaceWith.Contains("${oblast}"))
                    {
                        string cpv = "";
                        if (replaceWith.Contains("${oblast}"))
                        {
                            var oblastVal = ParseTools.GetRegexGroupValue(modifiedQ, @"oblast:(?<oblast>\w*)", "oblast");
                            var cpvs = Lib.Data.VZ.VerejnaZakazka.Searching.CPVOblastToCPV(oblastVal);
                            if (cpvs != null)
                            {
                                var q_cpv = "cPV:(" + cpvs.Select(s => s + "*").Aggregate((f, s) => f + " OR " + s) + ")";
                                modifiedQ = Regex.Replace(modifiedQ, @"oblast:(?<oblast>\w*)", q_cpv, regexQueryOption);
                            }
                        }
                    }
                    //VZs
                    else if (doFullReplace && replaceWith.Contains("${cpv}"))
                    {
                        string cpv = "";
                        //Match m = Regex.Match(modifiedQ, lookFor, regexQueryOption);
                        //string cpv = "";
                        //if (m.Success)
                        //    cpv = m.Groups["q"].Value;
                        cpv = ParseTools.GetRegexGroupValue(modifiedQ, @"cpv:(?<q>(-|,|\d)*)\s*", "q");
                        lookFor = @"cpv:(?<q>(-|,|\d)*)\s*";
                        if (!string.IsNullOrEmpty(cpv))
                        {
                            string[] cpvs = cpv.Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
                            string q_cpv = "";
                            if (cpvs.Length > 0)
                                q_cpv = "cPV:(" + cpvs.Select(s => s + "*").Aggregate((f, s) => f + " OR " + s) + ")";

                            modifiedQ = Regex.Replace(modifiedQ, lookFor, " (" + q_cpv + ") ", regexQueryOption);
                        }
                        else
                            modifiedQ = Regex.Replace(modifiedQ, lookFor, " ", regexQueryOption);
                    }
                    //VZ
                    else if (doFullReplace && replaceWith.Contains("${form}"))
                    {
                        lookFor = @"form:(?<q>((F|CZ)\d{1,2}(,)?)*)\s*";
                        Match m = Regex.Match(modifiedQ, lookFor, regexQueryOption);
                        string form = "";
                        if (m.Success)
                            form = m.Groups["q"].Value;
                        if (!string.IsNullOrEmpty(form))
                        {
                            string[] forms = form.Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
                            string q_form = "";
                            if (forms.Length > 0)
                                q_form = "formulare.druh:(" + forms.Select(s => s + "*").Aggregate((f, s) => f + " OR " + s) + ")";

                            modifiedQ = Regex.Replace(modifiedQ, lookFor, " (" + q_form + ") ", regexQueryOption);
                        }
                        else
                            modifiedQ = Regex.Replace(modifiedQ, lookFor, " ", regexQueryOption);
                    }

                    else if (replaceWith.Contains("${q}"))
                    {

                        modifiedQ = Regex.Replace(modifiedQ, string.Format(regexTemplate, lookFor), evalMatch, regexQueryOption);
                    } //do regex replace

                    else if (doFullReplace && lookFor.Contains("chyby:"))
                    {
                        string levelVal = ParseTools.GetRegexGroupValue(modifiedQ, @"chyby:(?<level>\w*)", "level")?.ToLower() ?? "";
                        string levelQ = "";
                        if (levelVal == "fatal" || levelVal == "zasadni")
                            levelQ = Lib.Issues.Util.IssuesByLevelQuery(Lib.Issues.ImportanceLevel.Fatal);
                        else if (levelVal == "major" || levelVal == "vazne")
                            levelQ = Lib.Issues.Util.IssuesByLevelQuery(Lib.Issues.ImportanceLevel.Major);

                        if (!string.IsNullOrEmpty(levelQ))
                        {
                            modifiedQ = Regex.Replace(modifiedQ, @"chyby:(\w*)", levelQ, regexQueryOption);
                        }

                    }
                    else if (!string.IsNullOrEmpty(replaceWith))
                    {
                        modifiedQ = Regex.Replace(modifiedQ, lookFor, evalMatch, regexQueryOption);

                    }

                    if (!string.IsNullOrEmpty(rules[i].AddLastCondition))
                    {
                        if (rules[i].AddLastCondition.Contains("${q}"))
                        {
                            rules[i].AddLastCondition = rules[i].AddLastCondition.Replace("${q}", foundValue);
                        }

                        modifiedQ = ModifyQueryOR(modifiedQ, rules[i].AddLastCondition);
                    }
                }
            }

            return modifiedQ;
        }

        public static string ModifyQueryAND(string origQuery, string anotherCondition)
        {
            if (string.IsNullOrEmpty(origQuery))
                return anotherCondition;
            else
            {
                var s1 = origQuery.Trim();
                var s2 = anotherCondition.Trim();
                if (!s1.StartsWith("(") && !s1.EndsWith("("))
                    s1 = $"( {s1} )";
                if (!s2.StartsWith("(") && !s2.EndsWith("("))
                    s2 = $"( {s2} )";
                return string.Format($"(  {s1}  AND  {s2}  )", origQuery, anotherCondition);
            }
        }
        public static string ModifyQueryOR(string origQuery, string anotherCondition)
        {
            if (string.IsNullOrEmpty(origQuery))
                return anotherCondition;
            else
            {
                var s1 = origQuery.Trim();
                var s2 = anotherCondition.Trim();
                if (!s1.StartsWith("(") && !s1.EndsWith("("))
                    s1 = $"( {s1} )";
                if (!s2.StartsWith("(") && !s2.EndsWith("("))
                    s2 = $"( {s2} )";
                return string.Format($"(  {s1}  OR  {s2}  )", origQuery, anotherCondition);
            }
        }





        static string ScrollLifeTime = "2m";
        public static void DoActionForAll<T>(
            System.Func<IHit<T>, object, ActionOutputData> action,
            object actionParameters,
            System.Action<string> logOutputFunc,
            System.Action<Devmasters.Batch.ActionProgressData> progressOutputFunc,
            bool parallel,
            int blockSize = 500, int? maxDegreeOfParallelism = null,
            bool IdOnly = false,
            ElasticClient elasticClient = null,
            string query = null,
            Indices indexes = null, string prefix = ""

            )
            where T : class
        {
            var client = elasticClient ?? Lib.ES.Manager.GetESClient();

            Func<int, int, ISearchResponse<T>> searchFunc = null;

            var qs = new QueryContainerDescriptor<T>().MatchAll();
            if (!string.IsNullOrEmpty(query))
            {
                qs = new QueryContainerDescriptor<T>()
                        .QueryString(qq => qq
                            .Query(query)
                            .DefaultOperator(Operator.And)
                        );
            }


            if (IdOnly)
                searchFunc = (size, page) =>
                {
                    return client.Search<T>(a => a
                                .Index(indexes ?? client.ConnectionSettings.DefaultIndex)
                                .Source(false)
                                //.Fields(f => f.Field("Id"))
                                .Size(size)
                                .From(page * size)
                                .Query(q => qs)
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
                            .Query(q => qs)
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
            System.Func<IHit<T>, object, Devmasters.Batch.ActionOutputData> action, object actionParameters,
            System.Action<string> logOutputFunc,
            System.Action<Devmasters.Batch.ActionProgressData> progressOutputFunc,
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
            //var total = NoveInzeraty.Lib.Data.Smlouva.Search.NumberOfDocs(null, null);
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
            client.ClearScroll(c => c.ScrollId(scrollId));

            if (logOutputFunc != null)
                logOutputFunc("Done");

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



        public static string ToElasticDate(DateTime? date, string defaultValue = "")
        {
            if (date.HasValue == false)
                return defaultValue;

            switch (date.Value.Kind)
            {
                case DateTimeKind.Unspecified:
                case DateTimeKind.Local:
                    return date.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                case DateTimeKind.Utc:
                    return date.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                default:
                    return date.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            }
        }

    }
}
