using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class Firma
    {
        public partial class Statistics
        {

            static Dictionary<int, Util.Cache.CouchbaseCacheManager<Analytics.StatisticsSubjectPerYear<Statistics.RegistrSmluv>, Firma>> registrSmluvCaches
                = new Dictionary<int, Util.Cache.CouchbaseCacheManager<Analytics.StatisticsSubjectPerYear<RegistrSmluv>, Firma>>();
            static object _registrSmluvCachesLock = new object();
            internal static Util.Cache.CouchbaseCacheManager<Analytics.StatisticsSubjectPerYear<Statistics.RegistrSmluv>, Firma> RegistrSmluvCache(int? obor)
            {
                obor = obor ?? 0;
                if (registrSmluvCaches.ContainsKey(obor.Value) == false)
                {
                    lock (_registrSmluvCachesLock)
                    {
                        if (registrSmluvCaches.ContainsKey(obor.Value) == false)
                        {
                            registrSmluvCaches.Add(obor.Value,
                                Util.Cache.CouchbaseCacheManager<Analytics.StatisticsSubjectPerYear<Statistics.RegistrSmluv>, Firma>
                                .GetSafeInstance("Firma_SmlouvyStatistics_"+obor.Value.ToString(),
                                    (firma) => RegistrSmluv.Create(firma, null),
                                    TimeSpan.FromHours(12), f => f.ICO)
                                );
                        }
                    }
                }
                return registrSmluvCaches[obor.Value];
            }


            public partial class RegistrSmluv
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



                public static Analytics.StatisticsSubjectPerYear<RegistrSmluv> Create(Firma f, int? obor)
                {

                    Dictionary<int, Lib.Analysis.BasicData> _calc_SeZasadnimNedostatkem = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} and chyby:zasadni", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_UzavrenoOVikendu = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND (hint.denUzavreni:>0)", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_ULimitu = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND ( hint.smlouvaULimitu:>0 )", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_NovaFirmaDodavatel = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND ( hint.pocetDniOdZalozeniFirmy:>-50 AND hint.pocetDniOdZalozeniFirmy:<30 )", Lib.Analytics.Consts.RegistrSmluvYearsList);


                    Dictionary<int, RegistrSmluv> data = new Dictionary<int, RegistrSmluv>();
                    foreach (var year in Lib.Analytics.Consts.RegistrSmluvYearsList)
                    {
                        var stat = f.Statistic().RatingPerYear[year];
                        data.Add(year, new RegistrSmluv()
                        {
                            PocetSmluv = f.Statistic().BasicStatPerYear[year].Pocet,
                            CelkovaHodnotaSmluv = f.Statistic().BasicStatPerYear[year].CelkemCena,
                            PocetSmluvBezCeny = stat.NumBezCeny,
                            PocetSmluvBezSmluvniStrany = stat.NumBezSmluvniStrany,
                            PocetSmluvPolitiky = stat.NumSPolitiky,
                            PercentSmluvBezCeny = stat.PercentBezCeny,
                            PercentSmluvBezSmluvniStrany = stat.PercentBezSmluvniStrany,
                            PercentKcBezSmluvniStrany = stat.PercentKcBezSmluvniStrany,
                            PercentKcSmluvPolitiky = stat.PercentKcSPolitiky,
                            PercentSmluvPolitiky = stat.PercentSPolitiky,
                            SumKcSmluvBezSmluvniStrany = stat.SumKcBezSmluvniStrany,
                            SumKcSmluvPolitiky = stat.SumKcSPolitiky,
                            PocetSmluvULimitu = _calc_ULimitu[year].Pocet,
                            PocetSmluvOVikendu = _calc_UzavrenoOVikendu[year].Pocet,
                            PocetSmluvSeZasadnimNedostatkem = _calc_SeZasadnimNedostatkem[year].Pocet,
                            PocetSmluvNovaFirma = _calc_NovaFirmaDodavatel[year].Pocet,
                        }
                        );
                    }
                    return new Analytics.StatisticsSubjectPerYear<Statistics.RegistrSmluv>(f.ICO, data);

                }
            }
        }
    }
}
