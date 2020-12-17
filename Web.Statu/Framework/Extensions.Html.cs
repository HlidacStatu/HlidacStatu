using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Newtonsoft.Json.Linq;

namespace HlidacStatu.Web.Framework
{
    public static class HtmlExtensions
    {
        public static string FormBotHoneyPotInputName_Default = "email2";
        public static System.Web.IHtmlString HoneyPotInput(this HtmlHelper self, string inputName = null)
        {
            inputName = inputName ?? FormBotHoneyPotInputName_Default;

            var input = $"<input type='text' name='{inputName}' id='{inputName}' value='' placeholder='Your Zip code' />";
            var js = @"
<script>
    $(function () {
        $('#" + inputName + @"').css({ ""font-size"": ""1pt"", ""color"": ""white"", ""width"": ""1px"", ""border"": ""none""});
    });    
    </script>
    <noscript>
        <style>
            #zip {
                font-size: 1pt; color: white; width:1px; border:none;
            }
        </style>
    </noscript>";

            return self.Raw(input + js);
        }

        public static bool DetectedBotWithHoneyPot(FormCollection form, string inputName = null)
        {
            inputName = inputName ?? FormBotHoneyPotInputName_Default;
            return (form[inputName] != "");
        }
        public static MvcHtmlString CurrentLink(this HtmlHelper self, string linkText)
        {
            return CurrentLink(self, linkText, null, null, null);
        }
        public static MvcHtmlString CurrentLink(this HtmlHelper self, string linkText, object additionalRouteValues, string[] ignoreRouteValues = null)
        {
            return CurrentLink(self, linkText, new RouteValueDictionary(additionalRouteValues), null, ignoreRouteValues);
        }
        public static MvcHtmlString CurrentLink(this HtmlHelper self, string linkText, RouteValueDictionary additionalRouteValues, string[] ignoreRouteValues = null)
        {
            return CurrentLink(self, linkText, additionalRouteValues, null, ignoreRouteValues);
        }
        public static MvcHtmlString CurrentLink(this HtmlHelper self, string linkText, object additionalRouteValues, object htmlAttributes, string[] ignoreRouteValues = null)
        {
            return CurrentLink(self, linkText, new RouteValueDictionary(additionalRouteValues), new RouteValueDictionary(htmlAttributes), ignoreRouteValues);
        }

        public static MvcHtmlString CurrentLink(this HtmlHelper self, string linkText, string actionName, string controllerName, object additionalRouteValues, object htmlAttributes, string[] ignoreRouteValues = null)
        {
            return CurrentLink(self, linkText, actionName, controllerName, new RouteValueDictionary(additionalRouteValues), new RouteValueDictionary(htmlAttributes), ignoreRouteValues);
        }
        public static MvcHtmlString CurrentLink(this HtmlHelper self, string linkText, RouteValueDictionary additionalRouteValues, IDictionary<string, object> htmlAttributes, string[] ignoreRouteValues = null)
        {
            return CurrentLink(self, linkText, null, null, additionalRouteValues, htmlAttributes, ignoreRouteValues);
        }
        public static MvcHtmlString CurrentLink(this HtmlHelper self, string linkText, string actionName, string controllerName, RouteValueDictionary additionalRouteValues, IDictionary<string, object> htmlAttributes, string[] ignoreRouteValues = null)
        {
            var routeValues = new RouteValueDictionary();

            var queryString = self.ViewContext.HttpContext.Request.QueryString;
            foreach (var key in queryString.AllKeys.Where(m => m != null))
                routeValues.Add(key, queryString[key]);

            routeValues = routeValues.Merge(self.ViewContext.RequestContext.RouteData.Values.Where(p => !(p.Value is DictionaryValueProvider<object>)));

            routeValues = routeValues.Merge(additionalRouteValues);

            if (self.ViewContext.IsChildAction)
            {
                var vctx = self.ViewContext;
                do
                {
                    if (vctx.IsChildAction)
                        vctx = vctx.ParentActionViewContext;
                } while (vctx != null && vctx?.IsChildAction == true);
                if (vctx != null)
                {
                    routeValues["action"] = vctx.RouteData.Values["action"];
                }
            }

            //remove values to ignore
            if (ignoreRouteValues != null)
                foreach (var k in ignoreRouteValues)
                {
                    if (routeValues.ContainsKey(k))
                        routeValues.Remove(k);
                }
            return self.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
        }
        public static IDisposable LowBox(this HtmlHelper htmlHelper, int width = 120, string gaPageEventId = null)
        {
            return new DisposableHelper(
                () =>
                {
                    var html = $@"<div class='low-box' style='max-height:{width}px'>
        <div class='low-box-line' style='top:{width - 55}px'><a href='#' onclick=""ga('send', 'event', 'btnLowBoxMore', 'showMore','{gaPageEventId}'); return true;"" class='more'></a></div>
        <div class='low-box-content'>";
                    htmlHelper.ViewContext.Writer.Write(html);
                },
                () =>
                {
                    var html = $@"</div></div>";
                    htmlHelper.ViewContext.Writer.Write(html);
                }
            );
        }

