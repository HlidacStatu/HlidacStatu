using Newtonsoft.Json.Linq;
using Scriban.Runtime;
using System;
using System.Linq;

namespace HlidacStatu.Lib.Data.External.DataSets
{
    public partial class Registration
    {
        public partial class Template
        {

            public class Functions : ScriptObject
            {
                public static string fn_LinkTextDocument(dynamic value, string datasetId, string dataId, string linkText = "")
                {
                    return fn_LinkTextDocumentWithHighlighting(value, datasetId, dataId, linkText);
                }
                public static string fn_LinkTextDocumentWithHighlighting(dynamic value, string datasetId, string dataId, string linkText = "",
                    Nest.HighlightFieldDictionary highlightingData = null, string highlPrefix = "", string highlPostfix = "")
                {

                    if (value == null)
                        return string.Empty;
                    else
                    {
                        if (string.IsNullOrEmpty(linkText))
                            linkText = "Textový obsah dokumentu";
                        var jobj = value as JObject;
                        if (jobj != null)
                        {
                            if (
                                jobj["HsProcessType"].Value<string>() == "document"
                                && Uri.TryCreate(jobj["DocumentUrl"].Value<string>(), UriKind.Absolute, out var uri)
                                && !string.IsNullOrEmpty(jobj["DocumentPlainText"].Value<string>())
                                )
                            {
                                string HLresult = null;
                                if (highlightingData != null)
                                {
                                    string path = System.Text.RegularExpressions.
                                        Regex.Replace(jobj.Path, @"\[\d{1,}\]", "")
                                        + ".DocumentPlainText";
                                    HLresult = ES.Highlighter.HighlightContentIntoHtmlBlock(highlightingData,
                                        path,
                                        jobj["DocumentPlainText"].Value<string>(), " ..... ",
                                        prefix: highlPrefix, postfix: highlPostfix);
                                }

                                string result = $"<a href=\"/data/DetailText/{datasetId}/{dataId}?p={jobj.Path}\"><b>{linkText}</b></a> (zde <a target=\"_blank\" href=\"{uri.AbsoluteUri}\">originál ke stažení</a>)";
                                if (!string.IsNullOrEmpty(HLresult))
                                    result = result + "<br/>" + HLresult;
                                return result;
                            }
                            else if (
                                jobj["HsProcessType"].Value<string>() == "document"
                                && Uri.TryCreate(jobj["DocumentUrl"].Value<string>(), UriKind.Absolute, out var uri2)
                                )
                            {
                                return $"<a href=\"{uri2.AbsoluteUri}\">{linkText}</a>";
                            }
                        }

                        return "";
                    }
                }


                public static string fn_RenderPersonWithLink(string osobaId, string jmeno, string prijmeni, string rokNarozeni = "")
                {
                    if (!string.IsNullOrEmpty(osobaId))
                    {
                        HlidacStatu.Lib.Data.Osoba o = HlidacStatu.Lib.Data.Osoby.GetByNameId.Get(osobaId);
                        if (o != null)
                            return $"<span><a href=\"{o.GetUrl(false)}\">{o.FullNameWithYear(false)}</a></span>";
                    }

                    var narozeni = "";
                    if (!string.IsNullOrEmpty(rokNarozeni))
                        narozeni = $"(* {rokNarozeni})";
                    return $"<span>{jmeno} {prijmeni} {narozeni}</span>";
                }

                public static string fn_RenderPersonStatistic(string osobaId, bool twoLines = false, string prefix = "", string postfix = "")
                {
                    if (!string.IsNullOrEmpty(osobaId))
                    {
                        HlidacStatu.Lib.Data.Osoba o = HlidacStatu.Lib.Data.Osoby.GetByNameId.Get(osobaId);
                        if (o != null)
                        {
                            var stat = o.Statistic(Relation.AktualnostType.Nedavny);
                            return $"<span>{prefix}{stat.BasicStatPerYear.SummaryAfter2016().ToNiceString(o, true, customUrl: "/hledatSmlouvy?q=osobaId:" + o.NameId, twoLines: twoLines)}{postfix}</span>";
                        }
                    }
                    return string.Empty;
                }

