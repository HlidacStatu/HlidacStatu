using HlidacStatu.Util.Cache;
using Newtonsoft.Json;
using System;
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

        public static Devmasters.Cache.V20.LocalMemory.AutoUpdatedLocalMemoryCache<string[]> BannedIPs = null;

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


            BannedIPs = new Devmasters.Cache.V20.LocalMemory.AutoUpdatedLocalMemoryCache<string[]>(
                TimeSpan.FromSeconds(30), "BannedIPs", (obj) =>
                {

                    var ret = new System.Collections.Generic.List<string>();
                    try
                    {
                        using (Devmasters.Core.PersistLib p = new Devmasters.Core.PersistLib())
                        {
                            var ds = p.ExecuteDataset(Devmasters.Core.Util.Config.GetConfigValue("CnnString"),
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

                    HlidacStatu.Util.Consts.Logger.Error(new Devmasters.Core.Logging.LogMessage()
                        .SetMessage("HTTP Error 400")
                        .SetException(exception)
                        .SetVersionOfAllAssemblies()
                        .SetCustomKeyValue("Url", this.Request.Url.AbsoluteUri)
                        .SetContext(Devmasters.Core.WebContextLogger.LogFatalWebError(exception, this.Context, true, string.Empty))
                        .SetCustomKeyValue("referrer", this.Request.UrlReferrer != null ? this.Request.UrlReferrer.AbsoluteUri : "")
                        );

                    goto finishResponse;
                }
                if (
                    httpException.Message == "External component has thrown an exception."
                    ||
                    httpException.Message.Contains("potentially dangerous Request.Path")
                    )
                {
                    HlidacStatu.Util.Consts.Logger.Warning(new Devmasters.Core.Logging.LogMessage()
                        .SetMessage("Hack")
                        .SetException(exception)
                        .SetVersionOfAllAssemblies()
                        .SetCustomKeyValue("adSourceUrl", this.Request.Url.AbsoluteUri)
                        .SetContext(Devmasters.Core.WebContextLogger.LogFatalWebError(exception, this.Context, true, string.Empty))
                        .SetCustomKeyValue("referrer", this.Request.UrlReferrer != null ? this.Request.UrlReferrer.AbsoluteUri : "")
                        );
                    goto finishResponse;
                }

            }
            if (exception.GetType() == typeof(System.InvalidOperationException) &&
                (
                    exception.Message.StartsWith("The view "))
                    || exception.Message.Contains("only be accessed via SSL")
                )
            {
                HlidacStatu.Util.Consts.Logger.Warning(new Devmasters.Core.Logging.LogMessage()
                    .SetMessage("HTTP Error SSL")
                    .SetException(exception)
                    .SetVersionOfAllAssemblies()
                    .SetCustomKeyValue("adSourceUrl", this.Request.Url.AbsoluteUri)
                    .SetContext(Devmasters.Core.WebContextLogger.LogFatalWebError(exception, this.Context, true, string.Empty))
                    .SetCustomKeyValue("referrer", this.Request.UrlReferrer != null ? this.Request.UrlReferrer.AbsoluteUri : "")
                    );
                goto finishResponse;
            }
            try
            {
                HlidacStatu.Util.Consts.Logger.Fatal(new Devmasters.Core.Logging.LogMessage()
                    .SetMessage("HTTP Error 500")
                    .SetContext(Devmasters.Core.WebContextLogger.LogFatalWebError(exception, this.Context, true, string.Empty))
                    .SetCustomKeyValue("referrer", this.Request?.UrlReferrer != null ? this.Request.UrlReferrer.AbsoluteUri : "")
                    .SetException(exception)
                    .SetVersionOfAllAssemblies()
                    .SetCustomKeyValue("Url", this.Request?.Url?.AbsoluteUri)
                    );


            }
            catch (Exception)
            {

            }


        finishResponse:

            return;
        }

    }
}
