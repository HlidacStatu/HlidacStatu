using HlidacStatu.Util.Cache;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HlidacStatu.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static string LoginRedirPath = "/Account/Login";


        public static volatile MemoryCacheManager<string, string> CachedDatasets = null;


        public static Devmasters.Cache.LocalMemory.AutoUpdatedLocalMemoryCache<string[]> BannedIPs = null;

        class attack { public int num; public DateTime last; public string ip; }
        static Dictionary<string, attack> attackers = new Dictionary<string, attack>();

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {

            if (BannedIPs.Get().Contains(HttpContext.Current.Request.UserHostAddress.ToLower()))
            {
                Response.Clear();
                Response.StatusCode = 403;
                Server.Transfer("~/banned.aspx");
            }

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configure(WebApiConfig.Register);


            BannedIPs = new Devmasters.Cache.LocalMemory.AutoUpdatedLocalMemoryCache<string[]>(
                TimeSpan.FromSeconds(30), "BannedIPs", (obj) =>
                {

                    var ret = new System.Collections.Generic.List<string>();
                    try
                    {
                        using (Devmasters.PersistLib p = new Devmasters.PersistLib())
                        {
                            //p.ExecuteNonQuery(Devmasters.Config.GetWebConfigValue("CnnString"),
                            //     System.Data.CommandType.Text, "delete from BannedIPs where expiration < GetDate()", null);

                            var ds = p.ExecuteDataset(Devmasters.Config.GetWebConfigValue("CnnString"),
                                 System.Data.CommandType.Text, "select IP from BannedIPs where Expiration is null or expiration > GetDate()",
                                 null);
                            foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                            {
                                ret.Add(((string)dr[0]).ToLower());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        HlidacStatu.Util.Consts.Logger.Error("Global.asax BannedIP db query", e);
                    }
                    return ret.ToArray();
                }
            );

            //if (ValueProviderFactories.Factories.Any(m => m.GetType() == typeof(System.Web.Mvc.JsonValueProviderFactory)))
            //{
            //    var f = ValueProviderFactories.Factories.Where(m => m.GetType() == typeof(System.Web.Mvc.JsonValueProviderFactory)).FirstOrDefault();
            //    if (f != null)
            //        ValueProviderFactories.Factories.Remove(f);
            //}

            var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            formatter.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                //ContractResolver = new HlidacStatu.Util.FirstCaseLowercaseContractResolver()
            };

            formatter.MediaTypeMappings
            .Add(new System.Net.Http.Formatting.RequestHeaderMapping("Accept",
                    "text/html",
                    StringComparison.InvariantCultureIgnoreCase,
                    true,
                    "application/json"));

            //System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(new Framework.DataSetsVirtualPathProvider());


            HlidacStatu.Lib.Init.Initialise();
        }


        protected void Application_Error(Object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            var httpException = exception as HttpException;
            if (httpException != null)
            {
                if (httpException.GetHttpCode() == 404)
                {

                    HlidacStatu.Util.Consts.Logger.Error(new Devmasters.Logging.LogMessage()
                        .SetMessage("HTTP Error 400")
                        .SetException(exception)
                        .SetVersionOfAllAssemblies()
                        .SetCustomKeyValue("Url", this.Request.Url.AbsoluteUri)
                        //.SetContext(Devmasters.Net.WebContextLogger.LogFatalWebError(exception, this.Context, true, string.Empty)) //TODO
                        .SetCustomKeyValue("referrer", this.Request.UrlReferrer != null ? this.Request.UrlReferrer.AbsoluteUri : "")
                        );
                    InvokeErrorAction(Controllers.HomeController.ErrorPages.NotFound, this.Context, httpException);
                    goto finishResponse;
                }
                if (
                    httpException.Message == "External component has thrown an exception."
                    ||
                    httpException.Message.Contains("potentially dangerous Request.Path")
                    )
                {
                    HlidacStatu.Util.Consts.Logger.Warning(new Devmasters.Logging.LogMessage()
                        .SetMessage("Hack")
                        .SetException(exception)
                        .SetVersionOfAllAssemblies()
                        .SetCustomKeyValue("adSourceUrl", this.Request.Url.AbsoluteUri)
                        //.SetContext(Devmasters.WebContextLogger.LogFatalWebError(exception, this.Context, true, string.Empty)) //TO
                        .SetCustomKeyValue("referrer", this.Request.UrlReferrer != null ? this.Request.UrlReferrer.AbsoluteUri : "")
                        );
                    InvokeErrorAction(Controllers.HomeController.ErrorPages.ErrorHack, this.Context, httpException);
                    goto finishResponse;
                }

            }
            if (exception.GetType() == typeof(System.InvalidOperationException) &&
                (
                    exception.Message.StartsWith("The view "))
                    || exception.Message.Contains("only be accessed via SSL")
                )
            {
                HlidacStatu.Util.Consts.Logger.Warning(new Devmasters.Logging.LogMessage()
                    .SetMessage("HTTP Error SSL")
                    .SetException(exception)
                    .SetVersionOfAllAssemblies()
                    .SetCustomKeyValue("adSourceUrl", this.Request.Url.AbsoluteUri)
                    //.SetContext(Devmasters.WebContextLogger.LogFatalWebError(exception, this.Context, true, string.Empty))
                    .SetCustomKeyValue("referrer", this.Request.UrlReferrer != null ? this.Request.UrlReferrer.AbsoluteUri : "")
                    );

                InvokeErrorAction(Controllers.HomeController.ErrorPages.ErrorHack, this.Context, httpException);
                goto finishResponse;
            }
            if (exception.GetType() == typeof(System.Web.HttpRequestValidationException)
                ||
                exception.GetType() == typeof(System.Web.HttpParseException)
                ||
                exception.GetType() == typeof(System.Web.HttpException)
                )
            {
                HlidacStatu.Util.Consts.Logger.Warning(new Devmasters.Logging.LogMessage()
                    .SetMessage("HTTP Error SSL")
                    .SetException(exception)
                    .SetVersionOfAllAssemblies()
                    .SetCustomKeyValue("adSourceUrl", this.Request.Url.AbsoluteUri)
                    //.SetContext(Devmasters.WebContextLogger.LogFatalWebError(exception, this.Context, true, string.Empty))
                    .SetCustomKeyValue("referrer", this.Request.UrlReferrer != null ? this.Request.UrlReferrer.AbsoluteUri : "")
                    );

                InvokeErrorAction(Controllers.HomeController.ErrorPages.ErrorHack, this.Context, httpException);
                goto finishResponse;
            }
            try
            {
                HlidacStatu.Util.Consts.Logger.Fatal(new Devmasters.Logging.LogMessage()
                    .SetMessage("HTTP Error 500")
                    //.SetContext(Devmasters.WebContextLogger.LogFatalWebError(exception, this.Context, true, string.Empty))
                    .SetCustomKeyValue("referrer", this.Request?.UrlReferrer != null ? this.Request.UrlReferrer.AbsoluteUri : "")
                    .SetException(exception)
                    .SetVersionOfAllAssemblies()
                    .SetCustomKeyValue("Url", this.Request?.Url?.AbsoluteUri)
                    );

                InvokeErrorAction(Controllers.HomeController.ErrorPages.ErrorHack, this.Context, httpException);

            }
            catch (Exception)
            {

            }


        finishResponse:

            return;
        }

        object _autoAddBlackList = new object();
        void AutoAddBlackList(Controllers.HomeController.ErrorPages errPage, HttpRequest req)
        {
            bool add = false;
            var dt = DateTime.Now;
            string ip = req.UserHostAddress;
            lock (_autoAddBlackList)
            {
                if (attackers.ContainsKey(ip))
                {
                    attackers[ip].num++;
                }
                else
                    attackers.Add(ip, new attack() { ip = ip, last = DateTime.Now, num = 1 });
            }

            var att = attackers[ip];
            var diff = (dt - att.last).TotalSeconds;

            if (diff >= 600)
                att.num = 1;
            else
                att.num = att.num + 1;

            switch (errPage)
            {
                case Controllers.HomeController.ErrorPages.Ok:
                    break;
                case Controllers.HomeController.ErrorPages.NotFound:
                    add = att.num > 20;
                    break;
                case Controllers.HomeController.ErrorPages.Error:
                    add = att.num > 15;
                    break;
                case Controllers.HomeController.ErrorPages.ErrorHack:
                    add = att.num > 10;
                    break;
                default:
                    break;
            }

            if (add)
            {
                HlidacStatu.Util.Consts.Logger.Warning($"added bannedIP {ip}");
                try
                {
                    using (Devmasters.PersistLib p = new Devmasters.PersistLib())
                    {
                        p.ExecuteNonQuery(Devmasters.Config.GetWebConfigValue("CnnString"),
                             System.Data.CommandType.Text, "INSERT INTO [dbo].[BannedIPs] ([IP],[Expiration],[Created]) "
                             + " VALUES(@ip,@dat,GetDate())", new System.Data.IDataParameter[] {
                            new System.Data.SqlClient.SqlParameter("ip", ip),
                            new System.Data.SqlClient.SqlParameter("dat", dt.AddHours(6))
                            });
                    }
                    BannedIPs.ForceRefreshCache();

                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Warning($"added bannedIP {ip}", e);

                }
            }
        }

        void InvokeErrorAction(Controllers.HomeController.ErrorPages errPage, HttpContext httpContext, Exception exception)
        {
            AutoAddBlackList(errPage, httpContext.Request);

            System.Configuration.Configuration configuration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");
            System.Web.Configuration.CustomErrorsSection section = (System.Web.Configuration.CustomErrorsSection)configuration.GetSection("system.web/customErrors");

#if DEBUG
            return;
#endif


            var routeData = new RouteData();
            routeData.Values["controller"] = "Home";
            routeData.Values["action"] = "Error";
            routeData.Values["errPage"] = errPage.ToString();
            routeData.Values["InvokeErrorAction"] = true;


            //var httpcx = new HttpContext(new HttpRequest("error", "https://www.hlidacstatu.cz/error",""), httpContext.Response);


            Response.Redirect("/Error/" + errPage);

            if (false)
            {
                using (var controller = new Controllers.HomeController())
                {
                    ((IController)controller).Execute(
                    new RequestContext(new HttpContextWrapper(httpContext), routeData));
                }
            }

        }
    }
}
