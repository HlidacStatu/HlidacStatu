using System;

using PuppeteerSharp;

namespace HlidacStatu.Util.WebShot
{
    public class Url2PngSingle_Puppeteer
    {

        static object lockObj = new object();

        public static byte[] Screenshot(string url, int cropWidth = 1000, int cropHeight = 1000)
        {
            return Screenshot(PuppeteerBrowser.TheOnlyStaticInstance, url, cropWidth, cropHeight);
        }
        public static byte[] Screenshot(PuppeteerBrowser instance, string url, int cropWidth = 1000, int cropHeight = 1000)
        {
            lock (lockObj)
            {
                try
                {
                    Chrome.logger.Debug($"{instance.Name}: Screenshot GoToUrl {url}");
                    instance.Page().GoToAsync(url).Wait();
                    System.Threading.Thread.Sleep(50);
                    Chrome.logger.Debug($"{instance.Name}: Screenshot GetScreenshot {url}");
                    var screenshot = instance.Page().ScreenshotDataAsync().Result;
                    Chrome.logger.Debug($"{instance.Name}: Screenshot Cropping {url}");
                    using (Devmasters.Imaging.InMemoryImage img = new Devmasters.Imaging.InMemoryImage(screenshot))
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
                    Chrome.logger.Error($"{instance.Name}: Screenshot Error {url}", e);
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
