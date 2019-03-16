using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HlidacStatu.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            if (ValueProviderFactories.Factories.Any(m => m.GetType() == typeof(System.Web.Mvc.JsonValueProviderFactory)))
            {
                var f = ValueProviderFactories.Factories.Where(m => m.GetType() == typeof(System.Web.Mvc.JsonValueProviderFactory)).FirstOrDefault();
                if (f != null)
                    ValueProviderFactories.Factories.Remove(f);
            }

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
