using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public static class StringTools
    {
        public static string[] SplitWithSeparators(this string s, char[] separators, StringSplitOptions splitOptions = StringSplitOptions.None)
        {
            return SplitWithSeparators(s, separators.Select(m => m.ToString()).ToArray(), splitOptions);
        }

            public static string[] SplitWithSeparators(this string s, string[] separators, 
                StringSplitOptions splitOptions = StringSplitOptions.None,
                StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(s))
                return new string[] { };

            List<string> parts = new List<string>();
            int prevStart = 0;
            for (int i = 0; i < s.Length; i++)
            {
                foreach (var sep in separators)
                {
                    if (i + sep.Length <= s.Length)
                    {
                        if (s.Substring(i,sep.Length).Equals(sep, comparisonType))
                        {
                            int partLen = i - prevStart;
                            if (partLen>0)
                                parts.Add(s.Substring(prevStart, partLen));
                            parts.Add(sep);
                            i = i + sep.Length;
                            prevStart = i;
                            break;
                        }
                    }
                }
            }
            if (prevStart < s.Length)
                parts.Add(s.Substring(prevStart));

            if (splitOptions == StringSplitOptions.RemoveEmptyEntries)
                return parts.Where(m => !string.IsNullOrEmpty(m)).ToArray();

            return parts.ToArray();
        }

    }
}
