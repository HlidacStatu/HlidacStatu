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
    public class VZController : Controller
    {


        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Napoveda()
        {
            return View();
        }

        public ActionResult IT(HlidacStatu.Lib.ES.VerejnaZakazkaSearchData model)
        {
            if (model == null)
                return View(new HlidacStatu.Lib.ES.SmlouvaSearchResult());
            else
            return View(model);
        }

        public ActionResult Zakazka(string id)
        {
            if (string.IsNullOrEmpty(id))
                return View("Error404");

            var vz = VerejnaZakazka.LoadFromES(id);
            if (vz == null)
                return View("Error404");
    
            return View(vz);
        }

        public ActionResult TextDokumentu(string id, string hash)
        {
            var vz = VerejnaZakazka.LoadFromES(id);
            if (vz == null)
                return View("Error404");
            if (vz.Dokumenty?.Any(d => d.StorageId == hash) == false)
                return View("Error404");

            return View(vz);
        }


        public ActionResult Hledat(HlidacStatu.Lib.ES.VerejnaZakazkaSearchData model, string cpv)
        {
            if (model == null || ModelState.IsValid == false)
                return View(new HlidacStatu.Lib.ES.VerejnaZakazkaSearchData());

            string[] cpvs = null;
            if (!string.IsNullOrEmpty(cpv))
            {
                cpvs = cpv.Split(',');
            }
            var res= VerejnaZakazka.Searching.SimpleSearch(model);
            return View(res);
        }



#if (!DEBUG)
        [OutputCache(Duration =10*60,VaryByParam ="id;h;embed")]
#endif
        public ActionResult CPVs()
        {
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                StaticData.CPVKody
                    .Select(kv => new {id=kv.Key, txt = kv.Value})
                ),"application/json");
        }

    }
}