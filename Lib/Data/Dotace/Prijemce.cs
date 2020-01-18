using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.Dotace
{
    public class Prijemce
    {
        [Nest.Keyword]
        public string Ico { get; set; }
        [Nest.Text]
        public string ObchodniJmeno { get; set; }
        [Nest.Text]
        public string JmenoPrijemce { get; set; }
        [Nest.Number]
        public int? RokNarozeni { get; set; }
        
        [Nest.Keyword]
        public string PrijemceObecNazev { get; set; }
        [Nest.Keyword]
        public string PrijemceOkresNazev { get; set; }
        [Nest.Keyword]
        public string PSC { get; set; }
    }
}
