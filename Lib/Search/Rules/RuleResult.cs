using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Search.Rules
{
    public class RuleResult
    {
        public RuleResult(SplittingQuery sq, bool stopFurther = false)
        {
            this.Query = sq;
            this.StopFurther = stopFurther;
        }

        public RuleResult(SplittingQuery.Part queryPart, bool stopFurther = false)
        {
            this.Query = new SplittingQuery(new[] { queryPart });
            this.StopFurther = stopFurther;
        }
        public RuleResult(SplittingQuery.Part[] queryParts, bool stopFurther = false)
        {
            this.Query = new SplittingQuery(queryParts);
            this.StopFurther = stopFurther;
        }

        public SplittingQuery Query { get; protected set; }
        public bool StopFurther { get; protected set; } = false;
    }
}
