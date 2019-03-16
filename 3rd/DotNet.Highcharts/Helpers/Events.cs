using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNet.Highcharts.Attributes;

namespace DotNet.Highcharts.Helpers
{
    public class Events
    {
        [JsonFormatter("{0}")]
        public string Click { get; set; }

        [JsonFormatter("{0}")]
        public string Mouseover { get; set; }

        [JsonFormatter("{0}")]
        public string Mouseout { get; set; }

        [JsonFormatter("{0}")]
        public string Mousemove { get; set; }
    }
}
