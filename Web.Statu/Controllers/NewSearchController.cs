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
        public JsonResult Search(string q)
        {
            
            Devmasters.Cache.LocalMemory.LocalMemoryCache<Index<ResultData>> FullTextSearchCache =
                new Devmasters.Cache.LocalMemory.LocalMemoryCache<Index<ResultData>>(TimeSpan.FromDays(7), "fcfs",
                o =>
                {
                    return BuildSearchIndex();
                });

            var searchCache = FullTextSearchCache.Get();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var searchResult = searchCache.Search(q, 5);
            sw.Stop();


            return Json(searchResult.Select(r => r.Original), JsonRequestBehavior.AllowGet);
        }

        private Index<ResultData> BuildSearchIndex()
        {
            var autocompleteFilePath = System.IO.Path.Combine(StaticData.App_Data_Path, "autocomplete.json");
            var file = System.IO.File.ReadAllText(autocompleteFilePath);

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