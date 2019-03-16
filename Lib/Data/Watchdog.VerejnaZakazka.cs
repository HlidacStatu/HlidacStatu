using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HlidacStatu.Lib.Data
{
    public class WatchdogVerejnaZakazka : WatchDogProcessor
    {

        public WatchdogVerejnaZakazka(WatchDog w)
            : base(w)
        {
            if (w.dataType != typeof(VZ.VerejnaZakazka).Name && w.dataType != WatchDog.AllDbDataType)
                throw new InvalidCastException("It's not VEREJNAZAKAZKA type");
        }


        public override string ToString()
        {
            return base.ToString() + $" ({this.OrigWD?.Id})";
        }

        protected override Lib.Emails.EmailMsg CreateNotificationEmail(string template, WatchDogProcessor.Result res, string toEmail = null, int maxCount = int.MaxValue)
        {
            var email = Emails.EmailMsg.CreateEmailMsgFromPostalTemplate(template);
            email.To = toEmail ?? "fake@email.com";

            email.Model.WDName = this.OrigWD.Name;
            email.Model.Interval = GetIntervalString(res);
            email.Model.Total = res.Total;
            email.Model.TotalTxt = HlidacStatu.Util.PluralForm.Get((int)res.Total, "přibyla {0} zakázka;přibyly {0} zakázky;přibylo {0} zakázek", HlidacStatu.Util.Consts.czCulture);
            email.Model.Query = this.OrigWD.SearchTerm;
            email.Model.SpecificQuery = res.RawQuery;
            email.Model.AddResults = res.Results.Count() > maxCount ? res.Results.Count() - maxCount : 0;
            email.Model.Zakazky = res.Results.Cast<VZ.VerejnaZakazka>()
                                .OrderByDescending(o => Math.Max(o.OdhadovanaHodnotaBezDPH ?? 0, o.KonecnaHodnotaBezDPH ?? 0))
                                .Take(maxCount)
                                .ToList();

            return email;
        }




        protected override WatchDogProcessor.Result DoFinalSearch(string query, DateTime fromDate, DateTime toDate, string order=null)
        {
            query += " AND lastUpdated:" + string.Format("[{0} TO {1}]", ES.SearchTools.ToElasticDate(fromDate), ES.SearchTools.ToElasticDate(toDate)); //AND platnyZaznam:1
            var res = Lib.Data.VZ.VerejnaZakazka.Searching.SimpleSearch(query, null, 0, 50, 
                order == null ? (int)ES.VerejnaZakazkaSearchData.VZOrderResult.DateAddedDesc : Convert.ToInt32(order ), 
                this.OrigWD.FocusId == 1);

            return new WatchDogProcessor.Result(res.ElasticResults.Hits.Select(m => (dynamic)m.Source), res.Total, 
                query, fromDate, toDate, res.IsValid, typeof(VZ.VerejnaZakazka).Name);

        }

        protected override DateTime? GetLatestRec(DateTime toDate)
        {
            var query = "lastUpdated:" + string.Format("[* TO {0}]", ES.SearchTools.ToElasticDate(toDate));
            var res = Lib.Data.VZ.VerejnaZakazka.Searching.SimpleSearch(query, null, 0, 1, (int)ES.VerejnaZakazkaSearchData.VZOrderResult.LastUpdate);

            if (res.IsValid == false)
                return null;
            if (res.Total == 0)
                return null;
            return res.ElasticResults.Hits.First().Source.LastUpdated;
        }


        protected override string EmailNotificationTemplate(WatchDogProcessor.Result res)
        {
            return "FoundZakazky";
        }

        protected override string HtmlNotificationTemplate(WatchDogProcessor.Result res)
        {
            if (res.Total == 0)
                return "NotFoundZakazkyMarkdownPost";
            else
                return "FoundZakazkyMarkdownPost";
        }
    }
}
