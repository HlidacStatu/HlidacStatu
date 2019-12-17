using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Search.Rules
{
    public class TransformPrefixWithValue
        : RuleBase
    {

        string _valueConstrain = "";
        string _prefix = "";
        public TransformPrefixWithValue(string prefix, string replaceWith, string valueConstrain, bool stopFurtherProcessing = false, string addLastCondition = "")
            : base(replaceWith, stopFurtherProcessing, addLastCondition)
        {
            this._valueConstrain = valueConstrain;
            this._prefix = prefix;
        }


        public override string[] Prefixes
        {
            get
            {
                return new string[] { _prefix };
            }
        }

        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (part == null)
                return null;

            if (string.IsNullOrWhiteSpace(this.ReplaceWith))
                return null; // new RuleResult(SplittingQuery.SplitQuery(" "), this.NextStep);

            if (//this.ReplaceWith.Contains("${q}") &&
                part.Prefix.Equals(_prefix, StringComparison.InvariantCultureIgnoreCase)
                && (
                    string.IsNullOrWhiteSpace(_valueConstrain) 
                    || Regex.IsMatch(part.Value, _valueConstrain, HlidacStatu.Util.Consts.DefaultRegexQueryOption)
                    )
                )
            {
                string rq = " " + ReplaceWith.Replace("${q}", part.Value);
                return new RuleResult(SplittingQuery.SplitQuery($" {rq} "), this.NextStep);
            }

            return null;
        }

    }
}
