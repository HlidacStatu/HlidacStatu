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
        public string IdRozhodnuti { get; set; }
        [Nest.Number]
        public float CastkaPozadovana { get; set; }
        [Nest.Number]
        public float CastkaRozhodnuta { get; set; }
        [Nest.Number]
        public int? RokRozhodnuti { get; set; }
        [Nest.Boolean]
        public bool InvesticeIndikator { get; set; }
        [Nest.Boolean]
        public bool NavratnostIndikator { get; set; }
        [Nest.Boolean]
        public bool RefundaceIndikator { get; set; }
        [Nest.Boolean]
        public bool TuzemskyZdroj { get; set; }
        [Nest.Keyword]
        public string FinancniZdrojKod { get; set; }
        [Nest.Text]
        public string FinancniZdrojNazev { get; set; }
        [Nest.Keyword]
        public string DotacePoskytovatelKod { get; set; }
        [Nest.Text]
        public string DotacePoskytovatelNazev { get; set; }
        [Nest.Keyword]
        public string PoskytovatelIco { get; set; }
    }
}
