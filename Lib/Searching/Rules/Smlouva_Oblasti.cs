﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching.Rules
{
    public class Smlouva_Oblasti
        : RuleBase
    {
        public Smlouva_Oblasti(bool stopFurtherProcessing = false, string addLastCondition = "")
            : base("", stopFurtherProcessing, addLastCondition)
        { }

        public override string[] Prefixes
        {
            get
            {
                return new string[] { "oblasti:" };
            }
        }

        private static Dictionary<string, string> GetOblastiValues()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            


            foreach (var v in Lib.Data.Smlouva.SClassification.AllTypes)
            {
                if (v.MainType)
                {
                    ret.Add(v.SearchShortcut, v.SearchExpression);
                    ret.Add(v.SearchShortcut+"_obecne", v.SearchExpression);
                }
                else
                    ret.Add(v.SearchShortcut, v.SearchExpression);

            }

            return ret;
        }

        public readonly static Dictionary<string, string> AllValues = GetOblastiValues();

        protected override RuleResult processQueryPart(SplittingQuery.Part part)
        {
            if (part == null)
                return null;

            if (part.Prefix.Equals("oblasti:", StringComparison.InvariantCultureIgnoreCase))
            {
                var oblastVal = part.Value;
                foreach (var key in AllValues.Keys)
                {
                    if (oblastVal.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var q_obl = $" ( classification.class1:${AllValues[key]} OR classification.class2:${AllValues[key]} OR classification.class3:${AllValues[key]} ) ";

                        return new RuleResult(SplittingQuery.SplitQuery($" {q_obl} "), this.NextStep);
                    }
                }
            }


            return null;
        }

    }
}