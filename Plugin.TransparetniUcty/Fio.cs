using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HlidacStatu.Util;
using Nest;

namespace HlidacStatu.Plugin.TransparetniUcty
{
    public class Fio : BaseTransparentniUcetParser
    {
        public override string Name => "Fio";

        public Fio(IBankovniUcet ucet) : base(ucet)
        { }

        protected override IEnumerable<IBankovniPolozka> DoParse(DateTime? fromDate = null, DateTime? toDate = null)
        {
            DateTime actFromDate;
            if (!fromDate.HasValue)
                actFromDate = DateTime.Now.Date.AddYears(-1);
            else
                actFromDate = (fromDate.Value < DateTime.Now.Date) ? DateTime.Now.Date.AddYears(-1) : fromDate.Value;

            DateTime actToDate;
            if (!toDate.HasValue)
                actToDate = DateTime.Now.Date.AddDays(1);
            else
                actToDate = (toDate.Value > DateTime.Now.Date) ? DateTime.Now.Date : toDate.Value;

            return ProcessStatement(actFromDate, actToDate);
        }

        private IEnumerable<IBankovniPolozka> ProcessStatement(DateTime actFromDate, DateTime actToDate, int splitDays = 30)
        {
            var url = $"{Ucet.Url}&f={actFromDate:dd.MM.yyyy}&t={actToDate:dd.MM.yyyy}";

            try
            {
                return ParseStatement(url);
            }
            catch (StatementTooLongException)
            {
                var statementItems = new List<IBankovniPolozka>();
                while (actFromDate <= actToDate)
                {
                    statementItems.AddRange(ProcessStatement(actFromDate, actFromDate.AddDays(splitDays), splitDays / 2));
                    actFromDate = actFromDate.AddDays(splitDays + 1);
                }
                return statementItems;
            }
            catch (Exception e)
            {
                TULogger.Error("FIO parse", e);
                return new List<IBankovniPolozka>();
            }
        }

        private IEnumerable<IBankovniPolozka> ParseStatement(string url)
        {
            var polozky = new HashSet<IBankovniPolozka>();

            using (var net = new Devmasters.Net.Web.URLContent(url))
            {
                net.IgnoreHttpErrors = true;
                var content = net.GetContent(Encoding.UTF8).Text;
                if (content.Contains("Některé pohyby nemusí být zobrazeny. Zmenšete datumový rozsah."))
                {
                    throw new StatementTooLongException();
                }
                var doc = new XPath(content);

                var xoverviewRows = "//div[contains(@class, 'pohybySum')]/table/tbody/tr";
                var overviewRows = doc.GetNodes(xoverviewRows)?.Count ?? 0;
                if (overviewRows == 0)
                {
                    TULogger.Warning($"FIO: Account statement page was not found for account {Ucet.CisloUctu}. Account has been probably canceled. Url: {url}");
                    return new List<IBankovniPolozka>();
                }

                var overview = new StatementOverview
                {
                    OpeningBalance = parseAmount(doc.GetNodeText(xoverviewRows + "/td[1]")),
                    FinalBalance = parseAmount(doc.GetNodeText(xoverviewRows + "/td[2]")),
                    CreditSum = parseAmount(doc.GetNodeText(xoverviewRows + "/td[3]")),
                    DebitSum = parseAmount(doc.GetNodeText(xoverviewRows + "/td[4]"))
                };

                var xrows = "//table[@class='table' and starts-with(@id,'id')]/tbody/tr";
                var rows = doc.GetNodes(xrows)?.Count ?? 0;
                for (var row = 1; row <= rows; row++)
                {
                    var xroot = xrows + "[" + row + "]";

                    var p = new SimpleBankovniPolozka
                    {
                        CisloUctu = Ucet.CisloUctu,
                        Datum = ParseTools.ToDateTime(doc.GetNodeText(xroot + "/td[1]"), "dd.MM.yyyy").Value,
                        Castka = parseAmount(System.Net.WebUtility.HtmlDecode(doc.GetNodeText(xroot + "/td[2]"))),
                        PopisTransakce = System.Net.WebUtility.HtmlDecode(doc.GetNodeText(xroot + "/td[3]")),
                        NazevProtiuctu = System.Net.WebUtility.HtmlDecode(doc.GetNodeText(xroot + "/td[4]")),
                        ZpravaProPrijemce = Devmasters.Core.TextUtil.NormalizeToBlockText(
                            System.Net.WebUtility.HtmlDecode(doc.GetNodeHtml(xroot + "/td[5]"))
                                ?.Replace("<br>", " \n")
                        )
                    };

                    var poznamka = Devmasters.Core.TextUtil.NormalizeToBlockText(
                        System.Net.WebUtility.HtmlDecode(doc.GetNodeHtml(xroot + "/td[9]"))
                        ?.Replace("<br>", " \n")
                        );
                    
                    if (poznamka != p.ZpravaProPrijemce)
                        p.ZpravaProPrijemce += " " + poznamka;

                    p.KS = doc.GetNodeText(xroot + "/td[6]");
                    p.VS = doc.GetNodeText(xroot + "/td[7]");
                    p.SS = doc.GetNodeText(xroot + "/td[8]");
                    p.ZdrojUrl = net.Url;


                    p.CisloProtiuctu = ""; //neni k dispozici

                    if (!polozky.Contains(p))
                        polozky.Add(p);

                }

                ValidateParsedItems(polozky, overview);
            }

            return polozky;
        }

        private decimal parseAmount(string value)
        {
            return ParseTools.ToDecimal(value.Replace(" CZK", "").Replace(" ", "")).Value;
        }

        private void ValidateParsedItems(HashSet<IBankovniPolozka> polozky, StatementOverview overview)
        {
            var currentCredit = polozky.Sum(p => p.Castka > 0 ? p.Castka : 0);
            if (overview.CreditSum != currentCredit)
            {
                throw new ApplicationException(
                    $"Invalid total credit (expected {overview.CreditSum}, found {currentCredit}) - {Ucet.Url}");
            }

            var currentDebit = polozky.Sum(p => p.Castka < 0 ? p.Castka : 0);
            if (overview.DebitSum != currentDebit)
            {
                throw new ApplicationException(
                    $"Invalid total debit (expected {overview.DebitSum}, found {currentDebit}) - {Ucet.Url}");
            }

            var currentFinalBalance = overview.OpeningBalance + currentCredit + currentDebit;
            if (overview.FinalBalance != currentFinalBalance)
            {
                throw new ApplicationException(
                    $"Invalid final balance (expected {overview.FinalBalance}, found {currentFinalBalance}) - {Ucet.Url}");
            }
        }

        private class StatementOverview
        {
            public decimal CreditSum { get; set; }
            public decimal DebitSum { get; set; }
            public decimal OpeningBalance { get; set; }
            public decimal FinalBalance { get; set; }
        }
    
        private class StatementTooLongException : Exception { }
    }
}
