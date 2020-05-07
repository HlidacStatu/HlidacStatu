using System;
using System.Collections.Generic;

namespace Newton.SpeechToText.Cloud
{
    public static class Util
    {
        private static DateTime epochTimeBegining = DateTime.MinValue.AddYears(1969);

        public static DateTime FromEpochTimeToUTC(long value)
        {
            return new DateTime(epochTimeBegining.AddMilliseconds((double)value * 1000.0).Ticks, DateTimeKind.Utc);
        }

        public static string ReplaceDuplicates(this string text, string duplicated)
        {
            return ReplaceDuplicates(text, duplicated, duplicated);
        }

        public static string ReplaceDuplicates(this string text, string duplicated, string replacement)
        {
            return ReplaceDuplicates(text, duplicated, replacement, 2);
        }
        public static string ReplaceDuplicates(this string text, string duplicated, string replacement, int minNumOfOccurences)
        {
            string regex = string.Format("([{0}]{{{1},}})", duplicated, minNumOfOccurences);
            return System.Text.RegularExpressions.Regex.Replace(text, regex, replacement.ToString());
        }
    }
}
