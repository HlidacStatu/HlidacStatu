using System.Linq;

namespace HlidacStatu.Web.Framework
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


    }

}