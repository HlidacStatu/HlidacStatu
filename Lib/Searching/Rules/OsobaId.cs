using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching.Rules
{
    public class OsobaId
        : RuleBase
    {
        string _specificPrefix = null;
        public OsobaId(string specificPrefix, string replaceWith, bool stopFurtherProcessing = false, string addLastCondition = "")
            : base(replaceWith, stopFurtherProcessing, addLastCondition)
        {
            _specificPrefix = specificPrefix;
        }

        public override string[] Prefixes
        {
            get
            {
                if (!string.IsNullOrEmpty(_specificPrefix))
                    return new string[] { _specificPrefix };
                else
                    return new string[] { "osobaid:",
                        "osobaidprijemce:", "osobaidplatce:",
                        "osobaiddluznik:", "osobaidveritel:", 
                        "osobaidspravce:", "osobaiddodavatel:", "osobaidzadavatel:"};
            }
        }

        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (part == null)
                return null;


            if (
                (
                    (!string.IsNullOrWhiteSpace(_specificPrefix) && part.Prefix.Equals(_specificPrefix, StringComparison.InvariantCultureIgnoreCase))
                    ||
                    (part.Prefix.Equals("osobaid:", StringComparison.InvariantCultureIgnoreCase)
                    || part.Prefix.Equals("osobaidprijemce:", StringComparison.InvariantCultureIgnoreCase)
                    || part.Prefix.Equals("osobaidplatce:", StringComparison.InvariantCultureIgnoreCase)

                    || part.Prefix.Equals("osobaidveritel:", StringComparison.InvariantCultureIgnoreCase)
                    || part.Prefix.Equals("osobaidveritel:", StringComparison.InvariantCultureIgnoreCase)

                    || part.Prefix.Equals("osobaidspravce:", StringComparison.InvariantCultureIgnoreCase)
                    || part.Prefix.Equals("osobaidzadavatel:", StringComparison.InvariantCultureIgnoreCase)
                    || part.Prefix.Equals("osobaiddodavatel:", StringComparison.InvariantCultureIgnoreCase)
                    )
                )
                && (Regex.IsMatch(part.Value, @"(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?))", HlidacStatu.Util.Consts.DefaultRegexQueryOption))
            )
            {
                if (!string.IsNullOrWhiteSpace(this.ReplaceWith))
                {
                    //list of ICO connected to this person
                    string nameId = part.Value;
                    Data.Osoba p = Data.Osoby.GetByNameId.Get(nameId);
                    string icosQuery = "";

                    //string icoprefix = replaceWith;
                    var templ = $" ( {ReplaceWith}{{0}} ) ";
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
                        return new RuleResult(SplittingQuery.SplitQuery($"{icosQuery}"), this.NextStep);
                    }
                } // if (!string.IsNullOrWhiteSpace(this.ReplaceWith))
                else if (!string.IsNullOrWhiteSpace(this.AddLastCondition))
                {
                    if (this.AddLastCondition.Contains("${q}"))
                    {
                        var q = Tools.ModifyQueryOR("", this.AddLastCondition.Replace("${q}", part.Value));
                        return new RuleResult(SplittingQuery.SplitQuery($"{q}"), this.NextStep);
                    }
                    else
                    {
                        var q = Tools.ModifyQueryOR("", this.AddLastCondition);
                        return new RuleResult(SplittingQuery.SplitQuery($"{q}"), this.NextStep);
                    }
                }

            }
            return null;
        }

    }
}
