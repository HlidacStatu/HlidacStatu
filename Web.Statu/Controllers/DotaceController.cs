using HlidacStatu.Lib.Data.Dotace;
using HlidacStatu.Lib.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    public class DotaceController : GenericAuthController
    {
        private DotaceService _dotaceService = new DotaceService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Hledat(DotaceSearchResult model)
        {
            if (model == null || ModelState.IsValid == false)
            {
                return View(new DotaceSearchResult());
            }
            var res = _dotaceService.SimpleSearch(model);
            return View(res);
        }
    }
}