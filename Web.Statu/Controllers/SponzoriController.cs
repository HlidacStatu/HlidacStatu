using HlidacStatu.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    public class SponzoriController : GenericAuthController
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Strany()
        {
            return View();
        }

        public ActionResult Strana(string id)
        {
            var result = Sponsors.Strany.StranaPerYears(id);
            ViewBag.Strana = id;
            return View(Sponsors.Strany.RenderPerYearsTable(result));
        }

        public ActionResult Seznam(string id, int? rok = null)
        {
            string strana = id;
            bool linkStrana = true;
            bool showYear = true;

            ViewBag.Firmy = strana;
            ViewBag.Strana = strana;
            ViewBag.Rok = rok;
            IEnumerable<Sponsors.Sponzorstvi<Bookmark.IBookmarkable>> dataF = null;
            IEnumerable<Sponsors.Sponzorstvi<Bookmark.IBookmarkable>> dataO = null;

            if (string.IsNullOrEmpty(strana))
                return RedirectToAction("top");

            if (rok.HasValue == false)
            {
                rok = Sponsors.AllSponzorsPerYearPerStranaOsoby.Get()
                    .Where(m => m.Strana == strana).Max(m => m.Rok);
                var rokF = Sponsors.AllSponzorsPerYearPerStranaFirmy.Get()
                    .Where(m => m.Strana == strana).Max(m => m.Rok);

                if (rok.HasValue && rokF.HasValue)
                    rok = Math.Max(rok.Value, rokF.Value);
                else if (rokF.HasValue)
                    rok = rokF;

                rok = rok ?? Sponsors.DefaultLastSponzoringYear;
            }

            ViewBag.Title = $"Sponzoři {strana}";
            ViewBag.SubTitle = $" v roce {rok.Value}";
            dataO = Sponsors.AllSponzorsPerYearPerStranaOsoby.Get()
                .Where(m => m.Strana == strana && m.Rok == rok)
                .OrderByDescending(m => m.CastkaCelkem)
                .Select(s => new Sponsors.Sponzorstvi<Bookmark.IBookmarkable>()
                {
                    CastkaCelkem = s.CastkaCelkem,
                    Rok = s.Rok,
                    Strana = s.Strana,
                    Sponzor = s.Sponzor
                });
            dataF = Sponsors.AllSponzorsPerYearPerStranaFirmy.Get()
                .Where(m => m.Strana == strana && m.Rok == rok)
                .OrderByDescending(m => m.CastkaCelkem)
                .Select(s => new Sponsors.Sponzorstvi<Bookmark.IBookmarkable>()
                {
                    CastkaCelkem = s.CastkaCelkem,
                    Rok = s.Rok,
                    Strana = s.Strana,
                    Sponzor = s.Sponzor
                });

            return View(
                new[] {
                    Sponsors.RenderSponzorství(dataO, showYear, linkStrana),
                    Sponsors.RenderSponzorství(dataF, showYear, linkStrana)
                }
                );
        }

        public ActionResult Top(int? rok)
        {
            bool linkStrana = true;
            bool showYear = true;

            IEnumerable<Sponsors.Sponzorstvi<Bookmark.IBookmarkable>> dataF = null;
            IEnumerable<Sponsors.Sponzorstvi<Bookmark.IBookmarkable>> dataO = null;

            dataO = Sponsors.AllSponzorsPerYearPerStranaOsoby.Get()
                .Where(m => (m.CastkaCelkem >= 100000 ) && (m.Rok == rok || rok == null)) 
                .GroupBy(g => g.Sponzor, sp => sp, (g, sp) => new Sponsors.Sponzorstvi<Osoba>()
                {
                    Sponzor = g,
                    Rok = null,
                    Strana = sp.OrderBy(s => s.Rok)
                        .Select(s => $"{Util.RenderData.ShortNicePrice(s.CastkaCelkem)} pro {Sponsors.GetStranaHtmlLink(s.Strana)}" + (s.Rok.HasValue ? $" ({s.Rok.Value})" : ""))
                        .Aggregate((f, s) => f + ", " + s),
                    CastkaCelkem = sp.Sum(m => m.CastkaCelkem)
                }
                )
                .OrderByDescending(m => m.CastkaCelkem)
                .Select(s => new Sponsors.Sponzorstvi<Bookmark.IBookmarkable>()
                {
                    CastkaCelkem = s.CastkaCelkem,
                    Rok = s.Rok,
                    Strana = s.Strana,
                    Sponzor = s.Sponzor
                });
            dataF = Sponsors.AllSponzorsPerYearPerStranaFirmy.Get()
                .Where(m => m.CastkaCelkem >= 100000 && (m.Rok == rok || rok == null))
                .GroupBy(g => g.Sponzor.Ico, sp => sp, (g, sp) => new Sponsors.Sponzorstvi<Firma.Lazy>()
                {
                    Sponzor = new Firma.Lazy(g),
                    Rok = null,
                    Strana = sp.OrderBy(s => s.Rok)
                        .Select(s => $"{Util.RenderData.ShortNicePrice(s.CastkaCelkem)} pro {Sponsors.GetStranaHtmlLink(s.Strana)}" + (s.Rok.HasValue ? $" ({s.Rok.Value})" : ""))
                        .Aggregate((f, s) => f + ", " + s),
                    CastkaCelkem = sp.Sum(m => m.CastkaCelkem)
                }
                )
                .OrderByDescending(m => m.CastkaCelkem)
#if (DEBUG)
                        .Take(30)
#endif
                        .Select(s => new Sponsors.Sponzorstvi<Bookmark.IBookmarkable>()
                        {
                            CastkaCelkem = s.CastkaCelkem,
                            Rok = s.Rok,
                            Strana = s.Strana,
                            Sponzor = s.Sponzor
                        });
            showYear = false;
            linkStrana = false;

            ViewBag.Rok = rok ?? 0;
            ViewBag.TopOsoba = Osoby.GetById.Get(Convert.ToInt32(dataO.FirstOrDefault()?.Sponzor?.ToAuditObjectId()));
            ViewBag.TopOsobaAmount = dataO.FirstOrDefault()?.CastkaCelkem ?? 0;

            return View(
        new[] {
                    Sponsors.RenderSponzorství(dataO, showYear, linkStrana),
                    Sponsors.RenderSponzorství(dataF, showYear, linkStrana)
        }
        );

        }



    }
}