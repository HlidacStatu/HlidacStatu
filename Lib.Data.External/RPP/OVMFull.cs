using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External.RPP
{
    public class OVMFull
    {

        public Agendaeditora agendaEditora { get; set; }
        public DateTime datumPosledniZmeny { get; set; }
        public int id { get; set; }
        public string kodOvm { get; set; }
        public string nazevOvm { get; set; }
        public bool orgJednotkaStatu { get; set; }
        public Ovmeditora ovmEditora { get; set; }
        public DateTime platnostOd { get; set; }
        public int pravniForma { get; set; }
        public bool primarni { get; set; }
        public DateTime prvotniDatum { get; set; }
        public DateTime pusobnostOd { get; set; }
        public string spravnost { get; set; }
        public string spravnostAdresa { get; set; }
        public string spravnostNazevOvm { get; set; }
        public string spravnostOrgJedn { get; set; }
        public string spravnostPozastaveni { get; set; }
        public string spravnostPp { get; set; }
        public string spravnostPreruseni { get; set; }
        public string spravnostPusobDo { get; set; }
        public string spravnostPusobOd { get; set; }
        public Stav stav { get; set; }
        public string typOvm { get; set; }
        public bool vznikAutomaticky { get; set; }
        public Sidloovm sidloOvm { get; set; }
        public Osoba[] angazovaneOsoby { get; set; }

        //z osobaros
        public int kodPravnihoStavu { get; set; }
        public string nazevPravnihoStavu { get; set; }
        public bool verejnaProspesnost { get; set; }

        public OVMSimple.Ovm[] podrizeneOVM { get; set; }

        public KategorieOVM[] kategorie { get; set; }

        public Osoba[] osobyVCele
        { get; set; }


        public class Osoba
        {
            public Fyzickaosoba fyzickaOsoba { get; set; }
            public int id { get; set; }
            public Adresaruian mistoPobytu { get; set; }
            public string spravnost { get; set; }
        }

        public class Fyzickaosoba
        {
            public string aifo { get; set; }
            public DateTime datumPosledniZmeny { get; set; }
            public object[] doklady { get; set; }
            public Adresaruian dorucAdresa { get; set; }
            public int id { get; set; }
            public string jmeno { get; set; }
            public Adresaruian mistoPobytu { get; set; }
            public bool platnostVZr { get; set; }
            public string prijmeni { get; set; }
            public string spravnostDatNar { get; set; }
            public string spravnostDatoveSchranky { get; set; }
            public string spravnostDatUmrti { get; set; }
            public string spravnostDorucAdr { get; set; }
            public string spravnostJmeno { get; set; }
            public string spravnostMistaNar { get; set; }
            public string spravnostMistaUmrti { get; set; }
            public string spravnostPobyt { get; set; }
            public string spravnostPrijmeni { get; set; }
            public object[] statniPrislusnost { get; set; }
        }

        public class Dorucadresa
        {
        }


        public class Ruiancastobce
        {
            public int castObceKod { get; set; }
            public string castObceNazev { get; set; }
            public DateTime datumPosledniZmeny { get; set; }
            public int id { get; set; }
            public bool platnostVZr { get; set; }
        }

        public class Ruianobec
        {
            public DateTime datumPosledniZmeny { get; set; }
            public int id { get; set; }
            public int obecKod { get; set; }
            public string obecNazev { get; set; }
            public bool platnostVZr { get; set; }
        }

        public class Ruianobjekt
        {
            public DateTime datumPosledniZmeny { get; set; }
            public int id { get; set; }
            public bool platnostVZr { get; set; }
            public int stavebniObjektKod { get; set; }
            public int typCislaDomovnihoKod { get; set; }
        }

        public class Ruianposta
        {
            public DateTime datumPosledniZmeny { get; set; }
            public int id { get; set; }
            public string nazevPosty { get; set; }
            public bool platnostVZR { get; set; }
            public int postaKod { get; set; }
        }



        public class DatovaSchranka
        {
            public int id { get; set; }
            public string identifikatorDs { get; set; }
            public string spravnost { get; set; }
            public Typds typDs { get; set; }
            public string zkratka { get; set; }
            public class Typds
            {
                public int ciselnyKod { get; set; }
                public int id { get; set; }
                public string popis { get; set; }
                public string zkratka { get; set; }
            }
        }








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
            public DateTime platnostOd { get; set; }
            public DateTime pusobnostOd { get; set; }
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
            public DateTime datumPosledniZmeny { get; set; }
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
                public DateTime datumPosledniZmeny { get; set; }
                public int id { get; set; }
                public bool platnostVZr { get; set; }
            }

            public class Ruianobec
            {
                public DateTime datumPosledniZmeny { get; set; }
                public int id { get; set; }
                public int obecKod { get; set; }
                public string obecNazev { get; set; }
                public bool platnostVZr { get; set; }
            }

            public class Ruianobjekt
            {
                public DateTime datumPosledniZmeny { get; set; }
                public int id { get; set; }
                public bool platnostVZr { get; set; }
                public int stavebniObjektKod { get; set; }
                public int typCislaDomovnihoKod { get; set; }
            }

            public class Ruianposta
            {
                public DateTime datumPosledniZmeny { get; set; }
                public int id { get; set; }
                public string nazevPosty { get; set; }
                public bool platnostVZR { get; set; }
                public int postaKod { get; set; }
            }

            public class Ruianulice
            {
                public DateTime datumPosledniZmeny { get; set; }
                public int id { get; set; }
                public bool platnostVZr { get; set; }
                public int uliceKod { get; set; }
                public string uliceNazev { get; set; }
            }
        }

    }

}
