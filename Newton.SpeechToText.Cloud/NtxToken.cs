using System;

namespace Newton.SpeechToText.Cloud
{
    public class NtxToken
    {
        private string _ntxToken = null;
        public string ntxToken
        {
            get
            {
                Validate();
                return _ntxToken;
            }
            private set { _ntxToken = value; }
        }
        public long expiresAt { get; private set; }
        public DateTime Expiration() { return Util.FromEpochTimeToUTC(this.expiresAt); }

        public string TaskId { get; private set; }
        public string TaskLabel { get; private set; }

        public AccessToken accessToken { get; private set; }

        public void Validate()
        {
            if ((this.Expiration() - DateTime.Now).TotalSeconds < 60)
            {
                var at = call(this.accessToken, this.TaskId, this.TaskLabel);
                this.accessToken = at.accessToken;
                this.expiresAt = at.expiresAt;
            }
        }

        public static NtxToken call(AccessToken accessToken, string taskId, string taskLabel)
        {
            using (RESTCall wc = new RESTCall())
            {
                wc.Headers.Add("ntx-token", accessToken.accessToken);
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(
                        new
                        {
                            id = taskId, //"ntx.v2t.engine.EngineService/cz/t-broadcast/v2t",
                            label = taskLabel //"vad+v2t+ppc+pnc",
                        }
                    );

                var ntxToken = Newtonsoft.Json.JsonConvert.DeserializeObject<NtxToken>(wc.UploadString(accessToken.Audience + "store/ntx-token", data));
                ntxToken.accessToken = accessToken;
                ntxToken.TaskId = taskId;
                ntxToken.TaskLabel = taskLabel;
                return ntxToken;
            }
        }

        public static NtxToken GetToken(AccessToken accessToken, string taskId, string taskLabel)
        {
            return call(accessToken, taskId, taskLabel);
        }
    }
}
