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
        public float RozhodnutiCastkaPozadovana { get; set; }
        [Nest.Number]
        public float RozhodnutiCastkaRozhodnuta { get; set; }
        [Nest.Number]
        public int? RozhodnutiRokRozhodnuti { get; set; }
        [Nest.Boolean]
        public bool RozhodnutiInvesticeIndikator { get; set; }
        [Nest.Boolean]
        public bool RozhodnutiNavratnostIndikator { get; set; }
        [Nest.Boolean]
        public bool RozhodnutiRefundaceIndikator { get; set; }
        [Nest.Boolean]
        public bool RozhodnutiTuzemskyZdroj { get; set; }
        [Nest.Keyword]
        public string RozhodnutiFinancniZdrojKod { get; set; }
        [Nest.Text]
        public string RozhodnutiFinancniZdrojNazev { get; set; }
        [Nest.Keyword]
        public string RozhodnutiDotacePoskytovatelKod { get; set; }
        [Nest.Text]
        public string RozhodnutiDotacePoskytovatelNazev { get; set; }
    }
}
