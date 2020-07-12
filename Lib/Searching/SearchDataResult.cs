using HlidacStatu.Util;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching
{
    public class SearchDataResult<T> : Lib.Data.Search.ISearchResult, ISocialInfo
        where T : class
    {

        public const int DefaultPageSizeGlobal = 25;
        public virtual int DefaultPageSize() { return DefaultPageSizeGlobal; }
        public virtual int MaxResultWindow() { return Lib.Searching.Tools.MaxResultWindow; }
        public string Query { get; set; }

        public object ToRouteValues(int page)
        {
            return new
            {
                Q = Query,
                Page = page,
            };
        }


        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Q
        {
            get { return this.Query; }
            set { this.Query = value; }
        }
        public string OrigQuery { get; set; }
        public bool IsValid { get; set; }
        public long Total { get; set; }
        public virtual bool HasResult { get { return IsValid && Total > 0; } }
        public string DataSource { get; set; }

        private string _order = "0";
        public string Order
        {
            get
            {
                return _order;
            }

            set
            {
                this._order = string.IsNullOrWhiteSpace(value) ? "0" : value;

                if (OrderList == null)
                    InitOrderList();
                if (OrderList != null && OrderList.Count > 0)
                {
                    foreach (var item in OrderList)
                    {
                        if (item.Value == this._order.ToString())
                            item.Selected = true;
                        else
                            item.Selected = false;
                    }
                }


            }
        }

        public bool ExactNumOfResults { get; set; } = false;

        public List<System.Web.Mvc.SelectListItem> OrderList { get; set; } = null;
        public Func<T, string> AdditionalRender = null;

        public Nest.ISearchResponse<T> ElasticResults { get; set; }
        public TimeSpan ElapsedTime { get; set; } = TimeSpan.Zero;

        protected Func<List<System.Web.Mvc.SelectListItem>> orderFill = null;
        protected static Func<List<System.Web.Mvc.SelectListItem>> getSmlouvyOrderList = () =>
        {
            return
                Devmasters.Core.Enums.EnumToEnumerable(typeof(HlidacStatu.Lib.Data.Smlouva.Search.OrderResult)).Select(
                    m => new System.Web.Mvc.SelectListItem() { Value = m.Value, Text = "Řadit " + m.Key }
                    ).ToList();
        };

        protected static Func<List<System.Web.Mvc.SelectListItem>> emptyOrderList = () =>
        {
            return new List<System.Web.Mvc.SelectListItem>();
        };
        public virtual string RenderQuery()
        {
            if (!string.IsNullOrEmpty(this.OrigQuery))
                return this.OrigQuery;
            else
                return this.Q;
        }

        public bool SmallRender { get; set; } = false;

        public SearchDataResult()
                : this(emptyOrderList)
        { }

        public SearchDataResult(Func<List<System.Web.Mvc.SelectListItem>> createdOrderList)
        {
            if (createdOrderList == null)
                createdOrderList = emptyOrderList;
            this.orderFill = createdOrderList;
            InitOrderList();
            this.PageSize = DefaultPageSize();

        }

        public void InitOrderList()
        {
            this.OrderList = this.orderFill();
        }


        #region ISocialInfo
        public bool NotInterestingToShow() { return false; }

        public virtual string SocialInfoTitle()
        {
            return string.Empty;
        }

        public virtual string SocialInfoSubTitle()
        {
            return string.Empty;
        }

        public virtual string SocialInfoBody()
        {
            return string.Empty;
        }

        public virtual string SocialInfoFooter()
        {
            return string.Empty;
        }

        public virtual string SocialInfoImageUrl()
        {
            return string.Empty;
        }

        public InfoFact[] InfoFacts()
        {
            return Array.Empty<InfoFact>();
        }
        #endregion

    }
}
