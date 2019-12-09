using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Search.Rules
{
    public class Smlouva_Chyby
        : RuleBase
    {
        public Smlouva_Chyby(string replaceWith, bool stopFurtherProcessing = false, string addLastCondition = "")
            : base(replaceWith, stopFurtherProcessing, addLastCondition)
        { }


        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {

            string levelVal = part.Value;
            string levelQ = "";
            if (levelVal == "fatal" || levelVal == "zasadni")
                levelQ = Lib.Issues.Util.IssuesByLevelQuery(Lib.Issues.ImportanceLevel.Fatal);
            else if (levelVal == "major" || levelVal == "vazne")
                levelQ = Lib.Issues.Util.IssuesByLevelQuery(Lib.Issues.ImportanceLevel.Major);


            if (!string.IsNullOrEmpty(levelQ))
            {
                return new RuleResult(SplittingQuery.SplitQuery($" {levelQ} "), this.StopFurtherProcessing);
            }
            else
                return new RuleResult(part, this.StopFurtherProcessing);
        }

    }
}
