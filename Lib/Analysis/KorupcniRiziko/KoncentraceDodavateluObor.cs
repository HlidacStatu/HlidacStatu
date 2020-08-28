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

        public decimal SmluvBezCenyMalusKoeficient { get; set; } = 1;

        //public KoncentraceDodavateluIndexy KoncentraceBezUvedeneCeny { get; set; }

        public decimal Combined_Herfindahl_Hirschman_Modified()
        {
            //if (KoncentraceBezUvedeneCeny == null)
            //    return Koncentrace.Herfindahl_Hirschman_Modified;

            return Koncentrace.Herfindahl_Hirschman_Modified * SmluvBezCenyMalusKoeficient;

        }

        public decimal PodilSmluvBezCeny
        { 
            get  {
                if (this.Koncentrace.PocetSmluvProVypocet == 0)
                    return 0m;

                return (decimal)this.Koncentrace.PocetSmluvBezCenyProVypocet / (decimal)this.Koncentrace.PocetSmluvProVypocet;
            }
        }
    } 

}
