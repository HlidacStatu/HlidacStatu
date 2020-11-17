using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analytics
{
    public partial class GlobalStatisticsPerYear<T>
    {
        public class OrderedList
        {
            public OrderedList(IEnumerable<decimal> data)
            {
                foreach (var perc in percentiles)
                {
                    PercentilesValue.Add(perc,
                        HlidacStatu.Util.MathTools.PercentileCont(perc / 100m, data)
                        );
                }
            }

            public Dictionary<int, decimal> PercentilesValue { get; set; } = new Dictionary<int, decimal>();

            static int[] percentiles = new int[] { 1, 5, 10, 25, 33, 50, 66, 75, 90, 95, 99 };



            public int? Rank(string ico)
            {
                var res = this.Items.FindIndex(m => m.ICO == ico);
                if (res == -1)
                    return null;
                else
                    return res + 1;
            }

            //public decimal Average()
            //{
            //    return Items.Average(i => i.Value);
            //}
            //public decimal Minimum()
            //{
            //    return Items.Min(i => i.Value);
            //}
            //public decimal Maximum()
            //{
            //    return Items.Max(i => i.Value);
            //}


        }


    }
}
