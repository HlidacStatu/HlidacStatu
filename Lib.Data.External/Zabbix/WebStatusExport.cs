using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External.Zabbix
{
    public class WebStatusExport
    {
        public class SslData
        {
            public string Grade { get; set; }
            public DateTime? LatestCheck { get; set; }
            public string Copyright { get; set; }
            public string FullReport { get; set; }
        }
        public ZabHostAvailability Availability { get; set; }
        public SslData SSL { get; set; }
        public string DetailUrl { get; set; }
    }
}
