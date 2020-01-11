using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching.Rules
{
    public class Holding
        : RuleBase
    {
        string _specificPrefix = null;
        public Holding(string specificPrefix, string replaceWith, bool stopFurtherProcessing = false, string addLastCondition = "")
            : base(replaceWith, stopFurtherProcessing, addLastCondition)
        {
            _specificPrefix = specificPrefix;
        }

        public override string[] Prefixes { 
            get {
                if (!string.IsNullOrEmpty(_specificPrefix))
                    return new string[] { _specificPrefix };
                else
                    return new string[] { "holding:", 
                        "holdingprijemce:", "holdingplatce:",
                        "holdingdluznik:", "holdingveritel:", "holdingspravce:",
                        "holdingdodavatel:", "holdingzadavatel:"};
            }
        }

        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (part == null)
                return null;

            if (
                (!string.IsNullOrWhiteSpace(_specificPrefix) && part.Prefix.Equals(_specificPrefix, StringComparison.InvariantCultureIgnoreCase))
                ||
                (string.IsNullOrWhiteSpace(_specificPrefix) &&
                    (part.Prefix.Equals("holding:", StringComparison.InvariantCultureIgnoreCase)
                    //RS
                    || part.Prefix.Equals("holdingprijemce:", StringComparison.InvariantCultureIgnoreCase)
                    || part.Prefix.Equals("holdingplatce:", StringComparison.InvariantCultureIgnoreCase)
                    //insolvence
                    || part.Prefix.Equals("holdingdluznik:", StringComparison.InvariantCultureIgnoreCase)
                    || part.Prefix.Equals("holdingveritel:", StringComparison.InvariantCultureIgnoreCase)
                    || part.Prefix.Equals("holdingspravce:", StringComparison.InvariantCultureIgnoreCase)
                    //VZ
                    || part.Prefix.Equals("holdingdodavatel:", StringComparison.InvariantCultureIgnoreCase)
                    || part.Prefix.Equals("holdingzadavatel:", StringComparison.InvariantCultureIgnoreCase)
                )
                )
            )
            {
                //list of ICO connected to this holding
                string holdingIco = part.Value;
                HlidacStatu.Lib.Data.Relation.AktualnostType aktualnost = HlidacStatu.Lib.Data.Relation.AktualnostType.Nedavny;
                Data.Firma f = Data.Firmy.Get(holdingIco);
                if (f != null && f.Valid)
                {
                    var icos = new string[] { f.ICO }
                        .Union(
                            f.AktualniVazby(aktualnost)
                            .Select(s => s.To.Id)
                        )
                        .Distinct();
                    string icosQuery = "";
                    var icosPresLidi = f.AktualniVazby(aktualnost)
                            .Where(o => o.To.Type == Data.Graph.Node.NodeType.Person)
                            .Select(o => Data.Osoby.GetById.Get(Convert.ToInt32(o.To.Id)))
                            .Where(o => o != null)
                            .SelectMany(o => o.AktualniVazby(aktualnost))
                            .Select(v => v.To.Id)
                            .Distinct();
                    icos = icos.Union(icosPresLidi).Distinct();

                    var templ = $" ( {this.ReplaceWith}{{0}} ) ";
                    if (this.ReplaceWith.Contains("${q}"))
                        templ = $" ( {this.ReplaceWith.Replace("${q}", "{0}")} )";

                    if (icos != null && icos.Count() > 0)
                    {
                        icosQuery = " ( " + icos
                            .Select(t => string.Format(templ, t))
                            .Aggregate((fi, s) => fi + " OR " + s) + " ) ";
                    }
                    else
                    {
                        icosQuery = string.Format(templ, "noOne"); //$" ( {icoprefix}:noOne ) ";
                    }

                    return new RuleResult(SplittingQuery.SplitQuery($"{icosQuery}"), this.NextStep);
                }
            }

            return null;
        }

    }
}
