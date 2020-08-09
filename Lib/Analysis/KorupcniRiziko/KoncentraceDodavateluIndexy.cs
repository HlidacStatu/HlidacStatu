using System;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class KoncentraceDodavateluIndexy
    {
        [Nest.Date]
        public DateTime LastUpdated { get; set; }

        public decimal Herfindahl_Hirschman_Index { get; set; }
        public decimal Herfindahl_Hirschman_Normalized { get; set; }
        public decimal Herfindahl_Hirschman_Modified { get; set; }
        public decimal Comprehensive_Industrial_Concentration_Index { get; set; }
        public decimal Hall_Tideman_Index { get; set; }
        public decimal Kwoka_Dominance_Index { get; set; }

        public decimal PrumernaHodnotaSmluvProVypocet { get; set; } //pouze tech s hodnotou vyssi nez 0 Kc
        public decimal HodnotaSmluvProVypocet { get; set; } //pouze tech s hodnotou vyssi nez 0 Kc
        public int PocetSmluvProVypocet { get; set; } 
        public int PocetSmluvBezCenyProVypocet { get; set; }
        [Nest.Keyword]
        public string Query { get; set; }

        [Nest.Keyword]
        public string Popis { get; set; }

    }

}
