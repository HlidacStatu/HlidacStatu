using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analytics
{
    // !!!!! Michale, za žádnou cenu sem nedávej ToNiceString !!!! 
    // Nebo začne bůh topit koťátka, dokud ti to nesmažu! :-D

    public class StatisticsSubjectPerYear<T>
        : StatisticsPerYear<T>
        where T : CoreStat, IAddable<T>,new()
    {
        public string ICO { get; set; }


        public StatisticsSubjectPerYear()
        : base()
        { }
        public StatisticsSubjectPerYear(string ico, StatisticsPerYear<T> baseObj)        
        {
            this.ICO = ico;
            this.Years = baseObj.Years;
        }

        public StatisticsSubjectPerYear(string ico, Func<T, int> yearSelector, IEnumerable<T> data)
            : base(yearSelector,data)
        {
            this.ICO = ico;
        }

        /// <summary>
        /// Creates new statistics
        /// </summary>
        /// <param name="ico">Subject Ico</param>
        /// <param name="data">Dictionary where key = Year, value = T</param>
        public StatisticsSubjectPerYear(string ico, Dictionary<int, T> data)
            :base(data)
        {
            this.ICO = ico;
        }


        public static StatisticsSubjectPerYear<T> Aggregate(IEnumerable<StatisticsSubjectPerYear<T>> statistics)
        {
            var aggregatedStatistics = new StatisticsSubjectPerYear<T>()
            {
                ICO = $"aggregated for {statistics.FirstOrDefault().ICO}"
            };
            aggregatedStatistics.Years = StatisticsPerYear<T>.AggregateStats(statistics).Years;

            return aggregatedStatistics;
        }

    }
}
