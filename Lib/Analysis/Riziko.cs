using System;
using System.Linq;

namespace HlidacStatu.Lib.Analysis
{
    public static class Riziko
    {
        [Devmasters.Enums.ShowNiceDisplayName()]
        public enum RizikoValues
        {
            [Devmasters.Enums.NiceDisplayName("-")]
            None = -1,
            A = 0,
            B = 1,
            C = 2,
            D = 3,
            E = 4,
            F = 5
        }


        public static string RizikoColor(RizikoValues label)
        {
            switch (label)
            {
                case RizikoValues.None:
                    return "#000000";
                case RizikoValues.A:
                    return "#48912A";
                case RizikoValues.B:
                    return "#5CBC3B";
                case RizikoValues.C:
                    return "#F6CA48";
                case RizikoValues.D:
                    return "#F09749";
                case RizikoValues.E:
                    return "#D85926";
                case RizikoValues.F:
                    return "#A7261C";
                default:
                    return "#000000";
            }
        }

        public static string RizikoText(RizikoValues label)
        {
            switch (label)
            {
                case RizikoValues.None:
                    return "";
                case RizikoValues.A:
                    return "Žádné";
                case RizikoValues.B:
                    return "Nízké";
                case RizikoValues.C:
                    return "Střední";
                case RizikoValues.D:
                    return "Střední";
                case RizikoValues.E:
                    return "Vysoké";
                case RizikoValues.F:
                    return "Vysoké";
                default:
                    return "";
            }
        }

        public static string ToHtml(RizikoValues label, bool fullText = true)
        {
            return $"<span style='color:{RizikoColor(label)}'>{RizikoText(label)}{(fullText ? " riziko" : "")}</span>";
        }

    }

}
