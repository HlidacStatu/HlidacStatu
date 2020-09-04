using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class Consts
    {
        public static int[] CalculationYears = Enumerable.Range(2017, DateTime.Now.Year - 2017 - (DateTime.Now.Month>=7 ? 0 : 1)).ToArray();

        public const decimal IntervalOkolo = 0.11m;

        public const decimal Limit1bezDPH_To = 2000000;
        public const decimal Limit1bezDPH_From = Limit1bezDPH_To - (Limit1bezDPH_To * IntervalOkolo);
        public const decimal Limit2bezDPH_To = 6000000;
        public const decimal Limit2bezDPH_From = Limit2bezDPH_To - (Limit2bezDPH_To * IntervalOkolo);


        public const int MinSmluvPerYear = 100;
        public const int MinSumSmluvPerYear = 60000000;
        public const decimal MinSmluvPerYearKIndexValue = -10000m;

        public const decimal BonusPod50K_1 = 0.25m;
        public const decimal BonusPod50K_2 = 0.5m;
        public const decimal BonusPod50K_3 = 0.75m;

        /// <summary>
        /// Checks if year is within the range (CalculationYears). 
        /// If null or later than max then sets it to the max year from the range.
        /// If earlier than the earliest year from range then sets it to the earliest one.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int FixKindexYear(int? year)
        {
            if (year < CalculationYears.Min())
                return CalculationYears.Min();
            if (year is null || year >= CalculationYears.Max())
                return CalculationYears.Max();

            return year.Value;
        }
    }
}
