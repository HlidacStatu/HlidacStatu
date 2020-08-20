using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

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
        <div class='low-box-line' style='top:{width - 55}px'><a href='#' onclick='ga('send', 'event', 'btnLowBoxMore', 'showMore','{gaPageEventId}'); return true;' class='more'></a></div>
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

        public static IHtmlString KIndexIcon(this HtmlHelper htmlHelper, string ico, string height = "15px", string hPadding = "3px", string vPadding = "0")
        {
            return htmlHelper.KIndexIcon(ico, $"padding:{vPadding} {hPadding};height:{height};width:auto");
        }
        public static IHtmlString KIndexIcon(this HtmlHelper htmlHelper, string ico, string style)
        {
            if (string.IsNullOrEmpty(ico))
                return htmlHelper.Raw("");

            ico = HlidacStatu.Util.ParseTools.NormalizeIco(ico);
            Tuple<int?, Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues> lbl = Lib.Analysis.KorupcniRiziko.KIndex.GetLastLabel(ico);
            if (lbl != null && lbl.Item2 != Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues.None)
            {
                return KIndexIcon(htmlHelper, lbl.Item2, style);
            }
            return htmlHelper.Raw("");
        }
        public static IHtmlString KIndexIcon(this HtmlHelper htmlHelper, Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues label, string style)
        {
            System.Security.Principal.IPrincipal user = htmlHelper.ViewContext.RequestContext.HttpContext.User;
            if (ShowKIndex(user))
            {
                return htmlHelper.Raw($"<img title='K-Index {label.ToString()} - Index korupčního rizika'  src='{Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelIconUrl(label)}' class='kindex' style='{style}'>");
            }
            else
                return htmlHelper.Raw("");
        }


        public static IHtmlString KIndexLabelLink(this HtmlHelper htmlHelper, string ico, string height = "15px", string hPadding = "3px", string vPadding = "0")
        {
            return htmlHelper.KIndexLabelLink(ico, $"padding:{vPadding} {hPadding};height:{height};width:auto");
        }
        public static IHtmlString KIndexLabelLink(this HtmlHelper htmlHelper, string ico, string style)
        {
            if (string.IsNullOrEmpty(ico))
                return htmlHelper.Raw("");

            ico = HlidacStatu.Util.ParseTools.NormalizeIco(ico);
            System.Security.Principal.IPrincipal user = htmlHelper.ViewContext.RequestContext.HttpContext.User;
            if (ShowKIndex(user))
            {
                Tuple<int?, Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues> lbl = Lib.Analysis.KorupcniRiziko.KIndex.GetLastLabel(ico);
                if (lbl != null && lbl.Item2 != Lib.Analysis.KorupcniRiziko.KIndexData.KIndexLabelValues.None)
                {
                    return htmlHelper.Raw($"<a href='/kindex/detail/{ico}'>"
                        + KIndexIcon(htmlHelper,lbl.Item2,style).ToHtmlString()
                        + "</a>");
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
            if (Devmasters.Core.Util.Config.GetConfigValue("KIndex") == "private")
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

    }
    public class Restricted : IDisposable
    {
        public bool Allow { get; set; }

        private StringBuilder _stringBuilderBackup;
        private StringBuilder _stringBuilder;
        private readonly HtmlHelper _htmlHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="Restricted"/> class.
        /// </summary>
        public Restricted(HtmlHelper htmlHelper, bool allow)
        {
            Allow = allow;
            _htmlHelper = htmlHelper;
            if (!allow) BackupCurrentContent();
        }

        private void BackupCurrentContent()
        {
            // make backup of current buffered content
            _stringBuilder = ((System.IO.StringWriter)_htmlHelper.ViewContext.Writer).GetStringBuilder();
            _stringBuilderBackup = new StringBuilder().Append(_stringBuilder);
        }

        private void DenyContent()
        {
            // restore buffered content backup (destroying any buffered content since Restricted object initialization)
            _stringBuilder.Length = 0;
            _stringBuilder.Append(_stringBuilderBackup);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!Allow)
                DenyContent();
        }
    }
    class DisposableHelper : IDisposable
    {
        private Action end;

        // When the object is created, write "begin" function
        public DisposableHelper(Action begin, Action end)
        {
            this.end = end;
            begin();
        }

        // When the object is disposed (end of using block), write "end" function
        public void Dispose()
        {
            end();
        }
    }

}