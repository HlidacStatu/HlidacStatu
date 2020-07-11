using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching.Rules
{
    public class Firmy_OVMKategorie
        : RuleBase
    {
        public Firmy_OVMKategorie(bool stopFurtherProcessing = false, string addLastCondition = "")
            : base("", stopFurtherProcessing, addLastCondition)
        { }

        public override string[] Prefixes
        {
            get
            {
                return new string[] { "kategorieid:","kategorie:" };
            }
        }


        public readonly static Dictionary<int, string[]> AllValues = GetAllValues();

        private static Dictionary<int, string[]> GetAllValues()
        {
            var res = Lib.ES.Manager.GetESClient_RPP_Kategorie().Search<Lib.Data.External.RPP.KategorieOVM>(
                s => s.Query(q => q.MatchAll()).Size(2000)
                );
            return res.Hits
                .Select(m => new { id = m.Source.id, icos = m.Source.OVM_v_kategorii.Select(o => o.kodOvm).ToArray() })
                .ToDictionary(k => k.id, v => v.icos);
        }
        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (part == null)
                return null;

            if (part.Prefix.Equals("kategorieid:", StringComparison.InvariantCultureIgnoreCase))
            {
                var katId = part.Value;
                foreach (var key in AllValues.Keys)
                {
                    if (katId.Equals(key.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        string icosQuery = " ( " + AllValues[key]
                                .Select(t => $"ico:{t}")
                                .Aggregate((f, s) => f + " OR " + s) + " ) ";
                        return new RuleResult(SplittingQuery.SplitQuery($"{icosQuery}"), this.NextStep);
                    }
                }
            }


            return null;
        }

    }
}
