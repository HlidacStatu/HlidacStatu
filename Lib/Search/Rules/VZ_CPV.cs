using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Search.Rules
{
    public class VZ_CPV
        : RuleBase
    {
        public VZ_CPV(string replaceWith, bool stopFurtherProcessing = false, string addLastCondition = "")
            : base(replaceWith, stopFurtherProcessing, addLastCondition)
        { }


        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (this.ReplaceWith.Contains("${cpv}"))
            {

                string cpv = "";
                //Match m = Regex.Match(modifiedQ, lookFor, regexQueryOption);
                //string cpv = "";
                //if (m.Success)
                //    cpv = m.Groups["q"].Value;
                cpv = part.Value;
                if (!string.IsNullOrEmpty(cpv))
                {
                    string[] cpvs = cpv.Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
                    string q_cpv = "";
                    if (cpvs.Length > 0)
                        q_cpv = " ( " + cpvs.Select(s => "cpv:" + s + "*").Aggregate((f, s) => f + " OR " + s) + " ) ";

                    return new RuleResult(SplittingQuery.SplitQuery("{q_cpv}"), this.NextStep);
                }
                else

                    return null;
            }


            return null;// new RuleResult(part, this.NextStep);
        }

    }
}
