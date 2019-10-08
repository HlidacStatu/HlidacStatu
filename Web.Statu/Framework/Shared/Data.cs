using HlidacStatu.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Web.Framework.Shared
{
    public static class Data
    {

        [Obsolete("use Lib.Data.Sponzors.AllSponzorsPerYearPerStranaOsoby")]
        public static Devmasters.Cache.V20.LocalMemory.AutoUpdatedLocalMemoryCache<IEnumerable<Sponsors.Sponzorstvi<Osoba>>>
            TopSponzoriOsoby = new Devmasters.Cache.V20.LocalMemory.AutoUpdatedLocalMemoryCache<IEnumerable<Sponsors.Sponzorstvi<Osoba>>>(
                    TimeSpan.FromHours(48), "ucty_index_topSponzoriOsoby", (obj) =>
                    {
                        List<Sponsors.Sponzorstvi<Osoba>> result = new List<Sponsors.Sponzorstvi<Osoba>>();
                        using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
                        {
                            int rok = DateTime.Now.Date.AddMonths(-5).AddYears(-1).Year;
                            foreach (var strana in HlidacStatu.Lib.Data.Sponsors.VelkeStrany)
                            {
                                var res = db.OsobaEvent
                                    .Where(m => m.Type == (int)OsobaEvent.Types.Sponzor && m.Organizace == strana)
                                    .Join(db.Osoba, oe => oe.OsobaId, o => o.InternalId, (oe, o) => new { osoba = o, oe = oe })
                                    .OrderByDescending(o => o.oe.AddInfoNum)
                                    .Take(1000)
                                    .ToArray()
                                    .GroupBy(g => new { osoba = g.osoba, rok = g.oe.DatumDo.Value.Year }, oe => oe.oe, (o, oe) => new Sponsors.Sponzorstvi<Osoba>()
                                    {
                                        Sponzor = o.osoba,
                                        CastkaCelkem = oe.Sum(e => e.AddInfoNum) ?? 0,
                                        Rok = o.rok,
                                        Strana = strana
                                    })
                                    .OrderByDescending(o => o.CastkaCelkem)
                                    .Take(30);
                                result.AddRange(res);
                            }
                        }


                        return result;

                    });


        [Obsolete("use Lib.Data.Sponzors.AllSponzorsPerYearPerStranaFirmy")]
        public static Devmasters.Cache.V20.LocalMemory.AutoUpdatedLocalMemoryCache<IEnumerable<Sponsors.Sponzorstvi<Firma>>>
    TopSponzoriFirmy = new Devmasters.Cache.V20.LocalMemory.AutoUpdatedLocalMemoryCache<IEnumerable<Sponsors.Sponzorstvi<Firma>>>(
            TimeSpan.FromHours(48), "ucty_index_topSponzoriFirmy", (obj) =>
            {
                List<Sponsors.Sponzorstvi<Firma>> result = new List<Sponsors.Sponzorstvi<Firma>>();
                using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
                {
                    int rok = DateTime.Now.Date.AddMonths(-5).AddYears(-1).Year;
                    foreach (var strana in HlidacStatu.Lib.Data.Sponsors.VelkeStrany)
                    {
                        var res = db.FirmaEvent
                            .Where(m => m.Type == (int)OsobaEvent.Types.Sponzor && m.AddInfo == strana && m.DatumOd.HasValue && m.DatumOd.Value.Year == rok)
                            .OrderByDescending(o => o.AddInfoNum)
                            .Take(1000)
                            .ToArray()
                            .GroupBy(g => g.ICO, oe => oe, (o, oe) => new Sponsors.Sponzorstvi<Firma>()
                            {
                                Sponzor = Firmy.Get(o),
                                CastkaCelkem = oe.Sum(e => e.AddInfoNum) ?? 0,
                                Rok = rok,
                                Strana = strana
                            })
                            .OrderByDescending(o => o.CastkaCelkem)
                            .Take(30);
                        result.AddRange(res);
                    }
                }


                return result;

            });
    }
}