using System;

namespace HlidacStatu.Lib.Data.External.RPP
{
    public class KategorieOVM
    {
        public static int[] preferredIds = new int[] {
            1081,1128,197,137,1124,1118,1119,178,1122,1126,155,11,1121,118,2051,1060,
            191,123,115,122,141,128,147,1132,164,173,1130,103,1129,1120,105,145,
            109,135,158,120,121,113,12,186,143,107,114,980
        };

        public Agendaeditora agendaEditora { get; set; }
        public bool automatickeZarazeni { get; set; }
        public DateTime datumOdeslani { get; set; }
        public DateTime datumPosledniZmeny { get; set; }
        public DateTime datumRegistrace { get; set; }
        public DateTime datumVzniku { get; set; }
        public int id { get; set; }
        public string identifikatorKo { get; set; }
        public bool nabizetVAgende { get; set; }
        public string nazev { get; set; }
        public Ovmeditora ovmEditora { get; set; }
        public DateTime platnostOd { get; set; }
        public bool primarni { get; set; }
        public string spravnost { get; set; }
        public string spravnostPp { get; set; }
        public string spravnostSeznamu { get; set; }
        public Stav stav { get; set; }
        public string uzivatelOdeslaniCeleJmeno { get; set; }
        public string uzivatelPosledniZmenyCeleJmeno { get; set; }
        public bool vznikAutomaticky { get; set; }

        public bool hlidac_preferred { get; set; }
        public OVMSimple.Ovm[] OVM_v_kategorii { get; set; }



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


    }

}
