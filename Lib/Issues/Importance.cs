using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devmasters.Core;
using Devmasters.Enums;

namespace HlidacStatu.Lib.Issues
{

    [Devmasters.Enums.ShowNiceDisplayName()]
    [Devmasters.Enums.Sortable(Devmasters.Enums.SortableAttribute.SortAlgorithm.BySortValue)]
    public enum ImportanceLevel : int
    {
        [Devmasters.Enums.Disabled()]
        NeedHumanReview = -1,


        [Devmasters.Enums.NiceDisplayName("V pořádku")]
        Ok = 0,
        [Devmasters.Enums.NiceDisplayName("Formální problém")]
        Formal = 1,
        [Devmasters.Enums.NiceDisplayName("Malý nedostatek")]
        Minor = 5,
        [Devmasters.Enums.NiceDisplayName("Vážný nedostatek")]
        Major = 20,
        [Devmasters.Enums.NiceDisplayName("Zásadní nedostatek s vlivem na platnost smlouvy")]
        Fatal = 100,
    }

    public class Importance {

        public static string GetCssClass(ImportanceLevel imp, bool withpreDash)
        {
            string res = "";
            if (withpreDash)
                res = "-";

            switch (imp)
            {
                case ImportanceLevel.Formal:
                    return string.Empty;
                case ImportanceLevel.Minor:
                    return res + "info";
                case ImportanceLevel.Major:
                    return res + "warning";
                case ImportanceLevel.Fatal:
                    return res + "danger";
                case ImportanceLevel.NeedHumanReview:
                default:
                    return string.Empty;
            }
        }

        public static string GetIcon(ImportanceLevel imp, string sizeInCss = "90%;", string glyphiconSymbol = "exclamation-sign")
        {
            string res = "<span class=\"text{0} glyphicon glyphicon-{3}\" style=\"font-size:{1}\" aria-hidden=\"true\" title=\"{2}\"></span>";

            return string.Format(res,GetCssClass(imp, true), sizeInCss, imp.ToNiceDisplayName(), glyphiconSymbol );
        }

    }
}
