using HlidacStatu.Lib.XSD;
using Nest;

namespace HlidacStatu.Lib.Data
{

    public partial class Smlouva
    {
        public class Subjekt
        {
            public Subjekt() { }

            public Subjekt(tSmlouvaSmluvniStrana s)
            {
                this.adresa = s.adresa;
                this.datovaSchranka = s.datovaSchranka;
                this.ico = s.ico;
                this.nazev = s.nazev;
                this.utvar = s.utvar;
            }

            public Subjekt(tSmlouvaSubjekt s)
            {
                this.adresa = s.adresa;
                this.datovaSchranka = s.datovaSchranka;
                this.ico = s.ico;
                this.nazev = s.nazev;
                this.utvar = s.utvar;
            }

            public string adresa { get; set; }

            [Keyword()]
            public string datovaSchranka { get; set; }
            [Keyword()]
            public string ico { get; set; }

            [Keyword()]
            public string nazev { get; set; }
            [Keyword()]
            public string utvar { get; set; }
        }
    }
}
