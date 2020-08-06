using System.Collections.Generic;

using HlidacStatu.Util;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class KoncentraceDodavateluObor
    {
        public int OborId { get; set; }
        [Nest.Keyword]
        public string OborName { get; set; }

        public KoncentraceDodavateluIndexy Koncentrace { get; set; }
        public KoncentraceDodavateluIndexy KoncentraceBezUvedeneCeny { get; set; }

        public decimal Combined_Herfindahl_Hirschman_Modified()
        {
            if (KoncentraceBezUvedeneCeny == null)
                return Koncentrace.Herfindahl_Hirschman_Modified;

            List<System.Tuple<int, decimal>> items = new List<System.Tuple<int, decimal>>{
                new System.Tuple<int, decimal> (Koncentrace.PocetSmluv, Koncentrace.Herfindahl_Hirschman_Modified),
                new System.Tuple<int, decimal> (KoncentraceBezUvedeneCeny.PocetSmluv, KoncentraceBezUvedeneCeny.Herfindahl_Hirschman_Modified)
            };

            return items.WeightedAverage(m => m.Item2, w => w.Item1);

        }

    } 

}
