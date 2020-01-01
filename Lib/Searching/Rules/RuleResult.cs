using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching.Rules
{
    public class RuleResult
    {
        public RuleResult()
        {
            this.Query = new SplittingQuery();
            this.NextStep = NextStepEnum.Process;
        }
        public RuleResult(SplittingQuery sq, NextStepEnum nextStep)
        {
            this.Query = sq;
            this.NextStep = nextStep;
        }

        public RuleResult(SplittingQuery.Part queryPart, NextStepEnum nextStep)
        {
            this.Query = new SplittingQuery(new[] { queryPart });
            this.NextStep = nextStep;
        }
        public RuleResult(SplittingQuery.Part[] queryParts, NextStepEnum nextStep)
        {
            this.Query = new SplittingQuery(queryParts);
            this.NextStep = nextStep;
        }

        public SplittingQuery Query { get; set; }
        public NextStepEnum NextStep { get; set; }
    }
}
