using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HlidacStatu.Lib.Data
{

    public abstract class WatchDogProcessor
    {

        protected virtual WatchDog originalWD { get; set; } = null;

        private WatchDogProcessor()
        {
        }

        public WatchDogProcessor(int watchdogId)
            : this(WatchDog.Load(watchdogId))
        {
        }

        public WatchDogProcessor(WatchDog wd)
            : this()
        {
            if (wd == null)
                throw new NullReferenceException();
            originalWD = wd;
        }

        public WatchDog OrigWD { get { return originalWD; } }

        public class Result
        {
            public string dataType = null;
            internal Result(IEnumerable<dynamic> results, long total, string rawQuery, 
                DateTime fromDate, DateTime toDate, bool isValid, string datatype)
            {
                this.Results = results;
                this.RawQuery = rawQuery;
                this.FromDate = fromDate;
                this.ToDate = toDate;
                this.Total = total;
                this.IsValid = isValid;
                this.dataType = datatype.ToLower();
            }
            public IEnumerable<dynamic> Results { get; private set; }
            public string RawQuery { get; private set; }
            public long Total { get; set; }
            public bool IsValid { get; set; }
            public DateTime FromDate { get; private set; }
            public DateTime ToDate { get; private set; }


            public string SearchUrl()
            {
                string q = System.Net.WebUtility.UrlEncode(this.RawQuery);
                if (dataType == typeof(Smlouva).Name.ToLower())
                {
                    return "https://www.hlidacstatu.cz/hledatSmlouvy?Q=" + q + "&order=1";
                }
                else if (dataType == typeof(VZ.VerejnaZakazka).Name.ToLower())
                {
                    return "https://www.hlidacstatu.cz/verejnezakazky/hledat?Q=" + q + "&order=1";
                }
                else
                    return "";

            }
        }
        //public class TwitterData
        //{
        //    public Tweetinvi.Models.TwitterCredentials Credentials { get; set; }
        //    public string TweetContent { get; set; }
        //}

        public Result DoSearch(string order = null)
        {
            return DoSearch(null, null, order);
        }
        public Result DoSearch(string dateInString, string order = null)
        {
            if (string.IsNullOrEmpty(dateInString))
                return DoSearch(order);

            DateTime tmp;
            if (DateTime.TryParseExact(dateInString, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out tmp))
            {
                return DoSearch(tmp, tmp.AddDays(1),order);
            }
            else
                throw new FormatException("dateInString is not in yyyy-MM-dd format. Value:" + dateInString);
        }
        public Result DoSearch(DateTime? fromDate, DateTime? toDate, string order = null)
        {
            string query = string.Format("{0} ", this.OrigWD.SearchTerm);

            if (fromDate.HasValue == false)
            {
                //fromDate = this.OrigWD.LatestRec;
                if (this.OrigWD.LatestRec.HasValue)
                    fromDate = new DateTime(this.originalWD.LatestRec.Value.Ticks, DateTimeKind.Utc);
            }
            if (fromDate.HasValue == false) //because of first search (=> no .LastSearched)
                fromDate = DateTime.Now.Date.AddMonths(-1); //from previous month

            
            if (toDate.HasValue == false)
                toDate = RoundWatchdogTime(DateTime.Now);

            //string date = forDate.Date.ToString("yyyy-MM-ddTHH:mm:ss");



            HlidacStatu.Util.Consts.Logger.Debug($"Processing specific watchdog id {this.OrigWD.Id} ({this.OrigWD.dataType}) SEARCHING, date {fromDate}-{toDate}");
            var res = DoFinalSearch(query, fromDate.Value, toDate.Value, order); //Lib.ES.SearchTools.SimpleSearch(query, 0, 50, order);
            HlidacStatu.Util.Consts.Logger.Debug($"Processing specific watchdog id {this.OrigWD.Id} ({this.OrigWD.dataType}) FOUND, date {fromDate}-{toDate}, found {res.Total} records");
            return res;

        }


        public static string GetIntervalString(Result res)
        {
            return GetIntervalString(res.FromDate, res.ToDate);
        }
        public static string GetIntervalString(DateTime from, DateTime to)
        {
            string sFrom = "";
            string sTo = "";
            if (from == from.Date)
                sFrom = from.ToString("d.M.");
            else
                sFrom = from.ToString("d.M. HH:mm");

            if (to == to.Date)
                sTo = to.ToString("d.M.");
            else
                sTo = to.ToString("d.M. HH:mm");


            return string.Format("od {0} do {1} ", sFrom, sTo);
        }

        public bool ReadyToRun()
        {
            return ReadyToRun(DateTime.Now);
        }

        public bool ReadyToRun(DateTime when)
        {
            if (this.OrigWD.LastSearched.HasValue == false)
                return true;

            //round lastsearch to the begin of the day

            var lastSearch = this.OrigWD.LastSearched.Value;
            lastSearch = RoundWatchdogTime(lastSearch);
            switch (this.OrigWD.Period)
            {
                case WatchDog.PeriodTime.Immediatelly:
                    return true;
                case WatchDog.PeriodTime.Hourly:
                    return (when - lastSearch).TotalHours >= 1.0;
                case WatchDog.PeriodTime.Daily:
                    return (when - lastSearch).TotalHours >= 23.0;
                case WatchDog.PeriodTime.Weekly:
                    return (when - lastSearch).TotalDays >= 6.5;
                case WatchDog.PeriodTime.Monthly:
                    return Devmasters.Core.DateTimeSpan.CompareDates(when, this.OrigWD.LastSearched.Value).Months > 0;
            }
            return true;
        }
        public virtual DateTime RoundWatchdogTime(DateTime dt)
        {
            return DefaultRoundWatchdogTime(this.OrigWD.Period, dt);
        }

        public static DateTime DefaultRoundWatchdogTime(WatchDog.PeriodTime period, DateTime dt)
        {
            switch (period)
            {
                case WatchDog.PeriodTime.Immediatelly:
                    return dt;
                case WatchDog.PeriodTime.Hourly:
                    return dt.AddMinutes(-1 * dt.Minute);
                case WatchDog.PeriodTime.Daily:
                case WatchDog.PeriodTime.Monthly:
                    return dt.Date;
                case WatchDog.PeriodTime.Weekly:
                    return dt.AddHours(-1 * dt.Hour);
                default:
                    return dt;
            }

        }

        public static string APIID_Prefix = "APIID:";
        public bool SendNotification(WatchDogProcessor.Result result, string specificContact = null)
        {
            string contact = specificContact;
            if (string.IsNullOrEmpty(contact))
            {
                if (!string.IsNullOrEmpty(this.OrigWD.SpecificContact))
                    contact = this.OrigWD.SpecificContact;
                else
                    contact = this.OrigWD.User().Email;
            }

            HlidacStatu.Util.Consts.Logger.Debug($"Processing specific watchdog id {this.OrigWD.Id} ({this.OrigWD.dataType}) SENDING to contact {contact}.");
            if (Devmasters.Core.TextUtil.IsValidEmail(contact))
            {
                return SendEmailNotification(contact, result);
            }

            if (contact == "HTTPPOSTBACK")
            {
                // https://daxiel.solcloud.cz/api/match
                /*
                {
                “id”:” id je ID hlidace ve vasem systemu”,
                “found”:[{ExportedVZ}, {ExportedVZ
                }]
                }
                kde ExportedVZ je datova struktura zakazky(viz dale)
                */
                if (result.dataType == typeof(VZ.VerejnaZakazka).Name.ToLower())
                {
                    var jsonResult = new
                    {
                        id = this.OrigWD.Name.Replace(APIID_Prefix, ""),
                        found = result.Results
                                .Cast<VZ.VerejnaZakazka>()
                                .Take(400)
                                .Select(m => m.Export())
                                .ToArray()
                    };

                    try
                    {
                        using (Devmasters.Net.Web.URLContent net = new Devmasters.Net.Web.URLContent("https://daxiel.solcloud.cz/api/match"))
                        {
                            net.Tries = 5;
                            net.TimeInMsBetweenTries = 2000;
                            net.Timeout = 30000;
                            net.Method = Devmasters.Net.Web.MethodEnum.POST;
                            net.RequestParams.RawContent = Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult);
                            net.ContentType = "application/json; charset=utf-8";
                            var apires = net.GetContent();
                        }
                    }
                    catch (Exception ex)
                    {
                        HlidacStatu.Util.Consts.Logger.Error("sending data to Solidis error", ex);
                        throw;
                    }
                }

            }

            var platformaTopicId = HlidacStatu.Util.ParseTools.GetRegexGroupValue(contact, @"(platforma\((?<platforma>\d*)\))", "platforma");
            if (Devmasters.Core.TextUtil.IsNumeric(platformaTopicId))
            {
                return SendPlatformaTopicAnswer(Convert.ToInt32(platformaTopicId), result);
            }

            var twitterCred = HlidacStatu.Util.ParseTools.GetRegexGroupValue(contact, @"^twitter\(\s*(?<json>{ .* })\s* \)", "json");
            if (!string.IsNullOrEmpty(twitterCred))
            {
                //return SendTwitter(twitterCred, result);
            }
            

            return false;
        }
        //public virtual bool SendTwitter(string credJson, WatchDogBase<T>.Result res)
        //{
        //    try
        //    {
        //        TwitterData td = Newtonsoft.Json.JsonConvert.DeserializeObject<TwitterData>(credJson);
        //        var total = res.QueryResult.Total;
        //        var zakazkyStr = Devmasters.Core.Lang.Plural.GetWithZero((int)total, "žádné nové zakázky", "jednu novou zakázku", "{0} nové zakázky", "{0} nových zakázek");
        //        var fromDate = res.FromDate.ToString("d.M.");
        //        var toDate = res.ToDate.ToString("d.M.");
        //        var queryUrl = res.SearchUrl();
        //        var tweet = string.Format(td.TweetContent, fromDate, toDate, total, zakazkyStr, queryUrl);

        //        return StaticData.TweetingManager.GetInstance(td.Credentials).Publish(tweet);
        //    }
        //    catch (Exception e)
        //    {
        //        HlidacStatu.Util.Consts.Logger.Error("WatchDog Notification error", e);
        //        return false;
        //    }
        //}

        public virtual bool SendEmailNotification(string toEmail, WatchDogProcessor.Result res)
        {
            int maxCount = 30;

            try
            {
                var email = CreateNotificationEmail(EmailNotificationTemplate(res), res, toEmail, maxCount);
                if (email == null)
                    return false;

                //copy common properties to Model
                email.Model.EmailFooterHtml = email.EmailFooterHtml;
                email.Model.EmailFooterText = email.EmailFooterText;
                email.From = "hlidac@hlidacstatu.cz";
                if (email != null)
                    email.SendMe();
                return true;
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("WatchDog Notification error", e);
                return false;
            }

        }


        public virtual string GetHtmlNotificationContent(WatchDogProcessor.Result res)
        {
            try
            {

                var email = CreateNotificationEmail(HtmlNotificationTemplate(res), res, null, 12);
                var s = email.RenderHtml();
                return s;


            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("WatchDog Notification error", e);
                return string.Empty;
            }

        }

        public virtual bool SendPlatformaTopicAnswer(int topicId, WatchDogProcessor.Result res, int maxCount = 10)
        {
            try
            {
                var s = this.GetHtmlNotificationContent(res);
                var dis = new Lib.Data.External.Discourse();
                var ok = dis.PostToTopic(topicId, s);
                if (res.Total > 0)
                    this.OrigWD.LastSent = this.OrigWD.LastSearched;

                return true;
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("WatchDog Notification error", e);
                return false;
            }

        }

        protected abstract string EmailNotificationTemplate(WatchDogProcessor.Result res);
        protected abstract string HtmlNotificationTemplate(WatchDogProcessor.Result res);

        protected abstract Lib.Emails.EmailMsg CreateNotificationEmail(string template, WatchDogProcessor.Result res, string toEmail = null, int maxCount = int.MaxValue);
        protected abstract WatchDogProcessor.Result DoFinalSearch(string query, DateTime fromDate, DateTime toDate, string order);

        protected abstract DateTime? GetLatestRec(DateTime toDate);

        public override string ToString()
        {
            return $"{this.GetType().Name}";
        }

    }
}