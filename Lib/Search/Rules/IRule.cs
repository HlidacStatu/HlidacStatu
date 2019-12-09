using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Search.Rules
{
    public interface IRule
    {
        RuleResult Process(SplittingQuery.Part queryPart);

        string ReplaceWith { get; set; }
        bool StopFurtherProcessing { get; set; } 
        string AddLastCondition { get; set; } 

    }
}
