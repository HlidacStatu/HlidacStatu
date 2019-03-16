using System;
using System.Collections.Generic;
using System.Linq;
using Devmasters.Core;

namespace HlidacStatu.Util
{
    public static class RenderData
    {

        public static string[,] NazvyStranŹkratky = {
            {"strana zelených","SZ"},
            {"česká pirátská strana","Piráti" },
            {"strana práv občanů","SPO" },
            {"svoboda a přímá demokracie","SPD" },
            {"rozumní - stop migraci a diktátu eu","Rozumní" },
            {"věci veřejné","VV" },
            {"starostové a nezávislí","STAN" },
        };

        public static string StranaZkratka(string strana, int maxlength = 20)
        {
            if (string.IsNullOrEmpty(strana))
                return string.Empty;

            var lstrana = strana.ToLower();
            //je na seznamu?
            for (int i = 0; i < NazvyStranŹkratky.GetLength(0); i++)
            {
                if (NazvyStranŹkratky[i, 0] == lstrana)
                    return NazvyStranŹkratky[i, 1];
            }

            var words = ParseTools.GetWords(strana);
            if (ParseTools.CountWords(strana) > 3)
            {
                //vratim zkratku z prvnich pismen
                var res = "";
                foreach (var w in words)
                {
                    char ch = w.First();
                    if (
                        System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.UppercaseLetter
                        ||
                        System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.TitlecaseLetter
                        )
                        res = res + ch;
                }
                if (res.Length > 2)
                    return res;
            }
            return Devmasters.Core.TextUtil.ShortenText(strana, maxlength);
        }

        public static string GetSocialBannerUrl(ISocialInfo si, bool ratio1x1 = false, bool localUrl = true)
        {
            string url = "";
            if (localUrl == false)
                url = "https://www.hlidacstatu.cz";

            url = url + "/imagebannercore/social"
                       + "?title=" + System.Net.WebUtility.UrlEncode(si.SocialInfoTitle())
                       + "&subtitle=" + System.Net.WebUtility.UrlEncode(si.SocialInfoSubTitle())
                       + "&body=" + System.Net.WebUtility.UrlEncode(si.SocialInfoBody())
                       + "&footer=" + System.Net.WebUtility.UrlEncode(si.SocialInfoFooter())
                       + "&img=" + System.Net.WebUtility.UrlEncode(si.SocialInfoImageUrl())
                       + "&ratio=" + (ratio1x1 ? "1x1" : "16x9");

            return url;
        }

        public static string NicePrice(decimal number, string valueIfZero = "0 {0}", string mena = "Kč", bool html = false, bool shortFormat = false, ShowDecimalVal showDecimal = ShowDecimalVal.Hide)
        {

            string s = string.Empty;
            if (number == 0)
            {
                if (valueIfZero.Contains("{0}"))
                    return string.Format(valueIfZero, mena);
                else
                    return valueIfZero;
            }
            else if (shortFormat)
            {
                return ShortNicePrice(number, valueIfZero, mena, html, showDecimal);
            }
            else
                return ShortNicePrice(number, valueIfZero, mena, html, showDecimal, MaxScale.Jeden);
        }
        static string tableOrderValueFormat = "00000000000000#";
        public static string OrderValueFormat(double n) { return (n * 1000000).ToString(tableOrderValueFormat); }
        public static string OrderValueFormat(decimal n) { return OrderValueFormat((double)n); }
        public static string OrderValueFormat(byte n) { return OrderValueFormat((double)n); }
        public static string OrderValueFormat(float n) { return OrderValueFormat((double)n); }
        public static string OrderValueFormat(int n) { return OrderValueFormat((double)n); }
        public static string OrderValueFormat(long n) { return OrderValueFormat((double)n); }
        public static string OrderValueFormat(short n) { return OrderValueFormat((double)n); }


        public static MaxScale GetBestScale(IEnumerable<double> numbers) => GetBestScale(numbers.Cast<decimal>());
        public static MaxScale GetBestScale(IEnumerable<int> numbers) => GetBestScale(numbers.Cast<decimal>());
        public static MaxScale GetBestScale(IEnumerable<float> numbers) => GetBestScale(numbers.Cast<decimal>());

        public static MaxScale GetBestScale(IEnumerable<decimal> numbers)
        {
            //logika: pokud je druhy nej rad zastoupen pod 10%, beru rad nejcastejsi
            // pokud u tri+ radu maji 2. a 3. pod 20%, beru nejcastejsi
            // pokud u tri+ ma jeden nad 10%, beru ten

            double threshold = 0.1d;

            if (numbers == null)
                throw new ArgumentNullException();
            if (numbers.Count() == 0)
                return MaxScale.Any;

            var stat = numbers
                //.Select(n=>new { sc = GetBestScale(n) })
                .GroupBy(k => GetBestScale(k), v => GetBestScale(v), (v, k) => new { sc = v, num = (double)k.Count() })
                .OrderByDescending(o=>o.num)
                .ToArray();

            if (stat.Count() == 1)
                return stat[0].sc;
            else if (stat.Count() == 2)
            {
                double sum = stat.Sum(m => m.num);
                var secCount = stat[1].num;
                if (secCount / sum > threshold && stat[0].sc < stat[1].sc)
                    return stat[1].sc;
                else
                    return stat[0].sc;
            }
            else //if (stat.Count() => 3)
            {
                double sum = stat.Sum(m => m.num);
                var top = stat[0];

                var rest = stat.Select(m => new { sc = m.sc, perc = m.num / sum })
                    .OrderByDescending(o => o.perc)
                    .Skip(1);

                if (rest.Any(m => m.perc > threshold && m.sc < top.sc))
                    return rest.Where(m => m.perc > threshold && m.sc < top.sc).Min(m => m.sc);
                else
                    return top.sc;
            }
            //else return stat.Min(m => m.sc);

        }

