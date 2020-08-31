using HlidacStatu.Util.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data
{
    public static class Firmy
    {

        static Firma nullObj = new Firma();

        
        private static string getNameByIco(string key)
        {
            var o = Firma.FromIco(key);
            if (o == null)
                return key;
            if (o.Valid == false)
                return key;
            return o.Jmeno;
        }

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
            = CouchbaseCacheManager<Firma, string>.GetSafeInstance("firmyByICO", getByIco, TimeSpan.FromHours(4));

        public static CouchbaseCacheManager<Firma, string> instanceByDS
            = CouchbaseCacheManager<Firma, string>.GetSafeInstance("firmyByDS", getByDS, TimeSpan.FromHours(4));

        public static CouchbaseCacheManager<string, string> instanceNameOnlyByIco
            = CouchbaseCacheManager<string, string>.GetSafeInstance("firmaNameOnlyByICO", getNameByIco, TimeSpan.FromHours(12));

        public static string GetJmeno(int ICO)
        {
            return GetJmeno(ICO.ToString().PadLeft(8, '0'));
        }
        public static string GetJmeno(string ico)
        {
            return instanceNameOnlyByIco.Get(Util.ParseTools.NormalizeIco(ico));
        }
        public static Firma Get(int ICO)
        {
            return Get(ICO.ToString().PadLeft(8, '0'));
        }
        public static Firma Get(string ICO)
        {
            if (string.IsNullOrEmpty(ICO))
                return Firma.LoadError;
            var f =  instanceByIco.Get(Util.ParseTools.NormalizeIco( ICO));
            if (f == null)
                return Firma.LoadError;
            else
                return f;

        }
        public static Firma GetByDS(string ds)
        {
            if (ds == null)
                ds = string.Empty;

            var f =  instanceByDS.Get(ds);
            if (f == null)
                return Firma.LoadError;
            else
                return f;

        }

    }
}
