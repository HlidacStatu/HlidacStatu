using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.Dotace
{
    public class Rozhodnuti
    {
        [Nest.Keyword]
        public string Id { get; set; }
        [Nest.Number]
        public float CastkaPozadovana { get; set; }
        [Nest.Number]
        public float CastkaRozhodnuta { get; set; }
        [Nest.Boolean]
        public bool JePujcka { get; set; }
        [Nest.Keyword]
        public string IcoPoskytovatele { get; set; }
        [Nest.Text]
        public string NazevPoskytovatele { get; set; }
        [Nest.Date]
        public DateTime? Datum { get; set; }
        [Nest.Keyword]
        public string ZdrojFinanci { get; set; }
    }
}
