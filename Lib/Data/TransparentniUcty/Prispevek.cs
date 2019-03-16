using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;


namespace HlidacStatu.Lib.Data.TransparentniUcty
{
    public abstract class Prispevek
    {
        public class Problem
        {
            [Nest.Keyword]
            public string Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Law { get; set; }

        }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool Valid { get; set; }
        public Problem[] Problemy { get; set; }

        public bool PodleVolebnihoZakona { get; set; }

        [Nest.Date]
        public DateTime Vlozeno { get; set; }
        public BankovniUcet Ucet { get; set; } 
    }

    public class PrispevekOdFirmy : Prispevek
    {
        //obchodní firmy nebo názvu, sídla a identifikačního čísla osoby, bylo-li přiděleno.
        public string ICO { get; set; }
        public string Sidlo { get; set; }
        public string Nazev { get; set; }

        public string FirmaId { get; set; }

    }

    public class PrispevekOdOsoby : Prispevek
    {
        // jména, příjmení, data narození a obce
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public DateTime? DatumNarozeni { get; set; }
        public string Obec { get; set; }
        public string Stat { get; set; }

        public string NameId { get; set; }
    }
}
