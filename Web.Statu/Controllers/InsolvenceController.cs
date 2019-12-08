using HlidacStatu.Lib.Data.Insolvence;
using HlidacStatu.Lib.ES;
using HlidacStatu.Web.Models;
using System.Web.Mvc;
using System.Linq;

namespace HlidacStatu.Web.Controllers
{
	public class InsolvenceController : GenericAuthController
	{

        public bool IsLimitedView()
        {
            return Framework.InsolvenceLimitedView.IsLimited(this);
        }
		// GET: Insolvence
		public ActionResult Index()
		{
			var model = new InsolvenceIndexViewModel
			{
				NoveFirmyVInsolvenci = Insolvence.NewFirmyVInsolvenci(10, IsLimitedView()),
				NoveOsobyVInsolvenci = Insolvence.NewOsobyVInsolvenci(10, IsLimitedView())
			};

			return View(model);
		}

		public ActionResult Hledat(InsolvenceSearchResult model)
		{
			if (model == null || ModelState.IsValid == false)
			{
				return View(new InsolvenceSearchResult());
			}
            model.LimitedView = IsLimitedView();
			var res = Insolvence.SimpleSearch(model);

			Lib.Data.Audit.Add(
				Lib.Data.Audit.Operations.UserSearch
				, this.User?.Identity?.Name
				, this.Request.UserHostAddress
				, "Insolvence"
				, res.IsValid ? "valid" : "invalid"
				, res.Q, res.OrigQuery);
				
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

			var model = Insolvence.LoadFromES(id,showHighliting ? true : false, false );
			if (model == null)
			{
				return View("Error404");
			}
            if (IsLimitedView() && model.Rizeni.OnRadar == false)
                return RedirectToAction("PristupOmezen", new { id = model.Rizeni.UrlId() });

            if (showHighliting)
            {
                var findRizeni = Insolvence.SimpleSearch($"_id:\"{model.Rizeni.SpisovaZnacka}\" AND ({this.Request.QueryString["qs"]})",1,1,0, true);
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

            var data = Insolvence.LoadFromES(id, showHighliting ? true : false, false);
			if (data == null)
			{
				return View("Error404");
			}

            if (IsLimitedView() && data.Rizeni.OnRadar == false)
                return RedirectToAction("PristupOmezen", new { id = data.Rizeni.UrlId() });

            Nest.HighlightFieldDictionary highlighting = null;
            if (showHighliting)
            {
                var findRizeni = Insolvence.SimpleSearch($"_id:\"{data.Rizeni.SpisovaZnacka}\" AND ({this.Request.QueryString["qs"]})", 1, 1, 0, true);
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

			var dokument = Insolvence.LoadDokument(id, false);
			if (dokument == null)
			{
				return View("Error404");
			}
            if (IsLimitedView() && dokument.Rizeni.OnRadar == false)
                return RedirectToAction("PristupOmezen", new { id = dokument.Rizeni.UrlId   () });

			return View(dokument);
		}

        public ActionResult PristupOmezen(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        }
    }