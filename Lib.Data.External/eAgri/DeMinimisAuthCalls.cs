using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HlidacStatu.Lib.Data.External.eAgri
{
  public static class DeMinimisAuthCalls
    {
        static string enveloReq = @"<SOAP:Envelope xmlns:SOAP=""http://schemas.xmlsoap.org/soap/envelope/"">
  <SOAP:Header>
    <vOKO-wss:Token type=""A01"" xmlns:vOKO-wss=""http://www.pds.eu/vOKO/v0200/wss"">#TOKEN#</vOKO-wss:Token>
  </SOAP:Header>
  #BODY#
</SOAP:Envelope>";

        //dn=""cn=PDS_RDM_All,cn=PDS,cn=Users,o=mze,c=cz"" certificateSN=""#NEDEF"" addressAD=""default"" certificateOwner=""#NEDEF""
        static string searchSubReq = @"<SOAP:Body>
    <vOKO:Request vOKOid=""RDM_SUS01A"" xmlns:vOKO=""http://www.pds.eu/vOKO/v0200"">
      <vOKO:UIDkey userID=""99michalblaha"" dn=""cn=99michalblaha,cn=Farmar,cn=Users,o=mze,c=cz"" certificateSN=""#NEDEF"" addressAD=""default"" certificateOwner=""#NEDEF"" />
      <vOKO:TimeStamp type=""base"">#TIME#+01:00</vOKO:TimeStamp>
      <vOKO:AppInfo>
        <vOKO:AppModule id=""hlidacstatu"" version=""1"" />
      </vOKO:AppInfo>
      <vOKO:RequestHeader>
        <vOKO:RequestID>STRNAD-test/2010/RDM_SUS01A</vOKO:RequestID>
        <vOKO:Subject subjectID=""1011816014"" />
        <vOKO:RequestType code="""">
        </vOKO:RequestType>
      </vOKO:RequestHeader>
      <vOKO:RequestContent>
        <rdm:Request xmlns:rdm=""http://www.pds.eu/RdmServices/RDM_SUS01A"">
     			<rdm:dotaz>
						<rdm:ic>#IC#</rdm:ic>
					</rdm:dotaz>
        </rdm:Request>
      </vOKO:RequestContent>
    </vOKO:Request>
  </SOAP:Body>
";
        public static void SearchSubj(string ico)
        {

            string url = "https://eagri.cz/ssl/nosso-app/EPO/WS/v2Online/vOKOsrv.ashx";
            string req = enveloReq.Replace("#BODY#",
                    searchSubReq.Replace("#TIME#", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")).Replace("#IC#", ico)
                    );
            string token = _sign("2ahKpoxgvuQRgFVsPUCX", req);
            req = req.Replace("#TOKEN#", token);

            Soap net = new Soap();
            var resp = net.UploadString(url, req);
        }


        class Soap : System.Net.WebClient
        {
            public int Timeout { get; set; }
            public Soap() : this(60 * 1000) { } //1min

            public Soap(int timeout)
            {
                this.Timeout = timeout;
                this.Encoding = System.Text.Encoding.UTF8;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {

                var request = base.GetWebRequest(address);
                request.ContentType = "text/xml";
                if (request != null)
                {
                    ((HttpWebRequest)request).KeepAlive = false;
                    ((HttpWebRequest)request).ReadWriteTimeout = Timeout;
                    request.Timeout = this.Timeout;
                }
                return request;
            }

        }

        /// <summary>
        /// Vytvoří autentitační token pro volání služby
        /// </summary>
        /// <param name="key">Přístupový klíč</param>
        /// <param name="request">Request služby včetně elementu <SOAP:Envelope></SOAP:Envelope></param>
        /// <returns></returns>
        private static string _sign(string key, string request)
        {
            //z request se vytvoří XmlDocument
            XmlDocument xmlRequest = new XmlDocument();
            xmlRequest.LoadXml(request);

            //spočítá se SHA1 hash přístupového klíče
            SHA1 sha1 = SHA1.Create();
            byte[] hashKey = sha1.ComputeHash(Encoding.UTF8.GetBytes(key));

            //z requestu se vytáhne element body včetně krajních XML značek - outer xml
            XmlDocument xmlBody = new XmlDocument();
            xmlBody.LoadXml(xmlRequest.GetElementsByTagName("Body", xmlRequest.DocumentElement.NamespaceURI)[0].OuterXml);

            //provede se kanonizace body
            XmlDsigC14NTransform tr = new XmlDsigC14NTransform();
            tr.LoadInput(xmlBody);
            MemoryStream transOutput = (MemoryStream)tr.GetOutput();
            byte[] bodyBytes = transOutput.ToArray(); ;

            //ke kanonizovanému body se bytově připojí SHA1 hash hesla
            byte[] combined = new byte[bodyBytes.Length + hashKey.Length];
            System.Buffer.BlockCopy(bodyBytes, 0, combined, 0, bodyBytes.Length);
            System.Buffer.BlockCopy(hashKey, 0, combined, bodyBytes.Length, hashKey.Length);

            //výsledek se zahashuje SHA512
            SHA512 sha512 = SHA512.Create();
            byte[] hashResult = sha512.ComputeHash(combined);
            //převede se do base64
            string result = Convert.ToBase64String(hashResult);

            return result;
        }

    }
}
