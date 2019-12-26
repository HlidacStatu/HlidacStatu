using Nancy;
using System;
using System.IO;

namespace HlidacStatu.Service.WebShots
{
    public class RootRoutes : NancyModule
    {
        public RootRoutes()
        {
            Get["/"] = parameters =>
            {
                return "OK";
            };

            Get["/time"] = parameters =>
            {
                return DateTime.Now.ToString("o") ;
            };

            Get["/png"] = parameters =>
            {
                string url = this.Request.Query["url"] ?? "";
                string ratio = this.Request.Query["ratio"] ?? "16x9";
                int width = 1920;
                int height = 1080;

                if (ratio == "1x1")
                {
                    width = 1000;
                    height = 1000;
                }
                Program.logger.Debug("png?url=" + url);

                if (string.IsNullOrEmpty(url))
                    return new ByteArrayResponse(new byte[] { });

                var data = HlidacStatu.Util.WebShot.Url2PngMulti.Screenshot(url, TimeSpan.FromSeconds(5), width, height);
                //if (data == null)
                //    HlidacStatu.Util.WebShot.Url2PngSingle_Chrome.Screenshot(url);

                Program.logger.Debug($"return {data?.Length ?? 0} bytes |  png?url={url}");

                return new ByteArrayResponse(data ?? new byte[] { }, "image/png");
                
            };
        }


    }

    public class ByteArrayResponse : Response
    {
        /// <summary>
        /// Byte array response
        /// </summary>
        /// <param name="body">Byte array to be the body of the response</param>
        /// <param name="contentType">Content type to use</param>
        public ByteArrayResponse(byte[] body, string contentType = null)
        {
            this.ContentType = contentType ?? "application/octet-stream";

            this.Contents = stream =>
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(body);
                }
            };
        }
    }
}

