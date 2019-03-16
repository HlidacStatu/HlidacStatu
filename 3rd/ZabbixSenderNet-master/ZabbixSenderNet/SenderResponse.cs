using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ysq.Zabbix
{
    public class SenderResponse
    {
        [JsonProperty(PropertyName = "response")]
        public string Response { get; set; }

        [JsonProperty(PropertyName = "info")]
        public string Info { get; set; }

        public bool Success { get { return Response == "success"; } }
    }
}
