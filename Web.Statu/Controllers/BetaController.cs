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
    public class BetaController : GenericAuthController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search()
        {
            return View();
        }


        // Used for searching
        public JsonResult Autocomplete(string q)
        {
            Devmasters.Cache.LocalMemory.AutoUpdatedLocalMemoryCache<Index<Autocomplete>> FullTextSearchCache =
                new Devmasters.Cache.LocalMemory.AutoUpdatedLocalMemoryCache<Index<Autocomplete>>(
                    TimeSpan.FromDays(1), 
                    "FulltextSearchForAutocomplete_main",
                    o =>
                    {
                        return BuildSearchIndex();
                    });

            var searchCache = FullTextSearchCache.Get();

            var searchResult = searchCache.Search(q, 5, ac => ac.Priority);
            
            return Json(searchResult.Select(r => r.Original), JsonRequestBehavior.AllowGet);
        }

        private Index<Autocomplete> BuildSearchIndex()
        {
            var results = StaticData.Autocomplete_Cache.Get();
            
            var index = new Index<Autocomplete>(results);

            return index;
        }

    }
}