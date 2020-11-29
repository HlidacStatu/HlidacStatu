using HlidacStatu.Util;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analysis
{
    public class TemplatedQuery
    {
        public class AHref
        {
            public AHref() { }
            public AHref(string url, string text)
            {
                this.Url = url; this.Text = text;
            }
            public string Url { get; set; }
            public string Text { get; set; }
        }
        public TemplatedQuery()
        {
        }

        public string Query { get; set; }
        public AHref[] Links { get; set; } = null;
        public string UrlTemplate { get; set; }
        public string Url()
        {
            return UrlTemplate.Contains("{0}") ? string.Format(UrlTemplate, Query) : UrlTemplate;
        }
        public string Text { get; set; }
        public string Description { get; set; }
        public string NameOfView { get; set; }
        public Lib.Analytics.StatisticsPerYear<Data.Smlouva.Statistics.Data> Data { get { return Lib.Data.Smlouva.Statistics.CachedStatisticsFroQuery(this.Query); } }

    }


}
