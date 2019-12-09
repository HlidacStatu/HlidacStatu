using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Search.Rules
{
    public class FullQueryReplace
        : RuleBase
    {
        MatchEvaluator ruleEvalMatch = null; 
        string _valueConstrain = "";

        public FullQueryReplace(string replaceWith, string valueConstrain, bool stopFurtherProcessing = false, string addLastCondition = "")
            : base(replaceWith, stopFurtherProcessing, addLastCondition)
        {

            this._valueConstrain = valueConstrain;

            ruleEvalMatch = (m) =>
            {
                var s = m.Value;
                if (string.IsNullOrEmpty(s))
                    return string.Empty;
                var newVal = this.ReplaceWith;
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
        }


        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (!string.IsNullOrEmpty(ReplaceWith)
                && (
                    string.IsNullOrWhiteSpace(_valueConstrain)
                    || Regex.IsMatch(part.Value, part.ToQueryString, HlidacStatu.Util.Consts.DefaultRegexQueryOption)
                    )
                )
            {
                string rq = Regex.Replace(part.ToQueryString, this.ReplaceWith, ruleEvalMatch, HlidacStatu.Util.Consts.DefaultRegexQueryOption);
                return new RuleResult(SplittingQuery.SplitQuery($" {rq} "), this.StopFurtherProcessing);

            }

            return new RuleResult(part, this.StopFurtherProcessing);
        }

    }
}
