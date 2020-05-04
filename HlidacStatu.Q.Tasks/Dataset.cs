using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Q.Tasks
{
    public class DataSet : BaseTask
    {
        public string DatasetId { get; set; }
        public string ItemId { get; set; }
        public bool Force { get; set; }
    }
}
