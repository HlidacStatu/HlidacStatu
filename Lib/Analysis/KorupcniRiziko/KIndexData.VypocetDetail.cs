﻿using System.Linq;

using Devmasters.Enums;


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

                [Nest.Object(Ignore = true)]
                public string VelicinaLongName { get => ((KIndexParts)this.Velicina).ToNiceDisplayName(); }

                [Nest.Object(Ignore = true)]
                public KIndexParts VelicinaPart { get => (KIndexParts)this.Velicina; }

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
                decimal vysledek = 0;
                if (Radky != null)
                    vysledek = Radky
                       .Select(m => m.Hodnota * m.Koeficient)
                       .Sum();

                if (vysledek < 0)
                    vysledek = 0;

                return vysledek;
            }



            public string ToDebugString()
            {
                string l = "";
                foreach (var r in this.Radky.Where(m => m.VelicinaPart != KIndexData.KIndexParts.PercSmlouvyPod50kBonus))
                {
                    l += r.VelicinaLongName;
                    l += "\t" + r.Hodnota.ToString("N4");
                    l += "\n";
                }
                l += "bonus\t" + (this.Radky.FirstOrDefault(m => m.VelicinaPart == KIndexData.KIndexParts.PercSmlouvyPod50kBonus)?.Hodnota ?? 0m).ToString("N2");
                return l;
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
