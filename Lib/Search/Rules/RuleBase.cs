using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Search.Rules
{

    public enum NextStepEnum
    {
        Finished,
        Process,
        StopFurtherProcessing
    }

    public abstract class RuleBase
        : IRule
    {
        public RuleBase(string replaceWith, bool stopFurtherProcessing = false, string addLastCondition = "")
        {
            this.ReplaceWith = replaceWith;
            this.NextStep = stopFurtherProcessing 
                ? Rules.NextStepEnum.StopFurtherProcessing : Rules.NextStepEnum.Process;
            this.AddLastCondition = addLastCondition;
        }

        public abstract string[] Prefixes { get; }

        public string ReplaceWith { get; set; }

        public Rules.NextStepEnum NextStep { get; set; } = Rules.NextStepEnum.Process;
        public string AddLastCondition { get; set; } = "";

        public virtual RuleResult Process(SplittingQuery.Part queryPart)
        {
            var res = processQueryPart(queryPart);

            if (res != null && !string.IsNullOrEmpty(this.AddLastCondition))
            {
                string rq = this.AddLastCondition;
                if (this.AddLastCondition.Contains("${q}"))
                {
                    rq = this.AddLastCondition.Replace("${q}", queryPart.Value);
                }
                rq = Tools.ModifyQueryOR(res.Query.FullQuery(), rq);
                return new RuleResult(SplittingQuery.SplitQuery($" {rq} "), this.NextStep);
            }
            return res;
        }
        protected abstract RuleResult processQueryPart(SplittingQuery.Part queryPart);
    }
}
