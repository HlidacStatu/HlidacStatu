namespace HlidacStatu.Lib.Analysis
{
    public class RatingData
    {
        public static RatingData Empty() { return new RatingData(); }

        public long NumBezCeny { get; set; } = 0;
        public decimal PercentBezCeny { get; set; } = 0;
        public long NumBezSmluvniStrany { get; set; } = 0;
        public decimal SumKcBezSmluvniStrany { get; set; } = 0;
        public decimal PercentBezSmluvniStrany { get; set; } = 0;
        public decimal PercentKcBezSmluvniStrany { get; set; } = 0;

        public long NumSPolitiky { get; set; } = 0;
        public decimal PercentSPolitiky { get; set; } = 0;
        public decimal SumKcSPolitiky { get; set; } = 0;
        public decimal PercentKcSPolitiky { get; set; } = 0;


    }

}
