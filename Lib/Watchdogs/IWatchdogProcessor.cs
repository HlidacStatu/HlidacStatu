using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data;

namespace HlidacStatu.Lib.Watchdogs
{
    public interface IWatchdogProcessor
        : IEmailFormatter
    {
        WatchDog OrigWD { get;  }
        Results GetResults(DateTime? fromDate = null, DateTime? toDate = null, int? maxItems = null, string order = null);
        DateTime GetLatestRec(DateTime toDate);

    }
    public interface IEmailFormatter
    {
        RenderedContent RenderResults(Results data, long numOfListed = 5);
    }
}
