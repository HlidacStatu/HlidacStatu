using System;

namespace HlidacStatu.Q.Messages
{
    public abstract class BaseTask
    {
        public string TaskID { get; set; } = Guid.NewGuid().ToString("D");
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
