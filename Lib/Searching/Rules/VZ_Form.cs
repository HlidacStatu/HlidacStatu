using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching.Rules
{
    public class VZ_Form
        : RuleBase
    {
        public VZ_Form( bool stopFurtherProcessing = false, string addLastCondition = "")
            : base("", stopFurtherProcessing, addLastCondition)
        { }

        public override string[] Prefixes
        {
            get
            {
                return new string[] { "form:" };
            }
        }


        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (part == null)
                return null;

            if (part.Prefix.Equals("form:", StringComparison.InvariantCultureIgnoreCase))
            {
                string form = part.Value;

                if (!string.IsNullOrEmpty(form))
                {
                    string[] forms = form.Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
                    string q_form = "";
                    if (forms.Length > 0)
                        q_form = "formulare.druh:(" + forms.Select(s => s + "*").Aggregate((f, s) => f + " OR " + s) + ")";

                    return new RuleResult(SplittingQuery.SplitQuery($" ( {q_form} ) "), this.NextStep);
                }
            }


            return null;
        }

    }
}
