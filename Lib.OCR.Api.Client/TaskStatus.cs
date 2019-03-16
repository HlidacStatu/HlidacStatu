using System;
using System.Collections.Generic;

namespace HlidacStatu.Lib.OCR.Api
{
    public class TaskStatus
    {
        public enum CurrentStatus
        {
            NotFound = -1,
            Waiting = 0,
            InProcess = 1,
            Done = 2
        }

        public CurrentStatus Status { get; set; }
        public string TaskId { get; set; }
        public decimal Progress { get; set; }
        public DateTime Created { get; set; }
        public DateTime? StartOfProcessing { get; set; }
        public int NumOfPreviousTasks { get; set; }
        public string Result { get; set; } = null;
    }
}
