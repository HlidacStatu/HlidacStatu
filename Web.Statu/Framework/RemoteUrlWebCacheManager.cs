using HlidacStatu.Util.Cache;
using System;
using System.Linq;

namespace HlidacStatu.Web.Framework
{

    public static class RemoteUrlFromWebCache
    {
        static RemoteUrlFromWebCache()
        {
        }

        public static volatile FileCacheManager Manager
                = FileCacheManager.GetSafeInstance("RemoteUrlFromWebCache",
                    urlfn => GetBinaryDataFromUrl(urlfn),
                    TimeSpan.FromHours(24*5));

        private static byte[] GetBinaryDataFromUrl(KeyAndId ki)
        {
            byte[] data = null;

            try
            {
                using (Devmasters.Net.HttpClient.URLContent net = new Devmasters.Net.HttpClient.URLContent(ki.ValueForData))
                {
                    net.Timeout = 7000;
                    net.IgnoreHttpErrors = true;
                    data = net.GetBinary().Binary;
                }
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("Manager Save", e);
            }
            if (data == null || data.Length == 0)
                return System.IO.File.ReadAllBytes(HlidacStatu.Lib.Init.WebAppRoot + @"content\icons\largetile.png");
            else
                return data;

        }


        public static byte[] GetScreenshot(string url, string cacheName = null, bool refreshCache = false)
        {
            string[] webShotServiceUrls = Devmasters.Config.GetWebConfigValue("WebShot.Service.Url")
            ?.Split(';')
            ?.Where(m => !string.IsNullOrEmpty(m))
            ?.ToArray();

            if (webShotServiceUrls == null || webShotServiceUrls?.Length == null || webShotServiceUrls?.Length == 0)
                webShotServiceUrls = new[] { "http://127.0.0.1:9090" };

            var webShotServiceUrl = webShotServiceUrls[HlidacStatu.Util.Consts.Rnd.Next(webShotServiceUrls.Length)];

            //string scr = webShotServiceUrl + "/png?ratio=" + rat + "&url=" + System.Net.WebUtility.UrlEncode(url);
            string scr = webShotServiceUrl + "/screenshot?vp_width=1920&vp_height=1080&url="
                + System.Net.WebUtility.UrlEncode(url);
            return GetBinary(scr, cacheName, refreshCache);
        }

        public static byte[] GetBinary(string url, string cacheName = null, bool refreshCache = false)
        {
            try
            {


                if (refreshCache)
                {
                    Framework.RemoteUrlFromWebCache.Manager.Delete(new Util.Cache.KeyAndId()
                    {
                        ValueForData = url,
                        CacheNameOnDisk = cacheName
                    });
                }

                byte[] data = Framework.RemoteUrlFromWebCache.Manager.Get(new Util.Cache.KeyAndId()
                {
                    ValueForData = url,
                    CacheNameOnDisk = cacheName
                });

                return data;
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("WebShot GetData error", e);
                return null;
            }
        }

    }
}