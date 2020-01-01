using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    public class UctyController : Controller
    {




        // GET: Ucty
#if (!DEBUG)
        [OutputCache(Duration =60*60*6, VaryByParam ="embed")]
#endif
    public ActionResult Index()
        {
            return View();
        }

#if (!DEBUG)
        [OutputCache(Duration =60*60*6, VaryByParam ="embed")]
#endif
        public ActionResult Prezidenti()
        {
            return View();
        }

        public ActionResult Transakce(string id)
        {
            var res = HlidacStatu.Lib.ES.Manager.GetESClient_BankovniPolozky()
                .Get<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>(id);
            if (res.Found == false)
                return View("Error404");

            return View(res.Source);
        }

        public ActionResult SubjektyTypu(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            if (id.ToLower() == "polická strana")
                return RedirectToActionPermanent("Strany", "Sponzori");


            //var es = HlidacStatu.Lib.ES.Manager.GetESClient_BankovniUcty();
            List<string> typy = HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcty
                            .GetAll()
                            .Select(m => m.TypSubjektu.ToLower())
                            .Distinct()
                            .ToList();

            if (typy.Contains(id.ToLower()))
                return View((object)id);
            else
                return RedirectToAction("Index");

        }

        public ActionResult Ucty(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index", "Ucty");


            List<HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcet> ucty = HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcty.GetAll()
                .Where(m=>m.Subjekt == id)
                .ToList();

            if (ucty.Count == 0)
                return RedirectToAction("Index", "Ucty");

            if (ucty.Count == 1)
                return Redirect("/ucty/ucet?id=" + System.Net.WebUtility.UrlEncode(ucty.First().CisloUctu));

            return View(ucty);
        }

        public ActionResult Ucet(string id, int from = 0)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index", "Ucty");
            if (from < 0)
                from = 0;



            HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcet bu = HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcty.Get(id);
            if (bu == null)
                return RedirectToAction("Index", "Ucty");

            List<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka> polozky = new List<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>();


            Func<int, int, Nest.ISearchResponse<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>> searchFunc = (size, page) =>
            {
                return HlidacStatu.Lib.ES.Manager.GetESClient_BankovniPolozky()
                        .Search<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>(a => a
                            .Size(size)
                            .From(page * size)
                            .Query(q => q.Term(t => t.Field(f => f.CisloUctu).Value(id)))
                            .Scroll("2m")
                            );
            };

            Lib.Searching.Tools.DoActionForQuery<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>(
                HlidacStatu.Lib.ES.Manager.GetESClient_BankovniPolozky(),
                searchFunc,
                (p, o) =>
                {
                    polozky.Add(p.Source);
                    return new Devmasters.Core.Batch.ActionOutputData();
                }, null, null, null, false, blockSize: 500

                );
            return View(
                new Tuple<HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcet,
                    HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka[]>(
                        bu, polozky.ToArray())
                );
        }



        public ActionResult Hledat(string id, string q, string osobaid, string ico)
        {

            var model = new Web.Models.UctySearchResult() { Query = q ?? "" };

            if (string.IsNullOrEmpty(id))
                model.BU = null;
            else
            {
                model.BU = HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcty.Get(id);
                if (model.BU == null)
                    return RedirectToAction("Index", "Ucty");
            }

            Nest.ISearchResponse<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka> res = null;
            if (!string.IsNullOrEmpty(osobaid))
            {
                res = HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcty.SearchPolozkyForOsoba(osobaid, model.BU, 500);
            }
            else if (!string.IsNullOrEmpty(ico))
            {
                res = HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcty.SearchPolozkyForFirma(ico, model.BU, 500);
            }
            else if (!string.IsNullOrEmpty(model.InternalQuery) || model.Query.Length > 2)
            {
                res = HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcty.SearchPolozkyRaw(model.InternalQuery ?? model.Query, model.BU, 500);
            }
            if (res != null && res.IsValid)
                model.Polozky = res.Hits.Select(m => m.Source).ToArray();

            return View(model);
        }

    }
}