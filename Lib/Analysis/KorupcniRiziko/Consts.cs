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
        public const decimal MinSmluvPerYearKIndexValue = -10000m;
    }
}
