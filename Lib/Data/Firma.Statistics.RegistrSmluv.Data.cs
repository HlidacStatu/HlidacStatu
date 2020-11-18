namespace HlidacStatu.Lib.Data
{
    public partial class Firma
    {
        public partial class Statistics
        {
            public partial class RegistrSmluv
            {
                public class Data
                {
                    public long PocetSmluv { get; set; } = 0;
                    public decimal CelkovaHodnotaSmluv { get; set; } = 0;
                    public long PocetSmluvSeSoukromymSubj { get; set; }
                    public decimal CelkovaHodnotaSmluvSeSoukrSubj { get; set; } = 0;
                    public long PocetSmluvBezCenySeSoukrSubj { get; set; }
                    public decimal PrumernaHodnotaSmluvSeSoukrSubj { get; set; }

                    public long PocetSmluvBezCeny { get; set; } = 0;
                    public decimal PercentSmluvBezCeny { get; set; } = 0;
                    public long PocetSmluvBezSmluvniStrany { get; set; } = 0;
                    public decimal SumKcSmluvBezSmluvniStrany { get; set; } = 0;
                    public decimal PercentSmluvBezSmluvniStrany { get; set; } = 0;
                    public decimal PercentKcBezSmluvniStrany { get; set; } = 0;

                    public long PocetSmluvPolitiky { get; set; } = 0;
                    public decimal PercentSmluvPolitiky { get; set; } = 0;
                    public decimal SumKcSmluvPolitiky { get; set; } = 0;
                    public decimal PercentKcSmluvPolitiky { get; set; } = 0;

                    public long PocetSmluvSeZasadnimNedostatkem { get; set; }
                    public long PocetSmluvULimitu { get; set; }
                    public long PocetSmluvOVikendu { get; set; }
                    public long PocetSmluvNovaFirma { get; set; }
                }
            }
        }
    }
}
