using HlidacStatu.Lib.Data.Insolvence;
using HlidacStatu.Lib.ES;
using HlidacStatu.Web.Models;
using System.Web.Mvc;
using System.Linq;

namespace HlidacStatu.Web.Controllers
{
	public class InsolvenceController : Controller
	{
		// GET: Insolvence
		public ActionResult Index()
		{
			var model = new InsolvenceIndexViewModel
			{
				NoveFirmyVInsolvenci = Insolvence.NewFirmyVInsolvenci(100),
				NoveOsobyVInsolvenci = Insolvence.NewOsobyVInsolvenci(100)
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

			var model = Insolvence.LoadFromES(id,false);
			if (model == null)
			{
				return View("Error404");
			}

			return View(model);
		}

		public ActionResult Dokumenty(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return View("Error404");
			}

			var data = Insolvence.LoadFromES(id,false);
			if (data == null)
			{
				return View("Error404");
			}

			return View(new DokumentyViewModel
			{
				SpisovaZnacka = data.Rizeni.SpisovaZnacka,
				UrlId = data.Rizeni.UrlId(),
				Dokumenty = data.Rizeni.Dokumenty.ToArray()
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