        public static IHtmlString KIndexIcon(this HtmlHelper htmlHelper, string ico, int heightInPx = 15, string hPadding = "3px", string vPadding = "0", bool showNone = false)
        {
            return htmlHelper.KIndexIcon(ico, $"padding:{vPadding} {hPadding};height:{heightInPx}px;width:auto", showNone);
        }
        public static IHtmlString KIndexIcon(this HtmlHelper htmlHelper, string ico, string style, bool showNone = false)
        {
            if (string.IsNullOrEmpty(ico))
                return htmlHelper.Raw("");

            ico = HlidacStatu.Util.ParseTools.NormalizeIco(ico);
            Tuple<int?, Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues> lbl = Lib.Analysis.KorupcniRiziko.KIndex.GetLastLabel(ico);
            if (lbl != null)
            {
                if (showNone || lbl.Item2 != Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues.None)
                    return KIndexIcon(htmlHelper, lbl.Item2, style, showNone);
            }
            return htmlHelper.Raw("");
        }
        public static IHtmlString KIndexIcon(this HtmlHelper htmlHelper, Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues label,
            int heightInPx = 15, string hPadding = "3px", string vPadding = "0", bool showNone = false)
        {
            return htmlHelper.KIndexIcon(label, $"padding:{vPadding} {hPadding};height:{heightInPx}px;width:auto", showNone);
        }

        public static IHtmlString KIndexIcon(this HtmlHelper htmlHelper, Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues label,
            string style, bool showNone = false, string title = "")
        {
            System.Security.Principal.IPrincipal user = htmlHelper.ViewContext.RequestContext.HttpContext.User;
            if (ShowKIndex(user))
            {
                return htmlHelper.Raw(Lib.Analysis.KorupcniRiziko.KIndexData.KindexImageIcon(label, style, showNone, title));
            }
            else
                return htmlHelper.Raw("");
        }


        public static IHtmlString KIndexLabelLink(this HtmlHelper htmlHelper, string ico,
            int heightInPx = 15, string hPadding = "3px", string vPadding = "0", bool showNone = false,
            int? rok = null, bool linkToKindex = false)
        {
            return htmlHelper.KIndexLabelLink(ico, $"padding:{vPadding} {hPadding};height:{heightInPx}px;width:auto", showNone, rok, linkToKindex);
        }


        public static IHtmlString KIndexLabelLink(this HtmlHelper htmlHelper, string ico, string style, bool showNone = false, int? rok = null, bool linkToKindex = false)
        {
            if (string.IsNullOrEmpty(ico))
                return htmlHelper.Raw("");

            ico = HlidacStatu.Util.ParseTools.NormalizeIco(ico);
            System.Security.Principal.IPrincipal user = htmlHelper.ViewContext.RequestContext.HttpContext.User;
            if (ShowKIndex(user))
            {
                var kidx = Lib.Analysis.KorupcniRiziko.KIndex.Get(ico);
                if (kidx == null)
                    kidx = Lib.Analysis.KorupcniRiziko.KIndexData.Empty(ico);
                var ann = kidx.ForYear(rok ?? Lib.Analysis.KorupcniRiziko.Consts.CalculationYears.Max());


                Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues lbl = ann.KIndexLabel;
                return htmlHelper.KIndexLabelLink(ico, lbl, style, showNone, rok, linkToKindex: linkToKindex);
            }
            return htmlHelper.Raw("");
        }
        public static IHtmlString KIndexLabelLink(this HtmlHelper htmlHelper, string ico,
            Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues label,
            string style, bool showNone = false, int? rok = null, bool linkToKindex = false)
        {
            if (string.IsNullOrEmpty(ico))
                return htmlHelper.Raw("");

            System.Security.Principal.IPrincipal user = htmlHelper.ViewContext.RequestContext.HttpContext.User;
            if (ShowKIndex(user))
            {
                if (linkToKindex)
                {
                    if (label != Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues.None || showNone)
                    {
                        if (label == Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues.None)
                            return htmlHelper.KIndexIcon(label, style, showNone);
                        else
                            return htmlHelper.Raw($"<a href='/kindex/detail/{ico}{(rok.HasValue ? "?rok=" + rok.Value : "")}'>"
                            + KIndexIcon(htmlHelper, label, style, showNone).ToHtmlString()
                            + "</a>");
                    }
                }
                else
                {
                    if (label != Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues.None || showNone)
                    {
                        if (label == Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues.None)
                            return htmlHelper.KIndexIcon(label, style, showNone);
                        else
                            return htmlHelper.Raw($"<a href='/Subjekt/{ico}'>"
                            + KIndexIcon(htmlHelper, label, style, showNone).ToHtmlString()
                            + "</a>");
                    }
                }
            }
            return htmlHelper.Raw("");

        }

