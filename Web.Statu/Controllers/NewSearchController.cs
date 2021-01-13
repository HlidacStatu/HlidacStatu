using Devmasters.Enums;
using HlidacStatu.Lib.Analysis.KorupcniRiziko;
using HlidacStatu.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FullTextSearch;
using HlidacStatu.Lib;
using System.Diagnostics;
using HlidacStatu.Util;

namespace HlidacStatu.Web.Controllers
{
    public class NewSearchController : GenericAuthController
    {
        public ActionResult Index()
        {
            return View();
        }

        
        // Used for searching
        public JsonResult Search(string id)
        {
            
            Devmasters.Cache.LocalMemory.LocalMemoryCache<Index<ResultData>> FullTextSearchCache =
                new Devmasters.Cache.LocalMemory.LocalMemoryCache<Index<ResultData>>(TimeSpan.FromDays(1), "fcfs",
                o =>
                {
                    return BuildSearchIndex();
                });

            var searchCache = FullTextSearchCache.Get();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var searchResult = searchCache.Search(id, 10);
            sw.Stop();


            return Json(searchResult.Select(r => r.Original), JsonRequestBehavior.AllowGet);
        }

        private Index<ResultData> BuildSearchIndex()
        {
            var file = System.IO.File.ReadAllText(StaticData.App_Data_Path);

            var results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResultData>>(file);
            
            var index = new Index<ResultData>(results);

            return index;
        }

        public class ResultData
        {
            public string id { get; set; }
            [FullTextSearch.Search]
            public string text { get; set; }
            public string imageUrl { get; set; }
            public string type { get; set; }
            public string description { get; set; }
        }
    }
}