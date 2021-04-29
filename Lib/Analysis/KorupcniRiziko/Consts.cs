using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class Consts
    {
        public static string[] KIndexExceptions = new string[] { "00297534" };

        public static int[] AvailableCalculationYears=null;
        public static int[] ToCalculationYears = null;

        public const decimal IntervalOkolo = 0.11m;

        public const decimal Limit1bezDPH_To = 2000000;
        public const decimal Limit1bezDPH_From = Limit1bezDPH_To - (Limit1bezDPH_To * IntervalOkolo);
        public const decimal Limit2bezDPH_To = 6000000;
        public const decimal Limit2bezDPH_From = Limit2bezDPH_To - (Limit2bezDPH_To * IntervalOkolo);


        public const int MinSmluvPerYear = 60;
        public const int MinSumSmluvPerYear = 48000000;
        public const decimal MinSmluvPerYearKIndexValue = -10000m;

        public const decimal BonusPod50K_1 = 0.25m;
        public const decimal BonusPod50K_2 = 0.5m;
        public const decimal BonusPod50K_3 = 0.75m;


        static Consts()
        {
            AvailableCalculationYears = Enumerable
                .Range(2017, DateTime.Now.Year - 2017 - (DateTime.Now.Month >= 4 ? 0 : 1))
                .Where(r => r <= Statistics.KIndexStatTotal.Get().Max(m => m.Rok))
                .ToArray();

            ToCalculationYears = Enumerable
                .Range(2017, DateTime.Now.Year - 2017 - (DateTime.Now.Month >= 4 ? 0 : 1))
                .ToArray();
        }


        /// <summary>
        /// Checks if year is within the range (CalculationYears). 
        /// If null or later than max then sets it to the max year from the range.
        /// If earlier than the earliest year from range then sets it to the earliest one.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int FixKindexYear(int? year)
        {
            if (year < AvailableCalculationYears.Min())
                return AvailableCalculationYears.Min();
            if (year is null || year >= AvailableCalculationYears.Max())
                return AvailableCalculationYears.Max();

            return year.Value;
        }
    }
}
