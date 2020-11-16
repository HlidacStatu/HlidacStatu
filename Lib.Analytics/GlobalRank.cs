using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analytics
{
    public partial class GlobalRankPerYear<T>
        where T:new()
    {
        protected GlobalRankPerYear() { }

        private readonly Dictionary<string, Func<T, decimal>> scalesDefD = new Dictionary<string, Func<T, decimal>>();
        private readonly Dictionary<string, Func<T, long>> scalesDefL = new Dictionary<string, Func<T, long>>();

        private readonly Dictionary<string, OrderedList> scalesD = new Dictionary<string, OrderedList>();
        private readonly Dictionary<int, List<(string ico, T value)>> dataPerIcoYear = new Dictionary<int, List<(string ico, T value)>>();

        public int[] CalculatedYears = null;

        public GlobalRankPerYear(int[] calculatedYears, IEnumerable<PerYear<T>> dataForAllIcos)
        {
            this.CalculatedYears = calculatedYears;
            foreach (var y in calculatedYears)
            {
                dataPerIcoYear.Add(y, new List<(string ico, T value)>());
            }
            foreach (var d in dataForAllIcos)
            {
                foreach (var y in calculatedYears)
                {
                    if (d.Years.ContainsKey(y))
                    {
                        dataPerIcoYear[y].Add((d.ICO,d.Years[y]));
                    }
                }
            }
        }

        private object _getScaleLock = new object();
        public virtual OrderedList GetScale(int year, string propertyName, Func<T, decimal> propertySelector)
        {
            lock (_getScaleLock)
            {
                if (!scalesDefD.ContainsKey(propertyName) && !scalesDefL.ContainsKey(propertyName))
                    scalesDefD.Add(propertyName, propertySelector);
            }

            return GetScaleInternal(year, propertyName);
        }
        public virtual OrderedList GetScale(int year, string propertyName, Func<T, long> propertySelector)
        {
            lock (_getScaleLock)
            {
                if (!scalesDefD.ContainsKey(propertyName) && !scalesDefL.ContainsKey(propertyName))
                    scalesDefL.Add(propertyName, propertySelector);
            }        
            return GetScaleInternal(year, propertyName);
        }

        protected virtual OrderedList GetScaleInternal(int year, string propertyName)
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
            var data = dataPerIcoYear[year];

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
