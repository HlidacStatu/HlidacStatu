using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching
{
    public class SmlouvaSearchResult
        : SearchDataResult<HlidacStatu.Lib.Data.Smlouva>
    {

        public bool Chyby { get; set; } = false;

        public bool ShowWatchdog { get; set; } = true;

        public bool IncludeNeplatne { get; set; } = false;

        public IEnumerable<Data.Smlouva> Results
        {
            get
            {
                if (this.ElasticResults != null)
                    return this.ElasticResults.Hits.Select(m => m.Source);
                else
                    return new Data.Smlouva[] { };
            }
        }

        public SmlouvaSearchResult()
                : base(getSmlouvyOrderList)
        { }


        public SmlouvaSearchResult(System.Collections.Specialized.NameValueCollection queryString, Lib.Data.Smlouva.Search.OrderResult defaultOrder = Lib.Data.Smlouva.Search.OrderResult.ConfidenceDesc)
                : base(getSmlouvyOrderList)
        {
            int page = 1;
            int iorder = (int)defaultOrder;

            if (!string.IsNullOrEmpty(queryString["Page"]))
            {
                int.TryParse(queryString["Page"], out page);
            }
            if (!string.IsNullOrEmpty(queryString["order"]))
            {
                int.TryParse(queryString["order"], out iorder);
            }
            if (queryString["chyby"] == "1")
            {
                this.Chyby = true;
            }
            if (queryString["neplatne"] == "2")
                this.IncludeNeplatne = true;

            if (this.Page * this.PageSize > MaxResultWindow())
            {
                this.Page = (MaxResultWindow() / this.PageSize) - 1;
            }
            this.Page = page;
            this.Order = iorder.ToString();

        }
        public object ToRouteValues(int page)
        {
            return new
            {
                Q = this.OrigQuery,
                Page = page,
                Order = this.Order,
                Chyby = this.Chyby,
            };
        }


        public static string GetSearchUrl(string pageUrl, string Q, Lib.Data.Smlouva.Search.OrderResult? order = null, int? page = null, bool chyby = false)
        {

            string ret = string.Format("{0}{1}",
                pageUrl,
               GetSearchUrlQueryString(Q, order, page, chyby));

            return ret;
        }


        public static string GetSearchUrlQueryString(string Q, Lib.Data.Smlouva.Search.OrderResult? order = null, int? page = null, bool chyby = false)
        {

            string ret = string.Format("?Q={0}",
               System.Web.HttpUtility.UrlEncode(Q));
            if (chyby)
                ret = ret + "&chyby=true";

            if (order.HasValue)
                ret = ret + "&order=" + ((int)order.Value).ToString();
            if (page.HasValue)
                ret = ret + "&page=" + page.Value.ToString();
            return ret;
        }
    }
}
