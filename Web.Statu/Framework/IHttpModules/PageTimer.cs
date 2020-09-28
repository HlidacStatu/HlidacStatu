using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HlidacStatu.Web.Framework.IHttpModules
{
    public class PageTimer : IHttpModule
    {
            #region IHttpModule Members

        private StopWatchLogger swl = null;

		public void Dispose()
		{

		}

		public void Init(HttpApplication context)
		{
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.EndRequest += new EventHandler(context_EndRequest);
		}

        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            swl = new StopWatchLogger(Devmasters.Logging.Logger.PageTimes, Devmasters.Logging.PriorityLevel.Information, app.Context, 0);
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            if (swl != null)
                swl.Dispose();            
        }


    #endregion

    }
}