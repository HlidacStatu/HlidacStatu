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

            var searchCache = StaticData.FulltextSearchForAutocomplete.Get();

            var searchResult = searchCache.Search(q, 5, ac => ac.Priority);
            
            return Json(searchResult.Select(r => r.Original), JsonRequestBehavior.AllowGet);
        }

    }
}