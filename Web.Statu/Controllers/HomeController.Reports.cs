using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HlidacStatu.Web.Framework;
using System.Web.Routing;

namespace HlidacStatu.Web.Controllers
{
    public partial class HomeController : GenericAuthController
    {

        [OutputCache(VaryByParam = "*", Duration = 60 * 60 * 12)]
        [ChildActionOnly]
        public ActionResult Report_child(int? id, string q, string page, string order, string chyby, 
            string ShowWatchdog, string IncludeNeplatne, string Zahajeny,string oblast, string cpv )
        {
            ViewBag.NameOfView = this.RouteData.Values["nameOfView"];
            return View("Report_child", Util.ParseTools.ToInt(RouteData.Values["id"]?.ToString()) ?? 0);
        }

#if (!DEBUG)
        [OutputCache(VaryByParam ="*", Duration =60*60*12)]
#endif
        public ActionResult Report(int? id)
        {
            if (id.HasValue == false)
                id = 1;

            if (id == 8)
            {
                Response.RedirectPermanent("/Osoby");
                Response.End();
            }

            if (id == 20)
            {
                Response.RedirectPermanent("/Porovnat");
                Response.End();
            }

            ViewBag.ReportNum = id;
            return View(id.Value);
        }

        public ActionResult Reporty()
        {
            return View();
        }


    }
}