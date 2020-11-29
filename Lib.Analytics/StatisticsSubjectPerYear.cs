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
        public string ICO { get { return Key; } set { this.Key = value; } }


        public StatisticsSubjectPerYear()
        : base()
        { }
        public StatisticsSubjectPerYear(string ico, StatisticsPerYear<T> baseObj)        
        {
            this.ICO = ico;
            this.Years = base.Years;
        }

        public StatisticsSubjectPerYear(string ico, Func<T, int> yearSelector, IEnumerable<T> data)
            : base(ico,yearSelector,data)
        {
        }

        /// <summary>
        /// Creates new statistics
        /// </summary>
        /// <param name="ico">Subject Ico</param>
        /// <param name="data">Dictionary where key = Year, value = T</param>
        public StatisticsSubjectPerYear(string ico, Dictionary<int, T> data)
            :base(ico,data)
        {
        }


        public static StatisticsSubjectPerYear<T> Aggregate(IEnumerable<StatisticsSubjectPerYear<T>> statistics)
        {
            var aggregatedStatistics = new StatisticsSubjectPerYear<T>()
            {
                Key = $"aggregated for {statistics.FirstOrDefault().ICO}"
            };
            aggregatedStatistics.Years = StatisticsPerYear<T>.AggregateStats(statistics).Years;

            return aggregatedStatistics;
        }

    }
}
