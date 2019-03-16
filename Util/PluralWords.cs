using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HlidacStatu.Util
{
    /// <summary>
    /// More info: https://developer.mozilla.org/en/Localization_and_Plurals
    /// </summary>
    public static class PluralForm
    {
        public static string Get(int number, Devmasters.Core.IResourceManager2 resources, string key)
        {
            return Get(number, CultureInfo.CurrentUICulture, resources, key);
        }
        public static string Get(int number, CultureInfo culture, Devmasters.Core.IResourceManager2 resources, string key)
        {
            return Get(number, resources.Manager.GetString(key, culture));
        }

        public static string Get(int number, string value)
        {
            return Get(number, value, CultureInfo.CurrentUICulture);
        }

        public static string Get(int number, string val, CultureInfo culture)
        {
            if (val == null)
                return String.Empty;


            switch (culture.TwoLetterISOLanguageName)
            {
                case "cs":
                    return GetCzechPlural(number, val);
                case "pl":
                    return GetPolandPlural(number, val);
                case "ru":
                    return GetRuPlural(number, val);

                case "en":
                    return GetEnglishPlural(number, val);

                case "de":
                    return GetGermanPlural(number, val);

                case "ja":
                    return GetJapanesePlural(number, val);

                default:
                    return GetEnglishPlural(number, val);
                //return val;
            }

            //return val;
        }

        private static string GetRuPlural(int number, string val)
        {
            string[] plural = val.Split(';');
            if (plural.Length != 3)
                throw new InvalidResourceException("Invalid RU resource. The resource string  " + val + " doesn't contains 3 options.");

            if (number == 1)
                return FormatString(plural[0], number);

            if (number > 1 && number < 5)
                return FormatString(plural[1], number);

            return FormatString(plural[2], number);
        }

        private static string GetCzechPlural(int number, string val)
        {
            string[] plural = val.Split(';');
            if (plural.Length != 3)
                throw new InvalidResourceException("Invalid czech resource. The resource string  " + val + " doesn't contains 3 options.");

            if (number == 1)
                return FormatString(plural[0], number);

            if (number > 1 && number < 5)
                return FormatString(plural[1], number);

            return FormatString(plural[2], number);
        }
        private static string GetPolandPlural(int number, string val)
        {
            string[] plural = val.Split(';');
            if (plural.Length != 3)
                throw new InvalidResourceException("Invalid poland resource. The resource string  " + val + " doesn't contains 3 options.");

            if (number == 1)
                return FormatString(plural[0], number);

            if (number > 1 && number < 5)
                return FormatString(plural[1], number);

            return FormatString(plural[2], number);
        }
        private static string GetEnglishPlural(int number, string val)
        {
            string[] plural = val.Split(';');
            if (plural.Length != 2)
                throw new InvalidResourceException("Invalid english resource. The resource string " + val + " doesn't contains 2 options.");

            return number == 1 ? FormatString(plural[0], number) : FormatString(plural[1], number);
        }

        private static string GetGermanPlural(int number, string val)
        {
            string[] plural = val.Split(';');
            if (plural.Length != 2)
                throw new InvalidResourceException("Invalid german resource. The resource string  " + val + " doesn't contains 2 options.");

            return number == 1 ? FormatString(plural[0], number) : FormatString(plural[1], number);
        }

        private static string GetJapanesePlural(int number, string val)
        {
            return FormatString(val, number);
        }


        private static string FormatString(string text, int number)
        {
            if (text.Contains("{") && text.Contains("}"))
                return string.Format(text, number);
            else
                return text;
        }
    }

    public class InvalidResourceException : ApplicationException
    {
        public InvalidResourceException(String message)
            : base(message)
        {
        }   
    }
}
