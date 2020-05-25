using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vybory_PSP
{
    public partial class XPath
    {
        
        public static class Tools
        {
            public static HtmlDocument HtmlDocumentFromHtml(string html)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                return doc;
            }

            public static HtmlNode GetNode(HtmlDocument doc, string xpath)
            {
                return GetNode(doc.DocumentNode, xpath);
            }
            public static HtmlNode GetNode(HtmlNode node, string xpath)
            {
                if (node == null)
                    return null;
                var n = node.SelectNodes(xpath);
                if (n != null)
                {
                    return n.FirstOrDefault();
                }
                else return null;
            }

            public static string GetNodeHtml(HtmlDocument doc, string xpath)
            {
                return GetNodeHtml(doc.DocumentNode, xpath);
            }

            public static string GetNodeHtml(HtmlNode doc, string xpath)
            {
                HtmlNode n = GetNode(doc, xpath);
                if (n != null)
                    return n.InnerHtml;
                else
                    return null;

            }

            public static List<HtmlNode> GetNodes(HtmlDocument doc, string xpath)
            {
                return GetNodes(doc.DocumentNode, xpath);
            }
            public static List<HtmlNode> GetNodes(HtmlNode node, string xpath)
            {
                var n = node.SelectNodes(xpath);
                if (n != null)
                {
                    return n.ToList();
                }
                else return null;
            }
            public static string GetNodeText(HtmlDocument doc, string xpath)
            {
                return GetNodeText(doc.DocumentNode, xpath);
            }
            public static string GetNodeText(HtmlNode doc, string xpath)
            {
                if (doc == null)
                    return null;
                HtmlNode n = GetNode(doc, xpath);
                if (n != null)
                    return Devmasters.Core.TextUtil.NormalizeToBlockText(n.InnerText);
                else
                    return null;
            }
            public static string GetBestNodeHtml(HtmlNode doc, params string[] xpaths)
            {
                foreach (var xpath in xpaths)
                {
                    var s = GetNodeHtml(doc, xpath);
                    if (!string.IsNullOrEmpty(s))
                        return s;
                }
                return null;
            }

            public static string GetBestNodeText(HtmlNode doc, params string[] xpaths)
            {
                foreach (var xpath in xpaths)
                {
                    var s = GetNodeText(doc, xpath);
                    if (!string.IsNullOrEmpty(s))
                        return s;
                }
                return null;
            }
            public static HtmlNode GetBestNode(HtmlNode doc, params string[] xpaths)
            {
                foreach (var xpath in xpaths)
                {
                    var s = GetNode(doc, xpath);
                    if (s != null)
                        return s;
                }
                return null;
            }

            public static string GetNodeAttributeValue(HtmlDocument doc, string xpath, string attributeName)
            {
                return GetNodeAttributeValue(doc.DocumentNode, xpath, attributeName);
            }
            public static string GetNodeAttributeValue(HtmlNode doc, string xpath, string attributeName)
            {
                if (doc == null)
                    return null;
                HtmlNode n = GetNode(doc, xpath);
                if (n != null)
                    return n.GetAttributeValue(attributeName, null);
                else
                    return null;
            }
        }
    }
}
