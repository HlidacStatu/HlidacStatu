using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HlidacStatu.Web.Framework
{
    public class LimitedAccess
    {
        public class MyTestCrawlerRule : Devmasters.Net.Crawlers.CrawlerBase
        {
            public override string Name => "MyTestCrawlerRule";

            public override string[] IP => new string[] {
                //"77.93.208.131/32", "94.124.109.246/32",
                "127.0.0.1/32" };

            public override string[] HostName => null;

            public override string[] UserAgent => new string[] { "Mozilla/5.0" };
        }

        public static Devmasters.Net.Crawlers.ICrawler[] allCrawl = Devmasters.Net.Crawlers.Helper
            .AllCrawlers
            .Union(new Devmasters.Net.Crawlers.ICrawler[] { new MyTestCrawlerRule() })
            .ToArray();

        public static bool IsAuthenticatedOrSearchCrawler(HttpRequestBase req)
        {
            return req.IsAuthenticated 
                || allCrawl.Any(cr=>cr.IsItCrawler(req.UserHostAddress, req.UserAgent));
            //return req.IsAuthenticated || MajorCrawler.Crawlers.Any(cr=>cr.Detected(req.UserHostAddress, req.UserAgent));
        }

    }
}