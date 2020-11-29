using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analytics
{
    public static class Extensions
    {
        public static StatisticsPerYear<T> AggregateStats<T>(this IEnumerable<StatisticsPerYear<T>> statistics, int[] onlyYears = null)
            where T : CoreStat, IAddable<T>,new()
        {
            return StatisticsPerYear<T>.AggregateStats(statistics,onlyYears);
        }

    }
}
