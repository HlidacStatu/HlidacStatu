using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External.RPP
{
    public class Agendaeditora
    {
        public int id { get; set; }
        public string kod { get; set; }
        public string nazev { get; set; }
    }

    public class Ovmeditora
    {
        public int id { get; set; }
        public string kodOvm { get; set; }
        public string nazevOvm { get; set; }
        public DateTime? platnostOd { get; set; }
        public DateTime? pusobnostOd { get; set; }
        public string spravnost { get; set; }
        public Stav stav { get; set; }
    }

    public class Stav
    {
        public int id { get; set; }
        public string kod { get; set; }
        public string nazev { get; set; }
        public string popis { get; set; }
        public int poradi { get; set; }
    }


    public class Sidloovm
    {
        public Adresaruian adresaRuian { get; set; }
    }

    public class Adresaruian
    {
        public int adresniMistoKod { get; set; }
        public int cisloDomovni { get; set; }
        public int cisloOrientacni { get; set; }
        public DateTime? datumPosledniZmeny { get; set; }
        public int id { get; set; }
        public int okresKod { get; set; }
        public bool platnostVZr { get; set; }
        public Ruiancastobce ruianCastObce { get; set; }
        public Ruianobec ruianObec { get; set; }
        public Ruianobjekt ruianObjekt { get; set; }
        public Ruianposta ruianPosta { get; set; }
        public Ruianulice ruianUlice { get; set; }

        public class Ruiancastobce
        {
            public int castObceKod { get; set; }
            public string castObceNazev { get; set; }
            public DateTime? datumPosledniZmeny { get; set; }
            public int id { get; set; }
            public bool platnostVZr { get; set; }
        }

        public class Ruianobec
        {
            public DateTime? datumPosledniZmeny { get; set; }
            public int id { get; set; }
            public int obecKod { get; set; }
            public string obecNazev { get; set; }
            public bool platnostVZr { get; set; }
        }

        public class Ruianobjekt
        {
            public DateTime? datumPosledniZmeny { get; set; }
            public int id { get; set; }
            public bool platnostVZr { get; set; }
            public int stavebniObjektKod { get; set; }
            public int typCislaDomovnihoKod { get; set; }
        }

        public class Ruianposta
        {
            public DateTime? datumPosledniZmeny { get; set; }
            public int id { get; set; }
            public string nazevPosty { get; set; }
            public bool platnostVZR { get; set; }
            public int postaKod { get; set; }
        }

        public class Ruianulice
        {
            public DateTime? datumPosledniZmeny { get; set; }
            public int id { get; set; }
            public bool platnostVZr { get; set; }
            public int uliceKod { get; set; }
            public string uliceNazev { get; set; }
        }
    }
}
