namespace FullTextSearch
{
    public class Options
    {
        public static Options DefaultOptions()
        {
            var opt = new Options();
            opt.FirstWordsBonus = new FirstWordsBonus
            {
                BonusWordsCount = 5,
                MaxBonusMultiplier = 0.6,
                BonusMultiplierDegradation = 0.1
            };

            opt.WholeWordBonusMultiplier = 1.3;

            opt.ExactMatchBonus = 20;

            opt.AlmostExactMatchBonus = 10;

            return opt;
        }

        public FirstWordsBonus FirstWordsBonus { get; set; }
        public double? WholeWordBonusMultiplier { get; set; }
        public double? ExactMatchBonus { get; set; }
        public double? AlmostExactMatchBonus { get; set; }

    }

    public class FirstWordsBonus
    {
        public double BonusWordsCount { get; set; }
        public double MaxBonusMultiplier { get; set; }
        public double BonusMultiplierDegradation { get; set; }
    }
}
