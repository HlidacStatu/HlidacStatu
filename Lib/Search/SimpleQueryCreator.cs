using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Search
{
    public static class SimpleQueryCreator
    {
        public static string GetSimpleQuery(string query, Lib.Search.Rules.IRule[] rules)
        {
            return GetSimpleQuery(SplittingQuery.SplitQuery(query), rules);
        }


        public static string GetSimpleQuery(Search.SplittingQuery sq, Lib.Search.Rules.IRule[] rules)
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
                    qToProcess = qToProcess ?? sq.Parts;
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
                        .ToArray();

                } //rules

                queryPartResults[qi] = new Rules.RuleResult(new SplittingQuery(qToProcess), qpResults.Last().NextStep);

            } //qi all query parts

            foreach (var qp in queryPartResults)
            {
                finalSq.AddParts(qp.Value.Query.Parts);
            }

            return finalSq.FullQuery;
        }
    }
}