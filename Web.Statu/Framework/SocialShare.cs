using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace HlidacStatu.Web.Framework
{
    public class SocialShare : IHtmlString
    {
        public string Title { get; set; }
        public string Type { get; set; } = "website";
        public string Url { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Dictionary<string, string> MoreOG { get; set; } = new Dictionary<string, string>();
        public string BannerHtmlTitle { get; set; }
        public string BannerHtmlBody { get; set; }
        public string BannerHtmlFooter { get; set; }
        public string BannerHtmlSubFooter { get; set; }
        public string BannerHtmlImg { get; set; }

        public string ToHtmlString()
        {
            StringBuilder sb = new StringBuilder();
            
            if (!string.IsNullOrEmpty(this.Title))
            sb.AppendLine()
        }
    }
}