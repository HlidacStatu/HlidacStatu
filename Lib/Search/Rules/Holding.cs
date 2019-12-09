using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Search.Rules
{
    public class Holding
        : RuleBase
    {
        public Holding(string replaceWith, bool stopFurtherProcessing = false, string addLastCondition = "")
            : base(replaceWith, stopFurtherProcessing, addLastCondition)
        { }


        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (part.Prefix.Contains("holding:")
                //RS
                || part.Prefix.Contains("holdingprijemce:")
                || part.Prefix.Contains("holdingplatce:")
                //insolvence
                || part.Prefix.Contains("holdingdluznik:")
                || part.Prefix.Contains("holdingveritel:")
                || part.Prefix.Contains("holdingspravce:")
                //VZ
                || part.Prefix.Contains("holdingdodavatel:")
                || part.Prefix.Contains("holdingzadavatel:")
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

                    var templ = $" ( {this.ReplaceWith}:{{0}} ) ";
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
                    if (!string.IsNullOrEmpty(this.AddLastCondition))
                    {
                        var condition = this.AddLastCondition;
                        if (this.AddLastCondition.Contains("${q}"))
                        {
                            condition = condition.Replace("${q}", part.Value);
                        }
                        icosQuery = Search.Tools.ModifyQueryOR(icosQuery, condition);
                    }

                    return new RuleResult(SplittingQuery.SplitQuery("{icosQuery}"), this.StopFurtherProcessing);
                }
            }

            return new RuleResult(part, this.StopFurtherProcessing);
        }

    }
}
