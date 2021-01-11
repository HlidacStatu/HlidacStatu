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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            string sql = "select Jmeno, ICO from Firma where IsInRS = 1 AND LEN(ico) = 8;";
            var results = DirectDB.GetList<string, string>(sql)
                .Select(f => new ResultData()
                {
                    id = $"ico:{f.Item2}",
                    text = f.Item1
                }).ToList();
            stopWatch.Stop();
            var firmyElapsed = stopWatch.Elapsed;

            stopWatch.Restart();
            using (DbEntities db = new DbEntities())
            {
                var politici = db.Osoba
                    .Where(o => o.Status == (int)Osoba.StatusOsobyEnum.Politik).ToList();
                    
                var politiciResult = politici.Select(o => new ResultData()
                {
                    id = $"nameid:{o.NameId}",
                    text = o.FullName(false)
                });

                results.AddRange(politiciResult);
            }
            stopWatch.Stop();

            var osobyElapsed = stopWatch.Elapsed;

            stopWatch.Restart();
            var index = new Index<ResultData>(results);
            stopWatch.Stop();
            var indexBuildElapsed = stopWatch.Elapsed;

            return index;
        }

        public class ResultData
        {
            public string id { get; set; }
            [FullTextSearch.Search]
            public string text { get; set; }
        }
    }
}