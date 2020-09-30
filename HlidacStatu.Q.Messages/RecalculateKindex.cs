using System;

namespace HlidacStatu.Q.Messages
{
    public class RecalculateKindex
    {
        public string Ico { get; set; }
        public string Comment { get; set; }
        public string User { get; set; }
        public DateTime Created { get; set; }
    }
}
