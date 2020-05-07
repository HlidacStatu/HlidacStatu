using System;
using System.Net;

namespace Newton.SpeechToText.Cloud
{
    public class AccessToken
    {
        private string _accessToken = null;
        public string accessToken
        {
            get
            {
                Validate();
                return _accessToken;
            }
            private set { _accessToken = value; }
        }


        public long expiresAt { get; private set; }
        public DateTime Expiration() { return Util.FromEpochTimeToUTC(this.expiresAt); }

        private string username;
        private string password;

        public void Validate()
        {
            if ((this.Expiration() - DateTime.Now).TotalSeconds < 60)

            {
                var at = call(this.username, this.password, this.Audience);
                this.accessToken = at.accessToken;
                this.expiresAt = at.expiresAt;
            }
        }

        private string _audience = "https://newton.nanogrid.cloud/";
        public string Audience
        {
            get { return _audience; }
            private set
            {
                var s = value;
                if (Uri.TryCreate(s, UriKind.Absolute, out Uri uri))
                {
                    s = uri.AbsoluteUri;
                    if (!s.EndsWith("/"))
                        s += "/";
                    _audience = s;
                }
                else
                    throw new ArgumentException("Invalid URL");
            }

        }

        private static AccessToken call(string username, string password, string audience)
        {
            using (RESTCall wc = new RESTCall())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(
                                    new
                                    {
                                        username = username,
                                        password = password,
                                    }
                    );

                var accessToken = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessToken>(wc.UploadString(audience + "login/access-token", data));
                accessToken.Audience = audience;
                accessToken.username = username;
                accessToken.password = password;
                return accessToken;
            }

        }

        public static AccessToken Login(string username, string password, string audience = "https://newton.nanogrid.cloud/")
        {
            return call(username, password, audience);
        }
    }
}
