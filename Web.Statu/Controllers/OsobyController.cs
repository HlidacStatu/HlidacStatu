using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HlidacStatu.Lib.Data;
using HlidacStatu.Lib.Searching;
using static HlidacStatu.Lib.Data.Search;

namespace HlidacStatu.Web.Controllers
{
    public class OsobyController : Controller
    {
        // GET: Osoby
        public ActionResult Index(string prefix, string q, bool ftx = false)
        {
            if (!string.IsNullOrEmpty(q))
                return RedirectToAction("Hledat",new { q=q });

            return View();
        }

        public ActionResult Hledat(string prefix, string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Redirect("/osoby");

            var res = Lib.Data.Osoba.Search.SimpleSearch(q, 1, 50, Osoba.Search.OrderResult.Relevance);
            return View(res);
        }

    }
}