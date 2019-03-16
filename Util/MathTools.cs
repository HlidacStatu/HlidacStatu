using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public static class MathTools
    {
    

        public static decimal PercentileCont(IEnumerable<decimal> seq, decimal percentile)
        {
            var elements = seq.ToArray();
            Array.Sort(elements);
            decimal realIndex = percentile * (elements.Length - 1);
            int index = (int)realIndex;
            decimal frac = realIndex - index;
            if (index + 1 < elements.Length)
                return elements[index] * (1 - frac) + elements[index + 1] * frac;
            else
                return elements[index];
        }
    }
}
