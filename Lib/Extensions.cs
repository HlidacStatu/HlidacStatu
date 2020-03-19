using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data;

namespace HlidacStatu.Lib
{
    public static class Extensions
    {
        public static IOrderedEnumerable<Osoba> OrderPoliticiByImportance(this IEnumerable<Osoba> source)
        {
            return source.OrderBy(o =>
                            {
                                var index = Osoba.Searching.PolitikImportanceOrder.IndexOf(o.Status);
                                return index == -1 ? int.MaxValue : index;
                            }
                        )
                        //podle posledni politicke funkce
                        .ThenByDescending(o => o.Events(e => Osoba.Searching.PolitikImportanceEventTypes.Contains(e.Type)).Max(e => e.DatumOd))
                        //podle poctu event
                        .ThenByDescending(o => o.Events_VerejnopravniUdalosti().Count());
        }

    }
}
