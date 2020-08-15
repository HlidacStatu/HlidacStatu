using System.Linq;
using Devmasters.Core;


namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public partial class KIndexData
    {
        public class VypocetDetail
        {
            public class Radek
            {
                public Radek(KIndexParts velicina, decimal hodnota, decimal koef)
                {
                    this.Velicina = (int)velicina;
                    this.VelicinaName = velicina.ToString();
                    this.Hodnota = hodnota;
                    this.Koeficient = koef;
                }

                [Nest.Object(Ignore =true)]
                public string VelicinaLongName { get => ((KIndexParts)this.Velicina).ToNiceDisplayName(); }

                public int Velicina { get; set; }
                [Nest.Keyword]
                public string VelicinaName { get; set; }
                public decimal Hodnota { get; set; }
                public decimal Koeficient { get; set; }
                
            }
            public VypocetOboroveKoncentrace OboroveKoncentrace { get; set; }
            public Radek[] Radky { get; set; }
            public System.DateTime LastUpdated { get; set; }

            public decimal Vypocet()
            {
                if (Radky == null)
                    return Consts.MinSmluvPerYearKIndexValue;
                var vysledek= Radky
                    .Select(m => m.Hodnota * m.Koeficient)
                    .Sum();

                if (vysledek < 0)
                    vysledek = 0;

                return vysledek;
            }



        }

        public class VypocetOboroveKoncentrace
        {
            public class RadekObor
            {
                [Nest.Keyword]
                public string Obor { get; set; }
                public decimal Hodnota { get; set; }
                public decimal Vaha { get; set; }
                public decimal PodilSmluvBezCeny { get; set; }
                public decimal CelkovaHodnotaSmluv { get; set; }
                public decimal PocetSmluvCelkem { get; set; }

            }
            public RadekObor[] Radky { get; set; }
            public decimal PrumernaCenaSmluv { get; set; }
        }
    }
}
