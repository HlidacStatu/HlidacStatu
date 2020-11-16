using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analytics
{
    //group data per year
    public class PerYear<T>
        where T: new()
    {

        public string ICO { get; set; }
        public Dictionary<int, T> Years { get; set; }  = new Dictionary<int, T>();

        public PerYear() { }

        public PerYear(string ico, Func<T,int> yearSelector, IEnumerable<T> data) 
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
        public PerYear(string ico, Dictionary<int,T> data) 
        {
            this.ICO = ico;
            Years = data ?? throw new ArgumentNullException("data");
        }


        public decimal Sum(Func<T, int> selector)
        {
            return Years.Select(v=>v.Value).Sum(selector);
        }
        public decimal Sum(Func<T, decimal> selector)
        {
            
            return Years.Select(v=>v.Value).Sum(selector);
        }

        public decimal Sum(int[] foryears, Func<T, int> selector)
        {
            return Years
                .Where(y=>foryears.Contains(y.Key))
                .Select(v=>v.Value)
                .Sum(selector);
        }
        public decimal Sum(int[] foryears, Func<T, decimal> selector)
        {
            return Years
                .Where(y=>foryears.Contains(y.Key))
                .Select(v=>v.Value)
                .Sum(selector);

        }

        public List<T> RegistrSmluvYears()
        {
            var returnValue = new List<T>();
            foreach (var y in Consts.RegistrSmluvYearsList)
            {
                returnValue.Add(Years[y]);
            }
            return returnValue;
        }

        int _delayedCurrYear = 0;
        public virtual int DelayedCurrentYear()
        {
            if (_delayedCurrYear == 0)
                _delayedCurrYear = DateTime.Now.Month > 6 ? DateTime.Now.Year : DateTime.Now.Year - 1;

            return _delayedCurrYear;
        }

        public virtual int CurrentYear()
        {
            return  DateTime.Now.Year;
        }

    }
}
