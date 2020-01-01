using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching.Rules
{
    public interface IRule
    {
        RuleResult Process(SplittingQuery.Part queryPart);
        string[] Prefixes { get; }
        string ReplaceWith { get; set; }
        NextStepEnum NextStep { get; set; } 
        string AddLastCondition { get; set; } 

    }
}
