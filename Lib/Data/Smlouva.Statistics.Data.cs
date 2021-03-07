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

                public long PocetSmluvBezCeny { get; set; } = 0;
                public long PocetSmluvBezSmluvniStrany { get; set; } = 0;
                public decimal SumKcSmluvBezSmluvniStrany { get; set; } = 0;

                public long PocetSmluvSponzorujiciFirmy { get; set; } = 0;
                public long PocetSmluvBezCenySponzorujiciFirmy  { get; set; }
                public decimal SumKcSmluvSponzorujiciFirmy { get; set; } = 0;

                public long PocetSmluvSeZasadnimNedostatkem { get; set; }
                public long PocetSmluvULimitu { get; set; }
                public long PocetSmluvOVikendu { get; set; }
                public long PocetSmluvNovaFirma { get; set; }

                public Dictionary<int, Analysis.BasicData> PoOblastech { get; set; } = new Dictionary<int, Analysis.BasicData>();


                public decimal PercentSmluvBezCeny => (PocetSmluv == 0 ? 0 : (decimal)PocetSmluvBezCeny / (decimal)PocetSmluv);

                public decimal PercentSmluvBezSmluvniStrany => (PocetSmluv == 0 ? 0 : (decimal)PocetSmluvBezSmluvniStrany / (decimal)PocetSmluv);

                public decimal PercentKcBezSmluvniStrany => (CelkovaHodnotaSmluv == 0 ? 0 : (decimal)SumKcSmluvBezSmluvniStrany / (decimal)CelkovaHodnotaSmluv);

                public decimal PercentSmluvPolitiky => (PocetSmluv == 0 ? 0 : (decimal)PocetSmluvSponzorujiciFirmy / (decimal)PocetSmluv);

                public decimal PercentKcSmluvPolitiky => (CelkovaHodnotaSmluv == 0 ? 0 : (decimal)SumKcSmluvSponzorujiciFirmy / (decimal)CelkovaHodnotaSmluv);


                public Data Add(Data other)
                {
                    var d = new Data()
                    {
                        PocetSmluv = PocetSmluv + (other?.PocetSmluv ?? 0),
                        CelkovaHodnotaSmluv = CelkovaHodnotaSmluv + (other?.CelkovaHodnotaSmluv ?? 0),
                        PocetSmluvSeSoukromymSubj = PocetSmluvSeSoukromymSubj + (other?.PocetSmluvSeSoukromymSubj ?? 0),
                        CelkovaHodnotaSmluvSeSoukrSubj = CelkovaHodnotaSmluvSeSoukrSubj + (other?.CelkovaHodnotaSmluvSeSoukrSubj ?? 0),
                        PocetSmluvBezCenySeSoukrSubj = PocetSmluvBezCenySeSoukrSubj + (other?.PocetSmluvBezCenySeSoukrSubj ?? 0),
                        PocetSmluvBezCeny = PocetSmluvBezCeny + (other?.PocetSmluvBezCeny ?? 0),
                        PocetSmluvBezSmluvniStrany = PocetSmluvBezSmluvniStrany + (other?.PocetSmluvBezSmluvniStrany ?? 0),
                        SumKcSmluvBezSmluvniStrany = SumKcSmluvBezSmluvniStrany + (other?.SumKcSmluvBezSmluvniStrany ?? 0),
                        PocetSmluvSponzorujiciFirmy = PocetSmluvSponzorujiciFirmy + (other?.PocetSmluvSponzorujiciFirmy ?? 0),
                        SumKcSmluvSponzorujiciFirmy = SumKcSmluvSponzorujiciFirmy + (other?.SumKcSmluvSponzorujiciFirmy ?? 0),
                        PocetSmluvSeZasadnimNedostatkem = PocetSmluvSeZasadnimNedostatkem + (other?.PocetSmluvSeZasadnimNedostatkem ?? 0),
                        PocetSmluvULimitu = PocetSmluvULimitu + (other?.PocetSmluvULimitu ?? 0),
                        PocetSmluvOVikendu = PocetSmluvOVikendu + (other?.PocetSmluvOVikendu ?? 0),
                        PocetSmluvNovaFirma = PocetSmluvNovaFirma + (other?.PocetSmluvNovaFirma ?? 0),
                        PoOblastech = this.PoOblastech
                    };

                    if (other.PoOblastech != null)
                        foreach (var o in other.PoOblastech)
                        {
                            if (d.PoOblastech.ContainsKey(o.Key))
                            {
                                d.PoOblastech[o.Key].Pocet = d.PoOblastech[o.Key].Pocet + o.Value.Pocet;
                                d.PoOblastech[o.Key].CelkemCena = d.PoOblastech[o.Key].CelkemCena + o.Value.CelkemCena;
                            }
                            else
                                d.PoOblastech.Add(o.Key, o.Value);
                        }

                    return d;
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
