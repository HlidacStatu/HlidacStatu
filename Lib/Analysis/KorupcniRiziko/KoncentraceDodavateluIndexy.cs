using System;
using System.Linq;
using System.Collections.Generic;
using HlidacStatu.Lib.Data;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class KoncentraceDodavateluIndexy
    {
        public class Souhrn
        {
            public string Ico { get; set; }
            public int PocetSmluv { get; set; }
            public decimal HodnotaSmluv { get; set; }
            public string Poznamka { get; set; }
        }

        [Nest.Date]
        public DateTime LastUpdated { get; set; }

        public decimal Herfindahl_Hirschman_Index { get; set; }
        public decimal Herfindahl_Hirschman_Normalized { get; set; }
        public decimal Herfindahl_Hirschman_Modified { get; set; }
        public decimal Comprehensive_Industrial_Concentration_Index { get; set; }
        public decimal Hall_Tideman_Index { get; set; }
        public decimal Kwoka_Dominance_Index { get; set; }

        public decimal PrumernaHodnotaSmluvProVypocet { get; set; } //pouze tech s hodnotou vyssi nez 0 Kc
        public decimal HodnotaSmluvProVypocet { get; set; } //pouze tech s hodnotou vyssi nez 0 Kc
        public int PocetSmluvProVypocet { get; set; } 
        public int PocetSmluvBezCenyProVypocet { get; set; }
        [Nest.Keyword]
        public string Query { get; set; }

        [Nest.Keyword]
        public string Popis { get; set; }

        public Souhrn[] Dodavatele { get; set; }
        public Souhrn[] TopDodavatele()
        {
            if (Dodavatele.Count() == 1)
                return Dodavatele;
            Dictionary<int, decimal> schody = new Dictionary<int, decimal>();
            Souhrn[] sortedDodav = Dodavatele.OrderByDescending(o => o.HodnotaSmluv).ToArray();

            decimal avgHodnota = Dodavatele.Average(m => m.HodnotaSmluv);
            int posOver60Perc = -1;
            decimal tmpSum = 0;
            for (int i = 0; i < sortedDodav.Count()-1; i++)
            {
                schody.Add(i, sortedDodav[i].HodnotaSmluv - sortedDodav[i + 1].HodnotaSmluv);
                tmpSum = tmpSum + sortedDodav[i].HodnotaSmluv;
                if (i==-1 && tmpSum >= this.HodnotaSmluvProVypocet * 0.65m)
                    posOver60Perc = i;
            }
            decimal avgDiff = schody.Average(m => m.Value);
            int lastPosOfDominant = posOver60Perc;

            //najdi dalsi po posOver60Perc, ktery ma schod vetsi nez avgDiff
            for (int i = posOver60Perc+1; i < schody.Count(); i++)
            {
                if (schody[i] > avgDiff)
                {
                    lastPosOfDominant = i;
                    break;
                }
            }

            return sortedDodav.Take(lastPosOfDominant + 1).ToArray();
        }
    }

}
