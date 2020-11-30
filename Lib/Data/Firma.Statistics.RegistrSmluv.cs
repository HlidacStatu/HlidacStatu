using HlidacStatu.Lib.Analytics;
using System;
using System.Collections.Generic;

namespace HlidacStatu.Lib.Data
{
    public partial class Firma
    {
        public partial class Statistics
        {

            static Util.Cache.CouchbaseCacheManager<Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data>, (Firma firma, int? obor)> _cache
                = Util.Cache.CouchbaseCacheManager<Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data>, (Firma firma, int? obor)>
                                .GetSafeInstance("Firma_SmlouvyStatistics_",
                                    (obj) => CalculateStats(obj.firma, obj.obor),
                                    TimeSpan.FromHours(12),
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseServers"].Split(','),
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseBucket"],
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseUsername"],
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbasePassword"],
                                    obj => obj.firma.ICO + "-"+(obj.obor??0));


            public static Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data> CachedStatistics(Firma firma, int? obor)
            {
                return _cache.Get( (firma, obor) );
            }


            public static Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data> CalculateStats(Firma f, int? obor)
            {
                StatisticsSubjectPerYear<Smlouva.Statistics.Data> res = null;
                if (obor.HasValue && obor != 0)
                    res = new StatisticsSubjectPerYear<Smlouva.Statistics.Data>(
                        f.ICO,
                        Smlouva.Statistics.Calculate($"ico:{f.ICO} AND oblast:{Smlouva.SClassification.Classification.ClassifSearchQuery(obor.Value)}")
                        );
                else
                    res = new StatisticsSubjectPerYear<Smlouva.Statistics.Data>(
                         f.ICO,
                         Smlouva.Statistics.Calculate($"ico:{f.ICO}")
                        );

                return res;
            }

        }
    }
}
