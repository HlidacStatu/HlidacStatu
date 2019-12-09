using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Search.Rules
{
    public class OsobaId
        : RuleBase
    {
        public OsobaId(string replaceWith, bool stopFurtherProcessing = false, string addLastCondition = "")
            : base(replaceWith, stopFurtherProcessing, addLastCondition)
        { }


        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (
                !string.IsNullOrEmpty(this.ReplaceWith)
            && (
                part.Prefix.Contains("osobaid:")
                || part.Prefix.Contains("osobaiddluznik:")
                || part.Prefix.Contains("osobaidveritel:")
                || part.Prefix.Contains("osobaidspravce:")
                || part.Prefix.Contains("osobaidzadavatel:")
                || part.Prefix.Contains("osobaiddodavatel:")
                )

            )
            {
                //list of ICO connected to this person
                string nameId = part.Value;
                Data.Osoba p = Data.Osoby.GetByNameId.Get(nameId);
                string icosQuery = "";

                //string icoprefix = replaceWith;
                var templ = $" ( {ReplaceWith}:{{0}} ) ";
                if (ReplaceWith.Contains("${q}"))
                    templ = $" ( {ReplaceWith.Replace("${q}", "{0}")} )";
                if (p != null)
                {
                    var icos = p
                                .AktualniVazby(Data.Relation.AktualnostType.Nedavny)
                                .Where(w => !string.IsNullOrEmpty(w.To.Id))
                                //.Where(w => Analysis.ACore.GetBasicStatisticForICO(w.To.Id).Summary.Pocet > 0)
                                .Select(w => w.To.Id)
                                .Distinct().ToArray();


                    if (icos != null && icos.Length > 0)
                    {
                        icosQuery = " ( " + icos
                            .Select(t => string.Format(templ, t))
                            .Aggregate((f, s) => f + " OR " + s) + " ) ";
                    }
                    else
                    {
                        icosQuery = string.Format(templ, "noOne"); //$" ( {icoprefix}:noOne ) ";
                    }
                    if (!string.IsNullOrEmpty(this.AddLastCondition))
                    {
                        if (this.AddLastCondition.Contains("${q}"))
                        {
                            icosQuery = Tools.ModifyQueryOR(icosQuery, this.AddLastCondition.Replace("${q}", part.Value));
                        }
                        else
                        {
                            icosQuery = Tools.ModifyQueryOR(icosQuery, this.AddLastCondition);
                        }

                        //this.AddLastCondition = null; //done, don't do it anywhere
                    }
                    return new RuleResult(SplittingQuery.SplitQuery("{icosQuery}"), this.StopFurtherProcessing);
                }
            }


            return new RuleResult(part, this.StopFurtherProcessing);
        }

    }
}
