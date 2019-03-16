using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Plugin.TransparetniUcty
{
    public class RB : BaseTransparentniUcetParser
    {

        public override string Name { get { return "RB"; } }


        public RB(IBankovniUcet ucet) : base(ucet)
        {
        }

        protected override IEnumerable<IBankovniPolozka> DoParse(DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<IBankovniPolozka> polozky = new List<IBankovniPolozka>();

            if (!fromDate.HasValue)
                fromDate = DateTime.Now.Date.AddYears(-1).AddDays(1);

            if (!toDate.HasValue)
                toDate = DateTime.Now.Date;

            int page = 0;

            //https://www.rb.cz/o-nas/povinne-uverejnovane-informace/transparentni-ucty?p_p_id=Transparentaccountportlet_WAR_Transparentaccountportlet_INSTANCE_e6cf4781&p_p_lifecycle=2&p_p_state=normal&p_p_mode=view&p_p_resource_id=nextTransactions&p_p_cacheability=cacheLevelPage&p_p_col_id=_DynamicNestedPortlet_INSTANCE_f5c4beca__column-1-1&p_p_col_count=1&idBankAccount=24389217&fromIndex=51&dateFrom=2016-3-1&dateTo=2018-3-9&q=
            //https://www.rb.cz/o-nas/povinne-uverejnovane-informace/transparentni-ucty?p_p_id=Transparentaccountportlet_WAR_Transparentaccountportlet_INSTANCE_e6cf4781&p_p_lifecycle=2&p_p_state=normal&p_p_mode=view&p_p_resource_id=nextTransactions&p_p_cacheability=cacheLevelPage&p_p_col_id=_DynamicNestedPortlet_INSTANCE_f5c4beca__column-1-1&p_p_col_count=1&idBankAccount=24389217&fromIndex=0&dateFrom=2016-3-1&dateTo=2018-3-17&q=

            using (Devmasters.Net.Web.URLContent baseUrl = new Devmasters.Net.Web.URLContent(this.Ucet.Url))
            {
                baseUrl.IgnoreHttpErrors = true;
                var html = baseUrl.GetContent(Encoding.UTF8);

                var webReqInstance = HlidacStatu.Util.ParseTools.GetRegexGroupValue(html.Text, "Transparentaccountportlet_INSTANCE_(?<inst>[a-z0-9]*)_", "inst");
                var dynamicInst = HlidacStatu.Util.ParseTools.GetRegexGroupValue(html.Text, "p_p_id_DynamicNestedPortlet_INSTANCE_(?<inst>[a-z0-9]*)_", "inst");
                var internalIdBankAccount = HlidacStatu.Util.ParseTools.GetRegexGroupValue(html.Text, @"idBankAccount=(?<id>\d*)", "id");
                if (!string.IsNullOrEmpty(webReqInstance))
                {
                    bool getSomeData = true;
                    string cisloUctu = this.Ucet.CisloUctu.Split('/')[0];

                    do
                    {
                        string url = string.Format(@"https://www.rb.cz/o-nas/povinne-uverejnovane-informace/transparentni-ucty?" +
                                "p_p_id=Transparentaccountportlet_WAR_Transparentaccountportlet_INSTANCE_{0}&p_p_lifecycle=2&p_p_state=normal" 
                                + "&p_p_mode=view&p_p_resource_id=nextTransactions&p_p_cacheability=cacheLevelPage"
                                + "&p_p_col_id=_DynamicNestedPortlet_INSTANCE_{1}__column-1-1&p_p_col_count=1" 
                                + "&idBankAccount={2}&fromIndex={3}&dateFrom={4}&dateTo={5}&q="
                            , webReqInstance, dynamicInst, internalIdBankAccount, page * 20 +1, fromDate.Value.ToString("yyyy-M-d"), toDate.Value.ToString("yyyy-M-d"));

                        using (Devmasters.Net.Web.URLContent net = new Devmasters.Net.Web.URLContent(url, html.Context))
                        {
                            net.IgnoreHttpErrors = true;
                            var json = net.GetContent().Text;

                            try
                            {
                                RBData data = Newtonsoft.Json.JsonConvert.DeserializeObject<RBData>(json);
                                page++;
                                if (data.transactions != null && data.transactions.Count() > 0)
                                {
                                    getSomeData = true;
                                    polozky.AddRange(
                                        data.transactions
                                            .Select(m=>new SimpleBankovniPolozka() {
                                                 Castka = HlidacStatu.Util.ParseTools.ToDecimal(m.amount) ?? 0,
                                                 CisloProtiuctu = "",
                                                 CisloUctu = this.Ucet.CisloUctu,
                                                 Datum = HlidacStatu.Util.ParseTools.ToDateTime(m.datumDate,"dd.MM.yyyy").Value,
                                                 KS = m.constSymbol,
                                                 NazevProtiuctu = m.accountName,
                                                 PopisTransakce = m.type,
                                                 SS = m.specSymbol,
                                                 VS = m.varSymbol,
                                                 ZdrojUrl = baseUrl.Url,
                                                 ZpravaProPrijemce = m.info,
                                                 
                                            })
                                        );
                                }
                                else
                                    getSomeData = false;
                            }
                            catch (Exception e)
                            {
                                TULogger.Error("RB parser JSON error",e);
                                return polozky;
                            }


                        }
                    } while (getSomeData);


                }

            }



            return polozky;
        }

        protected string GetPageUrl(int page)
        {
            return "";
        }




        public class RBData
        {
            public Transaction[] transactions { get; set; }
            public bool endOfRecords { get; set; }
            public int nextFromIndex { get; set; }


            public class Transaction
            {
                public string realizationDate { get; set; }
                public string realizationTime { get; set; }
                public string info { get; set; }
                public string accountName { get; set; }
                public string datumDate { get; set; }
                public string valutaDate { get; set; }
                public string type { get; set; }
                public string varSymbol { get; set; }
                public string constSymbol { get; set; }
                public string specSymbol { get; set; }
                public string amount { get; set; }
                public string charge { get; set; }
                public string chargeCnv { get; set; }
                public string chargeMsg { get; set; }
            }
        }
    }
}
