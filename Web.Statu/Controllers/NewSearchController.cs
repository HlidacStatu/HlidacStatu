﻿using Devmasters.Enums;
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
            Devmasters.Cache.LocalMemory.LocalMemoryCache<Index<Autocomplete>> FullTextSearchCache =
                new Devmasters.Cache.LocalMemory.LocalMemoryCache<Index<Autocomplete>>(TimeSpan.FromDays(7), "fcfs",
                o =>
                {
                    return BuildSearchIndex();
                });

            var searchCache = FullTextSearchCache.Get();

            var searchResult = searchCache.Search(q, 5);
            
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