using HlidacStatu.Lib.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data
{
    public partial class Osoba
    {
        public partial class Statistics
        {
            public class RegistrSmluv
            {
                public Dictionary<string, StatisticsSubjectPerYear<Smlouva.Statistics.Data>> StatniFirmy { get; set; }
                public Dictionary<string, StatisticsSubjectPerYear<Smlouva.Statistics.Data>> SoukromeFirmy { get; set; }

                int _neziskovkyCount = -1;
                public int NeziskovkyCount()
                {
                    if (_neziskovkyCount < 0)
                        _neziskovkyCount = this.SoukromeFirmy.Select(m => Data.Firmy.Get(m.Key)).Where(ff => ff.JsemNeziskovka()).Count();
                    return _neziskovkyCount;
                }

                public int KomercniFirmyCount()
                {
                    return this.SoukromeFirmy.Count - NeziskovkyCount();
                }

            }

            static Dictionary<int, Util.Cache.CouchbaseCacheManager<RegistrSmluv,Osoba>> registrSmluvCaches
                = new Dictionary<int, Util.Cache.CouchbaseCacheManager<RegistrSmluv, Osoba>>();

            static object _registrSmluvCachesLock = new object();

            internal static Util.Cache.CouchbaseCacheManager<RegistrSmluv, Osoba> RegistrSmluvCache(Data.Relation.AktualnostType aktualnost, int? obor)
            {
                obor = obor ?? 0;
                if (registrSmluvCaches.ContainsKey(obor.Value) == false)
                {
                    lock (_registrSmluvCachesLock)
                    {
                        if (registrSmluvCaches.ContainsKey(obor.Value) == false)
                        {
                            registrSmluvCaches.Add(obor.Value,
                                Util.Cache.CouchbaseCacheManager<RegistrSmluv, Osoba>
                                .GetSafeInstance($"Osoba_{aktualnost}_SmlouvyStatistics_{obor.Value.ToString()}",
                                    (osoba) => Create(osoba, aktualnost, obor),
                                    TimeSpan.FromHours(12),
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseServers"].Split(','),
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseBucket"],
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseUsername"],
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbasePassword"],
                                    o => o.InternalId.ToString())
                                );
                        }
                    }
                }
                return registrSmluvCaches[obor.Value];
            }

            public static RegistrSmluv Create(Osoba o, Data.Relation.AktualnostType aktualnost, int? obor)
            {
                RegistrSmluv res = null;

                Dictionary<string, StatisticsSubjectPerYear<Smlouva.Statistics.Data>> statni = new Dictionary<string, StatisticsSubjectPerYear<Smlouva.Statistics.Data>>();
                Dictionary<string, StatisticsSubjectPerYear<Smlouva.Statistics.Data>> soukr = new Dictionary<string, StatisticsSubjectPerYear<Smlouva.Statistics.Data>>();

                var perIcoStat = o.AktualniVazby(aktualnost)
                    .Where(v => !string.IsNullOrEmpty(v.To?.UniqId)
                                && v.To.Type == HlidacStatu.Lib.Data.Graph.Node.NodeType.Company)
                    .Select(v => v.To)
                    .Distinct(new HlidacStatu.Lib.Data.Graph.NodeComparer())
                    .Select(f => Firmy.Get(f.Id) )
                    .Where(f=>f.Valid == true)
                    .Select(f => new { f = f, ss = f.StatistikaRegistruSmluv(obor) });


                foreach (var it in perIcoStat)
                {
                    if (it.f.PatrimStatu())
                        statni.Add(it.f.ICO, it.ss);
                    else
                        soukr.Add(it.f.ICO, it.ss);
                }

                return res;
            }

        }
    }
}
