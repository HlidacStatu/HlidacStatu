using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analytics
{
    public class CalculatedProperty
    {
        public static int[] Percentiles { get; } = new int[] { 0, 1, 5, 10, 25, 33, 50, 66, 75, 90, 95, 99, 100 };
        public string PropertyName { get; set; }
        public int Year { get; set; }
        public Dictionary<int, decimal> PercentilesValue { get; set; } = new Dictionary<int, decimal>();


        [Obsolete()]
        public CalculatedProperty() { }
        public CalculatedProperty(string propertyName, int year, IEnumerable<decimal> data)
        {
            this.PropertyName = propertyName;
            this.Year = year;

            foreach (var perc in Percentiles)
            {
                PercentilesValue.Add(perc,
                    HlidacStatu.Util.MathTools.PercentileCont(perc / 100m, data)
                    );
            }
        }

        public int Rank(decimal value)
        {
            foreach (var perc in Percentiles)
            {
                if (value <= this.PercentilesValue[perc])
                    return perc;
            }
            return Percentiles.Max();
        }
    }
}
