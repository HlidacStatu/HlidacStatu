using Newtonsoft.Json.Linq;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Render
{
    public partial class ScribanT
    {
        public partial class Functions : ScriptObject
        {
            //PRIVATE functions

            public static string fn_Smlouva_GetConfidenceHtml(dynamic smlouva)
            {
                var s = smlouva as HlidacStatu.Lib.Data.Smlouva;
                if (s != null)
                {
                    string tmplt = "<span color='{0}' style='padding:3px 6px 3px 6px;margin-right:4px;font-weight:bold;background-color:{0};color:white;'><b>!</b></span>";
                    var confLevel = s.GetConfidenceLevel();
                    switch (confLevel)
                    {
                        case Issues.ImportanceLevel.Minor:
                            return string.Format(tmplt, "#31708f");
                        case Issues.ImportanceLevel.Major:
                            return string.Format(tmplt, "#8a6d3b");
                        case Issues.ImportanceLevel.Fatal:
                            return string.Format(tmplt, "#a94442");
                        case Issues.ImportanceLevel.NeedHumanReview:
                        case Issues.ImportanceLevel.Ok:
                        case Issues.ImportanceLevel.Formal:
                        default:
                            return "";
                    }
                }
                else
                    return "";
            }

        }
    }
}
