using Devmasters.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HlidacStatu.Lib.Data
{
    public class WatchdogSmlouva : WatchDogProcessor
    {

        public WatchdogSmlouva(WatchDog w)
            : base(w)
        {
            if (w.dataType != typeof(Smlouva).Name && w.dataType != WatchDog.AllDbDataType)
                throw new InvalidCastException("It's not SMLOUVA type");
        }

        public override string ToString()
        {
            return base.ToString() + $" ({this.OrigWD?.Id})";
        }

        protected override Lib.Emails.EmailMsg CreateNotificationEmail(string template, WatchDogProcessor.Result res, string toEmail = null, int maxCount = int.MaxValue)
        {
            Lib.Emails.EmailMsg email;
            if (this.OrigWD.Focus == WatchDog.FocusType.Search)
            {
                email = Emails.EmailMsg.CreateEmailMsgFromPostalTemplate("FoundContracts");
            }
            else
            {
                email = Emails.EmailMsg.CreateEmailMsgFromPostalTemplate("FoundContractsIssues");
                maxCount = 999;
            }

            IEnumerable<Smlouva> smlRes = res.Results.Cast<Smlouva>();

            email.To = toEmail;
            email.Model.WDName = this.OrigWD.Name;
            email.Model.Interval = GetIntervalString(res);
            email.Model.Total = res.Total;
            email.Model.TotalTxt = HlidacStatu.Util.PluralForm.Get((int)res.Total, "přibyla do Registru smluv {0} smlouva;přibyly do Registru smluv {0} smlouvy;přibylo do Registru smluv {0} smluv", HlidacStatu.Util.Consts.czCulture);
            email.Model.Query = this.OrigWD.SearchTerm;
            email.Model.SpecificQuery = res.RawQuery;
            email.Model.AddResults = res.Results.Count() > maxCount ? res.Results.Count() - maxCount : 0;
            email.Model.Smlouvy = smlRes
                                .OrderByDescending(h => this.OrigWD.Focus == WatchDog.FocusType.Search ? h.CalculatedPriceWithVATinCZK : h.ConfidenceValue)
                                .Take(maxCount)
                                .Select(m => new HlidacStatu.Lib.Emails.Templates.FoundContractItem()
                                {
                                    PlatceTxt = m.Platce.nazev,
                                    PlatceHtml = string.Format("<a href=\"{0}\">{1}</a>", "https://www.hlidacstatu.cz/subjekt/" + m.Platce.ico, m.Platce.nazev),
                                    PrijemceTxt = m.Prijemce.Select(f => f.nazev).Aggregate((f, s) => f + ", " + s),
                                    PrijemceHtml = m.Prijemce.Select(f => string.Format("<a href=\"{0}\">{1}</a>", "https://www.hlidacstatu.cz/subjekt/" + f.ico, f.nazev)).Aggregate((f, s) => f + "<br/>" + s),
                                    Cena = HlidacStatu.Lib.Data.Smlouva.NicePrice(m.CalculatedPriceWithVATinCZK,html:true),
                                    Predmet = m.predmet,
                                    Id = m.Id,
                                    Issues = m.Issues
                                        .Where(i => i.Importance > Issues.ImportanceLevel.Ok)
                                        .Select(i => string.Format("{0}: {1}; {2}", i.Importance.ToNiceDisplayName(), i.Title, i.TextDescription))
                                        .ToArray()
                                })
                                .ToList();

            return email;

        }
        protected override WatchDogProcessor.Result DoFinalSearch(string query, DateTime fromDate, DateTime toDate, string order = null)
        {
            query += " AND zverejneno:" + string.Format("[{0} TO {1}]", ES.SearchTools.ToElasticDate(fromDate), ES.SearchTools.ToElasticDate(toDate)); //AND platnyZaznam:1
            var res = Lib.ES.SearchTools.SimpleSearch(query, 0, 50, 
                (ES.SearchTools.OrderResult)Convert.ToInt32(order ?? "4")
                );
            return new WatchDogProcessor.Result(res.ElasticResults.Hits.Select(m => (dynamic)m.Source), res.Total, 
                query, fromDate, toDate, res.IsValid, 
                typeof(Smlouva).Name);
        }

        //public static IEnumerable<WatchdogSmlouva> GetDbSet(DbEntities db, Expression<Func<WatchDog, bool>> predicate)
        //{
        //    var res = getDbItems(db, predicate).Select(m => new WatchdogSmlouva() { originalWD = m });
        //    return res;

        //}


        protected override DateTime? GetLatestRec(DateTime toDate)
        {
            var query = "zverejneno:" + string.Format("[* TO {0}]", ES.SearchTools.ToElasticDate(toDate));
            var res = Lib.ES.SearchTools.SimpleSearch(query, 0, 1, ES.SearchTools.OrderResult.DateAddedDesc);

            if (res.IsValid == false)
                return null;
            if (res.Total == 0)
                return null;
            return res.ElasticResults.Hits.First().Source.LastUpdate;
        }


        protected override string EmailNotificationTemplate(WatchDogProcessor.Result res)
        {
            return "FoundContracts";
        }

        protected override string HtmlNotificationTemplate(WatchDogProcessor.Result res)
        {
            return "FoundContracts";
        }
    }
}
