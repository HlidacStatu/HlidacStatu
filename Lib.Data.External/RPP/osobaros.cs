using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External.RPP
{
    public class osobaros
    {

        public Adresasidla adresaSidla { get; set; }
        public DateTime datumPosledniZmeny { get; set; }
        public DateTime datumVznikuOpravneni { get; set; }
        public string ico { get; set; }
        public int id { get; set; }
        public string kodAgendy { get; set; }
        public string kodOvm { get; set; }
        public int kodPravniFormy { get; set; }
        public int kodPravnihoStavu { get; set; }
        public string nazevOsoby { get; set; }
        public string nazevPravnihoStavu { get; set; }
        public bool platnostVZr { get; set; }
        public bool pozastaveniPreruseni { get; set; }
        public string spravnostAdresa { get; set; }
        public string spravnostDo { get; set; }
        public string spravnostNazev { get; set; }
        public string spravnostOd { get; set; }
        public string spravnostProspesnost { get; set; }
        public string typOsoby { get; set; }
        public bool verejnaProspesnost { get; set; }
        public OVMFull.Osoba[] angazovaneOsoby { get; set; }
        public Datoveschranky[] datoveSchranky { get; set; }
    }

    public class Adresasidla
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
        public Ruianmop ruianMop { get; set; }
        public Ruianobec ruianObec { get; set; }
        public Ruianobjekt ruianObjekt { get; set; }
        public Ruianposta ruianPosta { get; set; }
        public Ruianulice ruianUlice { get; set; }
    }

    public class Ruiancastobce
    {
        public int castObceKod { get; set; }
        public string castObceNazev { get; set; }
        public DateTime datumPosledniZmeny { get; set; }
        public int id { get; set; }
        public bool platnostVZr { get; set; }
    }

    public class Ruianmop
    {
        public DateTime datumPosledniZmeny { get; set; }
        public int id { get; set; }
        public int mopKod { get; set; }
        public string mopNazev { get; set; }
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

    public class Angazovaneosoby
    {
        public Fyzickaosoba fyzickaOsoba { get; set; }
        public int id { get; set; }
        public string spravnost { get; set; }
        public string typAngazisty { get; set; }
        public string typAngazma { get; set; }
    }

    public class Fyzickaosoba
    {
        public string aifo { get; set; }
        public DateTime datumPosledniZmeny { get; set; }
        public object[] doklady { get; set; }
        public Dorucadresa dorucAdresa { get; set; }
        public int id { get; set; }
        public string jmeno { get; set; }
        public Mistopobytu mistoPobytu { get; set; }
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
        public Mistonarozeni mistoNarozeni { get; set; }
        public Mistoumrti mistoUmrti { get; set; }
        public object[] statniPrislusnost { get; set; }
    }

    public class Dorucadresa
    {
    }

    public class Mistopobytu
    {
        public int adresniMistoKod { get; set; }
        public int cisloDomovni { get; set; }
        public DateTime datumPosledniZmeny { get; set; }
        public int id { get; set; }
        public bool platnostVZr { get; set; }
        public Ruiancastobce1 ruianCastObce { get; set; }
        public Ruianobec1 ruianObec { get; set; }
        public Ruianobjekt1 ruianObjekt { get; set; }
        public Ruianposta1 ruianPosta { get; set; }
    }

    public class Ruiancastobce1
    {
        public int castObceKod { get; set; }
        public string castObceNazev { get; set; }
        public DateTime datumPosledniZmeny { get; set; }
        public int id { get; set; }
        public bool platnostVZr { get; set; }
    }

    public class Ruianobec1
    {
        public DateTime datumPosledniZmeny { get; set; }
        public int id { get; set; }
        public int obecKod { get; set; }
        public string obecNazev { get; set; }
        public bool platnostVZr { get; set; }
    }

    public class Ruianobjekt1
    {
        public DateTime datumPosledniZmeny { get; set; }
        public int id { get; set; }
        public bool platnostVZr { get; set; }
        public int stavebniObjektKod { get; set; }
        public int typCislaDomovnihoKod { get; set; }
    }

    public class Ruianposta1
    {
        public DateTime datumPosledniZmeny { get; set; }
        public int id { get; set; }
        public string nazevPosty { get; set; }
        public bool platnostVZR { get; set; }
        public int postaKod { get; set; }
    }

    public class Mistonarozeni
    {
    }

    public class Mistoumrti
    {
    }

    public class Datoveschranky
    {
        public int id { get; set; }
        public string identifikatorDs { get; set; }
        public string spravnost { get; set; }
        public Typds typDs { get; set; }
        public string zkratka { get; set; }
    }

    public class Typds
    {
        public int ciselnyKod { get; set; }
        public int id { get; set; }
        public string popis { get; set; }
        public string zkratka { get; set; }
    }

}
