using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    public class UctyController : Controller
    {




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
            return RedirectPermanent("/data/Detail/transparentni-ucty-transakce/" + id?.ToLower());

        }

        public ActionResult SubjektyTypu(string id)
        {
            return RedirectPermanent($"/data/Hledat/transparentni-ucty?Q=TypSubjektu%3A%22{System.Net.WebUtility.UrlEncode(id)}%22");

        }

        public ActionResult Ucty(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectPermanent($"/data/Index/transparentni-ucty");

            if (id.Contains("/"))
                id = id.Replace("/", "-");
            return RedirectPermanent($"/data/Hledat/transparentni-ucty?q=Subjekt%3A%22{System.Net.WebUtility.UrlEncode(id)}%22");
        }

        public ActionResult Ucet(string id, int from = 0)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectPermanent($"/data/Index/transparentni-ucty");

            if (id.Contains("/"))
                id = id.Replace("/", "-");
            return RedirectPermanent($"/data/Detail/transparentni-ucty/" + id);
        }



        public ActionResult Hledat(string id, string q, string osobaid, string ico)
        {

            string query = q;
            if (!string.IsNullOrEmpty(id))
                query = $"({query}) AND (CisloUctu:{id.Replace("/", "-")})";


            return RedirectPermanent($"/data/Hledat/transparentni-ucty-transakce?q={System.Net.WebUtility.UrlEncode(query)}");

        }

    }
}