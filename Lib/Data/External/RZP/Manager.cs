using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External
{

    public partial class RZP
    {
        public class Manager
        {

            class req : RequestLimiter<string, string>
            {
                public req()
                    : base(1000)
                { }

                protected override string DoRequest(string requestXml)
                {
                    string data = @"------WebKitFormBoundaryMNWX8XhCA6tmVZy7
Content-Disposition: form-data; name=""VSS_SERV""

ZVWSBJXML
------WebKitFormBoundaryMNWX8XhCA6tmVZy7
Content-Disposition: form-data; name=""filename""; filename=""req-query.xml""
Content-Type: text/xml

{0}

------WebKitFormBoundaryMNWX8XhCA6tmVZy7--
";

                    int tries = 0;

                    string xml = requestXml;

                    byte[] postData = Encoding.UTF8.GetBytes(string.Format(data, xml));


                    start:
                    try
                    {
                        tries++;

                        System.Net.ServicePointManager.Expect100Continue = false;
                        System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)WebRequest.Create("http://www.rzp.cz/cgi-bin/aps_cacheWEB.sh");
                        //request.Proxy = new WebProxy("10.211.55.2", 8888);
                        request.Method = "POST";
                        request.Headers.Clear();
                        request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                        request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us");
                        request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                        request.Headers.Add(HttpRequestHeader.CacheControl, "max-age:0");
                        request.ContentType = "multipart/form-data; boundary=----WebKitFormBoundaryMNWX8XhCA6tmVZy7";
                        request.Headers.Add("Upgrade-Insecure-Requests", "1");

                        //request.Headers.Add(HttpRequestHeader.KeepAlive, "300");
                        request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.7) Gecko/20091221 Firefox/3.5.7";
                        request.KeepAlive = true;
                        //request.Referer = "http://www.rzp.cz/cgi-bin/aps_cacheWEB.sh?VSS_SERV=ZVWSBJFND";
                        request.ContentLength = postData.Length;


                        Stream requestStream = request.GetRequestStream();
                        requestStream.Write(postData, 0, postData.Length);
                        requestStream.Flush();
                        requestStream.Close();


                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                        using (MemoryStream binaryHttpData = new MemoryStream())
                        {
                            using (BinaryReader binaryReaderFromHttp = new BinaryReader(response.GetResponseStream()))
                            {

                                StreamReader fromBinaryDataToTextReader = null;
                                byte[] read = new byte[2048];
                                int count = binaryReaderFromHttp.Read(read, 0, read.Length);
                                while (count > 0)
                                {
                                    binaryHttpData.Write(read, 0, count);
                                    count = binaryReaderFromHttp.Read(read, 0, read.Length);
                                }

                                if (binaryReaderFromHttp != null)
                                    binaryReaderFromHttp.Close();
                                if (fromBinaryDataToTextReader != null)
                                    fromBinaryDataToTextReader.Close();
                                if (binaryHttpData != null)
                                    binaryHttpData.Close();

                                read = null;
                                return Encoding.GetEncoding("iso-8859-2").GetString(binaryHttpData.ToArray());
                            }
                        }


                    }
                    catch (WebException e)
                    {
                        if (e.Message.Contains("(403) Forbidden") && tries <= 3)
                        {
                            System.Threading.Thread.Sleep(5000);
                            goto start;
                        }
                        HlidacStatu.Util.Consts.Logger.Error("RZP error", e);
                        return null;
                    }
                    catch (Exception e)
                    {
                        HlidacStatu.Util.Consts.Logger.Error("RZP error", e);
                        return null;
                    }

                }
            }


            static req CallReq = new req();


            public static RZP.TVerejnyWebOdpoved GetResponse(string xml)
            {
                if (string.IsNullOrWhiteSpace(xml))
                    return null;

                System.Xml.Serialization.XmlSerializer xmls = new System.Xml.Serialization.XmlSerializer(typeof(TVerejnyWebOdpoved));
                using (var xmlread = new StringReader(xml))
                {
                    return xmls.Deserialize(new StringReader(xml)) as RZP.TVerejnyWebOdpoved;
                }
            }

            public static RZP.TVerejnyWebOdpoved RawSearchIco(string ico)
            {
                string searchReqTemplate = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<VerejnyWebDotaz
    xmlns=""urn:cz:isvs:rzp:schemas:VerejnaCast:v1""
    version=""2.6"">
  <Kriteria>
    <IdentifikacniCislo>{0}</IdentifikacniCislo>
    <PlatnostZaznamu>0</PlatnostZaznamu>
  </Kriteria>
</VerejnyWebDotaz>";
                try
                {
                    return GetResponse(CallReq.Request(string.Format(searchReqTemplate, ico)));

                }
                catch (Exception e)
                {

                    HlidacStatu.Util.Consts.Logger.Error("RZP RawSearchIco request error. Ico:" + ico, e);
                    return null;
                }

            }

            public static RZP.TVerejnyWebOdpoved RawSearchDetail(string podnikatelId)
            {
                string searchReqTemplate = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<VerejnyWebDotaz
    xmlns=""urn:cz:isvs:rzp:schemas:VerejnaCast:v1""
    version=""2.6"">
    <PodnikatelID>{0}</PodnikatelID>
    <Historie>0</Historie>
</VerejnyWebDotaz>";

                try
                {
                    return GetResponse(CallReq.Request(string.Format(searchReqTemplate, podnikatelId)));
                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Error("RZP RawSearchDetail request error. podnikatelId:" + podnikatelId, e);
                    return null;

                }
            }



        }
    }
}