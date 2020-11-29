using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analytics
{
    // !!!!! Michale, za žádnou cenu sem nedávej ToNiceString !!!! 
    // Nebo začne bůh topit koťátka, dokud ti to nesmažu! :-D

    public class StatisticsPerYear<T>
        where T : CoreStat, IAddable<T>, new()
    {
        public Dictionary<int, T> Years { get; set; } = new Dictionary<int, T>();

        /// <summary>
        /// Tenhle měsíc určuje, za který rok se AKTUÁLNÍ data mají zobrazovat.
        /// Pokud chceme pro nějakou datovou sadu nastavit sezónu jinak, je potřeba ho změnit
        /// </summary>

        public StatisticsPerYear() { }

        public StatisticsPerYear(Func<T, int> yearSelector, IEnumerable<T> data)
        {
            if (yearSelector == null)
                throw new ArgumentNullException("yearSelector");
            if (data == null)
                throw new ArgumentNullException("data");
            foreach (var item in data)
            {
                Years.Add(yearSelector(item), item);
            }
        }

        /// <summary>
        /// Creates new statistics
        /// </summary>
        /// <param name="ico">Subject Ico</param>
        /// <param name="data">Dictionary where key = Year, value = T</param>
        public StatisticsPerYear(Dictionary<int, T> data)
        {
            Years = data ?? throw new ArgumentNullException("data");
        }

        public int FirstYear()
        {
            if (this.Years == null)
                return 0;
            if (this.Years.Count == 0)
                return 0;
            return this.Years.Keys.Min();
        }
        public int LastYear()
        {
            if (this.Years == null)
                return 0;
            if (this.Years.Count == 0)
                return 0;
            return this.Years.Keys.Max();
        }

        public int Sum(Func<T, int> selector)
        {
            return Years.Values.Sum(selector);
        }
        public long Sum(Func<T, long> selector)
        {
            return Years.Values.Sum(selector);
        }
        public decimal Sum(Func<T, decimal> selector)
        {
            return Years.Values.Sum(selector);
        }

        public int Sum(int[] forYears, Func<T, int> selector)
        {
            return Years
                .Where(y => forYears.Contains(y.Key))
                .Select(v => v.Value)
                .Sum(selector);
        }
        public long Sum(int[] forYears, Func<T, long> selector)
        {
            return Years
                .Where(y => forYears.Contains(y.Key))
                .Select(v => v.Value)
                .Sum(selector);
        }
        public decimal Sum(int[] forYears, Func<T, decimal> selector)
        {
            return Years
                .Where(y => forYears.Contains(y.Key))
                .Select(v => v.Value)
                .Sum(selector);
        }


        public int[] YearsAfter2016()
        {
            return Years
                .Select(y => y.Key)
                .Where(y => y >= 2016)
                .ToArray();
        }

        public int[] YearsBefore2016()
        {
            return Years
                .Select(y => y.Key)
                .Where(y => y < 2016)
                .ToArray();
        }

        public (decimal change, decimal percentage) ChangeBetweenYears(int forYear,
            Func<T, decimal> selector)
        {
            return ChangeBetweenYears(forYear - 1, forYear, selector);
        }

        //todo: promyslet výsledky
        public (decimal change, decimal percentage) ChangeBetweenYears(int firstYear,
        int lastYear,
        Func<T, decimal> selector)
        {
            var firstStat = StatisticsForYear(firstYear);
            var lastStat = StatisticsForYear(lastYear);

            if (firstStat == null && lastStat == null)
                return (0, 0); // možná vrátit chybu? protože neexistuje ani jeden

            if (firstStat == null)
            {
                return (selector(lastStat), 100);  // není hodnota v minulosti => nárůst 100 %
            }

            if (lastStat == null)
            {
                return (-selector(firstStat), -100); // není hodnota v současnosti => pokles 100 %
            }

            var firstValue = selector(firstStat);
            var lastValue = selector(lastStat);

            decimal change = lastValue - firstValue;
            decimal percentage = change / firstValue;

            return (change, percentage);
        }

        public virtual int CurrentSeasonYear()
        {
            var obj = new T();

            return DateTime.Now.Month >= obj.NewSeasonStartMonth() ?
                DateTime.Now.Year : DateTime.Now.Year - 1;
        }

        public virtual int CurrentYear()
        {
            return DateTime.Now.Year;
        }

        public T CurrentSeasonStatistics()
        {
            return StatisticsForYear(CurrentSeasonYear());
        }

        public T StatisticsForYear(int year)
        {
            if (Years.TryGetValue(year, out var statistics))
            {
                return statistics;
            }

            return default; //should we throw exception instead?
        }

        public static StatisticsPerYear<T> AggregateStats(IEnumerable<StatisticsPerYear<T>> statistics, int[] onlyYears = null)
        {
            var aggregatedStatistics = new StatisticsPerYear<T>();

            var years = statistics.SelectMany(x => x.Years.Keys.Select(k => k)).Distinct();
            if (onlyYears != null && onlyYears.Count() > 0)
                years = years.Where(y => onlyYears.Contains(y));
            foreach (var year in years)
            {
                var statsForYear = statistics.Select(s => s.StatisticsForYear(year));
                var val = statsForYear.Aggregate(new T(), (acc, s) => acc.Add(s));

                aggregatedStatistics.Years.Add(year, val);
            }

            return aggregatedStatistics;
        }

        /// <summary>
        /// Summarize all selected years to one - Good if you need global percentage stats.
        /// </summary>
        /// <param name="years"></param>
        /// <returns></returns>
        public T Summary(int year) => Summary(new int[] { year });
        public T Summary(int[] years)
        {

            var val = Years.Where(y => years.Contains(y.Key))
                .Select(y => y.Value)
                .Aggregate(new T(), (acc, s) => acc.Add(s));

            return val;
        }

        public T Summary(CoreStat.UsualYearsInterval summType)
        {
            CoreStat cs = new T();

            int[] years = cs.YearsFromUsual(summType);
            if (years == null)
                return Summary();
            else
                return Summary(years);
        }


        /// <summary>
        /// Summarize all years to one - Good if you need global percentage stats.
        /// </summary>
        /// <returns></returns>
        public virtual T Summary()
        {
            var val = Years
                .Select(y => y.Value)
                .Aggregate(new T(), (acc, s) => acc.Add(s));

            return val;
        }

    }
}
