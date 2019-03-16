using HlidacStatu.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Web.Framework.Shared
{
    public static class Data
    {
        public static string[] VelkeStrany = new string[] { "ANO 2011", "ODS", "ČSSD", "Piráti","KSČM",
                            "Svoboda a přímá demokracie", "Starostové a nezávislí","KDU-ČSL","TOP 09",
                            "Svobodní","Strana zelených"
                        };
        public static string[] TopStrany = VelkeStrany.Take(9).ToArray();

        public static Devmasters.Cache.V20.LocalMemory.AutoUpdatedLocalMemoryCache<IEnumerable<Sponsors.Sponzorstvi<Osoba>>>
            TopSponzoriOsoby = new Devmasters.Cache.V20.LocalMemory.AutoUpdatedLocalMemoryCache<IEnumerable<Sponsors.Sponzorstvi<Osoba>>>(
                    TimeSpan.FromHours(48), "ucty_index_topSponzoriOsoby", (obj) =>
                    {
                        List<Sponsors.Sponzorstvi<Osoba>> result = new List<Sponsors.Sponzorstvi<Osoba>>();
                        using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
                        {
                            int rok = DateTime.Now.Date.AddMonths(-5).AddYears(-1).Year;
                            foreach (var strana in VelkeStrany)
                            {
                                var res = db.OsobaEvent
                                                    .Where(m => m.Type == (int)OsobaEvent.Types.Sponzor && m.AddInfo == strana && m.DatumOd.HasValue && m.DatumOd.Value.Year == rok)
                                                    .Join(db.Osoba, oe => oe.OsobaId, o => o.InternalId, (oe, o) => new { osoba = o, oe = oe })
                                                    .OrderByDescending(o => o.oe.AddInfoNum)
                                                    .Take(1000)
                                                    .ToArray()
                                                    .GroupBy(g => g.osoba, oe => oe.oe, (o, oe) => new Sponsors.Sponzorstvi<Osoba>()
                                                    {
                                                        Sponzor = o,
                                                        CastkaCelkem = oe.Sum(e => e.AddInfoNum) ?? 0,
                                                        Rok = rok,
                                                        Strana = strana
                                                    })
                                                    .OrderByDescending(o => o.CastkaCelkem)
                                                    .Take(3);
                                result.AddRange(res);
                            }
                        }


                        return result;

                    });
    }
}