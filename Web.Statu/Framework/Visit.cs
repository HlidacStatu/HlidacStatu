using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HlidacStatu.Web.Framework
{

    public static class Visit
    {


        public class Crawler
        {
            public string pattern { get; set; }
            public string addition_date { get; set; }
            public string url { get; set; }
            public string[] instances { get; set; }
        }

        static object lockObj = new object();
        static HashSet<string> AllCrawlers = null;
        static Visit()
        {
            lock (lockObj)
            {
                if (AllCrawlers == null)
                    AllCrawlers = new HashSet<string>(
                        Newtonsoft.Json.JsonConvert.
                            DeserializeObject<Crawler[]>(System.IO.File.ReadAllText(HlidacStatu.Lib.Init.WebAppDataPath + "crawler-user-agents.json"))
                        .SelectMany(m => m.instances)
                        .Distinct()
                        );
            }
        }

        public static bool IsCrawler(string useragent)
        {
            if (string.IsNullOrEmpty(useragent))
                return false;

            return AllCrawlers.Contains(useragent);
        }

        public enum VisitChannel
        {
            Web = 0,
            Api = 1,
            Crawler = 2,
            Embed = 3
        }
        public static void AddVisit(string page, VisitChannel channel)
        {
            if (string.IsNullOrEmpty(page))
                return;
            HlidacStatu.Lib.DirectDB.NoResult("AddVisit", System.Data.CommandType.StoredProcedure,
                new System.Data.SqlClient.SqlParameter("@page", page.ToLower()),
                new System.Data.SqlClient.SqlParameter("@date", DateTime.Now.Date),
                new System.Data.SqlClient.SqlParameter("@channel", (int)channel)
                );
        }


    }

}