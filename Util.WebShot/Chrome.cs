using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zu.AsyncWebDriver.Remote;
using Zu.Chrome;
using Zu.WebBrowser.BasicTypes;

namespace HlidacStatu.Util.WebShot
{
    public class Chrome : IDisposable
    {
        public static Devmasters.Core.Logging.Logger logger = new Devmasters.Core.Logging.Logger("HlidacStatu.Util.Webshots");

        private static readonly Chrome theOnlyInstance = new Chrome("theOnlyInstance");

        public static Chrome TheOnlyStaticInstance
        {
            get
            {
                return theOnlyInstance;
            }
        }


        AsyncChromeDriver asyncChromeDriver = null;
        private WebDriver webDriver { get;  set; }  = null;

        int width = 00;
        int height = 0;
        public string Name  { get; set; } = null;
        public Chrome(string name = null)
            : this(1920,1080,name)
        {
        }

        public Chrome(int displaywidth, int displayheight, string name = null)
        {
            this.width = displaywidth;
            this.height = displayheight;
            this.Name = "ChromeInstance-" + ( name ?? Devmasters.Core.TextUtil.GenRandomString(10));
            Init();
        }

        private void Init()
        {
            logger.Debug($"{this.Name}: Initiating WebDriver");
            var chromeBinaryFileName = Devmasters.Core.Util.Config.GetConfigValue("ChromeBinaryFullPath");
            if (!string.IsNullOrEmpty(chromeBinaryFileName))
                Zu.Chrome.ChromeProfilesWorker.ChromeBinaryFileName = chromeBinaryFileName;

            var config = new ChromeDriverConfig()
                .SetHeadless()
                .SetWindowSize(width+30, height+30)
                .SetCommandLineArgumets("--disable-gpu")
                ;

            this.asyncChromeDriver = new AsyncChromeDriver(config);
            this.webDriver = new WebDriver(asyncChromeDriver);
            logger.Info($"{this.Name}: Initiated WebDriver");

        }

        public WebDriver WebDriver()
        {
            lock (this.asyncChromeDriver)
            {
                return webDriver;
            }
        }

        public void WarmUp()
        {
            logger.Debug($"{this.Name}: Warming up");
            WebDriver().GoToUrl("https://www.google.com").Wait();
        }

        internal void RecreatedChrome()
        {
            logger.Info($"{this.Name}: Recreating WebDriver");
            lock (this.asyncChromeDriver)
            {
                webDriver.Close().Wait();
            }
            System.Threading.Thread.Sleep(500);
            Init();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            logger.Info($"{this.Name}: Disposing Chrome Instance");
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                logger.Debug($"{this.Name}: Closing webDriver");
                webDriver.Close().Wait();
                logger.Debug($"{this.Name}: Closed webDriver");
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~Chrome()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
