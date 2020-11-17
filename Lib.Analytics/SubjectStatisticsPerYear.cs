using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analytics
{
    //group data per year
    public class SubjectStatisticsPerYear<T>
        where T : new()
    {
        public string ICO { get; set; }
        public Dictionary<int, T> Years { get; set; } = new Dictionary<int, T>();

        /// <summary>
        /// Tenhle měsíc určuje, za který rok se AKTUÁLNÍ data mají zobrazovat.
        /// Pokud chceme pro nějakou datovou sadu nastavit sezónu jinak, je potřeba ho změnit
        /// </summary>
        public int NewSeasonStartMonth { get; protected set;} = 6;
        
        public SubjectStatisticsPerYear() { }

        public SubjectStatisticsPerYear(string ico, Func<T,int> yearSelector, IEnumerable<T> data) 
        {
            if (yearSelector == null)
                throw new ArgumentNullException("yearSelector");
            if (data == null)
                throw new ArgumentNullException("data");
            this.ICO = ico;
            foreach (var item in data)
            {
                Years.Add(yearSelector(item), item);
            }
        }
        public SubjectStatisticsPerYear(string ico, Dictionary<int,T> data) 
        {
            this.ICO = ico;
            Years = data ?? throw new ArgumentNullException("data");
        }

        public decimal Sum(Func<T, int> selector)
        {
            return Years.Values.Sum(selector);
        }
        public decimal Sum(Func<T, decimal> selector)
        {
            return Years.Values.Sum(selector);
        }

        public decimal Sum(int[] forYears, Func<T, int> selector)
        {
            return Years
                .Where(y=>forYears.Contains(y.Key))
                .Select(v=>v.Value)
                .Sum(selector);
        }
        public decimal Sum(int[] forYears, Func<T, decimal> selector)
        {
            return Years
                .Where(y=>forYears.Contains(y.Key))
                .Select(v=>v.Value)
                .Sum(selector);
        }

        // tohle má být obecná třída - tohle by tu nemělo co dělat
        //public List<T> RegistrSmluvYears()
        //{
        //    var returnValue = new List<T>();
        //    foreach (var y in Consts.RegistrSmluvYearsList)
        //    {
        //        returnValue.Add(Years[y]);
        //    }
        //    return returnValue;
        //}

        public virtual int CurrentSeasonYear()
        {
            return DateTime.Now.Month >= NewSeasonStartMonth ? 
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

    }
}
