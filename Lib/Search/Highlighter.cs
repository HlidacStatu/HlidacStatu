using System.Linq;

namespace HlidacStatu.Lib.Search
{

    public class Highlighter
    {
        public static bool HasHighlightedContent(Nest.HighlightFieldDictionary highlights, string path, string content, string highlightPartDelimiter = " ..... ")
        {

            highlights = highlights ?? new Nest.HighlightFieldDictionary();
            string result = "";
            foreach (var hlk in highlights.Where(k => k.Key == path))
            {
                foreach (var txt in hlk.Value.Highlights)
                {
                    string stxt = txt.Replace("<highl>", "").Replace("</highl>", "");

                    if (content?.Contains(stxt) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static string HighlightFullContent(Nest.HighlightFieldDictionary highlights, string path, string content)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;

            highlights = highlights ?? new Nest.HighlightFieldDictionary();

            string hContent = content;

            foreach (var hlk in highlights.Where(k => k.Key == path))
            {
                foreach (var txt in hlk.Value.Highlights)
                {
                    string stxt = txt.Replace("<highl>", "").Replace("</highl>", "");

                    hContent = hContent.Replace(stxt, txt); //orig text replace with text with highl tags

                }
            }
            return hContent;
        }

        public static string HighlightContent(Nest.HighlightFieldDictionary highlights, string path, string content, string highlightPartDelimiter = " ..... ")
        {
            highlights = highlights ?? new Nest.HighlightFieldDictionary();
            string result = "";
            foreach (var hlk in highlights.Where(k => k.Key == path))
            {
                foreach (var txt in hlk.Value.Highlights)
                {
                    string stxt = txt.Replace("<highl>", "").Replace("</highl>", "");

                    if (content?.Contains(stxt) == true)
                    {
                        if (result.Length > 0)
                        {
                            result = result + highlightPartDelimiter + txt;
                        }
                        else
                        {
                            result = txt;
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(result))
                return null;
            else
                return result;
        }

        public static string HighlightContentIntoHtmlBlock(Nest.HighlightFieldDictionary highlights, string path, string contentToCompare,
        string foundContentFormat = null, string noHLContent = "", string prefix = "",
        string postfix = "", string highlightPartDelimiter = " ..... ", string icon = "far fa-search")
        {


            highlights = highlights ?? new Nest.HighlightFieldDictionary();
            //foundContentFormat = foundContentFormat ?? "<span class='highlighting'>{0}</span>";

            string result = HlidacStatu.Lib.Search.Highlighter.HighlightContent(highlights, path, contentToCompare, highlightPartDelimiter);
            if (string.IsNullOrEmpty(result))
            {
                return noHLContent;
            }
            else
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);
                sb.AppendLine(prefix);
                if (foundContentFormat?.Contains("{0}") == true)
                {
                    sb.AppendLine(string.Format(foundContentFormat, result));
                }
                else
                {
                    sb.AppendLine("<span class='highlighting snippet'>");
                    if (!string.IsNullOrEmpty(icon))
                    {
                        sb.AppendLine("<div class='row'>");
                        sb.AppendLine("  <div class='col-xs-offset-1 col-xs-1'>");
                        sb.AppendLine($"  <i class='{icon} fa-2x' ></i>");
                        sb.AppendLine("  </div>");
                        sb.AppendLine("  <div class='col-xs-9'>");
                        sb.AppendLine(result);
                        sb.AppendLine("  </div>");
                        sb.AppendLine("</div>");
                    }
                    else
                    {
                    }
                    sb.AppendLine("</span>");
                }
                sb.AppendLine(postfix);

                return sb.ToString();
            }


        }
    }

}