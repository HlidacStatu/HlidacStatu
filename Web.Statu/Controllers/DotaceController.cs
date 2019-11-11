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
        private readonly DotaceService _dotaceService = new DotaceService();

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

            var aggs = new Nest.AggregationContainerDescriptor<Dotace>()
                .Sum("souhrn", s => s
                    .Field(f => f.DotaceCelkem)
                
                );


            var res = _dotaceService.SimpleSearch(model, anyAggregation:aggs);
            return View(res);
        }

        public ActionResult Detail(string idDotace)
        {
            if (string.IsNullOrWhiteSpace(idDotace))
            {
                return View();
            }
            var dotace = _dotaceService.Get(idDotace);
            return View(dotace);
        }
    }
}