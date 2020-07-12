using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching.Rules
{
    public class RemoveAllOperators
        : RuleBase
    {
        public RemoveAllOperators(bool stopFurtherProcessing = false, string addLastCondition = "")
            : base("", stopFurtherProcessing, addLastCondition)
        { }

        public override string[] Prefixes
        {
            get
            {
                return new string[] { };
            }
        }

        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (part == null)
                return null;

            if (!string.IsNullOrEmpty(part.Prefix))
            {
                return new RuleResult(SplittingQuery.SplitQuery($""), this.NextStep);
            }
            else
            {
                if (
                    part.ExactValue == false
                    && Searching.Tools.DefaultQueryOperators.Contains(part.Value.Trim().ToUpper())
                    )
                {
                    return new RuleResult(SplittingQuery.SplitQuery($""), this.NextStep);
                }
            }


            return null;
        }

    }
}
