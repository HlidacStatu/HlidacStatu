using PuppeteerSharp;
using System;

namespace HlidacStatu.Util.WebShot
{
    public class PuppeteerBrowser : IDisposable
    {

        static PuppeteerBrowser()
        {
            new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision).Wait();
        }
        public static Devmasters.Core.Logging.Logger logger = new Devmasters.Core.Logging.Logger("HlidacStatu.Util.Webshots");

        private static readonly PuppeteerBrowser theOnlyInstance = new PuppeteerBrowser("theOnlyInstance");

        public static PuppeteerBrowser TheOnlyStaticInstance
        {
            get
            {
                return theOnlyInstance;
            }
        }

        PuppeteerSharp.Browser browser = null;
        PuppeteerSharp.Page page = null;

        int width = 00;
        int height = 0;
        public string Name { get; set; } = null;
        public PuppeteerBrowser(string name = null)
            : this(1920, 1080, name)
        {
        }

        public PuppeteerBrowser(int displaywidth, int displayheight, string name = null)
        {
            this.width = displaywidth;
            this.height = displayheight;
            this.Name = "PuppeteerBrowser-" + (name ?? Devmasters.Core.TextUtil.GenRandomString(10));
            Init();
        }

        private void Init()
        {
            logger.Debug($"{this.Name}: Initiating PuppeteerBrowser");
            var chromeBinaryFileName = Devmasters.Core.Util.Config.GetConfigValue("ChromeBinaryFullPath");

            var launchOptions = new LaunchOptions()
            {
                Headless = true,
                DefaultViewport = new ViewPortOptions()
                {
                    DeviceScaleFactor = 1,
                    IsLandscape = false,
                    HasTouch = false,
                    IsMobile = false,
                    Height = this.height,
                    Width = this.width,
                }
            };
            if (!string.IsNullOrEmpty(chromeBinaryFileName))
                launchOptions.ExecutablePath = chromeBinaryFileName;


            this.browser = Puppeteer.LaunchAsync(launchOptions).Result;
            this.page = browser.NewPageAsync().Result;
            logger.Info($"{this.Name}: Initiated PuppeteerBrowser");

        }

        object browserLock = new object();
        public PuppeteerSharp.Page Page()
        {
                return this.page;
        }

        public void WarmUp()
        {
            logger.Debug($"{this.Name}: Warming up");
            this.page.GoToAsync("https://www.google.com").Wait();
        }


        internal void RecreatedChrome()
        {
            logger.Info($"{this.Name}: Recreating PuppeteerBrowser");
            lock (browserLock)
            {
                this.page.Dispose();
                this.browser.CloseAsync().Wait();
            }
            System.Threading.Thread.Sleep(500);
            Init();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            logger.Info($"{this.Name}: Disposing PuppeteerBrowser Instance");
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                logger.Debug($"{this.Name}: Closing PuppeteerBrowser");
                this.browser.CloseAsync().Wait();
                logger.Debug($"{this.Name}: Closed PuppeteerBrowser");
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~PuppeteerBrowser()
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
