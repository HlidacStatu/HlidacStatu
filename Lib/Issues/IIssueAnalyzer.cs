using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Issues
{


    public interface IIssueAnalyzer
    {
        string Name { get; }
        string Description { get; }
        IEnumerable<Issue> FindIssues(Data.Smlouva item);
    }


}
