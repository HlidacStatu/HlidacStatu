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

            public static decimal PercentOf(long number, long total) => PercentOf((decimal)number, (decimal)total);
            public static decimal PercentOf(double number, double total) => PercentOf((decimal)number, (decimal)total);

            public static decimal PercentOf(decimal number, decimal total)
            {
                if (total == 0)
                    return 0;
                return number / total;
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

                    Dictionary<int, Lib.Analysis.BasicData> _calc_SeZasadnimNedostatkem =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} and chyby:zasadni", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_UzavrenoOVikendu =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND (hint.denUzavreni:>0)", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_ULimitu =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND ( hint.smlouvaULimitu:>0 )", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_NovaFirmaDodavatel =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND ( hint.pocetDniOdZalozeniFirmy:>-50 AND hint.pocetDniOdZalozeniFirmy:<30 )", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_smlouvy =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_bezCeny =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND ( issues.issueTypeId:100 ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_bezSmlStran =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND ( issues.issueTypeId:18 OR issues.issueTypeId:12 ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_sVazbouNaPolitikyNedavne =
                        Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND ( sVazbouNaPolitikyNedavne:true ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

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
                            SumKcSmluvBezSmluvniStrany = _calc_bezSmlStran[year].Pocet,
                            SumKcSmluvPolitiky = _calc_sVazbouNaPolitikyNedavne[year].CelkemCena,
                            PocetSmluvULimitu = _calc_ULimitu[year].Pocet,
                            PocetSmluvOVikendu = _calc_UzavrenoOVikendu[year].Pocet,
                            PocetSmluvSeZasadnimNedostatkem = _calc_SeZasadnimNedostatkem[year].Pocet,
                            PocetSmluvNovaFirma = _calc_NovaFirmaDodavatel[year].Pocet,

                            PercentSmluvBezCeny = PercentOf(_calc_bezCeny[year].Pocet, _calc_smlouvy[year].Pocet),
                            PercentSmluvBezSmluvniStrany = PercentOf(_calc_bezSmlStran[year].Pocet, _calc_smlouvy[year].Pocet),
                            PercentKcBezSmluvniStrany = PercentOf(_calc_bezCeny[year].CelkemCena, _calc_smlouvy[year].CelkemCena),
                            PercentKcSmluvPolitiky = PercentOf(_calc_sVazbouNaPolitikyNedavne[year].CelkemCena, _calc_smlouvy[year].CelkemCena),
                            PercentSmluvPolitiky = PercentOf(_calc_sVazbouNaPolitikyNedavne[year].Pocet, _calc_smlouvy[year].Pocet),

                        }
                        );
                    }
                    return new Analytics.StatisticsSubjectPerYear<Statistics.RegistrSmluv>(f.ICO, data);

                }
            }
        }
    }
}
