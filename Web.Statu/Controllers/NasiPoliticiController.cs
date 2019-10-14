using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Devmasters.Core;
using HlidacStatu.Lib;
using HlidacStatu.Lib.Data;
using HlidacStatu.Lib.Data.External.Zabbix;
using HlidacStatu.Lib.Data.VZ;
using HlidacStatu.Web.Models;
using ZabbixApi;
using static HlidacStatu.Web.Models.ZabbixData;

namespace HlidacStatu.Web.Controllers
{
    public class NasiPoliticiController : Controller
    {


        public ActionResult Index()
        {
            return RedirectToAction("politik", "NasiPolitici", new { id = "andrej-babis" } );
        }

        public ActionResult Politik(string id)
        {
            HlidacStatu.Lib.Data.Osoba model = HlidacStatu.Lib.Data.Osoby.GetByNameId.Get(id);

            return View(model);
        }

    }
}