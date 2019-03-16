using System;
using System.Net;

namespace HlidacStatu.Lib.OCR.Api
{
    public partial class Client
    {


        public class WebOcr : System.Net.WebClient
        {

            /// <summary>
            /// Time in milliseconds
            /// </summary>
            public int Timeout { get; set; }

            public WebOcr() : this(120000) { }

            public WebOcr(int timeout)
            {
                this.Timeout = timeout;
                this.Encoding = System.Text.Encoding.UTF8;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {

                var request = base.GetWebRequest(address);
                if (request != null)
                {
                    ((HttpWebRequest)request).KeepAlive = false;
                    ((HttpWebRequest)request).ReadWriteTimeout = Timeout;
                    request.Timeout = this.Timeout;
                }
                return request;
            }
        }
        

    }
}
