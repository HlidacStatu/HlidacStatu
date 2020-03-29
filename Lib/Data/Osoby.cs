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
= MemoryCacheManager<IEnumerable<OsobaEvent>, int>.GetSafeInstance("osobyEvents",
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
= MemoryCacheManager<IEnumerable<OsobaEvent>, int>.GetSafeInstance("osobyFirmySponzoring",
    osobaInternalId =>
    {
        using (DbEntities db = new DbEntities())
        {
            var res = db.FirmaEvent.SqlQuery(@"
                    select fe.* from firmaevent fe with (nolock)
	                    inner join osobaVazby ov with (nolock) on ov.vazbakico=fe.ico and fe.Type=" + (int)OsobaEvent.Types.Sponzor
                                + @" and dbo.IsSomehowInInterval(fe.datumOd,fe.datumDo, ov.datumOd, ov.DatumDo)=1
                    and osobaid=" + osobaInternalId)
                    .AsNoTracking();
            var res1 = res.Select(m =>
                    {
                        Osoba o = Osoby.GetById.Get(osobaInternalId);
                        var v = o.VazbyProICO(m.ICO, m.DatumOd, m.DatumDo).FirstOrDefault();
                        string vazba = $"{Firmy.GetJmeno(m.ICO)} sponzor {m.AddInfo} ({o.ShortName()} byl ve statut.orgánu)";
                        if (v != null)
                        {
                            vazba = $"{Firmy.GetJmeno(m.ICO)} sponzor {m.AddInfo} ({o.ShortName()} {v.Descr?.ToLower()} {v.Doba("{0}")})";
                        }
                        return new OsobaEvent()
                        {
                            OsobaId = osobaInternalId,
                            Organizace = m.AddInfo,
                            AddInfoNum = m.AddInfoNum,
                            Created = m.Created,
                            DatumDo = m.DatumDo,
                            DatumOd = m.DatumOd,
                            Note = "",
                            Title = vazba,
                            Type = m.Type,
                            Zdroj = m.Zdroj
                        };
                    }
                    )
                    .ToArray();
            return res1;
        }
    },
    TimeSpan.FromMinutes(2));




        static Osoba nullObj = new Osoba() { NameId="____NOTHING____" };
        private class OsobyMCMById : CouchbaseCacheManager<Osoba, int>
        {
            public OsobyMCMById() : base("PersonById",getById, TimeSpan.FromMinutes(10))
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
            public OsobyMCMByNameId() : base("PersonByNameId", getByNameId, TimeSpan.FromMinutes(10))
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
