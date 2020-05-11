using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HlidacStatu.Web.Framework.Report
{
    public class covid_pomoc
    {
        public float id { get; set; }
        public string ministerstvo { get; set; }
        public string typ_pomoci { get; set; }
        public string program { get; set; }
        public decimal odhadovana_celkova_vyse_v_mld_kc { get; set; }
        public decimal vyplacena { get; set; }
        public decimal pocet_subjektu { get; set; }
        public DateTime udaj_ke_dni { get; set; }
        public string poznamka { get; set; }
        public string url { get; set; }

        public static List<covid_pomoc> VsechnaPomoc()
        {
            return dataCache.Get();
        }

        static Devmasters.Cache.V20.LocalMemory.LocalMemoryCache<List<covid_pomoc>> dataCache = 
            new Devmasters.Cache.V20.LocalMemory.LocalMemoryCache<List<covid_pomoc>>(TimeSpan.FromMinutes(10), "covid_pomoc",
            (o) =>
            {
                var ds = HlidacStatu.Lib.Data.External.DataSets.DataSet.CachedDatasets.Get("pomoc-covid");
                return ds.GetAllData<covid_pomoc>().ToList();
            });

        }
}