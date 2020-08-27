using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class SubjectWithKIndex : Lib.Data.Firma.Zatrideni.Item
    {
        public decimal KIndex { get; set; }


    }


    public class SubjectWithKIndexTrend : Lib.Data.Firma.Zatrideni.Item
    {
        public decimal KIndex { get; set; }

        public Dictionary<int, decimal> Roky { get; set; }
    }

}
