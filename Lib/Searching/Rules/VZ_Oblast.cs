﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching.Rules
{
    public class VZ_Oblast
        : RuleBase
    {
        public VZ_Oblast(bool stopFurtherProcessing = false, string addLastCondition = "")
            : base("", stopFurtherProcessing, addLastCondition)
        { }

        public override string[] Prefixes
        {
            get
            {
                return new string[] { "oblast:" };
            }
        }


        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (part == null)
                return null;

            if (part.Prefix.Equals("oblast:", StringComparison.InvariantCultureIgnoreCase))
            {
                var oblastVal = part.Value;
                var cpvs = Lib.Data.VZ.VerejnaZakazka.Searching.CPVOblastToCPV(oblastVal);
                if (cpvs != null)
                {
                    //var q_cpv = "cPV:(" + cpvs.Select(s => s + "*").Aggregate((f, s) => f + " OR " + s) + ")";
                    var q_cpv = " ( " + cpvs.Select(s => "cPV:" + s + "*").Aggregate((f, s) => f + " OR " + s) + " ) ";
                    return new RuleResult(SplittingQuery.SplitQuery($" {q_cpv} "), this.NextStep);
                }
            }


            return null;
        }

    }
}
