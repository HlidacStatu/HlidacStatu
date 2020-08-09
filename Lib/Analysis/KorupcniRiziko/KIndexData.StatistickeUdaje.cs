namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public partial class KIndexData
    {
        public class StatistickeUdaje
        {
            public long PocetSmluv { get; set; } = 0;
            public decimal CelkovaHodnotaSmluv { get; set; } = 0;
            public long PocetSmluvSeSoukromymSubj { get; set; }
            public decimal CelkovaHodnotaSmluvSeSoukromymSubj { get; set; } = 0;
            public long PocetSmluvBezCenySeSoukromymSubj { get; set; }


            public long PocetBezCeny { get; set; } = 0;
            public decimal PercentBezCeny { get; set; } = 0;
            public long PocetBezSmluvniStrany { get; set; } = 0;
            public decimal SumKcBezSmluvniStrany { get; set; } = 0;
            public decimal PercentBezSmluvniStrany { get; set; } = 0;
            public decimal PercentKcBezSmluvniStrany { get; set; } = 0;

            public long PocetSPolitiky { get; set; } = 0;
            public decimal PercentSPolitiky { get; set; } = 0;
            public decimal SumKcSPolitiky { get; set; } = 0;
            public decimal PercentKcSPolitiky { get; set; } = 0;



        }

    }
}
