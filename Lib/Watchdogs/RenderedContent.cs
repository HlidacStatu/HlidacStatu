using System.Collections.Generic;
using System.Text;

namespace HlidacStatu.Lib.Watchdogs
{
    public class RenderedContent
    {
        public string ContentText { get; set; }
        public string ContentHtml { get; set; }

        public string ContentTitle { get; set; }

        public static RenderedContent Merge(IEnumerable<RenderedContent> tojoin)
        {
            StringBuilder sbT = new StringBuilder(1024);
            StringBuilder sbH = new StringBuilder(1024);
            foreach (var i in tojoin)
            {
                sbH.AppendLine(i.ContentHtml);
                sbT.AppendLine(i.ContentText);
            }
            return new RenderedContent() { ContentHtml = sbH.ToString(), ContentText = sbT.ToString() };
        }
    }
}
