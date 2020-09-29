using Devmasters;
using FullTextSearch;
using HlidacStatu.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class SubjectNameCache
    {
        public SubjectNameCache(string name, string ico)
        {
            Name = name;
            Ico = ico;
        }

        public static Devmasters.Cache.File.FileCache<Dictionary<string,SubjectNameCache>> CachedCompanies = 
            new Devmasters.Cache.File.FileCache<Dictionary<string, SubjectNameCache>>(
                Lib.Init.WebAppDataPath, TimeSpan.Zero, "KIndexCompanies",
                    (o) =>
                    {
                        return ListCompanies();
                    });

        private static Dictionary<string,SubjectNameCache> ListCompanies()
        {
            Dictionary<string, SubjectNameCache> companies = new Dictionary<string, SubjectNameCache>();
            foreach(var kindexRecord in KIndex.YieldExistingKindexes())
            {
                companies.Add(kindexRecord.Ico, new SubjectNameCache(kindexRecord.Jmeno, kindexRecord.Ico));
            }

            return companies;
        }

        public static Dictionary<string, SubjectNameCache> GetCompanies()
        {
            return CachedCompanies.Get();
        }

        [Search]
        public string Name { get; set; }
        [Search]
        public string Ico { get; set; }
        
    }
}
