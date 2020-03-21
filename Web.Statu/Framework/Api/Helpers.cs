using System.IO;
using System.Web;

namespace HlidacStatu.Web.Framework.Api
{
    public static class Helpers
    {
        public static string ReadRequestBody(HttpRequestBase req)
        {
            string ret = "";
            using (var stream = new MemoryStream())
            {
                req.InputStream.Seek(0, SeekOrigin.Begin);
                req.InputStream.CopyTo(stream);
                ret = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }
            return ret;
        }


    }
}