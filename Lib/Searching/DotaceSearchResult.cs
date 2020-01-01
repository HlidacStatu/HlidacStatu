using HlidacStatu.Lib.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using HlidacStatu.Lib.Data.Dotace;

namespace HlidacStatu.Lib.Searching
{
    public class DotaceSearchResult : SearchDataResult<Dotace>
    {
        public DotaceSearchResult()
            : base(getOrderList)
        {
        }

        public Nest.ISearchResponse<Dotace> Result
        {
            get
            {
                return ElasticResults;
            }
            set
            {
                ElasticResults = value;
            }
        }

        public bool LimitedView { get; set; } = false;

        public object ToRouteValues(int page)
        {
            return new
            {
                Q = string.IsNullOrEmpty(Q) ? OrigQuery : Q,
                Page = page,
            };
        }

        public bool ShowWatchdog { get; set; } = true;


        protected static Func<List<System.Web.Mvc.SelectListItem>> getOrderList = () =>
        {
            return
                new System.Web.Mvc.SelectListItem[] { new System.Web.Mvc.SelectListItem() { Value = "", Text = "---" } }
                .Union(
                    Devmasters.Core.Enums.EnumToEnumerable(typeof(DotaceOrderResult))
                    .Select(
                        m => new System.Web.Mvc.SelectListItem() { Value = m.Value, Text = "Řadit " + m.Key }
                    )
                )
                .ToList();
        };

        public static string GetSearchUrl(string pageUrl, string Q, Lib.Data.Smlouva.Search.OrderResult? order = null, int? page = null)
        {

            string ret = string.Format("{0}{1}",
                pageUrl,
               GetSearchUrlQueryString(Q, order, page));

            return ret;
        }


        public static string GetSearchUrlQueryString(string Q, Lib.Data.Smlouva.Search.OrderResult? order = null, int? page = null)
        {
            string ret = string.Format("?Q={0}",
               System.Web.HttpUtility.UrlEncode(Q));
            if (order.HasValue)
                ret = ret + "&order=" + ((int)order.Value).ToString();
            if (page.HasValue)
                ret = ret + "&page=" + page.Value.ToString();
            return ret;
        }


        [Devmasters.Core.ShowNiceDisplayName()]
        public enum DotaceOrderResult
        {
            [Devmasters.Core.SortValue(0)]
            [Devmasters.Core.NiceDisplayName("podle relevance")]
            Relevance = 0,

            [Devmasters.Core.SortValue(1)]
            [Devmasters.Core.NiceDisplayName("podle data podpisu od nejnovějších")]
            DateAddedDesc = 1,

            [Devmasters.Core.NiceDisplayName("podle data podpisu od nejstarších")]
            [Devmasters.Core.SortValue(2)]
            DateAddedAsc = 2,

            [Devmasters.Core.SortValue(3)]
            [Devmasters.Core.NiceDisplayName("podle částky od největší")]
            LatestUpdateDesc = 3,

            [Devmasters.Core.SortValue(4)]
            [Devmasters.Core.NiceDisplayName("podle částky od nejmenší")]
            LatestUpdateAsc = 4,

            [Devmasters.Core.SortValue(5)]
            [Devmasters.Core.NiceDisplayName("podle IČO od největšího")]
            ICODesc = 5,

            [Devmasters.Core.SortValue(6)]
            [Devmasters.Core.NiceDisplayName("podle IČO od nejmenšího")]
            ICOAsc = 6,

            [Devmasters.Core.Disabled]
            FastestForScroll = 666

        }
    }
}
