using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analytics
{
    public partial class GlobalScalePerYear<T>
    {
        public class OrderedList
        {
            public class Item
            {
                public string ICO { get; set; }
                public decimal Value { get; set; }
            }

            static int[] percentiles = new int[] { 1, 5, 10, 25, 33, 50, 66, 75, 90, 95, 99 };

            public OrderedList(IEnumerable<Item> data)
            {
                Items = data.OrderBy(o=>o.Value).ToList();
                foreach (var perc in percentiles)
                {
                    PercentilesValue.Add(perc,
                        HlidacStatu.Util.MathTools.PercentileCont(perc / 100m, data.Select(m => m.Value))
                        );
                }
            }

            public List<Item> Items { get; set; }
            public Dictionary<int, decimal> PercentilesValue { get; set; } = new Dictionary<int, decimal>();

            public int? Rank(string ico)
            {
                var res = this.Items.FindIndex(m => m.ICO == ico);
                if (res == -1)
                    return null;
                else
                    return res + 1;
            }

        }


    }
}
