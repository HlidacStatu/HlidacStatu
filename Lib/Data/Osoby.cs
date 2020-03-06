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
