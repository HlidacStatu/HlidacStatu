using System;
using System.Collections.Generic;

namespace HlidacStatu.Lib.OCR.Api
{
    public class OCRTask
    {
        public string TaskId { get; set; }
        public int Priority { get; set; }
        public int Intensity { get; set; }
        public string OrigFilename { get; set; }
        public string localTempFile { get; set; } = null;
    }
}
