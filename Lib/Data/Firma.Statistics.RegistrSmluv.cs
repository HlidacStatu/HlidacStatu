using HlidacStatu.Lib.Analytics;
using System;
using System.Collections.Generic;

namespace HlidacStatu.Lib.Data
{
    public partial class Firma
    {
        public partial class Statistics
        {

            static Dictionary<int, Util.Cache.CouchbaseCacheManager<Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data>, Firma>> registrSmluvCaches
                = new Dictionary<int, Util.Cache.CouchbaseCacheManager<Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data>, Firma>>();
            static object _registrSmluvCachesLock = new object();

            internal static Util.Cache.CouchbaseCacheManager<Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data>, Firma> RegistrSmluvCache(int? obor)
            {
                obor = obor ?? 0;
                if (registrSmluvCaches.ContainsKey(obor.Value) == false)
                {
                    lock (_registrSmluvCachesLock)
                    {
                        if (registrSmluvCaches.ContainsKey(obor.Value) == false)
                        {
                            registrSmluvCaches.Add(obor.Value,
                                Util.Cache.CouchbaseCacheManager<Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data>, Firma>
                                .GetSafeInstance("Firma_SmlouvyStatistics_" + obor.Value.ToString(),
                                    (firma) => Create(firma, obor),
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

            public static Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data> Create(Firma f, int? obor)
            {
                StatisticsSubjectPerYear<Smlouva.Statistics.Data> res = null;
                if (obor.HasValue && obor != 0)
                    res = new StatisticsSubjectPerYear<Smlouva.Statistics.Data>(
                        f.ICO,
                        Smlouva.Statistics.Create($"ico:{f.ICO} AND oblast:{Smlouva.SClassification.Classification.ClassifSearchQuery(obor.Value)}")
                        );
                else
                    res = new StatisticsSubjectPerYear<Smlouva.Statistics.Data>(
                         f.ICO,
                         Smlouva.Statistics.Create($"ico:{f.ICO}")
                        );

                return res;
            }

        }
    }
}
