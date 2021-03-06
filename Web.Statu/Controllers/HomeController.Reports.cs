﻿using Nest;
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
#if (!DEBUG)
        [OutputCache(VaryByParam = "*", Duration = 60 * 60 * 12)]
#endif
        [ChildActionOnly]
        public ActionResult Report_child_12H(int? id, string q, string page, string order, string chyby, 
            string ShowWatchdog, string IncludeNeplatne, string Zahajeny,string oblast, string cpv, 
            string obdobi, string strana, string dodavatelico)
        {
            ViewBag.NameOfView = this.RouteData.Values["nameOfView"];
            return View("Report_child", Util.ParseTools.ToInt(RouteData.Values["id"]?.ToString()) ?? 0);
        }

#if (!DEBUG)
        [OutputCache(VaryByParam = "*", Duration = 60 * 60 * 1)]
#endif
        [ChildActionOnly]
        public ActionResult Report_child_1H(int? id, string q, string page, string order, string chyby,
    string ShowWatchdog, string IncludeNeplatne, string Zahajeny, string oblast, string cpv,
    string obdobi, string strana, string dodavatelico)
        {
            ViewBag.NameOfView = this.RouteData.Values["nameOfView"];
            return View("Report_child", Util.ParseTools.ToInt(RouteData.Values["id"]?.ToString()) ?? 0);
        }

        [ChildActionOnly]
        public ActionResult Report_child_0H(int? id, string q, string page, string order, string chyby,
    string ShowWatchdog, string IncludeNeplatne, string Zahajeny, string oblast, string cpv,
    string obdobi, string strana, string dodavatelico)
        {
            ViewBag.NameOfView = this.RouteData.Values["nameOfView"];
            return View("Report_child", Util.ParseTools.ToInt(RouteData.Values["id"]?.ToString()) ?? 0);
        }

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