using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analytics
{
    public partial class GlobalRankPerYear<T>
        where T:new()
    {
        [Obsolete("dont use it. Only for serialization")]
        public GlobalRankPerYear() { }

        private readonly Dictionary<string, Func<T, decimal>> scalesDefD = new Dictionary<string, Func<T, decimal>>();
        private readonly Dictionary<string, Func<T, long>> scalesDefL = new Dictionary<string, Func<T, long>>();

        private readonly Dictionary<string, OrderedList> scalesD = new Dictionary<string, OrderedList>();
        public  Dictionary<int, List<(string ico, T value)>> DataPerIcoYear = new Dictionary<int, List<(string ico, T value)>>();

        public int[] CalculatedYears = null;

        public GlobalRankPerYear(int[] calculatedYears, IEnumerable<PerYear<T>> dataForAllIcos)
        {
            this.CalculatedYears = calculatedYears;
            foreach (var y in calculatedYears)
            {
                DataPerIcoYear.Add(y, new List<(string ico, T value)>());
            }
            foreach (var d in dataForAllIcos)
            {
                foreach (var y in calculatedYears)
                {
                    if (d.Years.ContainsKey(y))
                    {
                        DataPerIcoYear[y].Add((d.ICO,d.Years[y]));
                    }
                }
            }
        }

        private object _getScaleLock = new object();
        public virtual OrderedList GetRank(int year, string propertyName, Func<T, decimal> propertySelector)
        {
            lock (_getScaleLock)
            {
                if (!scalesDefD.ContainsKey(propertyName) && !scalesDefL.ContainsKey(propertyName))
                    scalesDefD.Add(propertyName, propertySelector);
            }

            return GetRankInternal(year, propertyName);
        }
        public virtual OrderedList GetRank(int year, string propertyName, Func<T, long> propertySelector)
        {
            lock (_getScaleLock)
            {
                if (!scalesDefD.ContainsKey(propertyName) && !scalesDefL.ContainsKey(propertyName))
                    scalesDefL.Add(propertyName, propertySelector);
            }        
            return GetRankInternal(year, propertyName);
        }

        protected virtual OrderedList GetRankInternal(int year, string propertyName)
        {
            string scaleName = $"{propertyName}_{year}";
            if (!scalesD.ContainsKey(scaleName))
            {
                lock (_getScaleLock)
                {
                    if (!scalesD.ContainsKey(scaleName))
                    {
                        scalesD[scaleName] = CalculateOrderList(year,propertyName);
                    }                
                }
            
            }
            return scalesD[scaleName];
        }

        private OrderedList CalculateOrderList(int year, string name)
        {
            var data = DataPerIcoYear[year];

            var orderedList = new OrderedList(
                data.Select(m=> new OrderedList.Item() {
                    ICO = m.ico, 
                    Value = GetValuePerName(name, m.value) })
                );
            return orderedList;
        }

        private decimal GetValuePerName(string name, T value)
        {
            if (scalesDefD.ContainsKey(name))
                return scalesDefD[name](value);

            if (scalesDefL.ContainsKey(name))
                return (decimal)scalesDefL[name](value);

            throw new ArgumentOutOfRangeException("name",$"{name} selector doesn't exists");
        }



    }
}
