using HlidacStatu.Lib.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data
{
    public partial class Firma
    {
        public partial class Statistics
        {

            internal static Util.Cache.CouchbaseCacheManager<Analytics.StatisticsSubjectPerYear<Statistics.Dotace>, Firma> DotaceCache()
            {

                var cache = Util.Cache.CouchbaseCacheManager<Analytics.StatisticsSubjectPerYear<Statistics.Dotace>, Firma>
                    .GetSafeInstance("Firma_DotaceStatistics",
                        (firma) => Dotace.Create(firma),
                        TimeSpan.FromHours(12),
                        System.Configuration.ConfigurationManager.AppSettings["CouchbaseServers"].Split(','),
                        System.Configuration.ConfigurationManager.AppSettings["CouchbaseBucket"],
                        System.Configuration.ConfigurationManager.AppSettings["CouchbaseUsername"],
                        System.Configuration.ConfigurationManager.AppSettings["CouchbasePassword"],
                        f => f.ICO);

                return cache;
            }

            public partial class Dotace : CoreStat, IAddable<Dotace>
            {
                public int PocetDotaci { get; set; } = 0;
                public int PocetCerpani { get; set; } = 0;
                public decimal CelkemCerpano { get; set; } = 0;

                public static Analytics.StatisticsSubjectPerYear<Statistics.Dotace> Create(Firma f)
                {
                    var dotaceService = new Data.Dotace.DotaceService();
                    var dotaceFirmy = dotaceService.GetDotaceForIco(f.ICO);

                    // doplnit počty dotací
                    var statistiky = dotaceFirmy.GroupBy(d => d.DatumPodpisu?.Year)
                        .ToDictionary(g => g.Key ?? 0,
                            g => new Statistics.Dotace()
                            {
                                PocetDotaci = g.Count()
                            }
                        );

                    var cerpani = dotaceFirmy
                        .SelectMany(d => d.Rozhodnuti)
                        .SelectMany(r => r.Cerpani);

                    var dataYearly = cerpani
                        .GroupBy(c => c.GuessedYear)
                        .ToDictionary(g => g.Key ?? 0,
                            g => (CelkemCerpano: g.Sum(c => c.CastkaSpotrebovana ?? 0),
                                PocetCerpani: g.Count(c => c.CastkaSpotrebovana.HasValue))
                            );

                    foreach(var dy in dataYearly)
                    {
                        if (!statistiky.TryGetValue(dy.Key, out var yearstat))
                        {
                            yearstat = new Statistics.Dotace();
                            statistiky.Add(dy.Key, yearstat);
                        }

                        yearstat.CelkemCerpano = dy.Value.CelkemCerpano;
                        yearstat.PocetCerpani = dy.Value.PocetCerpani;
                    }


                    return new Analytics.StatisticsSubjectPerYear<Statistics.Dotace>(f.ICO, statistiky);
                }

                public Dotace Add(Dotace other)
                {
                    return new Dotace()
                    {
                        CelkemCerpano = CelkemCerpano + (other?.CelkemCerpano ?? 0),
                        PocetCerpani = PocetCerpani + (other?.PocetCerpani ?? 0),
                        PocetDotaci = PocetDotaci + (other?.PocetDotaci ?? 0)
                    };
                }

                public override int NewSeasonStartMonth()
                {
                    return 1;
                }

                public override int UsualFirstYear()
                {
                    return 2000;
                }
            }
        }
    }
}