                public static string fn_RenderPersonWithLink2(string osobaId)
                {
                    if (!string.IsNullOrEmpty(osobaId))
                    {
                        HlidacStatu.Lib.Data.Osoba o = HlidacStatu.Lib.Data.Osoby.GetByNameId.Get(osobaId);
                        if (o != null)
                            return $"<span><a href=\"{o.GetUrl(false)}\">{o.FullNameWithYear(false)}</a></span>";
                    }
                    return string.Empty;
                }

                public static string fn_RenderCompanyWithLink(string ico)
                {
                    if (!string.IsNullOrEmpty(ico))
                    {
                        HlidacStatu.Lib.Data.Firma o = HlidacStatu.Lib.Data.Firmy.instanceByIco.Get(ico);
                        if (o.Valid)
                            return $"<span><a href=\"{o.GetUrl(false)}\">{o.Jmeno}</a></span>";
                        else
                            return $"<span>{ico}</span>";
                    }
                    return string.Empty;
                }

                public static string fn_RenderCompanyStatistic(string ico, bool twoLines = false, string prefix = "", string postfix = "")
                {
                    if (!string.IsNullOrEmpty(ico))
                    {
                        HlidacStatu.Lib.Data.Firma o = HlidacStatu.Lib.Data.Firmy.instanceByIco.Get(ico);
                        if (o.Valid)
                        {
                            var stat = o.Statistic();
                            return $"<span>{prefix}{stat.BasicStatPerYear.SummaryAfter2016().ToNiceString(o, true, customUrl: "/hledatSmlouvy?q=ico:" + o.ICO, twoLines: twoLines)}{postfix}</span>";
                        }
                        else
                            return $"";
                    }
                    return string.Empty;
                }

                public static string fn_ShortenText(dynamic value, int? length = null)
                {

                    if (value == null)
                        return string.Empty;
                    else
                    {
                        if (length.HasValue == false)
                            return value.ToString();
                        return Devmasters.Core.TextUtil.ShortenText(value.ToString(), length.Value);
                    }
                }

                public static string fn_FormatNumber(dynamic value, string format = null)
                {
                    if (value == null)
                        return string.Empty;

                    format = format ?? "cs";
                    decimal? dat = HlidacStatu.Util.ParseTools.ToDecimal(value.ToString());
                    if (dat.HasValue)
                    {
                        if (format == "en")
                        {
                            return dat.Value.ToString(HlidacStatu.Util.Consts.enCulture);
                        }
                        else
                        {
                            return dat.Value.ToString(HlidacStatu.Util.Consts.czCulture);
                        }
                    }
                    else
                    {
                        return value.ToString();
                    }
                }


                public static string fn_FormatDate(dynamic value, string format = null)
                {
                    return fn_FormatDate2(value, format,
                            "yyyy-MM-ddTHH:mm:ss.fffK", "yyyy-MM-ddTHH:mm:ss.ffffK", "yyyy-MM-ddTHH:mm:ss.fffffK",
                            "yyyy-MM-dd HH:mm:ss",
                            "yyyy-MM-dd HH:mm:ss.fK", "yyyy-MM-dd HH:mm:ss.ffK", "yyyy-MM-dd HH:mm:ss.fffK", "yyyy-MM-dd HH:mm:ss.ffffK",
                            "yyyy-MM-dd HH:mm:ss.f", "yyyy-MM-dd HH:mm:ss.ff", "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss.ffff",
                            "dd.MM.yyyy HH: mm:ss", "d.M.yyyy H:m:s", "dd.MM.yyyy", "d.M.yyyy",
                            "yyyy.MM.dd HH: mm:ss", "yyyy.M.d H:m:s", "yyyy.MM.dd", "yyyy.M.d",
                            "yy.MM.dd HH: mm:ss", "yy.M.d H:m:s", "yy.MM.dd", "yy.M.d",
                            "yyyy-MM-dd HH: mm:ss", "yyyy-M-d H:m:s", "yyyy-MM-dd", "yyyy-M-d",
                            "yy-MM-dd HH: mm:ss", "yy-M-d H:m:s", "yy-MM-dd", "yy-M-d"
                        );
                }
                public static string fn_FormatDate2(dynamic value, string format = null, params string[] inputformats)
                {
                    if (value == null)
                        return "";

                    if (inputformats == null)
                    {
                        inputformats = new string[] { };
                    }
                    format = format ?? "d.MM.yyyy";
                    DateTime? dat = HlidacStatu.Util.ParseTools.ToDateTime(value.ToString(), inputformats);
                    if (dat.HasValue)
                        return dat.Value.ToString(format);
                    else
                        return value.ToString();
                }

