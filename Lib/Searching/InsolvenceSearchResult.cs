using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Searching
{
	public class InsolvenceSearchResult : SearchDataResult<Data.Insolvence.Rizeni>
	{
        public InsolvenceSearchResult()
            : base(getVZOrderList)
                {
                }

       public IEnumerable<Data.Insolvence.Rizeni> Results
        {
            get
            {
                if (this.ElasticResults != null)
                    return this.ElasticResults.Hits.Select(m => m.Source);
                else
                    return new Data.Insolvence.Rizeni[] { };
            }
        }

        public bool LimitedView { get; set; } = true;

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
                    Devmasters.Enums.EnumTools.EnumToEnumerable(typeof(InsolvenceOrderResult))
                    .Select(
                        m => new System.Web.Mvc.SelectListItem() { Value = m.Value, Text = "Řadit " + m.Key }
                    )
                )
                .ToList();
        };


        [Devmasters.Enums.ShowNiceDisplayName()]
        public enum InsolvenceOrderResult
        {
            [Devmasters.Enums.SortValue(0)]
            [Devmasters.Enums.NiceDisplayName("podle relevance")]
            Relevance = 0,

            [Devmasters.Enums.SortValue(1)]
            [Devmasters.Enums.NiceDisplayName("nově zahájené první")]
            DateAddedDesc = 1,

            [Devmasters.Enums.NiceDisplayName("nově zveřejněné poslední")]
            [Devmasters.Enums.SortValue(2)]
            DateAddedAsc = 2,


            [Devmasters.Enums.SortValue(3)]
            [Devmasters.Enums.NiceDisplayName("naposled změněné první")]
            LatestUpdateDesc = 3,

            [Devmasters.Enums.SortValue(4)]
            [Devmasters.Enums.NiceDisplayName("naposled změněné poslední")]
            LatestUpdateAsc =43,


            [Devmasters.Enums.Disabled]
            FastestForScroll = 666

        }


    }
}
