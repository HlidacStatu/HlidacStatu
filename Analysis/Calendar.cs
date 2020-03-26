using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Analysis
{
    public class Calendar
    {
        public static int[] CalculationYears = Enumerable.Range(2017, DateTime.Now.Year - 2017).ToArray();

        public static int[] CalendarYears = Enumerable.Range(2017, 5).ToArray();
        public static Dictionary<int, DateTime[]> Svatky = new Dictionary<int, DateTime[]>();
        public static Dictionary<int, DateTime[]> Vikendy = new Dictionary<int, DateTime[]>();

        public static Dictionary<int, DateTime[]> NepracovniDny = new Dictionary<int, DateTime[]>();


        static Calendar()
        {
            //zdroj: Zákon č. 245/2000 Sb o statních svátcích
            // a https://www.kurzy.cz/kalendar/statni-svatky/2021/
            Svatky.Add(2021, new DateTime[] {
                Util.ParseTools.ToDate("01.01.2021").Value,
                Util.ParseTools.ToDate("02.04.2021").Value,
                Util.ParseTools.ToDate("05.04.2021").Value,
                Util.ParseTools.ToDate("01.05.2021").Value,
                Util.ParseTools.ToDate("08.05.2021").Value,
                Util.ParseTools.ToDate("05.07.2021").Value,
                Util.ParseTools.ToDate("06.07.2021").Value,
                Util.ParseTools.ToDate("28.09.2021").Value,
                Util.ParseTools.ToDate("28.10.2021").Value,
                Util.ParseTools.ToDate("17.11.2021").Value,
                Util.ParseTools.ToDate("24.12.2021").Value,
                Util.ParseTools.ToDate("25.12.2021").Value,
                Util.ParseTools.ToDate("26.12.2021").Value,
            });
            Svatky.Add(2020, new DateTime[] {
                Util.ParseTools.ToDate("01.01.2020").Value,
                Util.ParseTools.ToDate("10.04.2020").Value,
                Util.ParseTools.ToDate("13.04.2020").Value,
                Util.ParseTools.ToDate("01.05.2020").Value,
                Util.ParseTools.ToDate("08.05.2020").Value,
                Util.ParseTools.ToDate("05.07.2020").Value,
                Util.ParseTools.ToDate("06.07.2020").Value,
                Util.ParseTools.ToDate("28.09.2020").Value,
                Util.ParseTools.ToDate("28.10.2020").Value,
                Util.ParseTools.ToDate("17.11.2020").Value,
                Util.ParseTools.ToDate("24.12.2020").Value,
                Util.ParseTools.ToDate("25.12.2020").Value,
                Util.ParseTools.ToDate("26.12.2020").Value,
            });
            Svatky.Add(2019, new DateTime[] {
                Util.ParseTools.ToDate("01.01.2019").Value,
                Util.ParseTools.ToDate("19.04.2019").Value,
                Util.ParseTools.ToDate("22.04.2019").Value,
                Util.ParseTools.ToDate("01.05.2019").Value,
                Util.ParseTools.ToDate("08.05.2019").Value,
                Util.ParseTools.ToDate("05.07.2019").Value,
                Util.ParseTools.ToDate("06.07.2019").Value,
                Util.ParseTools.ToDate("28.09.2019").Value,
                Util.ParseTools.ToDate("28.10.2019").Value,
                Util.ParseTools.ToDate("17.11.2019").Value,
                Util.ParseTools.ToDate("24.12.2019").Value,
                Util.ParseTools.ToDate("25.12.2019").Value,
                Util.ParseTools.ToDate("26.12.2019").Value,
            });
            Svatky.Add(2018, new DateTime[] {
                Util.ParseTools.ToDate("01.01.2018").Value,
                Util.ParseTools.ToDate("30.03.2018").Value,
                Util.ParseTools.ToDate("02.04.2018").Value,
                Util.ParseTools.ToDate("01.05.2018").Value,
                Util.ParseTools.ToDate("08.05.2018").Value,
                Util.ParseTools.ToDate("05.07.2018").Value,
                Util.ParseTools.ToDate("06.07.2018").Value,
                Util.ParseTools.ToDate("28.09.2018").Value,
                Util.ParseTools.ToDate("28.10.2018").Value,
                Util.ParseTools.ToDate("17.11.2018").Value,
                Util.ParseTools.ToDate("24.12.2018").Value,
                Util.ParseTools.ToDate("25.12.2018").Value,
                Util.ParseTools.ToDate("26.12.2018").Value,
            });
            Svatky.Add(2017, new DateTime[] {
                Util.ParseTools.ToDate("01.01.2017").Value,
                Util.ParseTools.ToDate("14.04.2017").Value,
                Util.ParseTools.ToDate("17.04.2017").Value,
                Util.ParseTools.ToDate("01.05.2017").Value,
                Util.ParseTools.ToDate("08.05.2017").Value,
                Util.ParseTools.ToDate("05.07.2017").Value,
                Util.ParseTools.ToDate("06.07.2017").Value,
                Util.ParseTools.ToDate("28.09.2017").Value,
                Util.ParseTools.ToDate("28.10.2017").Value,
                Util.ParseTools.ToDate("17.11.2017").Value,
                Util.ParseTools.ToDate("24.12.2017").Value,
                Util.ParseTools.ToDate("25.12.2017").Value,
                Util.ParseTools.ToDate("26.12.2017").Value,
        });

            foreach (var year in CalendarYears)
            {
                DateTime start = new DateTime(year, 1, 1);
                Vikendy.Add(year,
                        Enumerable.Range(0, DateTime.IsLeapYear(year) ? 366 : 365)
                            .Select(day => start.AddDays(day))
                            .Where(dt => dt.DayOfWeek == DayOfWeek.Sunday ||
                                         dt.DayOfWeek == DayOfWeek.Saturday)
                            .ToArray()
                            );
            }
            foreach (var year in CalendarYears)
            {
                NepracovniDny.Add(year,Svatky[year].Concat(Vikendy[year]).Distinct().ToArray());
            }
        }

        public static string ToElasticQuery(IEnumerable<DateTime> dates)
        {
            if (dates == null)
                return string.Empty;
            if (dates.Count() == 0)
                return string.Empty;

            return "( " + string.Join(" OR ", dates) + " ) ";
        }
    }
}
