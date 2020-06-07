namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class KoncentraceDodavateluIndexy
    {
        public decimal Herfindahl_Hirschman_Index { get; set; }
        public decimal Herfindahl_Hirschman_Normalized { get; set; }
        public decimal Herfindahl_Hirschman_Modified { get; set; }
        public decimal Comprehensive_Industrial_Concentration_Index { get; set; }
        public decimal Hall_Tideman_Index { get; set; }
        public decimal Kwoka_Dominance_Index { get; set; }

        public decimal PrumernaHodnotaSmluv { get; set; } //pouze tech s hodnotou vyssi nez 0 Kc
        public decimal CelkovaHodnotaSmluv { get; set; } //pouze tech s hodnotou vyssi nez 0 Kc
        public int PocetSmluv { get; set; } 
        public int PocetSmluvBezCeny { get; set; }
        [Nest.Keyword]
        public string Query { get; set; }
    }

}
