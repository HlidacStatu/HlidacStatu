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

        public static string ToNiceString(this Lib.Data.Smlouva.Statistics.Data stat, Data.Bookmark.IBookmarkable item, bool html = true, string customUrl = null, bool twoLines = false)
        {

            if (html)
            {
                var s = "<a href='" + (customUrl ?? (item?.GetUrl(false) ?? "")) + "'>" +
                            Devmasters.Lang.Plural.Get(stat.PocetSmluv, "{0} smlouva;{0} smlouvy;{0} smluv") +
                        "</a>" + (twoLines ? "<br />" : " za ") +
                        "celkem " +
                        HlidacStatu.Lib.Data.Smlouva.NicePrice(stat.CelkovaHodnotaSmluv, html: true, shortFormat: true);
                return s;
            }
            else
                return Devmasters.Lang.Plural.Get(stat.PocetSmluv, "{0} smlouva;{0} smlouvy;{0} smluv") +
                    " za celkem " + HlidacStatu.Lib.Data.Smlouva.NicePrice(stat.CelkovaHodnotaSmluv, html: false, shortFormat: true);

        }



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
