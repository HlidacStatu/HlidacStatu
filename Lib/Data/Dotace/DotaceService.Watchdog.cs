using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HlidacStatu.Lib.Data.Dotace
{
    public partial class DotaceService
    {
        public class WatchdogDotace : WatchDogProcessor
        {


            public WatchdogDotace(WatchDog w)
                : base(w)
            {
                if (w.dataType != typeof(Dotace).Name && w.dataType != WatchDog.AllDbDataType)
                    throw new InvalidCastException("It's not DOTACE type");
            }


            public override string ToString()
            {
                return base.ToString() + $" ({this.OrigWD?.Id})";
            }

            protected override Lib.Emails.EmailMessageFromTemplate CreateNotificationEmail(string txtTemplate, string htmlTemplate, WatchDogProcessor.Result res, string toEmail = null, int maxCount = int.MaxValue)
            {

                var email = new Emails.EmailMessageFromTemplate(txtTemplate,htmlTemplate);
                email.To = toEmail ?? "fake@email.com";

                email.Model.WDName = this.OrigWD.Name;
                email.Model.Interval = GetIntervalString(res);
                email.Model.Total = res.Total;
                email.Model.TotalTxt = HlidacStatu.Util.PluralForm.Get((int)res.Total, "přibyla {0} zakázka;přibyly {0} zakázky;přibylo {0} zakázek", HlidacStatu.Util.Consts.czCulture);
                email.Model.Query = this.OrigWD.SearchTerm;
                email.Model.SpecificQuery = res.RawQuery;
                email.Model.AddResults = res.Results.Count() > maxCount ? res.Results.Count() - maxCount : 0;
                email.Model.Dotace = res.Results.Cast<Dotace>()
                                    .OrderByDescending(o => o.DotaceCelkem)
                                    .Take(maxCount)
                                    .ToList();

                return email;
            }


            DotaceService dotaceService = new DotaceService();

            protected override WatchDogProcessor.Result DoFinalSearch(string query, DateTime fromDate, DateTime toDate, string order = null)
            {
                query += " AND datumAktualizace:" + string.Format("[{0} TO {1}]", Searching.Tools.ToElasticDate(fromDate), Searching.Tools.ToElasticDate(toDate)); //AND platnyZaznam:1
                var res = dotaceService.SimpleSearch(query, 0, 50,
                    order == null ? (int)Searching.DotaceSearchResult.DotaceOrderResult.Relevance : Convert.ToInt32(order),
                    false);

                return new WatchDogProcessor.Result(res.ElasticResults.Hits.Select(m => (dynamic)m.Source), res.Total,
                    query, fromDate, toDate, res.IsValid, typeof(Insolvence.Rizeni).Name);

            }

            protected override DateTime? GetLatestRec(DateTime toDate)
            {
                var query = "datumAktualizace:" + string.Format("[* TO {0}]", Searching.Tools.ToElasticDate(toDate));
                var res = dotaceService.SimpleSearch(query, 0, 1, (int)Searching.DotaceSearchResult.DotaceOrderResult.LatestUpdateDesc,
                    false);

                if (res.IsValid == false)
                    return null;
                if (res.Total == 0)
                    return null;
                return res.ElasticResults.Hits.First().Source.DatumAktualizace;
            }


            protected override string TextTemplate()
            {
                return "FoundDotace";
            }

            protected override string HtmlTemplate()
            {
                return "FoundDotace";
            }
        }
    }
}
