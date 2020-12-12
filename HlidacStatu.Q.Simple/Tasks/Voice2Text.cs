using System;
using System.Collections.Generic;
using System.Text;

namespace HlidacStatu.Q.Simple.Tasks
{
    public class Voice2Text
    {
        public const string QName = "voice2text";
        public string dataset { get; set; }
        public string itemid { get; set; }
        public ulong internaltaskid { get; set; }
    }
}
