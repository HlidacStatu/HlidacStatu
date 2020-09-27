using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Devmasters;
using HlidacStatu.Lib;
using HlidacStatu.Lib.Data.External.Zabbix;
using HlidacStatu.Web.Framework;
using ZabbixApi;
using static HlidacStatu.Web.Models.ZabbixData;

namespace HlidacStatu.Web.Controllers
{
    public class WebyController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dalsi(string id)
        {
            ViewBag.ID = id;
            if (Devmasters.TextUtil.IsNumeric(id))
            { //priorita

                int iid = Convert.ToInt32(id);
                if (iid < 1)
                    return RedirectToAction("Index", "Weby");
                if (iid > 3)
                    return RedirectToAction("Index", "Weby");

                ViewBag.SubTitle = "Další státní weby";
                return View(ZabTools.WebyItems(iid.ToString()));
            }
            else if (id?.ToLower() == "ustredni")
            {
                var list = ZabTools.WebyItems(id);
                if (list == null)
                    return RedirectToAction("Index", "Weby");
                if (list.Count() == 0)
                    return RedirectToAction("Index", "Weby");

                ViewBag.SubTitle = "Weby ústředních orgánů státní správy";
                return View(list);

            }
            else if (id?.ToLower() == "krajske" && false)
            {
                var list = ZabTools.WebyItems(id);
                if (list == null)
                    return RedirectToAction("Index", "Weby");
                if (list.Count() == 0)
                    return RedirectToAction("Index", "Weby");

                ViewBag.SubTitle = "Weby a služby krajských úřadů";
                return View(list);

            }
            else
                return RedirectToAction("Index", "Weby");


        }

        public ActionResult JakMerime()
        {
            return View();
        }
        public ActionResult OpenData()
        {
            return View();
        }

        [GZipOrDeflate()]
        public ActionResult ChartData(string id, string hh, long? f, long? t, int? h = 24)
        {
            id = id?.ToLower() ?? "";
            string content = "{}";
            DateTime fromDate = DateTime.Now.AddHours(-1 * h.Value);
            DateTime toDate = DateTime.Now;
            if (f.HasValue)
                fromDate = new DateTime(f.Value);
            if (t.HasValue)
                toDate = new DateTime(t.Value);

            IEnumerable<ZabHostAvailability> data = null;

            switch (id)
            {
                case "index":
                    data = ZabTools.WebyData("0")
                        ?.OrderBy(o => o.Host.publicname)
                        ?.Reverse()
                        ?.ToList();

                    break;
                case "1":
                case "2":
                case "3":
                    data = ZabTools.WebyData(ZabTools.WebyItems(id))
                        ?.OrderBy(o => o.Host.publicname)
                        ?.Reverse()
                        ?.ToList();
                    break;
                case "ustredni":
                    data = ZabTools.WebyData(ZabTools.WebyItems("ustredni"))
                        ?.OrderBy(o => o.Host.publicname)
                        ?.Reverse()
                        ?.ToList();
                    break;
                case "krajske":
                    break;
                default:
                    break;
            }
            if (id.StartsWith("w"))
            {
                id = id.Replace("w", "");
                ZabHost host = ZabTools.Weby().Where(w => w.hostid == id.ToString() & w.itemIdResponseTime != null).FirstOrDefault();
                if (host != null)
                {
                    if (host.ValidHash(hh))
                    {
                        data = new ZabHostAvailability[] { ZabTools.GetHostAvailabilityLong(host) };
                    }
                }
            }


            if (data != null)
            {
                var dataready = new
                {
                    data = data.AsEnumerable()
                      .Select((x, l) => x.DataForChart(fromDate, toDate, l))
                      .SelectMany(x => x)
                      .ToArray(),
                    cats = data.ToDictionary(k => k.Host.hostid, d => new { host = d.Host, lastResponse = d.Data.Last() }),
                    categories = data.Select(m => m.Host.hostid).ToArray(),
                    colsize = data.Select(d => d.ColSize(fromDate, toDate)).Max(),
                };
                content = Newtonsoft.Json.JsonConvert.SerializeObject(dataready);
            }
            return Content(content, "application/json");
        }

#if (!DEBUG)
        [OutputCache(Duration =10*60,VaryByParam ="id;h;embed")]
#endif
        public ActionResult Info(int id, string h)
        {
            ZabHost host = ZabTools.Weby().Where(w => w.hostid == id.ToString() & w.itemIdResponseTime != null).FirstOrDefault();
            if (host == null)
                return RedirectToAction("Index", "Weby");

            if (host.ValidHash(h))
                return View(host);
            else
                return RedirectToAction("Index", "Weby");
        }

        [GZipOrDeflate()]
        public ActionResult Data(int? id, string h)
        {
            return Json(new { error = "API presunuto. Viz hlidacStatu.cz/api. Omlouvame se." }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Https()
        {
            return View();
        }






    }
}