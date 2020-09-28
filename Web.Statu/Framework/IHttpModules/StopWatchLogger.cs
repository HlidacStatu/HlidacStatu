using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Web;

namespace HlidacStatu.Web.Framework.IHttpModules
{

    public class StopWatchLogger : Stopwatch, IDisposable
    {
        private static double defaultSlowLoggerThreshold = 5000;
        private bool isDisposing = false;
        private object objLock = new object();
        private Devmasters.Logging.PriorityLevel level = Devmasters.Logging.PriorityLevel.Debug;
        private string textTemplate = "Elapsed in {0} ms";
        private double slowLoggerThreshold = defaultSlowLoggerThreshold;
        private Devmasters.Logging.Logger logger = null;
        private HttpContext context = null;

        #region Constructors

        public StopWatchLogger(Devmasters.Logging.Logger logger)
            : this(logger, Devmasters.Logging.PriorityLevel.Debug, string.Empty, defaultSlowLoggerThreshold)
        { }

        public StopWatchLogger(Devmasters.Logging.Logger logger, Devmasters.Logging.PriorityLevel level)
            : this(logger, level, string.Empty, defaultSlowLoggerThreshold)
        { }

        public StopWatchLogger(Devmasters.Logging.Logger logger, string template)
            : this(logger, Devmasters.Logging.PriorityLevel.Debug, template, defaultSlowLoggerThreshold)
        { }
        public StopWatchLogger(Devmasters.Logging.Logger logger, string template, double slowLoggerThreshold)
            : this(logger, Devmasters.Logging.PriorityLevel.Debug, template, slowLoggerThreshold)
        { }

        public StopWatchLogger(Devmasters.Logging.Logger logger, Devmasters.Logging.PriorityLevel level, HttpContext httpContext, double slowLoggerThreshold)
            : base()
        {
            this.logger = logger;
            if (slowLoggerThreshold <= 0)
                if (!string.IsNullOrEmpty(Devmasters.Config.GetWebConfigValue("PageTimerDefaultThreshold")))
                    slowLoggerThreshold = Convert.ToDouble(Devmasters.Config.GetWebConfigValue("PageTimerDefaultThreshold"), Devmasters.TextUtil.USCulture);

            this.slowLoggerThreshold = slowLoggerThreshold;
            this.level = level;
            this.context = httpContext;
            this.Start();
        }

        public StopWatchLogger(Devmasters.Logging.Logger logger, Devmasters.Logging.PriorityLevel level, string template, double slowLoggerThreshold)
            : base()
        {
            this.logger = logger;
            this.slowLoggerThreshold = slowLoggerThreshold;
            this.level = level;
            if (!string.IsNullOrEmpty(template))
            {
                textTemplate = template;
                if (!textTemplate.Contains("{0}"))
                    textTemplate += " finished in {0} ms";
            }

            this.Start();
        }


        #endregion

        /// <summary>
        /// Gets the exact elapsed miliseconds.
        /// </summary>
        /// <value>The exact elapsed miliseconds.</value>
        public double ExactElapsedMiliseconds
        {

            get
            {

                //long ticketsperMs = TimeSpan.TicksPerMillisecond ;

                double d = (double)this.Elapsed.Ticks / (double)TimeSpan.TicksPerMillisecond;

                return Math.Round(d, 5);

            }

        }

        private string FormatParams(HttpContext context)
        {
            StringBuilder sb = new StringBuilder(512);
            foreach (string key in context.Request.QueryString.AllKeys)
            {
                string val = context.Request.QueryString[key];
                if (key == "__VIEWSTATE")
                    sb.Append("&" + key + string.Format("=<{0} bytes>", val.Length));
                else
                {
                    if (val.Length > 30)
                    {
                        val = val.Substring(0, 30);
                        if (val.Contains("\n"))
                            val = val.Replace("\n", string.Empty);
                    }
                    sb.Append("&" + key + "=" + val);
                }
            }
            foreach (string key in context.Request.Form.AllKeys)
            {
                string val = context.Request.Form[key];
                if (key == "__VIEWSTATE")
                    sb.Append("&" + key + string.Format("=<{0} bytes>", val.Length));
                else
                {
                    if (val.Length > 30)
                    {
                        val = val.Substring(0, 30);
                        if (val.Contains("\n"))
                            val = val.Replace("\n", string.Empty);
                    }
                    sb.Append("&" + key + "=" + val);
                }
            }

            return sb.ToString();
        }

        private void Log()
        {
            Devmasters.Logging.LogMessage msg = null;
            if (this.ExactElapsedMiliseconds > slowLoggerThreshold)
            {
                if (context != null)
                {
                    if (context.Request != null)
                    {
                        msg = new Devmasters.Logging.LogMessage();
                        //<conversionPattern value="%date|%property{page}|%property{params}|%property{user}|%property{elapsedtime}" />
                        msg.SetCustomKeyValue("web_page", context.Request.Url.AbsolutePath);
                        msg.SetCustomKeyValue("web_params", FormatParams(context));
                        msg.SetCustomKeyValue("web_elapsedtime", this.ExactElapsedMiliseconds);

                        if (context.User != null && context.User.Identity != null && context.User.Identity.Name != null)
                            msg.SetCustomKeyValue("web_user", context.User.Identity.Name);


                    }
                }
                else
                {

                    if (this.IsRunning || this.ElapsedTicks == 0)
                        return;

                    if (
                        (level != Devmasters.Logging.PriorityLevel.Fatal | level != Devmasters.Logging.PriorityLevel.Error)
                        && this.ExactElapsedMiliseconds > slowLoggerThreshold && slowLoggerThreshold > 0)
                        logger.Error(string.Format(textTemplate + " TOO SLOW", this.ExactElapsedMiliseconds));
                }

                switch (level)
                {
                    case Devmasters.Logging.PriorityLevel.Debug:
                        if (msg != null)
                            logger.Debug(msg);
                        else
                            logger.Debug(string.Format(textTemplate, this.ExactElapsedMiliseconds));
                        break;
                    case Devmasters.Logging.PriorityLevel.Information:
                        if (msg != null)
                            logger.Info(msg);
                        else
                            logger.Info(string.Format(textTemplate, this.ExactElapsedMiliseconds));
                        break;
                    case Devmasters.Logging.PriorityLevel.Warning:
                        if (msg != null)
                            logger.Warning(msg);
                        else
                            logger.Warning(string.Format(textTemplate, this.ExactElapsedMiliseconds));
                        break;
                    case Devmasters.Logging.PriorityLevel.Error:
                        if (msg != null)
                            logger.Error(msg);
                        else
                            logger.Error(string.Format(textTemplate, this.ExactElapsedMiliseconds));
                        break;
                    case Devmasters.Logging.PriorityLevel.Fatal:
                        if (msg != null)
                            logger.Fatal(msg);
                        else
                            logger.Fatal(string.Format(textTemplate, this.ExactElapsedMiliseconds));
                        break;
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!isDisposing)
            {
                lock (objLock)
                {
                    isDisposing = true;
                    if (this.IsRunning)
                        this.Stop();
                    Log();
                }
            }
        }

        #endregion
    }

}
