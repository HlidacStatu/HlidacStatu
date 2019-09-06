using System.Collections.Generic;
using System.Linq;
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

    }
}