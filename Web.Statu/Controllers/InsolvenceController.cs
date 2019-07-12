using HlidacStatu.Lib.Data.Insolvence;
using HlidacStatu.Lib.ES;
using HlidacStatu.Web.Models;
using System.Web.Mvc;
using System.Linq;

namespace HlidacStatu.Web.Controllers
{
	public class InsolvenceController : GenericAuthController
	{
		// GET: Insolvence
		public ActionResult Index()
		{
			var model = new InsolvenceIndexViewModel
			{
				NoveFirmyVInsolvenci = Insolvence.NewFirmyVInsolvenci(10),
				NoveOsobyVInsolvenci = Insolvence.NewOsobyVInsolvenci(10)
			};

			return View(model);
		}

		public ActionResult Hledat(InsolvenceSearchResult model)
		{
			if (model == null || ModelState.IsValid == false)
			{
				return View(new InsolvenceSearchResult());
			}

			var res = Insolvence.SimpleSearch(model);
			return View(res);
		}



		public ActionResult Rizeni(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return View("Error404");
			}

            //show highlighting
            bool showHighliting = !string.IsNullOrEmpty(this.Request.QueryString["qs"]);

			var model = Insolvence.LoadFromES(id,showHighliting ? true : false );
			if (model == null)
			{
				return View("Error404");
			}
            if (showHighliting)
            {
                var findRizeni = Insolvence.SimpleSearch($"_id:\"{model.Rizeni.SpisovaZnacka}\" AND ({this.Request.QueryString["qs"]})",1,1, true);
                if (findRizeni.Total > 0)
                    ViewBag.Highlighting = findRizeni.Result.Hits.First().Highlights;
            }
            ViewBag.showHighliting = showHighliting;
            return View(model);
		}

		public ActionResult Dokumenty(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return View("Error404");
			}

            bool showHighliting = !string.IsNullOrEmpty(this.Request.QueryString["qs"]);

            var data = Insolvence.LoadFromES(id, showHighliting ? true : false);
            //var data = Insolvence.LoadFromES(id,false);
			if (data == null)
			{
				return View("Error404");
			}
            Nest.HighlightFieldDictionary highlighting = null;
            if (showHighliting)
            {
                var findRizeni = Insolvence.SimpleSearch($"_id:\"{data.Rizeni.SpisovaZnacka}\" AND ({this.Request.QueryString["qs"]})", 1, 1, true);
                if (findRizeni.Total > 0)
                {
                    highlighting = findRizeni.Result.Hits.First().Highlights;
                }
            }

            return View(new DokumentyViewModel
			{
				SpisovaZnacka = data.Rizeni.SpisovaZnacka,
				UrlId = data.Rizeni.UrlId(),
				Dokumenty = data.Rizeni.Dokumenty.ToArray(),
                HighlightingData = highlighting
			});
		}

		public ActionResult TextDokumentu(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return View("Error404");
			}

			var dokument = Insolvence.LoadDokument(id);
			if (dokument == null)
			{
				return View("Error404");
			}

			return View(dokument);
		}
	}
}