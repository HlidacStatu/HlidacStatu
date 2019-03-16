using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HlidacStatu.Lib.Data.External.DataSets
{
    public class WatchdogDataset : WatchDogProcessor
    {
        protected override WatchDog originalWD
        {
            get => base.originalWD;
            set
            {
                if (value != null)
                {
                    if (value.dataType != WatchDog.AllDbDataType)
                    {
                        var dataSetId = value.dataType.Replace("DataSet.", "");
                        try
                        {
                            this.DataSet = DataSet.CachedDatasets.Get(dataSetId);
                        }
                        catch
                        {
                            //removed dataset, disable wd
                            this.DataSet = null;
                            base.originalWD = value;
                            base.originalWD.DisableWDBySystem(WatchDog.DisabledBySystemReasons.NoDataset);
                        }
                    }
                }
                base.originalWD = value;
            }

        }

        DataSet _dataset = null;
        protected DataSet DataSet
        {
            get
            {
                if (_dataset == null)
                {
                    var dataSetId = OrigWD.dataType.Replace("DataSet.", "");
                    _dataset = DataSet.CachedDatasets.Get(dataSetId);
                }
                return _dataset;
            }

            set
            {
                _dataset = value;
            }
        }

        public WatchdogDataset(WatchDog w, DataSet dataset )
            : base(w)
        {
            if (w.dataType != WatchDog.AllDbDataType)
                throw new InvalidCastException("It's not #ALL# type");

            this.DataSet = dataset;

        }

        public WatchdogDataset(WatchDog w)
            : base(w)
        {
            if (w.dataType?.StartsWith("DataSet") == false)
                throw new InvalidCastException("It's not DATASET type");

            var dataSetId = w.dataType.Replace("DataSet.", "");
            this.DataSet = DataSet.CachedDatasets.Get(dataSetId);

        }

        public override string ToString()
        {
            return base.ToString() + $".{this.DataSet?.DatasetId} ({this.OrigWD?.Id})";
        }

        protected override Lib.Emails.EmailMsg CreateNotificationEmail(string viewName, WatchDogProcessor.Result res, string toEmail = null, int maxCount = int.MaxValue)
        {
            //dont send anything until there is template
            if (!this.DataSet.Registration().searchResultTemplate.IsFullTemplate())
                return null;

            var email = Emails.EmailMsg.CreateEmailMsgFromPostalTemplate(viewName);

            //remove text version of mail.
            email.TextTemplate = null;

            //update html template with sub templates from dataset

            email.HtmlTemplate = email.HtmlTemplate.Replace("#HEADERTEMPLATE#", "");

            //result to the expected class structure
            var model = new HlidacStatu.Lib.Data.External.DataSets.Registration.Template.SearchTemplateResults();
            model.Total = res.Total;
            model.Page = 1;
            model.Q = res.RawQuery;
            model.Result = res.Results;

            email.HtmlTemplate = email.HtmlTemplate.Replace("#ITEMTEMPLATE#",
                this.DataSet.Registration().searchResultTemplate.Render(this.DataSet, model)
                );


            //add functions
            //read App_Code.Fn.cshtml

            email.To = toEmail ?? "fake@email.com";

            email.Model.WDName = this.OrigWD.Name;
            email.Model.Interval = GetIntervalString(res);
            email.Model.Dataset = this.DataSet;
            email.Model.Total = res.Total;
            email.Model.TotalTxt = HlidacStatu.Util.PluralForm.Get((int)res.Total, " {0} záznam; {0} záznamy; {0} záznamů", HlidacStatu.Util.Consts.czCulture);
            email.Model.Query = this.OrigWD.SearchTerm;
            email.Model.SpecificQuery = res.RawQuery;
            email.Model.SearchQuery = this.DataSet.DatasetSearchUrl(this.originalWD.SearchTerm, false);
            email.Model.AddResults = res.Results.Count() > maxCount ? res.Results.Count() - maxCount : 0;
            email.Model.Results = res.Results
                                //.OrderByDescending(o=> Math.Max(o.Source.OdhadovanaHodnotaBezDPH ?? 0, o.Source.KonecnaHodnotaBezDPH ?? 0))
                                .Take(maxCount)
                                //.Select(h => h.Source)
                                .ToList();
            ;
            email.Subject = email.Subject.Trim() + " " + this.DataSet.Registration().name;
            return email;
        }




        protected override WatchDogProcessor.Result DoFinalSearch(string query, DateTime fromDate, DateTime toDate, string order)
        {
            query += " AND DbCreated:" + string.Format("{{{0} TO {1}]", ES.SearchTools.ToElasticDate(fromDate), ES.SearchTools.ToElasticDate(toDate));

            DataSearchResult res = this.DataSet.SearchData(query, 1, 50, order);

            return new WatchDogProcessor.Result(res.Result, res.Total, query, fromDate, toDate, res.IsValid, WatchDog.AllDbDataType );

        }

        protected override DateTime? GetLatestRec(DateTime toDate)
        {
            var query = "DbCreated:" + string.Format("[* TO {0}]", ES.SearchTools.ToElasticDate(toDate));

            DataSearchResult res = this.DataSet.SearchData(query, 1, 1, "DbCreated desc");
            //var res = Lib.Data.VZ.VerejnaZakazka.Searching.SimpleSearch(query, null, 0, 1, (int)ES.VerejnaZakazkaSearchData.VZOrderResult.LastUpdate);

            if (res.IsValid == false)
                return null;
            if (res.Total == 0)
                return null;
            return (DateTime)res.Result.First().DbCreated;
        }

        public override DateTime RoundWatchdogTime(DateTime dt)
        {
            if (this.OrigWD.Period == WatchDog.PeriodTime.Daily)
                return dt;
            else
                return base.RoundWatchdogTime(dt);
        }

        protected override string EmailNotificationTemplate(WatchDogProcessor.Result res)
        {
            return "FoundDataset";
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
