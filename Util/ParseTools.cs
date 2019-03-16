using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public static class ParseTools
    {

        public static string NormalizeIco(string ico)
        {
            if (ico == null)
                return string.Empty;
            else if (ico.Contains("cz-"))
                return MerkIcoToICO(ico);
            else
                return ico.PadLeft(8, '0');
        }


        public static string MerkIcoToICO(string merkIco)
        {
            if (merkIco.ToLower().Contains("cz-"))
                merkIco = merkIco.Replace("cz-", "");

            return merkIco.PadLeft(8, '0');
        }
        public static string IcoToMerkIco(string ico)
        {
            if (Devmasters.Core.TextUtil.IsNumeric(ico))
            {
                long tmp;
                if (long.TryParse(ico.Trim(), out tmp))
                {
                    return tmp.ToString();
                }
            }

            return ico;
        }


        public static string NormalizePrilohaPlaintextText(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            //\u0000
            string ns = s.Replace("\0", "");

            //remove /n/r on the beginning
            ns = System.Text.RegularExpressions.Regex.Replace(ns, "^(\\s)*", "");
            ns = Devmasters.Core.TextUtil.ReplaceDuplicates(ns, "\n\n");
            ns = Devmasters.Core.TextUtil.ReplaceDuplicates(ns, "\r\r");
            ns = Devmasters.Core.TextUtil.ReplaceDuplicates(ns, "\t\t");
            ns = ns.Trim();

            //remove /n/r on the beginning again
            ns = System.Text.RegularExpressions.Regex.Replace(ns, "^(\\s)*", "");
            return ns;
        }

        public static int CountWords(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }
        public static string[] GetWords(string s)
        {
            if (string.IsNullOrEmpty(s))
                return new string[] { };
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            List<string> words = new List<string>();
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].Value.Length>0)
                    words.Add(collection[i].Value);
            }

            return words.ToArray();
        }

        public static string NormalizaceStranaShortName(string strana)
        {
            if (string.IsNullOrWhiteSpace(strana))
                return string.Empty;

            var s = strana.ToLower();
            s = Devmasters.Core.TextUtil.ReplaceDuplicates(s, ' ');

            if (s == "kdu" || s.Contains("československá strana lidová"))
                return "KDU-ČSL";
            else if (s == "sz")
                return "Strana zelených";
            else if (s == "top09")
                return "TOP 09";
            else if (s == "piráti")
                return "Česká pirátská strana";
            else if (s == "česká strana sociálně demokratická")
                return "ČSSD";
            else if (s == "ano2011" || s == "ano" || s == "hnutí ano")
                return "ANO 2011";
            else if (s == "občanská demokratická strana" || s.StartsWith("ODS "))
                return "ODS";
            else if (s == "spo")
                return "Strana Práv Občanů";
            else if (s.Contains("svoboda a přímá demokracie") || s == "spd")
                return "Svoboda a přímá demokracie";
            else if (s == "strana svobodných občanů")
                return "Svobodní";
            else if (s == "úsvit - národní koalice" || s == "úsvit-národní koalice")
                return "Úsvit";
            else if (s == "komunistická strana čech a moravy")
                return "KSČM";
            else if (s.Contains("lev21") || s.Contains("lev 21"))
                return "LEV21";
            else if (s.Contains("rozumní") && s.Contains("stop migraci a diktátu eu - peníze našim"))
                return "ROZUMNÍ - stop migraci a diktátu EU";
            else if (s == "vv" || s=="veci verejne")
                return "Věci veřejné";
            else
                return strana;


        }

        public static DateTime? ToDateTimeFromCZ(string value)
        {
            return ToDateTime(value,
                "d.M.yyyy", "d. M. yyyy",
                "dd.MM.yyyy", "dd. MM. yyyy",
                "dd.MM.yy", "dd. MM. yy",
                "d.M.yy", "d. M. yy"
                );
        }
        public static DateTime? ToDateTimeFromCode(string value)
        {
            return ToDateTime(value, "yyyy-MM-dd", "yyyy-M-d", "yy-MM-D");
        }

        public static DateTime? ToDateTime(string value, params string[] formats)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            foreach (var f in formats)
            {
                var dt = ToDateTime(value, f);
                if (dt.HasValue)
                    return dt;
            }
            return null;
        }

        public static DateTime? ToDateTime(string value, string format)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            DateTime tmp;
            if (DateTime.TryParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces , out tmp))
                return new DateTime?(tmp);
            else
                return null;
        }

        public static int? ToInt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            int ret = 0;
            if (int.TryParse(value, out ret))
            {
                return ret;
            }
            else
                return null;
        }

        public static decimal? FromTextToDecimal(string value)
        {
            return ToDecimal(Devmasters.Core.TextUtil.NormalizeToNumbersOnly(value));
        }

        public static decimal? ToDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            value = value.Replace(" ", "").Trim();
            if (value.StartsWith(",") || value.StartsWith("."))
                value = value.Substring(1);
            if (value.EndsWith(",") || value.EndsWith("."))
                value = value.Substring(0, value.Length - 1);
            if (value.EndsWith(",-") || value.EndsWith(".-"))
                value = value.Substring(0, value.Length - 2);
            if (value.EndsWith(",--") || value.EndsWith(".--"))
                value = value.Substring(0, value.Length - 3);
            if (value.EndsWith(",00") || value.EndsWith(".00"))
                value = value.Substring(0, value.Length - 3);

            System.Globalization.NumberStyles styles = System.Globalization.NumberStyles.AllowLeadingWhite
                | System.Globalization.NumberStyles.AllowTrailingWhite
                | System.Globalization.NumberStyles.AllowThousands
                | System.Globalization.NumberStyles.AllowCurrencySymbol
                ;
            decimal tmp;
            if (decimal.TryParse(value, System.Globalization.NumberStyles.Any, Consts.czCulture, out tmp))
                return tmp;
            else if (decimal.TryParse(value, System.Globalization.NumberStyles.Any, Consts.enCulture, out tmp))
                return tmp;
            else
                return null;
        }



        public static string ValueFromTheBestRegex(this string txt, string[] regexs, string groupname)
        {
            return GetValueFromTheBestRegex(txt, regexs, groupname);
        }
        public static string GetValueFromTheBestRegex(string txt, string[] regexs, string groupname)
        {
            string[] rets = GetValuesFromTheBestRegex(txt, regexs, groupname);
            if (rets != null && rets.Length > 0)
                return rets[0];
            else
                return null;
        }


        public static string[] ValuesFromTheBestRegex(this string txt, string[] regexs, string groupname)
        {
            return GetValuesFromTheBestRegex(txt, regexs, groupname);
        }
        public static string[] GetValuesFromTheBestRegex(string txt, string[] regexs, string groupname)
        {
            string[] ret = null;
            foreach (var r in regexs)
            {
                ret = GetRegexGroupValues(txt, r, groupname);
                if (ret != null && ret.Length > 0)
                    return ret;
            }

            return new string[] { };
        }

        public static string RegexGroupValue(this string txt, string regex, string groupname)
        {
            return GetRegexGroupValue(txt, regex, groupname);
        }

        public static string GetRegexGroupValue(string txt, string regex, string groupname)
        {
            if (string.IsNullOrEmpty(txt))
                return null;
            Regex myRegex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
            foreach (Match match in myRegex.Matches(txt))
            {
                if (match.Success)
                {
                    return match.Groups[groupname].Value;
                }
            }
            return string.Empty;
        }

        public static string[] RegexGroupValues(this string txt, string regex, string groupname)
        {
            return GetRegexGroupValues(txt, regex, groupname);
        }
        public static string[] GetRegexGroupValues(string txt, string regex, string groupname)
        {
            if (string.IsNullOrEmpty(txt))
                return null;
            Regex myRegex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
            List<string> results = new List<string>();
            foreach (Match match in myRegex.Matches(txt))
            {
                if (match.Success)
                {
                    results.Add(match.Groups[groupname].Value);
                }
            }
            return results.ToArray();
        }

        public static string ReplaceGroupMatchNameWithRegex(this string txt, string regex, string groupname, string replacement)
        {
            if (string.IsNullOrEmpty(txt))
                return txt;
            var dateIntervalM = Regex.Matches(txt, regex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
            foreach (Match m in dateIntervalM)
            {
                if (m.Success && m.Groups[groupname].Success)
                {
                    txt = txt.Substring(0, m.Groups[groupname].Index)
                        + replacement
                        + txt.Substring(m.Groups[groupname].Index + m.Groups[groupname].Length);

                }
            }
            return txt;
        }


        public static string ReplaceWithRegex(this string txt, string replacement, string regex)
        {
            return GetStringReplaceWithRegex(regex, txt, replacement);
        }
        public static string GetStringReplaceWithRegex(string regex, string txt, string replacement)
        {
            Regex myRegex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
            return myRegex.Replace(txt, replacement);
        }


        public static T ChangeType<T>(object value)
        {
            var t = typeof(T);

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return default(T);
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return (T)Convert.ChangeType(value, t);
        }

        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }
    }
}
