using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HlidacStatu.Web.Framework
{
    public class RobotFromIP : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            string ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            string[] blackList = new[] { "195.208.220.174"};
            if (blackList.Contains(ip) &&
                !(
                    filterContext?.ActionDescriptor?.ActionName?.ToLower() == "bot"
                    ||
                    filterContext?.ActionDescriptor?.ActionName?.ToLower()?.StartsWith("error") == true
                    )
                )
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Bot"
                }));
            }
        }
    }
}