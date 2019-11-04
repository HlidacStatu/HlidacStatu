using HlidacStatu.Lib.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using HlidacStatu.Lib.Data.Dotace;

namespace HlidacStatu.Lib.ES
{
    public class DotaceSearchResult : SearchDataResult<Dotace>
    {
        public DotaceSearchResult()
            : base(getVZOrderList)
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


        protected static Func<List<System.Web.Mvc.SelectListItem>> getVZOrderList = () =>
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


        [Devmasters.Core.ShowNiceDisplayName()]
        public enum DotaceOrderResult
        {
            [Devmasters.Core.SortValue(0)]
            [Devmasters.Core.NiceDisplayName("podle relevance")]
            Relevance = 0,

            [Devmasters.Core.SortValue(1)]
            [Devmasters.Core.NiceDisplayName("nové první")]
            DateAddedDesc = 1,

            [Devmasters.Core.NiceDisplayName("nové poslední")]
            [Devmasters.Core.SortValue(2)]
            DateAddedAsc = 2,


            [Devmasters.Core.SortValue(3)]
            [Devmasters.Core.NiceDisplayName("naposled změněné první")]
            LatestUpdateDesc = 3,

            [Devmasters.Core.SortValue(4)]
            [Devmasters.Core.NiceDisplayName("naposled změněné poslední")]
            LatestUpdateAsc = 4,


            [Devmasters.Core.Disabled]
            FastestForScroll = 666

        }
    }
}
