using HlidacStatu.Lib.Analytics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class Smlouva
    {
        public partial class Statistics
        {
            public partial class Data : HlidacStatu.Lib.Analytics.CoreStat, IAddable<Data>
            {
                public long PocetSmluv { get; set; } = 0;
                public decimal CelkovaHodnotaSmluv { get; set; } = 0;
                public long PocetSmluvSeSoukromymSubj { get; set; }
                public decimal CelkovaHodnotaSmluvSeSoukrSubj { get; set; } = 0;
                public long PocetSmluvBezCenySeSoukrSubj { get; set; }
                public decimal PrumernaHodnotaSmluvSeSoukrSubj { get; set; }

                public long PocetSmluvBezCeny { get; set; } = 0;
                public long PocetSmluvBezSmluvniStrany { get; set; } = 0;
                public decimal SumKcSmluvBezSmluvniStrany { get; set; } = 0;

                public long PocetSmluvPolitiky { get; set; } = 0;
                public decimal SumKcSmluvPolitiky { get; set; } = 0;

                public long PocetSmluvSeZasadnimNedostatkem { get; set; }
                public long PocetSmluvULimitu { get; set; }
                public long PocetSmluvOVikendu { get; set; }
                public long PocetSmluvNovaFirma { get; set; }

                public decimal PercentSmluvBezCeny => (PocetSmluv == 0 ? 0 : (decimal)PocetSmluvBezCeny / (decimal)PocetSmluv);

                public decimal PercentSmluvBezSmluvniStrany => (PocetSmluv == 0 ? 0 : (decimal)PocetSmluvBezSmluvniStrany / (decimal)PocetSmluv);

                public decimal PercentKcBezSmluvniStrany => (CelkovaHodnotaSmluv == 0 ? 0 : (decimal)SumKcSmluvBezSmluvniStrany / (decimal)CelkovaHodnotaSmluv);

                public decimal PercentSmluvPolitiky => (PocetSmluv == 0 ? 0 : (decimal)PocetSmluvPolitiky / (decimal)PocetSmluv);

                public decimal PercentKcSmluvPolitiky => (CelkovaHodnotaSmluv == 0 ? 0 : (decimal)SumKcSmluvPolitiky / (decimal)CelkovaHodnotaSmluv);

                public Data Add(Data other)
                {
                    return new Data()
                    {
                        PocetSmluv = PocetSmluv + (other?.PocetSmluv ?? 0),
                        CelkovaHodnotaSmluv = CelkovaHodnotaSmluv + (other?.CelkovaHodnotaSmluv ?? 0),
                        PocetSmluvSeSoukromymSubj = PocetSmluvSeSoukromymSubj + (other?.PocetSmluvSeSoukromymSubj ?? 0),
                        CelkovaHodnotaSmluvSeSoukrSubj = CelkovaHodnotaSmluvSeSoukrSubj + (other?.CelkovaHodnotaSmluvSeSoukrSubj ?? 0),
                        PocetSmluvBezCenySeSoukrSubj = PocetSmluvBezCenySeSoukrSubj + (other?.PocetSmluvBezCenySeSoukrSubj ?? 0),
                        PrumernaHodnotaSmluvSeSoukrSubj = PrumernaHodnotaSmluvSeSoukrSubj + (other?.PrumernaHodnotaSmluvSeSoukrSubj ?? 0),
                        PocetSmluvBezCeny = PocetSmluvBezCeny + (other?.PocetSmluvBezCeny ?? 0),
                        PocetSmluvBezSmluvniStrany = PocetSmluvBezSmluvniStrany + (other?.PocetSmluvBezSmluvniStrany ?? 0),
                        SumKcSmluvBezSmluvniStrany = SumKcSmluvBezSmluvniStrany + (other?.SumKcSmluvBezSmluvniStrany ?? 0),
                        PocetSmluvPolitiky = PocetSmluvPolitiky + (other?.PocetSmluvPolitiky ?? 0),
                        SumKcSmluvPolitiky = SumKcSmluvPolitiky + (other?.SumKcSmluvPolitiky ?? 0),
                        PocetSmluvSeZasadnimNedostatkem = PocetSmluvSeZasadnimNedostatkem + (other?.PocetSmluvSeZasadnimNedostatkem ?? 0),
                        PocetSmluvULimitu = PocetSmluvULimitu + (other?.PocetSmluvULimitu ?? 0),
                        PocetSmluvOVikendu = PocetSmluvOVikendu + (other?.PocetSmluvOVikendu ?? 0),
                        PocetSmluvNovaFirma = PocetSmluvNovaFirma + (other?.PocetSmluvNovaFirma ?? 0),

                    };
                }

                public override int NewSeasonStartMonth()
                {
                    return 4;
                }

                public override int UsualFirstYear()
                {
                    return 2017;
                }
            }

        }

    }
}
