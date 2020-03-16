using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching
{
    public class VerejnaZakazkaSearchData
        : SearchDataResult<HlidacStatu.Lib.Data.VZ.VerejnaZakazka>
    {
        public VerejnaZakazkaSearchData()
        : base(getVZOrderList)
        {
            this.Page = 1;
            OblastiList = new System.Web.Mvc.SelectListItem[] { new System.Web.Mvc.SelectListItem() { Value = "", Text = "---" } }
                .Union(
                    Devmasters.Core.Enums.EnumToEnumerable(typeof(Lib.Data.VZ.VerejnaZakazka.Searching.CPVSkupiny))
                    .Select(
                        m => new System.Web.Mvc.SelectListItem() { Value = m.Value, Text = m.Key }
                    )
                )
                .ToList();
        }

        public VerejnaZakazkaSearchData(System.Collections.Specialized.NameValueCollection queryString, VZOrderResult defaultOrder = VZOrderResult.DateAddedDesc)
                : this()
        {
            int page = 1;
            int iorder = (int)defaultOrder;
            bool Zahajeny = false;

            if (!string.IsNullOrEmpty(queryString["Page"]))
            {
                int.TryParse(queryString["Page"], out page);
            }
            if (!string.IsNullOrEmpty(queryString["order"]))
            {
                int.TryParse(queryString["order"], out iorder);
            }
            if (!string.IsNullOrEmpty(queryString["Zahajeny"]))
            {
                bool.TryParse(queryString["Zahajeny"], out Zahajeny);

            }
            this.Page = page;
            this.Order = iorder.ToString();
            this.Zahajeny = Zahajeny;



        }

        public override int DefaultPageSize()
        {
            return 50;
        }

        //public static string PouzeZahajenyQuery = "form:F01,F02,F04,F05,F07,F12,F15,F16,F17,F21,F24,CZ01,CZ02";

        [Devmasters.Core.ShowNiceDisplayName()]
        public enum VZOrderResult
        {
            [Devmasters.Core.SortValue(0)]
            [Devmasters.Core.NiceDisplayName("podle relevance")]
            Relevance = 0,

            [Devmasters.Core.SortValue(5)]
            [Devmasters.Core.NiceDisplayName("nově zveřejněné první")]
            DateAddedDesc = 1,

            [Devmasters.Core.NiceDisplayName("nově zveřejněné poslední")]
            [Devmasters.Core.SortValue(6)]
            DateAddedAsc = 2,

            [Devmasters.Core.SortValue(1)]
            [Devmasters.Core.NiceDisplayName("nejlevnější první")]
            PriceAsc = 3,

            [Devmasters.Core.SortValue(2)]
            [Devmasters.Core.NiceDisplayName("nejdražší první")]
            PriceDesc = 4,

            [Devmasters.Core.SortValue(7)]
            [Devmasters.Core.NiceDisplayName("nově uzavřené první")]
            DateSignedDesc = 5,

            [Devmasters.Core.NiceDisplayName("nově uzavřené poslední")]
            [Devmasters.Core.SortValue(8)]
            DateSignedAsc = 6,

            //[Devmasters.Core.NiceDisplayName("nejvíce chybové první")]
            //[Devmasters.Core.SortValue(10)]
            //ConfidenceDesc = 7,

            [Devmasters.Core.NiceDisplayName("podle odběratele")]
            [Devmasters.Core.SortValue(98)]
            CustomerAsc = 8,
    
            [Devmasters.Core.NiceDisplayName("podle dodavatele")]
            [Devmasters.Core.SortValue(99)]
            ContractorAsc = 9,


            [Devmasters.Core.Disabled]
            FastestForScroll = 666,
            [Devmasters.Core.Disabled]
            LastUpdate = 667,
            [Devmasters.Core.Disabled]
            PosledniZmena = 668,

        }

        public List<System.Web.Mvc.SelectListItem> OblastiList { get; set; } = null;
        
        private string _oblast = "";
        public string Oblast
        {
            get
            {
                return _oblast;
            }

            set
            {
                this._oblast= value;
                if (OblastiList != null && OblastiList.Count > 0)
                {
                    foreach (var item in OrderList)
                    {
                        if (item.Value == this._oblast)
                            item.Selected = true;
                        else
                            item.Selected = false;
                    }
                }


            }
        }
        public string[] CPV = null;

        public bool Zahajeny { get; set; } = false;

        public bool ShowWatchdog { get; set; } = true;

        public Nest.ISearchResponse<Data.VZ.VerejnaZakazka> Result
        {
            get
            {
                return this.ElasticResults;
            }
            set
            {
                this.ElasticResults = value;
            }
        }

        protected static Func<List<System.Web.Mvc.SelectListItem>> getVZOrderList = () =>
        {
            return
                new System.Web.Mvc.SelectListItem[] { new System.Web.Mvc.SelectListItem() { Value="", Text = "---" } }
                .Union(
                    Devmasters.Core.Enums.EnumToEnumerable(typeof(VZOrderResult))
                    .Select(
                        m => new System.Web.Mvc.SelectListItem() { Value = m.Value, Text = "Řadit " + m.Key }
                    )
                )
                .ToList();
        };


        public object ToRouteValues(int page)
        {
            return new
            {
                Q = string.IsNullOrEmpty(this.Q) ? this.OrigQuery : this.Q,
                Page = page,
                Order = this.Order,
                Zahajeny = this.Zahajeny,
                Oblast = this.Oblast
            };
        }


        public static string GetSearchUrl(string pageUrl, string Q, string order = null, int? page = null, bool Zahajeny = false, string oblast = null)
        {

            string ret = string.Format("{0}{1}",
                pageUrl,
               GetSearchUrlQueryString(Q, order, page, Zahajeny,oblast));

            return ret;
        }


        public static string GetSearchUrlQueryString(string Q, string order = null, int? page = null, bool Zahajeny = false, string oblast = null)
        {
            Lib.Searching.VerejnaZakazkaSearchData.VZOrderResult? eorder = null;

            if (Enum.TryParse<VerejnaZakazkaSearchData.VZOrderResult>(order, out var eeorder))
                eorder = eeorder;

            string ret = string.Format("?Q={0}",
               System.Web.HttpUtility.UrlEncode(Q));
            if (Zahajeny)
                ret = ret + "&Zahajeny=true" ;

            if (eorder.HasValue)
                ret = ret + "&order=" + ((int)eorder.Value).ToString();
            if (page.HasValue)
                ret = ret + "&page=" + page.Value.ToString();
            if (oblast != null)
                ret = ret + "&oblast=" + oblast.ToString();
            return ret;
        }

        public override string SocialInfoTitle()
        {
            string spocet = Devmasters.Core.Lang.Plural.GetWithZero((int)this.Total, 
                "Žádné zakázky jsme na H"
                );
            return $"Nalezeno {this.Total}";
        }
    }
}
