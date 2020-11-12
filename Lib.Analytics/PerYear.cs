using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analytics
{
    public class PerYear<T>
        where T : BaseData<T>
    {

        public Dictionary<int, BaseData<T>> years { get; set; }  = new Dictionary<int, BaseData<T>>();

        public decimal AllYearsSum()
        {
            if (years.Count == 0)
                return 0;
            return years.Sum(m => m.Value.TheValue);
        }

        public decimal RegistrSmluvYearsSum()
        {
            if (years.Count == 0)
                return 0;
            return years.Where(m=>m.Key>=2016).Sum(m => m.Value.TheValue);
        }


        int _currYear = 0;
        public virtual int CurrentYear()
        {
            if (_currYear == 0)
                _currYear = DateTime.Now.Month > 6 ? DateTime.Now.Year : DateTime.Now.Year - 1;

            return _currYear;
        }

    }
}
