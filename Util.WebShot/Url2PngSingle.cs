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
    public class Url2PngSingle
    {

        static object lockObj = new object();

        public static byte[] Screenshot(string url, int cropWidth = 1000, int cropHeight = 1000)
        {
            return Screenshot(Chrome.TheOnlyStaticInstance, url, cropWidth, cropHeight);
        }
        public static byte[] Screenshot(Chrome instance, string url, int cropWidth = 1000, int cropHeight=1000)
        {
            lock (lockObj)
            {
                try
                {
                    Chrome.logger.Debug($"{instance.Name}: Screenshot GoToUrl {url}");
                    instance.WebDriver().GoToUrl(url).Wait();
                    System.Threading.Thread.Sleep(50);
                    Chrome.logger.Debug($"{instance.Name}: Screenshot GetScreenshot {url}");
                    var screenshot = instance.WebDriver().GetScreenshot().Result;
                    Chrome.logger.Debug($"{instance.Name}: Screenshot Cropping {url}");
                    using (Devmasters.Imaging.InMemoryImage img = new Devmasters.Imaging.InMemoryImage(screenshot.AsByteArray))
                    {
                        img.Crop(new System.Drawing.Rectangle(0, 0, cropWidth, cropHeight));

                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                        {
                            img.SaveAsPNG(ms);
                            return ms.ToArray();
                        }
                    }

                }
                catch (Exception e)
                {
                    Chrome.logger.Error($"{instance.Name}: Screenshot Error {url}",e);
                    instance.RecreatedChrome();
                    return null;
                }
                finally
                {
                    Chrome.logger.Debug($"{instance.Name}: Screenshot DONE {url}");

                }
            }
        }
    }
}
