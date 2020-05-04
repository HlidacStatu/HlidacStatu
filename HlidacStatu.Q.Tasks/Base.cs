using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Q.Tasks
{
    public abstract class Base
    {
        public string TaskID { get; set; } = Guid.NewGuid().ToString("D");
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
