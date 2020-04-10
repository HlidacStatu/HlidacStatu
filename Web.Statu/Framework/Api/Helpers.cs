using System.IO;

namespace HlidacStatu.Web.Framework.Api
{
    public static class Helpers
    {
        public static string ReadRequestBody(Stream req)
        {
            string ret = "";
            using (var stream = new StreamReader(req))
            {
                stream.BaseStream.Position = 0;
                ret = stream.ReadToEnd();
            }
            
            return ret;
        }


    }
}