        static decimal OneTh = 1000;
        static decimal OneMil = OneTh * 1000;
        static decimal OneMld = OneMil * 1000;
        static decimal OneBil = OneMld * 1000;
        private static MaxScale GetBestScale(decimal number)
        {

            if (number >= OneBil)
                return MaxScale.Bilion;
            else if (number >= OneMld)
                return MaxScale.Miliarda;
            else if (number >= OneMil)
                return MaxScale.Milion;
            else if (number >= OneTh)
                return MaxScale.Tisic;
            else
                return MaxScale.Jeden;
        }

        [ShowNiceDisplayName()]
        public enum MaxScale
        {
            [NiceDisplayName("")]
            Jeden = 1,
            [NiceDisplayName("tis.")]
            Tisic = 3,
            [NiceDisplayName("mil.")]
            Milion = 6,
            [NiceDisplayName("mld.")]
            Miliarda = 9,
            [NiceDisplayName("bil.")]
            Bilion = 12,

            [NiceDisplayName("")]
            Any = 99
        }

        public static string ShortNiceNumber(decimal number,
            bool html = false, 
            ShowDecimalVal showDecimal = ShowDecimalVal.AsNeeded, 
            MaxScale exactScale = MaxScale.Any,
            bool hideSuffix = false)
        {
            return ShortNicePrice(number, "0", "", html, showDecimal, exactScale, hideSuffix);
        }

        public static string ShortNicePrice(decimal number,
            string valueIfZero = "0 {0}", string mena = "Kč", bool html = false,
            ShowDecimalVal showDecimal = ShowDecimalVal.Hide, 
            MaxScale exactScale = MaxScale.Any,
            bool hideSuffix = false)
        {

            decimal n = number;

            string suffix;
            if ((n > OneBil && exactScale == MaxScale.Any) || exactScale == MaxScale.Bilion )
            {
                n /= OneBil;
                suffix = "bil.";
            }
            else if ((n > OneMld && exactScale == MaxScale.Any) || exactScale == MaxScale.Miliarda)
            {
                n /= OneMld;
                suffix = "mld.";
            }
            else if ((n > OneMil && exactScale == MaxScale.Any) || exactScale == MaxScale.Milion )
            {
                n /= OneMil;
                suffix = "mil.";
            }
            else if (exactScale == MaxScale.Tisic )
            {
                n /= OneTh;
                suffix = "";
            }
            else
            {
                suffix = "";
            }

            if (hideSuffix)
                suffix = "";

            string ret = string.Empty;

            string formatString = "{0:### ### ### ### ### ##0} " + suffix + " {1}";
            if (showDecimal == ShowDecimalVal.Show)
                formatString = "{0:### ### ### ### ### ##0.00} " + suffix + " {1}";
            else if (showDecimal == ShowDecimalVal.AsNeeded)
                formatString = "{0:### ### ### ### ### ##0.##} " + suffix + " {1}";


            if (number == 0)
            {
                if (valueIfZero.Contains("{0}"))
                    ret = string.Format(valueIfZero, mena);
                else
                    ret = valueIfZero;
            }
            else
            {
                ret = String.Format(formatString, n, mena).Trim();
            }

            ret = ret.Trim();
            

            if (html)
            {
                return String.Format("<span title=\"{2:### ### ### ### ### ##0} {1}\">{0}</span>", ret.Replace(" ", "&nbsp;"), mena, number);

            }
            return ret;


        }

        public enum ShowDecimalVal
        {
            Hide = 0,
            Show = 1,
            AsNeeded = -1,
        }

        public static string NiceNumber(decimal number, bool html = false, ShowDecimalVal showDecimal = ShowDecimalVal.AsNeeded)
        {
            return ShortNiceNumber(number, html, showDecimal, MaxScale.Jeden, hideSuffix: true);
        }

        public static string NiceNumber(long number, bool html = false, ShowDecimalVal showDecimal = ShowDecimalVal.AsNeeded)
            =>  NiceNumber((decimal) number, html, showDecimal);

        public static string TextToHtml(string txt)
        {
            return txt.Replace("\n", "<br/>");
        }


        public static string ToElasticDate(DateTime date)
        {
            switch (date.Kind)
            {
                case DateTimeKind.Unspecified:
                case DateTimeKind.Local:
                    return date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                case DateTimeKind.Utc:
                    return date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                default:
                    return date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            }
        }

    }
}
