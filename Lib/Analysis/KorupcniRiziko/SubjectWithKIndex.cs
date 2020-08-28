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

    public class SubjectWithKIndexAnnualData : Lib.Data.Firma.Zatrideni.Item
    {
        public SubjectWithKIndexAnnualData() { }
        
        public SubjectWithKIndexAnnualData(Data.Firma.Zatrideni.Item item)
        {
            Group = item.Group;
            Ico = item.Ico;
            Jmeno = item.Jmeno;
            Kraj = item.Kraj;
            KrajId = item.KrajId;
        }

        public void PopulateWithAnnualData(int year)
        {
            if (string.IsNullOrWhiteSpace(Ico))
                throw new Exception("Ico is missing");

            year = Consts.FixKindexYear(year);
            var kindex = KIndex.Get(Ico);

            if (kindex != null)
            {
                AnnualData = kindex.ForYear(year);
            }
            else
                AnnualData = KIndexData.Annual.Empty(year);
        }
            
        public KIndexData.Annual AnnualData { get; set; }

    }

}
