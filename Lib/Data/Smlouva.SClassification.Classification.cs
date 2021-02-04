using Devmasters.Enums;

using System;

namespace HlidacStatu.Lib.Data
{
    public partial class Smlouva
    {
        public partial class SClassification
        {
            public class Classification
            {
                [Nest.Number]
                public int TypeValue { get; set; }
                [Nest.Number]
                public decimal ClassifProbability { get; set; }

                public ClassificationsTypes ClassifType() { return (ClassificationsTypes)TypeValue; }

                public string ClassifTypeName()
                {
                    return ClassifTypeName(this.TypeValue);
                }

                public bool RootClassification
                {
                    get
                    {
                        return (this.TypeValue % 100) == 0;
                    }
                }
                public static string ClassifTypeName(int value)
                {
                    ClassificationsTypes t;
                    if (Enum.TryParse(value.ToString(), out t))
                    {
                        if (Devmasters.TextUtil.IsNumeric(t.ToString()))
                        {
                            Util.Consts.Logger.Warning("Missing Classification value" + value);
                            return "(neznámý)";
                        }
                        return t.ToNiceDisplayName();
                    }
                    else
                    {
                        Util.Consts.Logger.Warning("Missing Classification value" + value);
                        return "(neznámý)";
                    }
                }
                public static ClassificationsTypes? ToClassifType(int value)
                {
                    ClassificationsTypes t;
                    if (Enum.TryParse(value.ToString(), out t))
                    {
                        if (Devmasters.TextUtil.IsNumeric(t.ToString()))
                            return null;
                        else
                            return t;
                    }
                    else
                        return null;
                }
                public static ClassificationsTypes? ToClassifType(string value, ClassificationsTypes? ifNotFound = null)
                {
                    ClassificationsTypes t;
                    if (value.Contains("_") == false)
                        value = value + "_obecne";
                    if (Enum.TryParse(value, out t))
                    {
                        if (Devmasters.TextUtil.IsNumeric(t.ToString()))
                            return ifNotFound;
                        else
                            return t;
                    }
                    else
                        return ifNotFound;
                }

                public string ClassifSearchQuery()
                {
                    return ClassifSearchQuery(ClassifType());
                }
                public static string ClassifSearchQuery(ClassificationsTypes t)
                {
                    var val = t.ToString();
                    if (val.EndsWith("_obecne"))
                        val = val.Replace("_obecne", "");
                    return val;
                }
                public static string ClassifSearchQuery(int it)
                {
                    var val = ((ClassificationsTypes)it);
                    return ClassifSearchQuery(val);
                }

                public static string GetSearchUrl(ClassificationsTypes t, bool local = true)
                {
                    string url = "/HledatSmlouvy?Q=oblast:" + ClassifSearchQuery(t);
                    if (local == false)
                        return "https://www.hlidacstatu.cz" + url;
                    else
                        return url;
                }


            }

        }
    }
}
