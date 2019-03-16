using System;
using System.Threading.Tasks;

namespace HlidacStatu.Util.WebShot
{
    public class Url2PngMulti
    {

        static object lockObj = new object();

        public static byte[] Screenshot(string url, int cropWidth = 1000, int cropHeight = 1000)
        {
            return Screenshot(url, TimeSpan.FromSeconds(10), cropWidth, cropHeight);
        }
        public static byte[] Screenshot(string url, TimeSpan timeout, int cropWidth = 1000, int cropHeight = 1000)
        {

            DateTime start = DateTime.Now;
            DateTime end = start.Add(timeout);
            byte[] result = null;
            Workers.WorkRequest wr = new Workers.WorkRequest()
            {
                Url = url,
                cropWidth = cropWidth,
                cropHeight = cropHeight
            };
            string wrs = wr.ToString();
            Workers.UrlToProcess.Enqueue(wrs);
            do
            {
                if (Workers.Results.ContainsKey(wrs))
                {
                    if (Workers.Results.TryRemove(wrs, out result))
                    {
                        return result;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(5);
                        if (Workers.Results.TryRemove(wrs, out result))
                        {
                            return result;
                        }
                        else
                            Chrome.logger.Warning("Cannot get result");
                    }
                    //break;
                }
                System.Threading.Thread.Sleep(200);
            } while (end > DateTime.Now);
            return result;
        }



        public static async Task<byte[]> ScreenshotAsync(string url, int cropWidth = 1000, int cropHeight = 1000)
        {
            return await ScreenshotAsync(url, TimeSpan.FromSeconds(10), cropWidth, cropHeight);
        }
        public static async Task<byte[]> ScreenshotAsync(string url, TimeSpan timeout, int cropWidth = 1000, int cropHeight = 1000)
        {

            DateTime start = DateTime.Now;
            DateTime end = start.Add(timeout);
            byte[] result = null;
            Workers.WorkRequest wr = new Workers.WorkRequest()
            {
                Url = url,
                cropWidth = cropWidth,
                cropHeight = cropHeight
            };
            string wrs = wr.ToString();
            Workers.UrlToProcess.Enqueue(wrs);
            do
            {
                if (Workers.Results.ContainsKey(wrs))
                {
                    if (Workers.Results.TryRemove(wrs, out result))
                    {
                        return result;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(5);
                        if (Workers.Results.TryRemove(wrs, out result))
                        {
                            return result;
                        }
                        else
                            Chrome.logger.Warning("Cannot get result");
                    }
                    //break;
                }
                 await System.Threading.Tasks.Task.Delay(200);
            } while (end > DateTime.Now);
            return result;
        }

    }
}