                public static string fn_FormatPrice(dynamic value, string mena = null)
                {
                    mena = mena ?? "Kč";
                    decimal? val = HlidacStatu.Util.ParseTools.ToDecimal(value.ToString());
                    if (val.HasValue)
                    {
                        return HlidacStatu.Util.RenderData.NicePrice(val.Value, mena: mena);
                    }
                    return "";
                }


                public static string fn_FixPlainText(dynamic text)
                {
                    if (text == null)
                        return string.Empty;
                    var s = text.ToString();

                    //remove /n/r on the beginning
                    s = System.Text.RegularExpressions.Regex.Replace(s, "^(\\s)*", "");
                    s = Devmasters.Core.TextUtil.ReplaceDuplicates(s, "\n\n");
                    s = Devmasters.Core.TextUtil.ReplaceDuplicates(s, "\r\r");
                    s = Devmasters.Core.TextUtil.ReplaceDuplicates(s, "\t\t");

                    return s;
                    //return s;
                }
                public static string fn_NormalizeText(dynamic text)
                {
                    if (text == null)
                        return string.Empty;
                    else
                        return Devmasters.Core.TextUtil.ReplaceHTMLEntities(text.ToString());
                }
                public static bool fn_IsNullOrEmpty(dynamic text)
                {
                    if (text == null)
                    {
                        return true;
                    }
                    try
                    {
                        string s = (string)text;
                        return string.IsNullOrEmpty(s);

                    }
                    catch (Exception)
                    {
                        return false;
                    }

                }


                public static string xfn_RenderObject(JToken jo, int level, int maxLevel, int? maxLength = null)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("(");
                    foreach (JProperty jtoken in jo.Children())
                    {
                        sb.Append(string.Format("{0}:{1}, ", jtoken.Name, xfn_RenderProperty(jtoken, level, maxLevel, maxLength)));
                    }
                    if (sb.Length > 3)
                        sb.Remove(sb.Length - 3, 2); //remove last ,_
                    sb.Append(")");
                    return sb.ToString();
                }

                public static string xfn_RenderProperty(JToken jp, int level, int maxLevel, int? maxLength = null)
                {
                    if (jp == null)
                        return string.Empty;
                    if (level > maxLevel)
                        return string.Empty;

                    switch (jp.Type)
                    {
                        case JTokenType.None:
                            return string.Empty;
                        case JTokenType.Object:
                            if (level < maxLevel)
                                return xfn_RenderObject(jp, level + 1, maxLevel, maxLength);
                            break;
                        case JTokenType.Array:
                            var vals = jp.Values<JToken>();
                            if (vals != null && vals.Count() > 0)
                            {
                                return vals.Select(v => xfn_RenderProperty(v, level, maxLevel, maxLength)).Aggregate((f, s) => f + "\n" + s);
                            }
                            break;
                        case JTokenType.Constructor:
                            //
                            break;
                        case JTokenType.Property:
                            return xfn_RenderProperty(jp.Value<JProperty>().Children().FirstOrDefault(), level, maxLevel, maxLength);
                        case JTokenType.Comment:
                            break;
                        case JTokenType.Integer:
                            return jp.Value<int>().ToString();
                        case JTokenType.Float:
                            return jp.Value<float>().ToString(HlidacStatu.Util.Consts.czCulture);
                        case JTokenType.String:
                            return fn_ShortenText(jp.Value<string>(), maxLength);
                        case JTokenType.Boolean:
                            break;
                        case JTokenType.Null:
                            break;
                        case JTokenType.Undefined:
                            break;
                        case JTokenType.Date:
                            return jp.Value<DateTime>().ToString("d.M.yyyy");
                        case JTokenType.Raw:
                            return fn_ShortenText(jp.Value<string>().ToString(), maxLength);
                        case JTokenType.Bytes:
                            break;
                        case JTokenType.Guid:
                            return jp.Value<Guid>().ToString();
                        case JTokenType.Uri:
                            return fn_ShortenText(jp.Value<Uri>().ToString(), maxLength);
                        case JTokenType.TimeSpan:
                            break;
                        default:
                            break;
                    }

                    return string.Empty;

                }
            }
        }
    }
}
