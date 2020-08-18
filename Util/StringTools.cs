using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public static class StringTools
    {

        public static Tuple<decimal,long> WordsVarianceInText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new Tuple<decimal, long>(0,0);

            var words = text.Split( new char[] {
                ' ','\t', '\n','\r','\v','\b', // regex \s
                ';',',',';','.','!',
                '(',')','<','>','{','}','[',']','_','-','~','|','=','&','#' }, StringSplitOptions.RemoveEmptyEntries )
                .Select(m=>m.ToLower());

            var wordNums = words
                .GroupBy(w => w, w => w, (w1, w2) => new { w = w1, c = w2.LongCount() })
                .ToDictionary(k => k.w, v => v.c);

            //var idxNorm = MathTools.Herfindahl_Hirschman_IndexNormalized(wordNums);
            var idx = MathTools.Herfindahl_Hirschman_Index(wordNums.Values.Select(m => (decimal)m));
            return new Tuple<decimal, long>(idx, wordNums.LongCount());
        }

        public static List<Tuple<string, bool>> SplitStringToPartsWithQuotes(string query, char quoteDelimiter)
        {
            //split newquery into part based on ", mark "xyz" parts
            //string , bool = true ...> part withint ""
            List<Tuple<string, bool>> textParts = new List<Tuple<string, bool>>();
            int[] found = CharacterPositionsInString(query, quoteDelimiter);
            if (found.Length > 0 && found.Length % 2 == 0)
            {
                int start = 0;
                bool withIn = false;
                foreach (var idx in found)
                {
                    int startIdx = start;
                    int endIdx = idx;
                    if (withIn)
                        endIdx++;
                    textParts.Add(new Tuple<string, bool>(query.Substring(startIdx, endIdx - startIdx), withIn));
                    start = endIdx;
                    withIn = !withIn;
                }
                if (start < query.Length)
                    textParts.Add(new Tuple<string, bool>(query.Substring(start), withIn));
            }
            else
            {
                textParts.Add(new Tuple<string, bool>(query, false));
            }
            return textParts;
        }

        public static int[] CharacterPositionsInString(string text, char lookingFor)
        {
            if (string.IsNullOrEmpty(text))
                return new int[] { };

            char[] textarray = text.ToCharArray();
            List<int> found = new List<int>();
            for (int i = 0; i < text.Length; i++)
            {
                if (textarray[i].Equals(lookingFor))
                {
                    found.Add(i);
                }
            }
            return found.ToArray();
        }



        public static string[] SplitWithSeparators(this string s, StringSplitOptions splitOptions = StringSplitOptions.None, params char[] separators)
        {
            return SplitWithSeparators(s, splitOptions, StringComparison.OrdinalIgnoreCase , separators.Select(m => m.ToString()).ToArray());
        }
        public static string[] SplitWithSeparators(this string s,
            StringSplitOptions splitOptions,
            StringComparison comparisonType,
            params char[] separators)
        {
            return SplitWithSeparators(s, splitOptions, StringComparison.OrdinalIgnoreCase, separators.Select(m => m.ToString()).ToArray());
        }
        public static string[] SplitWithSeparators(this string s, 
            StringSplitOptions splitOptions, 
            StringComparison comparisonType,
            params string[] separators)
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
                        if (s.Substring(i, sep.Length).Equals(sep, comparisonType))
                        {
                            int partLen = i - prevStart;
                            if (partLen > 0)
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
        
        public static string RemoveAccents(this string input)
        {
            var normalizedString = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string KeepLettersNumbersAndSpace(this string input)
        {
            var res = Regex.Replace(input, @"[^\w ]", "", RegexOptions.CultureInvariant);
            res = res.Replace("_", "");
            return res;
        }

    }
}
