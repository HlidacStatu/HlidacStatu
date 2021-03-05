using HlidacStatu.Lib.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using HlidacStatu.Lib.Analytics;

namespace HlidacStatu.Lib.Data
{
    public partial class Osoba
    {
        public partial class Statistics
        {
            public class RegistrSmluv
            {

                public string OsobaNameId { get; set; }
                public Relation.AktualnostType Aktualnost { get; set; }
                public Smlouva.SClassification.ClassificationsTypes? Obor { get; set; } = null;

                public Dictionary<string, StatisticsSubjectPerYear<Smlouva.Statistics.Data>> StatniFirmy { get; set; }
                public Dictionary<string, StatisticsSubjectPerYear<Smlouva.Statistics.Data>> SoukromeFirmy { get; set; }

                string[] _neziskovkyIcos = null;
                public string[] Neziskovky()
                {
                    if (_neziskovkyIcos == null)
                    {
                        _neziskovkyIcos = this.SoukromeFirmy
                            .Select(m => Data.Firmy.Get(m.Key))
                            .Where(ff => ff.JsemNeziskovka())
                            .Select(s => s.ICO)
                            .ToArray();
                    }
                    return _neziskovkyIcos;
                }

                public int NeziskovkyCount() => Neziskovky().Count();

                public int KomercniFirmyCount()
                {
                    return this.SoukromeFirmy.Count - NeziskovkyCount();
                }

                StatisticsPerYear<Smlouva.Statistics.Data> _soukromeFirmySummary = null;
                public StatisticsPerYear<Smlouva.Statistics.Data> SoukromeFirmySummary()
                {
                    if (_soukromeFirmySummary == null)
                        _soukromeFirmySummary = this.SoukromeFirmy.Values.AggregateStats();

                    return _soukromeFirmySummary;
                }

                StatisticsPerYear<Smlouva.Statistics.Data> _statniFirmySummary = null;
                public StatisticsPerYear<Smlouva.Statistics.Data> StatniFirmySummary()
                {
                    if (_statniFirmySummary == null)
                        _statniFirmySummary = this.StatniFirmy.Values.AggregateStats();

                    return _statniFirmySummary;
                }

                StatisticsPerYear<Smlouva.Statistics.Data> _neziskovkySummary = null;
                public StatisticsPerYear<Smlouva.Statistics.Data> NeziskovkySummary()
                {
                    if (_neziskovkySummary == null)
                        _neziskovkySummary = this.SoukromeFirmy
                            .Where(k => Neziskovky().Contains(k.Key))
                            .Select(m => m.Value)
                            .AggregateStats();

                    return _neziskovkySummary;
                }

            }
            static Util.Cache.CouchbaseCacheManager<RegistrSmluv, (Osoba os, int aktualnost, int? obor)> _cache
                = Util.Cache.CouchbaseCacheManager<RegistrSmluv, (Osoba os, int aktualnost, int? obor)>
                                .GetSafeInstance("Osoba_SmlouvyStatistics_v1_",
                                    (obj) => Calculate(obj.os, (Relation.AktualnostType)obj.aktualnost, obj.obor),
                                    TimeSpan.FromHours(12),
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseServers"].Split(','),
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseBucket"],
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseUsername"],
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbasePassword"],
                                    obj => $"{obj.os.NameId}/{obj.aktualnost}/{(obj.obor ?? 0)}");


            public static RegistrSmluv CachedStatistics(Osoba os, Relation.AktualnostType aktualnost, int? obor)
            {
                return _cache.Get((os, (int)aktualnost, obor));
            }


            public static RegistrSmluv Calculate(Osoba o, Data.Relation.AktualnostType aktualnost, int? obor)
            {
                RegistrSmluv res = new RegistrSmluv();
                res.OsobaNameId = o.NameId;
                res.Aktualnost = aktualnost;
                res.Obor = (Smlouva.SClassification.ClassificationsTypes?)obor;

                Dictionary<string, StatisticsSubjectPerYear<Smlouva.Statistics.Data>> statni = new Dictionary<string, StatisticsSubjectPerYear<Smlouva.Statistics.Data>>();
                Dictionary<string, StatisticsSubjectPerYear<Smlouva.Statistics.Data>> soukr = new Dictionary<string, StatisticsSubjectPerYear<Smlouva.Statistics.Data>>();

                var perIcoStat = o.AktualniVazby(aktualnost)
                    .Where(v => !string.IsNullOrEmpty(v.To?.UniqId)
                                && v.To.Type == HlidacStatu.Lib.Data.Graph.Node.NodeType.Company)
                    .Select(v => v.To)
                    .Distinct(new HlidacStatu.Lib.Data.Graph.NodeComparer())
                    .Select(f => Firmy.Get(f.Id))
                    .Where(f => f.Valid == true)
                    .Select(f => new { f = f, ss = f.StatistikaRegistruSmluv(obor) });


                foreach (var it in perIcoStat)
                {
                    if (it.f.PatrimStatu())
                        statni.Add(it.f.ICO, it.ss);
                    else
                        soukr.Add(it.f.ICO, it.ss);
                }
                res.StatniFirmy = statni;
                res.SoukromeFirmy = soukr;

                return res;
            }

        }
    }
}
