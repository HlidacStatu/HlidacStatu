using System;
using System.Collections.Generic;

namespace HlidacStatu.Lib.OCR.Api
{
    public class Result
    {
        public enum ResultStatus
        {
            Valid = 1,
            Invalid = 0,
            InQueueWithCallback = 3,
            Unknown = -1
        }
        public string Id { get; set; }

        public class Document
        {
            private static MimeSharp.Mime mime = new MimeSharp.Mime();
            public string ContentType { get; set; }

            private string _filename = default(string);
            public string Filename {
                get { return _filename; }
                set {
                    _filename = value ?? "neznamy.bin";
                    this.ContentType =  mime.Lookup(_filename );
                }
            }

            public string Text { get; set; }
            public float Confidence { get; set; }
            public bool UsedOCR { get; set; } = false;
            public int Pages { get; set; }

            public decimal RemainsInSec { get; set; }

            public string UsedTool { get; set; }
            public string Server { get; set; }

        }

        public List<Document> Documents { get; set; } = new List<Document>();

        public string Server { get; set; }
        public DateTime Started { get; set; } = DateTime.Now;
        public DateTime Ends { get; set; }
 
        public ResultStatus IsValid { get; set; } = ResultStatus.Unknown;
        public string Error { get; set; } = null;


        public TimeSpan Remains()
        {
            return Ends - Started;
        }

        public void FinishOK()
        {
            this.Ends = DateTime.Now;
            this.IsValid = Result.ResultStatus.Valid;
        }

        public void SetException(Exception e)
        {
            this.Error = e.Message;
            this.Ends = DateTime.Now;
            if (e.InnerException != null)
                this.Error = this.Error + "\n" + e.InnerException.Message;
            this.IsValid = Result.ResultStatus.Invalid;
        }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
