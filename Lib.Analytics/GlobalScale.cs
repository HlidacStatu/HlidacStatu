using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analytics
{
    public partial class GlobalScalePerYear<T>
    {


        protected GlobalScalePerYear() { }

        Dictionary<string, Func<T, decimal>> scalesDefD = new System.Collections.Generic.Dictionary<string, Func<T, decimal>>();
        Dictionary<string, Func<T, int>> scalesDefI = new System.Collections.Generic.Dictionary<string, Func<T, int>>();

        Dictionary<string, OrderedList> scalesD = new Dictionary<string, OrderedList>();

        int[] calculatedYears = null;

        Dictionary<int, List<(string ico, T value)>> dataPerIcoYear = new Dictionary<int, List<(string ico, T value)>>();

        public GlobalScalePerYear(int[] calculatedYears, IEnumerable<PerYear<T>> dataForAllIcos)
        {
            this.calculatedYears = calculatedYears;
            foreach (var y in calculatedYears)
            {
                dataPerIcoYear.Add(y, new List<(string ico, T value)>());
            }
            foreach (var d in dataForAllIcos)
            {
                foreach (var y in calculatedYears)
                {
                    if (d.years.ContainsKey(y))
                    {
                        dataPerIcoYear[y].Add((d.ICO,d.years[y]));
                    }
                }
            }
        }

        object _getScaleLock = new object();
        public virtual OrderedList GetScale(int year, string name)
        {
            string scaleName = $"{name}_{year}";
            if (!scalesD.ContainsKey(scaleName))
            {
                lock (_getScaleLock)
                {
                    if (!scalesD.ContainsKey(scaleName))
                    {
                        scalesD[scaleName] = CalculateOrderList(year,name);
                    }                
                }
            
            }
            return scalesD[scaleName];
        }

        private OrderedList CalculateOrderList(int year, string name)
        {
            List<(string ico, T value)> data = dataPerIcoYear[year];

            OrderedList ol = new OrderedList(
                data.Select(m=> new OrderedList.Item() {ICO= m.ico, Value = GetValuePerName(name, m.value) })
                );
            return ol;
        }

        private decimal GetValuePerName(string name, T value)
        {
            if (scalesDefD.ContainsKey(name))
                return scalesDefD[name](value);

            if (scalesDefI.ContainsKey(name))
                return (decimal)scalesDefI[name](value);

            throw new ArgumentOutOfRangeException("name",$"{name} selector doesn't exists");
        }

        public virtual void AddScale(string scaleName, Func<T, decimal> selector)
        {
            if (!scalesDefD.ContainsKey(scaleName) && !scalesDefI.ContainsKey(scaleName))
                scalesDefD.Add(scaleName, selector);
        }

        public virtual void AddScale(string scaleName, Func<T, int> selector)
        {
            if (!scalesDefD.ContainsKey(scaleName) && !scalesDefI.ContainsKey(scaleName))
            {
                scalesDefI.Add(scaleName, selector);
            }
        }


    }
}
