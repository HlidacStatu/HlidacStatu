using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class Consts
    {
        public const decimal IntervalOkolo = 0.11m;

        public const decimal Limit1bezDPH_To = 2000000;
        public const decimal Limit1bezDPH_From = Limit1bezDPH_To - (Limit1bezDPH_To * IntervalOkolo);
        public const decimal Limit2bezDPH_To = 6000000;
        public const decimal Limit2bezDPH_From = Limit2bezDPH_To - (Limit2bezDPH_To * IntervalOkolo);


        public const int MinSmluvPerYear = 100;
        public const int MinSumSmluvPerYear = 60000000;
        //public const decimal MinSmluvPerYearKIndexValue = -10000m;

        public const decimal BonusPod50K_1 = 0.25m;
        public const decimal BonusPod50K_2 = 0.5m;
        public const decimal BonusPod50K_3 = 0.75m;

    }
}
