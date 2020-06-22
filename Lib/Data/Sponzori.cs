using HlidacStatu.Lib.Render;
using HlidacStatu.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data
{
    public class Sponsors
    {
        public static string[] VelkeStrany = new string[] { "ANO", "ODS", "ČSSD", "Česká pirátská strana","KSČM",
                            "SPD", "STAN","KDU-ČSL","TOP 09",
                            "Svobodní","Strana zelených"
                        };
        public static string[] TopStrany = VelkeStrany.Take(9).ToArray();

        public static int DefaultLastSponzoringYear = DateTime.Now.AddMonths(-7).AddYears(-1).Year;

        public class Strany
        {
            public class AggSum
            {
                public int Num { get; set; } = 0;
                public decimal Sum { get; set; } = 0;
            }

            public class PerStrana : AggSum
            {
                public string Strana { get; set; }
            }
            public class StranaPerYear
            {
                public string Strana { get; set; }

                public int Rok { get; set; }
                public AggSum Osoby { get; set; } = new AggSum();

                public AggSum Firmy { get; set; } = new AggSum();

                public decimal TotalKc { get { return Osoby.Sum + Firmy.Sum; } }


                public class PerYearStranaEquality : IEqualityComparer<StranaPerYear>
                {
                    public bool Equals(StranaPerYear x, StranaPerYear y)
                    {
                        if (x == null || y == null)
                            return false;

                        return x.Rok == y.Rok && x.Strana == y.Strana;
                    }

                    public int GetHashCode(StranaPerYear obj)
                    {
                        //http://stackoverflow.com/a/4630550
                        return
                            new
                            {
                                obj.Rok,
                                obj.Strana
                            }.GetHashCode();
                    }
                }
            }



            public static Devmasters.Cache.V20.LocalMemory.LocalMemoryCache<IEnumerable<StranaPerYear>> GetStranyPerYear
                = new Devmasters.Cache.V20.LocalMemory.LocalMemoryCache<IEnumerable<StranaPerYear>>(
                TimeSpan.FromDays(4), "sponzori_stranyPerYear", (obj) =>
                {
                    using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
                    {
                        var resultO = db.OsobaEvent
                                .Where(m => m.Type == 3 && m.DatumOd.HasValue)
                                .ToArray()
                                .Select(m => new { rok = m.DatumOd.Value.Year, oe = m })
                                .GroupBy(g => new { rok = g.rok, strana = g.oe.Organizace }, oe => oe.oe, (r, oe) => new StranaPerYear()
                                {
                                    Rok = r.rok,
                                    Strana = r.strana,
                                    Osoby = new AggSum() { Num = oe.Count(), Sum = oe.Sum(s => s.AddInfoNum) ?? 0 }
                                });

                        var resultF = db.FirmaEvent
                                .Where(m => m.Type == 3 && m.DatumOd.HasValue)
                                .ToArray()
                                .Select(m => new { rok = m.DatumOd.Value.Year, oe = m })
                                .GroupBy(g => new { rok = g.rok, strana = g.oe.AddInfo }, oe => oe.oe, (r, oe) => new StranaPerYear()
                                {
                                    Rok = r.rok,
                                    Strana = r.strana,
                                    Firmy = new AggSum() { Num = oe.Count(), Sum = oe.Sum(s => s.AddInfoNum) ?? 0 }
                                });

                        var roky = resultO.FullOuterJoin(resultF, o => o, f => f,
                                    (o, f, k) => new StranaPerYear()
                                    {
                                        Strana = k.Strana,
                                        Rok = k.Rok,
                                        Osoby = o?.Osoby ?? new AggSum(),
                                        Firmy = f?.Firmy ?? new AggSum()
                                    }, cmp: new StranaPerYear.PerYearStranaEquality()
                         );

                        return roky;
                    }

                });

            public static IEnumerable<StranaPerYear> StranaPerYears(string strana)
            {
                return GetStranyPerYear.Get().Where(m => m.Strana == strana);
            }
            public static StranaPerYear StranaPerYears(string strana, int year)
            {
                var ret = GetStranyPerYear.Get().Where(m => m.Strana == strana && m.Rok == year).FirstOrDefault();
                if (ret == null)
                    ret = new StranaPerYear() { Strana = strana, Rok = year };
                return ret;
            }

            public static Lib.Render.ReportDataSource<StranaPerYear> RenderPerYearsTable(IEnumerable<StranaPerYear> dataPerYear)
            {
                ReportDataSource<StranaPerYear> rokyTable = new ReportDataSource<StranaPerYear>(new ReportDataSource<StranaPerYear>.Column[]
              {
                    new ReportDataSource<StranaPerYear>.Column() { Name="Rok",
                        HtmlRender = (s) => {
                            return s.Rok.ToString();
                            }
                        }
                ,
                new ReportDataSource<StranaPerYear>.Column() { Name="Sponzoring osob",
                    HtmlRender = (s) => {
                             StranaPerYear data = (StranaPerYear)s;
                                if (data.Osoby.Num>0)
                                        return string.Format(@"{0}, počet darů: {1} za {2}", GetStranaSponzoringHtmlLink(data.Strana,data.Rok, SponzoringDataType.Osoby), data.Osoby.Num, HlidacStatu.Util.RenderData.NicePrice(data.Osoby.Sum, "výši neznáme"));
                                else
                                    return "";
                            }
                        },
                new ReportDataSource<StranaPerYear>.Column() { Name="Sponzoring firem",
                    HtmlRender = (s) => {
                             StranaPerYear data = (StranaPerYear)s;
                                if (data.Firmy.Num>0)
                                        return string.Format(@"{0}, počet darů: {1} za {2}", GetStranaSponzoringHtmlLink(data.Strana,data.Rok, SponzoringDataType.Firmy), data.Firmy.Num, HlidacStatu.Util.RenderData.NicePrice(data.Firmy.Sum, "výši neznáme"));
                                else
                                    return "";
                            }
                        }
                ,
              });


                foreach (var r in dataPerYear.OrderBy(m => m.Rok))
                {
                    rokyTable.AddRow(r);
                }

                return rokyTable;
            }

        }

        public static IEnumerable<Sponsors.Sponzorstvi<Bookmark.IBookmarkable>> AllTimeTopSponzorsPerStrana(string strana, int top = int.MaxValue)
        {
            var o = AllTimeTopSponzorsPerStranaOsoby(strana, top * 50);
            var f = AllTimeTopSponzorsPerStranaFirmy(strana, top * 50);

            return o.Select(m=> (Sponzorstvi<Bookmark.IBookmarkable>)m)
                    .Union(f.Select(m=> (Sponzorstvi<Bookmark.IBookmarkable>)m))
                    .OrderByDescending(m => m.CastkaCelkem)
                    .Take(top);
        }
        public static IEnumerable<Sponsors.Sponzorstvi<Osoba>> AllTimeTopSponzorsPerStranaOsoby(string strana, int top = int.MaxValue)
        {
            return AllSponzorsPerYearPerStranaOsoby.Get()
                .Where(m => m.Strana == strana)
                .GroupBy(k => k.Sponzor, sp => sp, (k, sp) => new Sponzorstvi<Osoba>()
                {
                    Sponzor = k,
                    CastkaCelkem = sp.Sum(m => m.CastkaCelkem),
                    Rok = 0,
                    Strana = strana
                })
                .OrderByDescending(o => o.CastkaCelkem)
                .Take(top)
                ;

        }
        public static IEnumerable<Sponsors.Sponzorstvi<Firma.Lazy>> AllTimeTopSponzorsPerStranaFirmy(string strana, int top = int.MaxValue)
        {
            return AllSponzorsPerYearPerStranaFirmy.Get()
                .Where(m => m.Strana == strana)
                .GroupBy(k => k.Sponzor, sp => sp, (k, sp) => new Sponzorstvi<Firma.Lazy>()
                {
                    Sponzor = k,
                    CastkaCelkem = sp.Sum(m => m.CastkaCelkem),
                    Rok = 0,
                    Strana = strana
                })
                .OrderByDescending(o => o.CastkaCelkem)
                .Take(top)
                ;
        }

        public static Devmasters.Cache.V20.LocalMemory.LocalMemoryCache<IEnumerable<Sponsors.Sponzorstvi<Osoba>>> AllSponzorsPerYearPerStranaOsoby
            = new Devmasters.Cache.V20.LocalMemory.LocalMemoryCache<IEnumerable<Sponsors.Sponzorstvi<Osoba>>>(
                TimeSpan.FromDays(4), "ucty_index_allSponzoriOsoby", (obj) =>
                {
                    List<Sponsors.Sponzorstvi<Osoba>> result = new List<Sponsors.Sponzorstvi<Osoba>>();
                    using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
                    {
                        var res = db.OsobaEvent
                            .Where(Osoba._sponzoringLimitsPredicate)
                            .Where(m => m.Type == (int)OsobaEvent.Types.Sponzor)
                            .Join(db.Osoba, oe => oe.OsobaId, o => o.InternalId, (oe, o) => new { osoba = o, oe = oe })
                            .OrderByDescending(o => o.oe.AddInfoNum)
                            .ToArray()
                            .GroupBy(g => new { osoba = g.osoba, rok = g.oe.DatumOd.Value.Year, strana = g.oe.Organizace }, oe => oe.oe, (o, oe) => new Sponsors.Sponzorstvi<Osoba>()
                            {
                                Sponzor = o.osoba,
                                CastkaCelkem = oe.Sum(e => e.AddInfoNum) ?? 0,
                                Rok = o.rok,
                                Strana = o.strana
                            })
                            .OrderByDescending(o => o.CastkaCelkem);
                        result.AddRange(res);

                    }


                    return result;

                });

        public static Devmasters.Cache.V20.LocalMemory.LocalMemoryCache<IEnumerable<Sponsors.Sponzorstvi<Firma.Lazy>>> AllSponzorsPerYearPerStranaFirmy
        = new Devmasters.Cache.V20.LocalMemory.LocalMemoryCache<IEnumerable<Sponsors.Sponzorstvi<Firma.Lazy>>>(
            TimeSpan.FromDays(4), "sponzori_index_allSponzoriFirmy", (obj) =>
            {
                List<Sponsors.Sponzorstvi<Firma.Lazy>> result = new List<Sponsors.Sponzorstvi<Firma.Lazy>>();
                using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
                {
                    var res = db.FirmaEvent
                        .Where(m => m.Type == (int)FirmaEvent.Types.Sponzor)
                        .OrderByDescending(o => o.AddInfoNum)
                        .ToArray()
                        .GroupBy(g => new { Ico = g.ICO, rok = g.DatumOd.Value.Year, strana = g.AddInfo }, oe => oe, (o, oe) => new Sponsors.Sponzorstvi<Firma.Lazy>()
                        {
                            Sponzor = new Firma.Lazy(o.Ico),
                            CastkaCelkem = oe.Sum(e => e.AddInfoNum) ?? 0,
                            Rok = o.rok,
                            Strana = o.strana
                        })
                        .OrderByDescending(o => o.CastkaCelkem);
                    result.AddRange(res);

                }


                return result;

            });

        public static Dictionary<int, Osoba> GetAllSponsors()
        {
            using (DbEntities db = new Data.DbEntities())
            {
                var ids = db.OsobaEvent
                        .Where(Osoba._sponzoringLimitsPredicate)
                        .Where(m => m.Type == (int)OsobaEvent.Types.Sponzor).Distinct().Select(m => m.OsobaId)
                        .Distinct()
                        .ToArray();

                return ids
                    .Join(StaticData.PolitickyAktivni.Get(), i => i, p => p.InternalId, (i, p) => p)
                    .ToDictionary(k => k.InternalId, o => o);
            }
        }



        public class Sponzorstvi<T>
            where T : class, HlidacStatu.Lib.Data.Bookmark.IBookmarkable //T Osoba nebo Firma
        {
            public T Sponzor { get; set; }
            public String Strana { get; set; }
            public decimal CastkaCelkem { get; set; }
            public int? Rok { get; set; }

            public static explicit operator Sponzorstvi<Bookmark.IBookmarkable>(Sponzorstvi<T> d)  // implicit digit to byte conversion operator
            {
                return new Sponzorstvi<Bookmark.IBookmarkable>()
                {
                    Strana = d.Strana,
                    CastkaCelkem = d.CastkaCelkem,
                    Rok = d.Rok,
                    Sponzor = (Bookmark.IBookmarkable)d.Sponzor
                };
            }
        }

        public static Lib.Render.ReportDataSource<Sponzorstvi<Bookmark.IBookmarkable>> RenderSponzorství(IEnumerable<Sponzorstvi<Bookmark.IBookmarkable>> data, bool showYear = true, bool linkStrana = true)
        {
            var yearCol = new ReportDataSource<Sponzorstvi<Bookmark.IBookmarkable>>.Column()
            {
                Name = "Rok",
                HtmlRender = (s) =>
                {
                    return s.Rok?.ToString();
                },
                OrderValueRender = (s) => { return HlidacStatu.Util.RenderData.OrderValueFormat(s.Rok ?? 0); },
                CssClass = "number"

            };
            ReportDataSource<Sponzorstvi<Bookmark.IBookmarkable>> rokyTable = new ReportDataSource<Sponzorstvi<Bookmark.IBookmarkable>>(
                new ReportDataSource<Sponzorstvi<Bookmark.IBookmarkable>>.Column[] {
                new ReportDataSource<Sponzorstvi<Bookmark.IBookmarkable>>.Column() { Name="Sponzor",
                    HtmlRender = (s) => {
                                    return $"<a href='{s.Sponzor.GetUrl(true)}'>{s.Sponzor.BookmarkName()}</a>";
                            }
                    ,OrderValueRender = (s) => { return HlidacStatu.Util.RenderData.OrderValueFormat(s.Sponzor.BookmarkName()); },
                        },
                new ReportDataSource<Sponzorstvi<Bookmark.IBookmarkable>>.Column() { Name="Částka",
                    HtmlRender = (s) => {
                                    return HlidacStatu.Util.RenderData.NicePrice(s.CastkaCelkem, html:true);
                            },
                    OrderValueRender = (s) => { return HlidacStatu.Util.RenderData.OrderValueFormat(s.CastkaCelkem); },
                    CssClass = "number"
                        },
                new ReportDataSource<Sponzorstvi<Bookmark.IBookmarkable>>.Column() { Name="Strana",
                    HtmlRender = (s) => {
                        if (linkStrana)
                            return GetStranaHtmlLink(s.Strana);
                        else
                            return s.Strana;
                    }
                    ,OrderValueRender = (s) => { return HlidacStatu.Util.RenderData.OrderValueFormat(s.Strana); },
                }
                ,
          });
            if (showYear)
                rokyTable.Columns.Add(yearCol);


            foreach (var r in data.OrderBy(m => m.Rok))
            {
                rokyTable.AddRow(r);
            }

            return rokyTable;
        }

        public static string GetStranaUrl(string strana, bool local = true)
        {
            string url = $"/Sponzori/strana?id={System.Net.WebUtility.UrlEncode(strana)}";
            if (local == false)
                return "http://www.hlidacstatu.cz" + url;
            else
                return url;
        }
        public static string GetStranaHtmlLink(string strana, bool local = true)
        {
            return $"<a href='{GetStranaUrl(strana, local)}'>{strana}</a>";
        }

        public enum SponzoringDataType
        {
            All = 0,
            Osoby = 1,
            Firmy = 1
        }

        public static string GetStranaSponzoringUrl(string strana, int rok, SponzoringDataType typ, bool local = true)
        {
            string url = $"/Sponzori/seznam?id={System.Net.WebUtility.UrlEncode(strana)}&rok={rok}&typ={(int)typ}";
            if (local == false)
                return "http://www.hlidacstatu.cz" + url;
            else
                return url;
        }
        public static string GetStranaSponzoringHtmlLink(string strana, int rok, SponzoringDataType typ, bool local = true)
        {
            return $"<a href='{GetStranaSponzoringUrl(strana, rok, typ, local)}'>Dary osob pro {strana} v {rok}</a>";
        }

    }
}
