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


        public static IEnumerable<string> FindAllIco(string query, int limit)
        {
            if (string.IsNullOrEmpty(query))
                return new string[] { };

            var items = new List<Tuple<string, decimal>>();

            var resExact = StaticData.FirmyNazvy.Get()
                            .Where(m => m.Value.Jmeno == query) // Data.External.FirmyDB.AllFromName(query)
                            .Select(m => new Tuple<string, decimal>(m.Key, 1m));

            if (resExact.Count() > 0)
                items.AddRange(resExact);

            string aQuery = Devmasters.Core.TextUtil.RemoveDiacritics(query).ToLower();
            if (items.Count < limit)
            {
                //add more
                if (StaticData.FirmyNazvyOnlyAscii.ContainsKey(aQuery))
                {
                    var res = StaticData.FirmyNazvyOnlyAscii[aQuery]
                                .Where(ico => !string.IsNullOrEmpty(ico)).Select(ico => new Tuple<string, decimal>(ico, 0.9m))
                                .GroupBy(g => g.Item1, v => v.Item2, (g, v) => new Tuple<string, decimal>(g, v.Max()));
                    items.AddRange(res);
                }
            }
            if (items.Count < limit)
            {
                //add more
                var res = StaticData.FirmyNazvyOnlyAscii
                            .Where(m => m.Key.StartsWith(aQuery, StringComparison.Ordinal))
                            .Take(limit - items.Count)
                            .SelectMany(m => m.Value.Where(ico => !string.IsNullOrEmpty(ico)).Select(ico => new Tuple<string, decimal>(ico, 0.5m)))
                            .GroupBy(g => g.Item1, v => v.Item2, (g, v) => new Tuple<string, decimal>(g, v.Max()));
                items.AddRange(res);
            }
            if (items.Count < limit && aQuery.Length >= 5)
            {
                //add more
                var res = StaticData.FirmyNazvyOnlyAscii
                            .Where(m => m.Key.Contains(aQuery))
                            .OrderBy(m => Validators.LevenshteinDistanceCompute(m.Key, aQuery))
                            .Take(limit - items.Count)
                            .Where(m => Validators.LevenshteinDistanceCompute(m.Key, aQuery) < 10)
                            .SelectMany(m => m.Value.Where(ico => !string.IsNullOrEmpty(ico)).Select(ico => new Tuple<string, decimal>(ico, 0.5m)))
                            .GroupBy(g => g.Item1, v => v.Item2, (g, v) => new Tuple<string, decimal>(g, v.Max()));
                items.AddRange(res);
            }
            return items
                .Take(limit)
                .Select(m => m.Item1);
        }
        public static IEnumerable<Firma> FindAll(string query, int limit)
        {
            return FindAllIco(query,limit)
                .Select(m => Get(m));
        }
    }
}
