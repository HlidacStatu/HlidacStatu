using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching.Rules
{
    public class Smlouva_Oblast
        : RuleBase
    {
        int place = 0;
        public Smlouva_Oblast(int place, bool stopFurtherProcessing = false, string addLastCondition = "")
            : base("", stopFurtherProcessing, addLastCondition)
        {
            this.place = place;
            if (this.place < 1)
                this.place = 1;
            if (this.place > 2)
                this.place = 2;
        }

        public override string[] Prefixes
        {
            get
            {
                return new string[] { $"oblast{(this.place==1 ? "" : "2")}:" };
            }
        }

        private static Dictionary<string, string> GetOblastiValues()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            


            foreach (var v in Lib.Data.Smlouva.SClassification.AllTypes)
            {
                if (v.MainType)
                {
                    ret.Add(v.SearchShortcut, v.SearchExpression);
                    ret.Add(v.SearchShortcut+"_obecne", v.SearchExpression);
                }
                else
                    ret.Add(v.SearchShortcut, v.SearchExpression);

            }

            return ret;
        }

        public readonly static Dictionary<string, string> AllValues = GetOblastiValues();

        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (part == null)
                return null;

            if (part.Prefix.Equals(this.Prefixes.First(), StringComparison.InvariantCultureIgnoreCase))
            {
                var oblastVal = part.Value;
                foreach (var key in AllValues.Keys)
                {
                    if (oblastVal.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var q_obl = $"classification.class{this.place}.typeValue:" + AllValues[key];
                        return new RuleResult(SplittingQuery.SplitQuery($" {q_obl} "), this.NextStep);
                    }
                }
            }


            return null;
        }

    }
}
