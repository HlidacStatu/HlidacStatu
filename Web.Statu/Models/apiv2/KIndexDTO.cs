using HlidacStatu.Lib.Analysis.KorupcniRiziko;
using System;
using System.Collections.Generic;

namespace HlidacStatu.Web.Models.Apiv2
{
    public class KIndexDTO
    {
        public string Ico { get; set; }
        public string Name { get; set; }
        public DateTime LastChange { get; set; }
        public IEnumerable<KIndexYearsDTO> AnnualCalculations { get; set; }
    }

    public class KIndexYearsDTO
    {
        public decimal KIndex { get; set; }
        public KIndexData.VypocetDetail Calculation { get; set; }

    }
}
