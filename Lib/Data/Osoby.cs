using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Util;
using HlidacStatu.Util.Cache;

namespace HlidacStatu.Lib.Data
{

    public class Osoby
    {
        internal static volatile MemoryCacheManager<IEnumerable<OsobaEvent>, int> CachedEvents
            = MemoryCacheManager<IEnumerable<OsobaEvent>, int>
                .GetSafeInstance("osobyEvents", 
                osobaInternalId =>
                {
                    using (DbEntities db = new DbEntities())
                    {
                        return db.OsobaEvent
                            .AsNoTracking()
                            .Where(m => m.OsobaId == osobaInternalId)
                            .ToArray();
                    }
                },
                TimeSpan.FromMinutes(2));

        internal static volatile MemoryCacheManager<IEnumerable<OsobaEvent>, int> CachedFirmySponzoring
            = MemoryCacheManager<IEnumerable<OsobaEvent>, int>
                .GetSafeInstance("osobyFirmySponzoring",
                osobaInternalId =>
                {
                    using (DbEntities db = new DbEntities())
                    {
                        var res = db.Sponzoring.SqlQuery(@"
                            select fe.* from Sponzoring fe with (nolock)
	                          join osobaVazby ov with (nolock) on ov.vazbakico=fe.IcoDarce 
                               and dbo.IsSomehowInInterval(fe.DarovanoDne,fe.DarovanoDne, ov.datumOd, ov.DatumDo)=1
                            and osobaid=" + osobaInternalId)
                            .AsNoTracking();
                        var res1 = res.Select(m =>
                        {
                            Osoba o = Osoby.GetById.Get(osobaInternalId);
                            //var v = o.VazbyProICO(m.ICO, m.DatumOd, m.DatumDo).FirstOrDefault();
                            string nazevFirmy = Firmy.GetJmeno(m.IcoDarce);
                            string vazba = $"Člen statut. orgánu ve firmě {nazevFirmy} sponzorující";
                            //if (v != null)
                            //{
                            //    vazba = $"{Firmy.GetJmeno(m.ICO)} sponzor {m.AddInfo} ({o.ShortName()} {v.Descr?.ToLower()} {v.Doba("{0}")})";
                            //}
                            return new OsobaEvent()
                            {
                                OsobaId = osobaInternalId,
                                Organizace = nazevFirmy,
                                AddInfoNum = m.Hodnota,
                                Created = m.Edited ?? DateTime.Now,
                                DatumDo = m.DarovanoDne,
                                DatumOd = m.DarovanoDne,
                                Note = vazba,
                                Title = "",
                                Type = OsobaEvent.Types.Sponzor,
                                Zdroj = m.Zdroj
                            };
                        })
                        .ToArray();
                        return res1;
                    }
                },
                TimeSpan.FromMinutes(2));




        static Osoba nullObj = new Osoba() { NameId="____NOTHING____" };
        private class OsobyMCMById : CouchbaseCacheManager<Osoba, int>
        {
            public OsobyMCMById() : base("PersonById",getById, TimeSpan.FromMinutes(10), System.Configuration.ConfigurationManager.AppSettings["CouchbaseServers"].Split(','),
                System.Configuration.ConfigurationManager.AppSettings["CouchbaseBucket"],
                System.Configuration.ConfigurationManager.AppSettings["CouchbaseUsername"],
                System.Configuration.ConfigurationManager.AppSettings["CouchbasePassword"])
            { }

            public override Osoba Get(int key)
            {
                var o = base.Get(key);
                if (o.NameId == nullObj.NameId)
                    return null;
                else
                    return o;
            }
            private static Osoba getById(int key)
            {
                var o = Osoba.Get(key);
                return o ?? nullObj;
            }

        }


        private static object lockObj = new object();

        private static OsobyMCMById instanceById;
        public static CouchbaseCacheManager<Osoba, int> GetById
        {
            get
            {
                if (instanceById == null)
                {
                    lock (lockObj)
                    {
                        if (instanceById == null)
                        {
                            instanceById = new OsobyMCMById();
                        }
                    }
                }
                return instanceById;
            }
        }

        private class OsobyMCMByNameId : CouchbaseCacheManager<Osoba, string>
        {
            public OsobyMCMByNameId() : base("PersonByNameId", getByNameId, TimeSpan.FromMinutes(10), System.Configuration.ConfigurationManager.AppSettings["CouchbaseServers"].Split(','),
                System.Configuration.ConfigurationManager.AppSettings["CouchbaseBucket"],
                System.Configuration.ConfigurationManager.AppSettings["CouchbaseUsername"],
                System.Configuration.ConfigurationManager.AppSettings["CouchbasePassword"])
            { }

            public override Osoba Get(string key)
            {
                if (string.IsNullOrEmpty(key))
                    return null;

                var o = base.Get(key);
                if (o == null || o?.NameId == nullObj.NameId)
                    return null;
                else
                    return o;
            }
            private static Osoba getByNameId(string nameId)
            {
                var o = Osoba.GetByNameId(nameId);
                return o ?? nullObj;
            }

        }

        private static OsobyMCMByNameId instanceNameId;
        public static CouchbaseCacheManager<Osoba, string> GetByNameId
        {
            get
            {
                if (instanceNameId == null)
                {
                    lock (lockObj)
                    {
                        if (instanceNameId == null)
                        {
                            instanceNameId = new OsobyMCMByNameId();
                        }
                    }
                }
                return instanceNameId;
            }
        }

    }
}