        public static IHtmlString KIndexLimitedRaw(this HtmlHelper htmlHelper, params IHtmlString[] htmls)
        {
            System.Security.Principal.IPrincipal user = htmlHelper.ViewContext.RequestContext.HttpContext.User;
            if (ShowKIndex(user))
            {
                var s = string.Join("", htmls.Select(m => m.ToHtmlString().Replace("\n", "").Trim()));
                return htmlHelper.Raw(s);
            }
            return htmlHelper.Raw("");
        }

        public static bool ShowKIndex(System.Security.Principal.IPrincipal user)
        {
            if (Devmasters.Config.GetWebConfigValue("KIndex") == "private")
            {
                return IfInRoles(user, "KIndex");
            }
            else
                return true;
        }

        public static Restricted ShowKIndex(this HtmlHelper self, System.Security.Principal.IPrincipal user)
        {
            return new Restricted(self, ShowKIndex(user));
        }
        public static bool IfInRoles(System.Security.Principal.IPrincipal user, params string[] roles)
        {
            bool show = false;
            if (roles.Count() > 0)
            {
                if (user?.Identity?.IsAuthenticated == true)
                {
                    foreach (var r in roles)
                    {
                        if (user.IsInRole(r))
                        {
                            show = true;
                            break;
                        }
                    }
                }
            }
            else
                show = true;
            return show;
        }


        public static Restricted IfInRoles(this HtmlHelper self, System.Security.Principal.IPrincipal user, params string[] roles)
        {
            return new Restricted(self, IfInRoles(user, roles));
        }

        public enum CachedActionLength
        {
            Cache1H,
            Cache20min,
            Cache2H,
            Cache4H,
            Cache12H,
            Cache24H,
            Cache48H,
            debug
        }
        public static MvcHtmlString CachedAction(this HtmlHelper self, CachedActionLength length, string viewName, object model,
            string primaryKey, bool? auth = null,
            object param1 = null, object param2 = null, object param3 = null, object param4 = null,
            object param5 = null, object param6 = null, object param7 = null, object param8 = null)
        {
            string cacheLength = length.ToString().Replace("Cache", "");
            return self.Action("CachedAction_Child_" + cacheLength, new
            {
                model = model,
                NameOfView = viewName,
                auth = auth ?? self.ViewContext.RequestContext.HttpContext.User?.Identity?.IsAuthenticated,
                key = primaryKey,
                param1,
                param2,
                param3,
                param4,
                param5,
                param6,
                param7,
                param8
            }

            );
        }

        public static IHtmlString Toggleable(this HtmlHelper htmlHelper, 
            IHtmlString first, string firstButton, 
            IHtmlString second, string secondButton)
        {
            string random = Guid.NewGuid().ToString("N");
            var sb = new System.Text.StringBuilder();

            sb.Append($"<script>");
            sb.Append($"$(function () {{");
            sb.Append($"$('.{random}_first.btn').click(function () {{");
            sb.Append($"$('.{random}_first.content').show();");
            sb.Append($"$('.{random}_second.content').hide();");
            sb.Append($"$('.{random}_first.btn').addClass(\"active\");");
            sb.Append($"$('.{random}_second.btn').removeClass(\"active\");");
            sb.Append($"}});");
            sb.Append($"$('.{random}_second.btn').click(function () {{");
            sb.Append($"$('.{random}_first.content').hide();");
            sb.Append($"$('.{random}_second.content').show();");
            sb.Append($"$('.{random}_first.btn').removeClass(\"active\");");
            sb.Append($"$('.{random}_second.btn').addClass(\"active\");");
            sb.Append($"}});");
            sb.Append($"}});");
            sb.Append($"</script>");
            sb.Append($"<div class=\"btn btn-default {random}_first active\" style=\"border-top-right-radius: 0px;border-bottom-right-radius: 0px;\">{firstButton}</div>");
            sb.Append($"<div class=\"btn btn-default {random}_second\" style=\"border-top-left-radius: 0px;border-bottom-left-radius: 0px;\">{secondButton}</div>");
            sb.Append($"<div class=\"{random}_first content\">{first.ToHtmlString()}</div>");
            sb.Append($"<div class=\"{random}_second content\" style=\"display: none; \">{second.ToHtmlString()}</div>");

            //sb.Append("<script>");
            //sb.Append("$(function () {");
            //sb.Append($"$('.{random}.btn').click(function () {{");
            //sb.Append($"$('.{random}').toggle();}});}});");
            //sb.Append("</script>");
            //sb.Append($"<div class=\"btn btn-default {random}\"><span style=\"color:black;\">{firstButton}/</span><small>{secondButton}</small></div>");
            //sb.Append($"<div class=\"btn btn-default {random}\" style=\"display:none;\"><small>{firstButton}</small><span style=\"color:black;\">/{secondButton}</span></div>");
            //sb.Append($"<div class=\"{random}\">{first.ToHtmlString()}</div>");
            //sb.Append($"<div class=\"{random}\" style=\"display:none;\">{second.ToHtmlString()}</div>");

            return htmlHelper.Raw(sb.ToString());
        }

