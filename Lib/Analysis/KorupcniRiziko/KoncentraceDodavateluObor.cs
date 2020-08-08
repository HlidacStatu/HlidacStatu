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

            List<System.Tuple<decimal, int>> items = new List<System.Tuple<decimal, int>>{
                new System.Tuple<decimal, int> ( Koncentrace.Herfindahl_Hirschman_Modified, Koncentrace.PocetSmluv),
                new System.Tuple<decimal, int> (KoncentraceBezUvedeneCeny.Herfindahl_Hirschman_Modified, KoncentraceBezUvedeneCeny.PocetSmluv)
            };

            return items.WeightedAverage(m => m.Item1, w => w.Item2);

        }

        public decimal PodilSmluvBezCeny
        { 
            get  {
                if (this.Koncentrace.PocetSmluv == 0)
                    return 0m;

                return (decimal)this.Koncentrace.PocetSmluvBezCeny / (decimal)this.Koncentrace.PocetSmluv;
            }
        }
    } 

}
