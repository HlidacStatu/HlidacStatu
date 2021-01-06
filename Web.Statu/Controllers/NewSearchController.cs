using Devmasters.Enums;
using HlidacStatu.Lib.Analysis.KorupcniRiziko;
using HlidacStatu.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FullTextSearch;

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
            Devmasters.Cache.LocalMemory.LocalMemoryCache<Index<SubjectNameCache>> FullTextSearchCache =
                new Devmasters.Cache.LocalMemory.LocalMemoryCache<Index<SubjectNameCache>>(TimeSpan.Zero,
                o =>
                {
                    return new Index<SubjectNameCache>(SubjectNameCache.GetCompanies().Values);
                });

            var searchResult = FullTextSearchCache.Get().Search(id, 10);

            return Json(searchResult.Select(r => r.Original), JsonRequestBehavior.AllowGet);
        }


    }
}