        public static IHtmlString ColumnGraph(this HtmlHelper htmlHelper, 
            string title, 
            Lib.Render.Series[] series,
            int height = 300,
            string xTooltip = "Rok",
            string yTitleLeft = "Hodnota (Kč)",
            string yTitleRight = "")
        {
            string random = Guid.NewGuid().ToString("N");
            var sb = new System.Text.StringBuilder();

            sb.AppendLine($"<div id='{random}' ></div>");
            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine($"var g_{random};");
            sb.AppendLine("$(document).ready(function () {");
            //sb.AppendLine(GraphTheme());
            sb.AppendLine($"g_{random} = new Highcharts.Chart(");

            var anon = new
            {
                chart = new
                {
                    spacingTop = 30,
                    renderTo = random,
                    height = height,
                    type = "column",
                },
                legend = new
                {
                    //enabled = false,
                    //reversed = true,
                    symbolHeight = 15,
                    symbolWidth = 15,
                    squareSymbol = true
                },
                
                title = new
                {
                    y = -10,
                    useHtml = true,
                    align = "left",
                    text = $"<span class=\"chart-title-shared\">{title}</span>",
                },
                tooltip = new
                {
                    useHTML = true,
                    shared = true,
                    valueDecimals = 0,
                    headerFormat = $"<table class=\"chart-tooltip-shared\"><tr><td>{xTooltip}:</td><td>{{point.key}}</td>",
                    pointFormat = "<tr><td><span class=\"small-circle\" style=\" background-color: {series.color};\" ></span> {series.name}: </td><td style=\"text-align: right\"><b>{tooltip.valuePrefix}{point.y}{tooltip.valueSuffix}</b></td></tr>",
                    footerFormat = "</table>",
                    
                },
                xAxis = new
                {
                    labels = new
                    {
                        staggerLines = 1
                    },
                    title = new
                    {
                        text = ""
                    }
                },
                yAxis = new object[]
                {
                    new
                    {
                        allowDecimals = false,
                        min = 0,
                        lineWidth = 0,
                        tickWidth = 1,
                        title = new
                        {
                            align = "high",
                            offset = 0,
                            rotation = 0,
                            y = -20,
                            text = yTitleLeft,
                        },
                        type = "linear",
                    },
                    new
                    {
                        opposite = true,
                        allowDecimals = false,
                        min = 0,
                        lineWidth = 0,
                        tickWidth = 1,
                        title = new
                        {
                            align = "high",
                            offset = 0,
                            rotation = 0,
                            y = -20,
                            text = yTitleRight,
                        },
                        type = "linear",
                        gridLineWidth = 0
                    },
                },
                navigation = new { buttonOptions = new { enabled = false } },
                series = series
                
            };


            var ser = Newtonsoft.Json.JsonConvert.SerializeObject(anon);
            sb.Append(ser);
            sb.Append(");});");
            sb.AppendLine("</script>");
            return htmlHelper.Raw(sb.ToString());
        }

