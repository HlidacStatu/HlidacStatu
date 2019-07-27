using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HlidacStatu.Lib.Data
{
    public class WatchdogInsolvence : WatchDogProcessor
    {

        bool isLimited = true;

        public WatchdogInsolvence(WatchDog w)
            : base(w)
        {
            if (w.dataType != typeof(Insolvence.Rizeni).Name && w.dataType != WatchDog.AllDbDataType)
                throw new InvalidCastException("It's not INSOLVENCE type");

            isLimited = !(
                w.User().IsInRole("novinar") 
                ||
                w.User().IsInRole("Admin")
                );
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
            email.Model.Insolvence = res.Results.Cast<Insolvence.Rizeni>()
                                .OrderByDescending(o => o.PosledniZmena)
                                .Take(maxCount)
                                .ToList();

            return email;
        }




        protected override WatchDogProcessor.Result DoFinalSearch(string query, DateTime fromDate, DateTime toDate, string order=null)
        {
            query += " AND posledniZmena:" + string.Format("[{0} TO {1}]", ES.SearchTools.ToElasticDate(fromDate), ES.SearchTools.ToElasticDate(toDate)); //AND platnyZaznam:1
            var res = Lib.Data.Insolvence.Insolvence.SimpleSearch(query, 0, 50, 
                order == null ? (int)ES.InsolvenceSearchResult.InsolvenceOrderResult.LatestUpdateDesc : Convert.ToInt32(order ),
                false, isLimited);

            return new WatchDogProcessor.Result(res.ElasticResults.Hits.Select(m => (dynamic)m.Source), res.Total, 
                query, fromDate, toDate, res.IsValid, typeof(Insolvence.Rizeni).Name);

        }

        protected override DateTime? GetLatestRec(DateTime toDate)
        {
            var query = "posledniZmena:" + string.Format("[* TO {0}]", ES.SearchTools.ToElasticDate(toDate));
            var res = Lib.Data.Insolvence.Insolvence.SimpleSearch(query, 0, 1, (int)ES.InsolvenceSearchResult.InsolvenceOrderResult.LatestUpdateDesc, 
                false, isLimited);

            if (res.IsValid == false)
                return null;
            if (res.Total == 0)
                return null;
            return res.ElasticResults.Hits.First().Source.PosledniZmena;
        }


        protected override string EmailNotificationTemplate(WatchDogProcessor.Result res)
        {
            return "FoundInsolvence";
        }

        protected override string HtmlNotificationTemplate(WatchDogProcessor.Result res)
        {
            return "FoundInsolvence";
        }
    }
}
