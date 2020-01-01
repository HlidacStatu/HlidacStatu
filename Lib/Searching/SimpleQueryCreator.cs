using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace HlidacStatu.Lib.Searching
{
    public static class SimpleQueryCreator
    {
        public static SplittingQuery GetSimpleQuery(string query, Lib.Searching.Rules.IRule[] rules)
        {
            var fixedQuery = Tools.FixInvalidQuery(query,rules) ?? "";
            return GetSimpleQuery(SplittingQuery.SplitQuery(fixedQuery), rules);
        }


        public static SplittingQuery GetSimpleQuery(Searching.SplittingQuery sq, Lib.Searching.Rules.IRule[] rules)
        {
            SplittingQuery finalSq = new SplittingQuery();

            Dictionary<int, Rules.RuleResult> queryPartResults = new Dictionary<int, Rules.RuleResult>();
            for (int qi = 0; qi < sq.Parts.Length; qi++)
            {
                //beru cast dotazu
                queryPartResults.Add(qi, new Rules.RuleResult());

                //aplikuju na nej jednotliva pravidla, nasledujici na vysledek predchoziho
                SplittingQuery.Part[] qToProcess = null;
                List<Rules.RuleResult> qpResults = null;
                foreach (var rule in rules)
                {
                    qToProcess = qToProcess ?? new SplittingQuery.Part[] { sq.Parts[qi] };
                    qpResults = new List<Rules.RuleResult>();
                    foreach (var qp in qToProcess)
                    {
                        var partRest = rule.Process(qp);
                        if (partRest != null)
                            qpResults.Add(partRest);
                        else
                            qpResults.Add(new Rules.RuleResult(qp, rule.NextStep));
                    }
                    if (qpResults.Last().NextStep == Rules.NextStepEnum.StopFurtherProcessing)
                        break;

                    qToProcess = qpResults
                        .SelectMany(m => m.Query.Parts)
                        .Where(m=>m.ToQueryString.Length > 0)
                        .ToArray();

                } //rules

                queryPartResults[qi] = new Rules.RuleResult(new SplittingQuery(qToProcess), qpResults.Last().NextStep);

            } //qi all query parts

            foreach (var qp in queryPartResults)
            {
                finalSq.AddParts(qp.Value.Query.Parts);
            }

            return finalSq;
        }


        public static QueryContainer GetSimpleQuery<T>(string query, Rules.IRule[] rules)
            where T : class
        {
            string modifiedQ = GetSimpleQuery(query, rules).FullQuery();

            QueryContainer qc = null;
            if (modifiedQ == null)
                qc = new QueryContainerDescriptor<T>().MatchNone();
            else if (string.IsNullOrEmpty(modifiedQ) || modifiedQ == "*")
                qc = new QueryContainerDescriptor<T>().MatchAll();
            else
            {
                modifiedQ = modifiedQ.Replace(" | ", " OR ").Trim();
                qc = new QueryContainerDescriptor<T>()
                    .QueryString(qs => qs
                        .Query(modifiedQ)
                        .DefaultOperator(Operator.And)
                    );
            }
            return qc;
        }

    }
}