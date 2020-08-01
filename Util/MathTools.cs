using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public static class MathTools
    {

        /// <summary>
        /// Nabyva hodnoty 0-1
        /// 0 - idealni distribuce, rovnomerna
        /// 1 - monopol, nulova distribuce prvku
        /// 
        /// example: Assume a market with two players and equally distributed market share; H = 1/N = 1/2 = 0.5 and H* = 0. 
        /// Now compare that to a situation with three players and again an equally distributed market share; H = 1/N = 1/3 = 0.333..., 
        /// note that H* = 0 like the situation with two players. The market with three players is less concentrated, 
        /// but this is not obvious looking at just H*. 
        /// Thus, the normalized Herfindahl index can serve as a measure for the equality of distributions, 
        /// but is less suitable for concentration.
        /// </summary>
        /// <param name="valuesGroupedByCompany"></param>
        /// <returns></returns>
        public static decimal Herfindahl_Hirschman_IndexNormalized(Dictionary<string,long> items)
        {
            if (items == null)
                return 0;
            if (items.Count == 0)
                return 0;

            if (items.Count() == 1)
                return 1;
            decimal H = Herfindahl_Hirschman_Index(items.Values.Select(m=> (decimal)m));

            return Herfindahl_Hirschman_IndexNormalized_FromHHI(H, items.LongCount());
            //return hindexNorm;
        }
        public static decimal Herfindahl_Hirschman_IndexNormalized_FromHHI(decimal HHI, long itemsCount)
        {
            decimal N = (decimal)itemsCount;
            decimal hindexNorm = (HHI - 1 / N) / (1 - 1 / N);
            if (hindexNorm < 0)
                hindexNorm = 0;
            if (hindexNorm > 1)
                hindexNorm = 1;
            return hindexNorm;

        }
        /// <summary>
        /// 0 - trh
        /// 1 - monopol
        /// nabyva hodnoty 1/N az 1
        /// </summary>
        /// <param name="valuesGroupedByCompany"></param>
        /// <returns></returns>
        public static decimal Herfindahl_Hirschman_Index(IEnumerable<decimal> valuesGroupedByCompany)
        {
            decimal total = valuesGroupedByCompany.Sum();
            if (total == 0)
                return 0m;
            decimal hindex = valuesGroupedByCompany
                .Select(v => v / total) //podil na trhu
                .Select(v => v * v) // ^2
                .Sum(); //SUM
            return hindex;
        }

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
