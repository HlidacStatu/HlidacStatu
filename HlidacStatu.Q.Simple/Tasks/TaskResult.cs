using System;
using System.Collections.Generic;
using System.Text;

namespace HlidacStatu.Q.Simple.Tasks
{
    public class TaskResult<T>
    {
        public T Payload { get; set; }
        public string  FromIP { get; set; }
        public string User { get; set; }
        public string Result { get; set; }
        public DateTime Created { get; set; }
    }
}
