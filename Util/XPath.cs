using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public partial class XPath
    {

        HtmlDocument doc = null;
        public XPath(string html)
        {
            doc = new HtmlDocument();
            doc.LoadHtml(html??"");
        }

        public HtmlDocument Document { get { return doc; } }
        public HtmlNode GetNode(string xpath)
        {
            return Tools.GetNode(doc, xpath);
        }


        public string GetNodeHtml(string xpath)
        {
            return Tools.GetNodeHtml(doc, xpath);
        }
        public List<HtmlNode> GetNodes(string xpath)
        {
            return Tools.GetNodes(doc, xpath);
        }
        public string GetNodeText(string xpath)
        {
            return Tools.GetNodeText(doc, xpath);
        }

        public HtmlNode GetBestNode(params string[] xpaths)
        {
            return Tools.GetBestNode(doc.DocumentNode, xpaths);
        }
        public string GetBestNodeText(params string[] xpaths)
        {
            return Tools.GetBestNodeText(doc.DocumentNode, xpaths);
        }
        public string GetBestNodeHtml(params string[] xpaths)
        {
            return Tools.GetBestNodeHtml(doc.DocumentNode, xpaths);
        }


        public string GetNodeAttributeValue(string xpath, string attributeName)
        {
            return Tools.GetNodeAttributeValue(doc, xpath, attributeName);
        }

    }
}
