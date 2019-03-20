using HlidacStatu.Util.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data
{
    public static class Firmy
    {

        static Firma nullObj = new Firma();
        private static Firma getByIco(string key)
        {
            var o = Firma.FromIco(key);
            return o ?? nullObj;
        }
        private static Firma getByDS(string key)
        {
            var o = Firma.FromDS(key);
            return o ?? nullObj;
        }

        public static CouchbaseCacheManager<Firma, string> instanceByIco
            = CouchbaseCacheManager<Firma, string>.GetSafeInstance("firmyByICO", getByIco, TimeSpan.FromMinutes(120));

        public static CouchbaseCacheManager<Firma, string> instanceByDS
            = CouchbaseCacheManager<Firma, string>.GetSafeInstance("firmyByDS", getByDS, TimeSpan.FromMinutes(120));


        public static string GetJmeno(int ICO)
        {
            return GetJmeno(ICO.ToString().PadLeft(8, '0'));
        }
        public static string GetJmeno(string ico)
        {
            var f = Firmy.Get(ico);
            if (f.Valid)
                return f.Jmeno;
            else
                return "(neznámé)";
        }
        public static Firma Get(int ICO)
        {
            return Get(ICO.ToString().PadLeft(8, '0'));
        }
        public static Firma Get(string ICO)
        {
            if (ICO == null)
                ICO = string.Empty;

            return instanceByIco.Get(ICO);
        }
        public static Firma GetByDS(string ds)
        {
            if (ds == null)
                ds = string.Empty;

            return instanceByDS.Get(ds);
        }

    }
}
