using System;

namespace HlidacStatu.Util
{
    public static class DateTools
    {
        public enum DateTimePart
        {
            Year = 1,
            Month = 2,
            Day = 3,
            Hour = 4,
            Minute = 5,
            Second = 6
        }

        public static string DateDiffShort(DateTime first, DateTime sec, string beforeTemplate = "{0}", string afterTemplate = "{0}")
        {
            if (first < DateTime.MinValue)
                first = DateTime.MinValue;
            if (sec < DateTime.MinValue)
                sec = DateTime.MinValue;
            if (first > DateTime.MaxValue)
                first = DateTime.MaxValue;
            if (sec > DateTime.MaxValue)
                sec = DateTime.MaxValue;

            bool after = first > sec;
            Devmasters.Core.DateTimeSpan dateDiff = Devmasters.Core.DateTimeSpan.CompareDates(first, sec);
            string txtDiff = string.Empty;
            if (dateDiff.Years > 0)
            {
                txtDiff = HlidacStatu.Util.PluralForm.Get(dateDiff.Years, "rok;{0} roky;{0} let");
            }
            else if (dateDiff.Months > 3)
            {
                txtDiff = HlidacStatu.Util.PluralForm.Get(dateDiff.Months, "měsíc;{0} měsíce;{0} měsíců");
            }
            else
            {
                txtDiff = Devmasters.Core.Lang.Plural.GetWithZero(dateDiff.Days, "dnes", "den", "{0} dny", "{0} dnů");
            }

            if (after)
                return string.Format(afterTemplate, txtDiff);
            else
                return string.Format(beforeTemplate, txtDiff);

        }

        public static string FormatIntervalSinglePart(TimeSpan ts, DateTimePart minDatePart, string numFormat = "N1")
        {

            var end = DateTime.Now;
            Devmasters.Core.DateTimeSpan dts = Devmasters.Core.DateTimeSpan.CompareDates(end - ts, end);
            string s = "";
            if (dts.Years > 0 && minDatePart > DateTimePart.Year)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Years, "{0} rok;{0} roky;{0} let");
            }
            else if (dts.Years > 0)
            {
                decimal part = dts.Years + dts.Months / 12m;
                if (part % 1 > 0)
                    s += string.Format(" {0:" + numFormat + "} let", part);
                else
                    s += HlidacStatu.Util.PluralForm.Get((int)part, " {0} rok; {0} roky; {0} let"); ;
                return s;
            }

            if (dts.Months > 0 && minDatePart > DateTimePart.Month)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Months, "{0} měsíc;{0} měsíce;{0} měsíců");
            }
            else if (dts.Months > 0)
            {
                decimal part = dts.Months + dts.Days / 30m;
                if (part % 1 > 0)
                    s += string.Format(" {0:" + numFormat + "} měsíců", part);
                else
                    s += HlidacStatu.Util.PluralForm.Get((int)part, " {0} měsíc; {0} měsíce; {0} měsíců"); ;
                return s;
            }

            if (dts.Days > 0 && minDatePart > DateTimePart.Day)
            {
                s = " " + HlidacStatu.Util.PluralForm.Get(dts.Days, " {0} den;{0} dny;{0} dnů");
            }
            else if (dts.Days > 0)
            {
                decimal part = dts.Days + dts.Hours / 24m;
                if (part % 1 > 0)
                    s = " " + string.Format(" {0:" + numFormat + "} dní", part);
                else
                    s = " " + HlidacStatu.Util.PluralForm.Get((int)part, " {0} den;{0} dny;{0} dnů"); ;
                return s;
            }

            if (dts.Hours > 0 && minDatePart > DateTimePart.Hour)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Hours, " {0} hodinu;{0} hodiny;{0} hodin");
            }
            else if (dts.Hours > 0)
            {
                decimal part = dts.Hours + dts.Minutes / 60m;
                if (part % 1 > 0)
                    s += string.Format(" {0:" + numFormat + "} hodin", part);
                else
                    s += " " + HlidacStatu.Util.PluralForm.Get((int)part, " {0} hodinu;{0} hodiny;{0} hodin");
                return s;
            }

            if (dts.Minutes > 0 && minDatePart > DateTimePart.Minute)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Minutes, "minutu;{0} minuty;{0} minut");
            }
            else if (dts.Minutes > 0)
            {
                decimal part = dts.Minutes + dts.Seconds / 60m;
                if (part % 1 > 0)
                    s += string.Format(" {0:" + numFormat + "} minut", part);
                else
                    s += " " + HlidacStatu.Util.PluralForm.Get((int)part, "minutu;{0} minuty;{0} minut"); ;
                return s;
            }

            if (dts.Seconds > 0 && minDatePart > DateTimePart.Second)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Seconds, "sekundu;{0} sekundy;{0} sekund");
            }
            else
            {
                decimal part = dts.Seconds + dts.Milliseconds / 1000m;
                if (part % 1 > 0)
                    s += string.Format(" {0:" + numFormat + "} sekund", part);
                else
                    s += " " + HlidacStatu.Util.PluralForm.Get((int)part, "sekundu;{0} sekundy;{0} sekund"); ;
                return s;
            }

            //if (dts.Milliseconds > 0)
            //    s += " " + HlidacStatu.Util.PluralForm.Get(dts.Milliseconds, "{0} ms;{0} ms;{0} ms");

            return s.Trim();

        }

        public static string FormatInterval(TimeSpan ts)
        {
            return FormatInterval(ts, System.Globalization.CultureInfo.CurrentUICulture);
        }
        public static string FormatInterval(TimeSpan ts, System.Globalization.CultureInfo culture)
        {
            var end = DateTime.Now;
            Devmasters.Core.DateTimeSpan dts = Devmasters.Core.DateTimeSpan.CompareDates(end - ts, end);
            string s = "";
            if (dts.Years > 0)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Years, "rok;{0} roky;{0} let", culture);
            }
            if (dts.Months > 0)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Months, "měsíc;{0} měsíce;{0} měsíců", culture);
            }
            if (dts.Days > 0)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Days, "den;{0} dny;{0} dnů", culture);
            }
            if (dts.Hours > 0)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Hours, "hodinu;{0} hodiny;{0} hodin", culture);
            }
            if (dts.Minutes > 0)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Minutes, "minutu;{0} minuty;{0} minut", culture);
            }
            if (dts.Seconds > 0)
            {
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Seconds, "sekundu;{0} sekundy;{0} sekund", culture);
            }
            if (dts.Milliseconds > 0)
                s += " " + HlidacStatu.Util.PluralForm.Get(dts.Milliseconds, "{0} ms;{0} ms;{0} ms", culture);

            return s.Trim();

        }

    }
}
