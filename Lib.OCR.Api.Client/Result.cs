using System;
using System.Collections.Generic;
using System.Linq;

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

        Document _mergedDocument = null;
        public Document MergedDocuments()
        {
            if (_mergedDocument == null)
            {
                Document doc = new Document();

                if (this.Documents.Count == 0)
                {
                    doc.Text = "";
                }
                else if (this.Documents.Count == 1)
                {
                    doc = this.Documents[0];
                }
                else if (this.Documents.Count > 1)
                {
                    doc.Text = this.Documents
                        .Select(m => $"--------- soubor : {m.Filename} ---------" + m.Text)
                        .Aggregate((f, s) => f + "\n\n\n\n\n\n" + s);
                    doc.Pages = this.Documents.Sum(m => m.Pages);
                    doc.RemainsInSec = this.Documents.Sum(m => m.RemainsInSec);
                    doc.UsedOCR = this.Documents.Any(m => m.UsedOCR);
                    doc.UsedTool = this.Documents.Select(m => m.UsedTool).Aggregate((f, s) => f + "|" + s);
                    doc.Confidence = this.Documents.Average(m => m.Confidence);
                }
                _mergedDocument = doc;
            }
            return _mergedDocument;
        }

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
