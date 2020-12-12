using System;
using System.Collections.Generic;
using System.Text;

namespace HlidacStatu.Q.Simple.Tasks
{
    public class VoiceDownloadSave
    {
        public const string QName = "VoiceDownloadSave";
        public string dataset { get; set; }
        public string itemid { get; set; }
        public string uri { get; set; }
        public string serverAddInfo { get; set; }
    }
}
