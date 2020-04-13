using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.Dotace
{
    public class Cerpani
    {
        [Nest.Number]
        public decimal? CastkaSpotrebovana { get; set; }
        [Nest.Number]
        public int? Rok { get; set; }
    }
}
