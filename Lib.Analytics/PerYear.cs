using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analytics
{
    public class PerYear<T>
    {

        public string ICO { get; set; }

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
                years.Add(yearSelector(item), item);
            }
        }
        public PerYear(string ico, Dictionary<int,T> data) 
        {
            if (data == null)
                throw new ArgumentNullException("data");

            this.ICO = ico;
            years = data;
        }

        public Dictionary<int, T> years { get; set; }  = new Dictionary<int, T>();



        public decimal Sum(Func<T, int> selector)
        {
            return years.Select(v=>v.Value).Sum(selector);
        }
        public decimal Sum(Func<T, decimal> selector)
        {
            return years.Select(v=>v.Value).Sum(selector);
        }

        public decimal Sum(int[] foryears, Func<T, int> selector)
        {
            return years
                .Where(y=>foryears.Contains(y.Key))
                .Select(v=>v.Value)
                .Sum(selector);
        }
        public decimal Sum(int[] foryears, Func<T, decimal> selector)
        {
            return years
                .Where(y=>foryears.Contains(y.Key))
                .Select(v=>v.Value)
                .Sum(selector);

        }

        public List<T> RegistrSmluvYears()
        {
            List<T> ret = new List<T>();
            foreach (var y in Consts.RegistrSmluvYearsList)
            {
                ret.Add(years[y]);
            }
            return ret;
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