        public static IHtmlString GraphTheme(this HtmlHelper htmlHelper)
        {
            var sb = new System.Text.StringBuilder();

            var fontStyle = new
            {
                fontFamily = "Cabin",
                fontSize = 14,
                color = "#AEBCCB"
            };

            var options = new
            {
                chart = new
                {
                    style = fontStyle
                },
                colors = new[]
                {
                    "#DDE3E9", "#AFB9C5", "#2975DC", "#E76605"
                },
                tooltip = new
                {
                    backgroundColor = "#FFFFFF",
                    borderWidth = 1,
                    shadow = false,
                },
                plotOptions = new
                {
                    line = new
                    {
                        animation = true,
                        borderWidth = 0,
                        groupPadding = 0,
                        shadow = false
                    },
                    column = new
                    {
                        animation = true,
                        borderWidth = 0,
                        grouping = false,
                        groupPadding = 0,
                        shadow = false
                    },
                    bar = new
                    {
                        animation = true,
                        borderWidth = 0,
                        grouping = false,
                        groupPadding = 0,
                        shadow = false
                    }
                },
                xAxis = new
                {
                    labels = new
                    {
                        style = fontStyle
                    },
                    title = new
                    {
                        style = fontStyle
                    }
                },
                yAxis = new
                {
                    labels = new
                    {
                        style = fontStyle
                    },
                    title = new
                    {
                        style = fontStyle
                    }
                },
                legend = new
                {
                    itemStyle = fontStyle
                },
                title = new
                {
                    style = new 
                    {
                        fontFamily = "Cabin",
                        fontSize = 18,
                        color = "#28313B",
                        fontWeight = "bold"
                    }
                }

            };

            string optionsSerialized = Newtonsoft.Json.JsonConvert.SerializeObject(options);
            
            sb.AppendLine("<script>");
            sb.AppendLine($"Highcharts.setOptions({optionsSerialized});");
            sb.AppendLine("</script>");
            sb.AppendLine("<style>");
            sb.AppendLine(".small-circle {height: 12px;width: 12px;border-radius: 50%;display: inline-block;}");
            sb.AppendLine(".zone-light {fill-opacity: 0.5;}");
            //sb.AppendLine(".chart-style {font-family: \"Cabin\", \"sans-serif\"; fontSize: \"14px\"; color: \"#AEBCCB\" }");
            sb.AppendLine("</style>");
            //return $"Highcharts.setOptions({optionsSerialized});";
            return htmlHelper.Raw(sb.ToString());
        }

        public static IHtmlString DataToHTMLTable<T>(this HtmlHelper htmlHelper, 
            HlidacStatu.Lib.Render.ReportDataSource<T> rds,
            string tableId = "", 
            string dataTableOptions = @"{
                         'language': {
                            'url': '//cdn.datatables.net/plug-ins/1.10.19/i18n/Czech.json'
                        },
                        'order': [],
                        'lengthChange': false,
                        'info': false,
                        }", 
            string customTableHeader = null)
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);
            string _tableId = tableId;
            if (string.IsNullOrEmpty(tableId))
            {
                _tableId = Devmasters.TextUtil.GenRandomString("abcdefghijklmnopqrstuvwxyz", 10);
            }

            sb.AppendLine(@"<script>
var tbl_" + _tableId + @";
$(document).ready(function () {
tbl_" + _tableId + @" = $('#" + _tableId + @"').DataTable(" + dataTableOptions + @");
});
</script>");

            sb.AppendFormat("<h3>{0}</h3>", rds?.Title ?? "");
            sb.AppendFormat("<table id=\"{0}\" class=\"table-sorted table table-bordered table-striped\">", _tableId);
            if (customTableHeader == null)
            {
                sb.Append("<thead><tr>");
                foreach (var column in rds.Columns)
                {
                    sb.AppendFormat("<th>{0}</th>", column.Name);
                }
                sb.Append("</tr></thead>");
            }
            else
            {
                sb.AppendFormat(customTableHeader, _tableId);
            }
            sb.Append("<tbody class=\"list\">");
            foreach (var row in rds.Data)
            {
                sb.Append("<tr>");
                foreach (var d in row)
                {
                    sb.AppendFormat("<td {2} class=\"{0}\">{1}</td>",
                        d.Column.CssClass,
                        d.Column.HtmlRender(d.Value),
                        string.IsNullOrEmpty(d.Column.OrderValueRender(d.Value))
                                ? string.Empty : string.Format("data-order=\"{0}\"", d.Column.OrderValueRender(d.Value))
                        );

                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            return htmlHelper.Raw(sb.ToString());
        }


        public static System.Web.IHtmlString SubjektTypTrojice(this HtmlHelper self, Lib.Data.Firma firma, string htmlProOVM , string htmlProStatniFirmu, string htmlProSoukromou)
        {
            if (firma == null)
                return self.Raw("");
            if (firma.JsemOVM())
                return self.Raw(htmlProOVM);
            else if (firma.JsemStatniFirma())
                return self.Raw(htmlProStatniFirmu);
            else
                return self.Raw(htmlProSoukromou);
        }



    }


    }