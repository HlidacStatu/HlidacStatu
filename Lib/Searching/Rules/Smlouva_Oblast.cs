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
        public Smlouva_Oblast(bool stopFurtherProcessing = false, string addLastCondition = "")
            : base("", stopFurtherProcessing, addLastCondition)
        { }

        public override string[] Prefixes
        {
            get
            {
                return new string[] { "oblast:" };
            }
        }

        private static Dictionary<string, string> GetOblastiValues()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            Type ot = typeof(Lib.Data.Smlouva.SClassification.ClassificationsTypes);
            var vals = System.Enum.GetValues(typeof(Lib.Data.Smlouva.SClassification.ClassificationsTypes)) 
                as Lib.Data.Smlouva.SClassification.ClassificationsTypes[];

            foreach (int v in vals)
            {
                string sval = v.ToString();
                var name = (Lib.Data.Smlouva.SClassification.ClassificationsTypes)v;
                string sname = name.ToString().ToLower();

                if (sname.EndsWith("_obecne"))
                {
                    string range = $"[{sval.Substring(0, 3)}00 TO {sval.Substring(0, 3)}99]";
                    ret.Add(sname.Replace("_obecne", ""), range);
                    ret.Add(sname, range);
                }
                else
                    ret.Add(sname, sval);

            }

            return ret;
        }

        public readonly static Dictionary<string, string> AllValues = GetOblastiValues();

        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (part == null)
                return null;

            if (part.Prefix.Equals("oblast:", StringComparison.InvariantCultureIgnoreCase))
            {
                var oblastVal = part.Value;
                foreach (var key in AllValues.Keys)
                {
                    if (oblastVal.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var q_obl = "classification.types.typeValue:" + AllValues[key];
                        return new RuleResult(SplittingQuery.SplitQuery($" {q_obl} "), this.NextStep);
                    }
                }
            }


            return null;
        }

    }
}
