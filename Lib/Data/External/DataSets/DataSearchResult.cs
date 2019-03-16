using HlidacStatu.Lib.ES;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace HlidacStatu.Lib.Data.External.DataSets
{
    public class DataSearchResult
    : DataSearchResultBase<dynamic>
    {
        public DataSearchResult() : base()
        {
        }

        public DataSearchResult(NameValueCollection queryString, SearchTools.OrderResult defaultOrder = SearchTools.OrderResult.Relevance)
            : base(queryString, defaultOrder)
        {
        }
    }
    public class DataSearchRawResult
    : DataSearchResultBase<Tuple<string, string>>
    {

        public DataSearchRawResult() : base()
        {
        }

        public DataSearchRawResult(NameValueCollection queryString, SearchTools.OrderResult defaultOrder = SearchTools.OrderResult.Relevance)
            : base(queryString, defaultOrder)
        {
        }
    }

    public class DataSearchResultBase<T> : SearchData<T>
    where T : class
    {

        public bool ShowWatchdog { get; set; } = true;

        public IEnumerable<T> Result { get; set; }

        public DataSearchResultBase()
                : base(null)
        {
            this.orderFill = getDatasetOrderList;
            InitOrderList();
            this.Page = 1;
        }

        private DataSet _dataset = null;
        public DataSet DataSet
        {
            get
            {
                return _dataset;
            }
            set
            {
                _dataset = value;
                InitOrderList();

            }
        }


        public const string OrderAsc = " vzestupně";
        public const string OrderAscUrl = " asc";
        public const string OrderDesc = " sestupně";
        public const string OrderDescUrl = " desc";
        protected List<System.Web.Mvc.SelectListItem> getDatasetOrderList()
        {
            if (this.DataSet == null)
                return new List<System.Web.Mvc.SelectListItem>();

            List<System.Web.Mvc.SelectListItem> list = new List<System.Web.Mvc.SelectListItem>();
            list.Add(new System.Web.Mvc.SelectListItem()
            {
                Text = "Relevance",
                Value = "0"
            });

            for (int i = 0; i < this.DataSet.Registration().orderList.GetLength(0); i++)
            {
                list.Add(new System.Web.Mvc.SelectListItem()
                {
                    Text = this.DataSet.Registration().orderList[i, 0] + OrderAsc,
                    Value = this.DataSet.Registration().orderList[i, 1] + OrderAscUrl
                });
                list.Add(new System.Web.Mvc.SelectListItem()
                {
                    Text = this.DataSet.Registration().orderList[i, 0] + OrderDesc,
                    Value = this.DataSet.Registration().orderList[i, 1] + OrderDescUrl
                });
            }
            return list;
                //Devmasters.Core.Enums.EnumToEnumerable(typeof(HlidacStatu.Lib.ES.SearchTools.OrderResult)).Select(
                //    m => new System.Web.Mvc.SelectListItem() { Value = m.Value, Text = "Řadit " + m.Key }
                //    ).ToList();
        }


        public DataSearchResultBase(System.Collections.Specialized.NameValueCollection queryString,
            Lib.ES.SearchTools.OrderResult defaultOrder = Lib.ES.SearchTools.OrderResult.Relevance)
                : base(getSmlouvyOrderList)
        {
            int page = 1;
            int iorder = (int)defaultOrder;

            if (!string.IsNullOrEmpty(queryString["Page"]))
            {
                int.TryParse(queryString["Page"], out page);
            }
            if (this.Page * this.PageSize > MaxResultWindow())
            {
                this.Page = (MaxResultWindow() / this.PageSize) - 1;
            }
            this.Page = page;
            this.Order = iorder.ToString();
        }

        protected static Func<List<System.Web.Mvc.SelectListItem>> getVZOrderList = () =>
        {
            return new List<System.Web.Mvc.SelectListItem>();
        };


        public object ToRouteValues(int page)
        {
            return new
            {
                Q = this.Q,
                Page = page,
                Order = this.Order,
                Id = this.DataSet.DatasetId
            };
        }


        public static string GetSearchUrl(string pageUrl, string Q, Lib.ES.SearchTools.OrderResult? order = null, int? page = null)
        {

            string ret = string.Format("{0}{1}",
                pageUrl,
               GetSearchUrlQueryString(Q, order, page));

            return ret;
        }


        public static string GetSearchUrlQueryString(string Q, Lib.ES.SearchTools.OrderResult? order = null, int? page = null)
        {

            string ret = string.Format("?Q={0}",
               System.Web.HttpUtility.UrlEncode(Q));

            if (order.HasValue)
                ret = ret + "&order=" + ((int)order.Value).ToString();
            if (page.HasValue)
                ret = ret + "&page=" + page.Value.ToString();
            return ret;
        }
    }
}
