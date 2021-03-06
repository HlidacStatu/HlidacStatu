﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching.Rules
{
    public class Smlouva_Chyby
        : RuleBase
    {
        public Smlouva_Chyby(bool stopFurtherProcessing = false, string addLastCondition = "")
            : base("", stopFurtherProcessing, addLastCondition)
        { }

        public override string[] Prefixes
        {
            get
            {
                return new string[] { "chyby:"};
            }
        }

        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (part == null)
                return null;

            if (part.Prefix.Equals("chyby:", StringComparison.InvariantCultureIgnoreCase))
            {
                string levelVal = part.Value;
                string levelQ = "";
                if (levelVal == "fatal" || levelVal == "zasadni")
                    levelQ = Lib.Issues.Util.IssuesByLevelQuery(Lib.Issues.ImportanceLevel.Fatal);
                else if (levelVal == "major" || levelVal == "vazne")
                    levelQ = Lib.Issues.Util.IssuesByLevelQuery(Lib.Issues.ImportanceLevel.Major);


                return new RuleResult(SplittingQuery.SplitQuery($" {levelQ} "), this.NextStep);
            }
            return null;//new RuleResult(part, this.NextStep);
        }

    }
}
