using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Search.Rules
{
    public abstract class RuleBase
        : IRule
    {
        public RuleBase(string replaceWith, bool stopFurtherProcessing = false, string addLastCondition = "")
        {
            this.ReplaceWith = replaceWith;
            this.StopFurtherProcessing = stopFurtherProcessing;
            this.AddLastCondition = addLastCondition;
        }


        public string ReplaceWith { get; set; }

        public bool StopFurtherProcessing { get; set; } = false;
        public string AddLastCondition { get; set; } = "";

        public virtual RuleResult Process(SplittingQuery.Part queryPart)
        {
            var res = processQueryPart(queryPart);

            if (!string.IsNullOrEmpty(this.AddLastCondition))
            {
                string rq = this.AddLastCondition;
                if (this.AddLastCondition.Contains("${q}"))
                {
                    rq = this.AddLastCondition.Replace("${q}", queryPart.Value);
                }
                rq = Tools.ModifyQueryOR(res.Query.FullQuery, rq);
                return new RuleResult(SplittingQuery.SplitQuery($" {rq} "), this.StopFurtherProcessing);
            }
            return res;
        }
        protected abstract RuleResult processQueryPart(SplittingQuery.Part queryPart);
    }
}
