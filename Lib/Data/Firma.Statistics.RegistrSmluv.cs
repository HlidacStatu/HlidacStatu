using HlidacStatu.Lib.Analytics;
using System;
using System.Collections.Generic;

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
                                .GetSafeInstance("Firma_SmlouvyStatistics_" + obor.Value.ToString(),
                                    (firma) => RegistrSmluv.Create(firma, null),
                                    TimeSpan.FromHours(12),
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseServers"].Split(','),
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseBucket"],
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseUsername"],
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbasePassword"],
                                    f => f.ICO)
                                );
                        }
                    }
                }
                return registrSmluvCaches[obor.Value];
            }

            public partial class RegistrSmluv : IAddable<RegistrSmluv>
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

                public decimal PercentSmluvBezCeny()
                {
                    return (decimal)PocetSmluvBezCeny / PocetSmluv;
                }
                public decimal PercentSmluvBezSmluvniStrany()
                {
                    return (decimal)PocetSmluvBezSmluvniStrany / PocetSmluv;
                }
                public decimal PercentKcBezSmluvniStrany()
                {
                    return (decimal)SumKcSmluvBezSmluvniStrany / CelkovaHodnotaSmluv;
                }
                public decimal PercentSmluvPolitiky()
                {
                    return (decimal)PocetSmluvPolitiky / PocetSmluv;
                }
                public decimal PercentKcSmluvPolitiky()
                {
                    return (decimal)SumKcSmluvPolitiky / CelkovaHodnotaSmluv;
                }

                public static Analytics.StatisticsSubjectPerYear<RegistrSmluv> Create(Firma f, int? obor)
                {
                    if (obor.HasValue)
                        return Create($"ico:{f.ICO} AND oblast:{Smlouva.SClassification.Classification.ClassifSearchQuery(obor.Value)}", f.ICO);
                    else
                        return Create($"ico:{f.ICO}", f.ICO);
                }

                public static Analytics.StatisticsSubjectPerYear<RegistrSmluv> Create(string query, string ico)
                {

                    Dictionary<int, Lib.Analysis.BasicData> _calc_SeZasadnimNedostatkem =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND chyby:zasadni", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_UzavrenoOVikendu =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND (hint.denUzavreni:>0)", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_ULimitu =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( hint.smlouvaULimitu:>0 )", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_NovaFirmaDodavatel =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( hint.pocetDniOdZalozeniFirmy:>-50 AND hint.pocetDniOdZalozeniFirmy:<30 )", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_smlouvy =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_bezCeny =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( issues.issueTypeId:100 ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_bezSmlStran =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( issues.issueTypeId:18 OR issues.issueTypeId:12 ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_sVazbouNaPolitikyNedavne =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( sVazbouNaPolitikyNedavne:true ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, RegistrSmluv> data = new Dictionary<int, RegistrSmluv>();
                    foreach (var year in Lib.Analytics.Consts.RegistrSmluvYearsList)
                    {
                        data.Add(year, new RegistrSmluv()
                        {
                            PocetSmluv = _calc_smlouvy[year].Pocet,
                            CelkovaHodnotaSmluv = _calc_smlouvy[year].CelkemCena,
                            PocetSmluvBezCeny = _calc_bezCeny[year].Pocet,
                            PocetSmluvBezSmluvniStrany = _calc_bezSmlStran[year].Pocet,
                            PocetSmluvPolitiky = _calc_sVazbouNaPolitikyNedavne[year].Pocet,
                            SumKcSmluvBezSmluvniStrany = _calc_bezSmlStran[year].CelkemCena,
                            SumKcSmluvPolitiky = _calc_sVazbouNaPolitikyNedavne[year].CelkemCena,
                            PocetSmluvULimitu = _calc_ULimitu[year].Pocet,
                            PocetSmluvOVikendu = _calc_UzavrenoOVikendu[year].Pocet,
                            PocetSmluvSeZasadnimNedostatkem = _calc_SeZasadnimNedostatkem[year].Pocet,
                            PocetSmluvNovaFirma = _calc_NovaFirmaDodavatel[year].Pocet,
                        }
                        );
                    }
                    return new Analytics.StatisticsSubjectPerYear<Statistics.RegistrSmluv>(ico, data);

                }

                public RegistrSmluv Add(RegistrSmluv other)
                {
                    return new RegistrSmluv()
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
            }
        }
    }
}
