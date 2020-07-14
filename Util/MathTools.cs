using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public static class MathTools
    {

        static double CalculateStdDev(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //Compute the Average
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }

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
