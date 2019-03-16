using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Devmasters.Core;
using Devmasters.Net.Web;
using Newtonsoft.Json;

namespace HlidacStatu.Plugin.TransparetniUcty
{
    public class CS : BaseTransparentniUcetParser
    {
        public override string Name { get { return "ČS"; } }

        public CS(IBankovniUcet ucet) : base(ucet)
        { }

        protected override IEnumerable<IBankovniPolozka> DoParse(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var polozky = new List<IBankovniPolozka>();
            var accountDetail = GetAccountDetail();
            if (accountDetail?.accountNumber == null)
            {
                TULogger.Warning($"Account ${Ucet.CisloUctu} was probably canceled");
                return polozky;
            }
            var from = GetNewest(fromDate, accountDetail.transparencyFrom);
            var page = 0;
            var totalRecords = 0;

            do
            {
                var content = GetContent(GetTransactionsPageUrl(accountDetail.accountNumber, from, page));
                var result = JsonConvert.DeserializeObject<CSResult>(content);
                totalRecords = result.recordCount;
                page = result.nextPage;

                foreach (var t in result.transactions ?? new Transaction[0])
                {
                    polozky.Add(new SimpleBankovniPolozka
                    {
                        CisloUctu = Ucet.CisloUctu,
                        Castka = t.amount.value,
                        CisloProtiuctu = t.sender.accountNumber + "/" + t.sender.bankCode,
                        Datum = t.processingDate,
                        KS = t.sender.constantSymbol,
                        NazevProtiuctu = t.sender.name,
                        PopisTransakce = t.typeDescription,
                        SS = t.sender.specificSymbol,
                        VS = t.sender.variableSymbol,
                        ZdrojUrl = "https://www.csas.cz/cs/transparentni-ucty#/" + accountDetail.accountNumber,
                        ZpravaProPrijemce = t.sender.description
                    });
                }
            } while (page > 0);

            if (totalRecords != polozky.Count)
            {
                TULogger.Error($"WE read {polozky.Count} records for account {Ucet.CisloUctu} instead of {totalRecords}");
                throw new ApplicationException($"We read {polozky.Count} records for account {Ucet.CisloUctu} instead of {totalRecords}");
            }

            return polozky;
        }

        private DateTime GetNewest(DateTime? d1, DateTime d2)
        {
            return !d1.HasValue
                ? d2
                : d1.Value > d2
                    ? d1.Value
                    : d2;
        }

        private string GetContent(string url)
        {
            using (var urlContent = new URLContent(url))
            {
                //apikey je v https://www.csas.cz/applications/inlets/_commons/aem/configuration/config.js?cacheBust=2035
                urlContent.RequestParams.Headers.Add("WEB-API-key: 08aef2f7-8b72-4ae1-831e-2155d81f46dd");
                urlContent.Referer = "https://www.csas.cz/cs/transparentni-ucty";
                urlContent.IgnoreHttpErrors = true;

                return urlContent.GetContent(Encoding.UTF8).Text;
            }
        }

        private CSAccountDetail GetAccountDetail()
        {
            var content = GetContent(GetAccountDetailPageUrl(FormatAccountNumber()));
            return JsonConvert.DeserializeObject<CSAccountDetail>(content);
        }

        // CS API docs - https://developers.erstegroup.com/docs/apis/bank.csas/v2%2FtransparentAccounts

        private string GetAccountDetailPageUrl(string accountNumber)
        {
            return $"https://www.csas.cz/webapi/api/v2/transparentAccounts/{accountNumber}";
        }

        private string GetTransactionsPageUrl(string accountNumber, DateTime from, int page)
        {
            return $"https://www.csas.cz/webapi/api/v2/transparentAccounts/{accountNumber}/transactions?dateFrom={from.ToString("yyyy-MM-dd")}&dateTo={DateTime.Now.ToString("yyyy-MM-dd")}&order=DESC&page={page}&size=100&sort=processingDate";
        }

        private string FormatAccountNumber()
        {
            var fullUcet = Ucet.CisloUctu.Replace("/0800", "");
            if (fullUcet.Contains("-"))
                return fullUcet.PadLeft(6, '0');
            return "000000-" + fullUcet.PadLeft(10, '0');
        }

        public class CSAccountDetail
        {
            public string accountNumber { get; set; }
            public string bankCode { get; set; }
            public DateTime transparencyFrom { get; set; }
            public DateTime transparencyTo { get; set; }
            public DateTime publicationTo { get; set; }
            public DateTime actualizationDate { get; set; }
            public decimal balance { get; set; }
            public string currency { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string note { get; set; }
            public string iban { get; set; }
        }

        public class CSResult
        {
            public int nextPage { get; set; } = -1;
            public int pageCount { get; set; }
            public int pageNumber { get; set; }
            public int pageSize { get; set; }
            public int recordCount { get; set; }
            public Transaction[] transactions { get; set; }
        }

        public class Transaction
        {
            public Amount amount { get; set; }
            public DateTime dueDate { get; set; }
            public DateTime processingDate { get; set; }
            public Receiver receiver { get; set; }
            public Sender sender { get; set; }
            public string typeDescription { get; set; }
        }

        public class Amount
        {
            public int precision { get; set; }
            public decimal value { get; set; }
        }

        public class Receiver
        {
            public string accountNumber { get; set; }
            public string bankCode { get; set; }
            public string iban { get; set; }
        }

        public class Sender
        {
            public string accountNumber { get; set; }
            public string bankCode { get; set; }
            public string constantSymbol { get; set; }
            public string description { get; set; }
            public string iban { get; set; }
            public string name { get; set; }
            public string variableSymbol { get; set; }
            public string specificSymbol { get; set; }
            public string specificSymbolParty { get; set; }
        }
    }
}
