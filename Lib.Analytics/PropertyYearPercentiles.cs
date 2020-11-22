using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analytics
{
    public class PropertyYearPercentiles
    {
        public static int[] Percentiles { get; } = new int[] { 0, 1, 5, 10, 25, 33, 50, 66, 75, 90, 95, 99, 100 };
        public string PropertyName { get; set; }
        public int Year { get; set; }
        public Dictionary<int, decimal> PercentilesValue { get; set; } = new Dictionary<int, decimal>();

        public int NumberOfItems { get; set; }

        [Obsolete()]
        public PropertyYearPercentiles() { }
        public PropertyYearPercentiles(string propertyName, int year, IEnumerable<decimal> data, bool skipZeros = true)
        {
            this.PropertyName = propertyName;
            this.Year = year;

            var fdata = data;
            if (skipZeros)
                fdata = data.Where(m => m != 0);
            this.NumberOfItems = fdata.Count();
            foreach (var perc in Percentiles)
            {
                PercentilesValue.Add(perc,
                    HlidacStatu.Util.MathTools.PercentileCont(perc / 100m, fdata)
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
