using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analysis
{

    public abstract class DataPerYear
    {
        public static int AllYearsSummaryKey = 0;
        public static int UsualFirstYear = 2016;

        public int FirstYear = UsualFirstYear;
        public int LastYear = System.DateTime.Now.Year;

        
    }


    public abstract class DataPerYear<T> : DataPerYear
    {
        public Dictionary<int, T> Data { get; set; } = new Dictionary<int, T>();

        protected abstract T EmptyRec();

        public T this[int year]
        {
            get
            {
                if (Data.ContainsKey(year))
                    return Data[year];
                else
                    return EmptyRec();
            }
        }

        public T Summary
        {
            get { return Data[AllYearsSummaryKey]; }
        }

        [Newtonsoft.Json.JsonIgnore]
        public IEnumerable<KeyValuePair<int, T>> RegistrSmluvTime
        {
            get
            {
                return this.Data.Where(k => k.Key >= 2016 && k.Key <= System.DateTime.Now.Year);
            }
        }


    }

}
