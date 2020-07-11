using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External.RPP
{
    public class ISVS
    {

        public string autorCeleJmeno { get; set; }
        public string autorEmail { get; set; }
        public DateTime? datumPosledniZmeny { get; set; }
        public DateTime? datumVytvoreni { get; set; }
        public DateTime? datumVzniku { get; set; }
        public DateTime? datumZverejneni { get; set; }
        public Etapa etapa { get; set; }
        public string charakteristika { get; set; }
        public int? id { get; set; }
        public int? identifikator { get; set; }
        public string informaceEmail { get; set; }
        public bool? jeKontaktProvozovateleShodnySeSpravcem { get; set; }
        public bool? jeOvmProvozovateleShodneSeSpravcem { get; set; }
        public Kategorie kategorie { get; set; }
        public string nazev { get; set; }
        public decimal? nakladyCelkove { get; set; }
        public decimal? nakladyRocni { get; set; }
        public DateTime? platnostOd { get; set; }
        public bool? primarni { get; set; }
        public string provozovatelEmail { get; set; }
        public Provozovatelovm provozovatelOvm { get; set; }

        public bool? sluzbyPoskytuje { get; set; }
        public bool? sluzbyVyuziva { get; set; }
        public string spravceEmail { get; set; }
        public Spravceovm spravceOvm { get; set; }
        public Stav stav { get; set; }
        public bool? urcenySystem { get; set; }
        public Urovensdileni urovenSdileni { get; set; }
        public string uzivatelOdeslaniCeleJmeno { get; set; }
        public string uzivatelPosledniZmenyCeleJmeno { get; set; }
        public string verze { get; set; }
        public string vyuzitiOvm { get; set; }

        public vypisvyuziti vyuzitiIS { get; set; } //https://rpp-ais.egon.gov.cz/AISP/rest/verejne/isvs/20228/vypisvyuziti


        public aplikacnicleneni[] aplikacniCleneni { get; set; } //https://rpp-ais.egon.gov.cz/AISP/rest/verejne/isvs/20228/polozkystruktury

        public pravnipredpisy[] pravniPredpisy { get; set; } //https://rpp-ais.egon.gov.cz/AISP/rest/verejne/isvs/20293/pp

        public milniky[] milnikyZivotnihoCyklu { get; set; } //https://rpp-ais.egon.gov.cz/AISP/rest/verejne/isvs/20293/milniky

        public gdpr[] Gdpr { get; set; } //https://rpp-ais.egon.gov.cz/AISP/rest/verejne/isvs/20293/gdpr
        public finance financniUdaje { get; set; } //https://rpp-ais.egon.gov.cz/AISP/rest/verejne/isvs/20228/finance

        public Subjekt[] Dodavatele { get; set; }
        public Subjekt[] Spravci { get; set; }
        public Subjekt[] Provozovatele { get; set; }


        public class Subjekt
        {
            public int id { get; set; }
            public Kontakt kontakt { get; set; }
            public string kontaktniEmail { get; set; }
            public string typSubjektu { get; set; }
            public OVMFull ovm { get; set; }
            public class Kontakt
            {
                public string email { get; set; }
                public int id { get; set; }
                public string telefon { get; set; }
                public string utvar { get; set; }
            }

        }


        public class Kategorie
        {
            public int? id { get; set; }
            public string kod { get; set; }
            public string nazev { get; set; }
            public bool? platnost { get; set; }
        }



        public class Etapa
        {
            public int? id { get; set; }
            public string kod { get; set; }
            public string nazev { get; set; }
            public bool? opakovat { get; set; }
            public int? poradi { get; set; }
        }

        public class Provozovatelovm
        {
            public Agendaeditora agendaEditora { get; set; }
            public DateTime? datumOdeslani { get; set; }
            public DateTime? datumPosledniZmeny { get; set; }
            public DateTime? datumRegistrace { get; set; }
            public int? id { get; set; }
            public string kodOvm { get; set; }
            public string nazevOvm { get; set; }
            public bool? orgJednotkaStatu { get; set; }
            public Ovmeditora ovmEditora { get; set; }
            public DateTime? platnostOd { get; set; }
            public int? pravniForma { get; set; }
            public bool? primarni { get; set; }
            public DateTime? prvotniDatum { get; set; }
            public DateTime? pusobnostOd { get; set; }
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
            public string uzivatelOdeslaniCeleJmeno { get; set; }
            public string uzivatelPosledniZmenyCeleJmeno { get; set; }
            public bool? vznikAutomaticky { get; set; }
            public Datoveschranky[] datoveSchranky { get; set; }

        }

        public class Spravceovm
        {
            public Agendaeditora agendaEditora { get; set; }
            public DateTime? datumOdeslani { get; set; }
            public DateTime? datumPosledniZmeny { get; set; }
            public DateTime? datumRegistrace { get; set; }
            public int? id { get; set; }
            public string kodOvm { get; set; }
            public string nazevOvm { get; set; }
            public bool? orgJednotkaStatu { get; set; }
            public Ovmeditora ovmEditora { get; set; }
            public DateTime? platnostOd { get; set; }
            public int? pravniForma { get; set; }
            public bool? primarni { get; set; }
            public DateTime? prvotniDatum { get; set; }
            public DateTime? pusobnostOd { get; set; }
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
            public string uzivatelOdeslaniCeleJmeno { get; set; }
            public string uzivatelPosledniZmenyCeleJmeno { get; set; }
            public bool? vznikAutomaticky { get; set; }
            public Datoveschranky[] datoveSchranky { get; set; }
            public Sidloovm sidloOvm { get; set; }
        }


        public class Urovensdileni
        {
            public int? id { get; set; }
            public string kod { get; set; }
            public string nazev { get; set; }
        }


        public class vypisvyuziti
        {
            //https://rpp-ais.egon.gov.cz/AISP/rest/verejne/isvs/20228/vypisvyuziti
            public Seznamkategoriiovm[] seznamKategoriiOvm { get; set; }
            public Seznamovm[] seznamOvm { get; set; }
            public string urovenVyuziti { get; set; }

            public class Seznamkategoriiovm
            {
                public DateTime? datumVzniku { get; set; }
                public int? id { get; set; }
                public string identifikatorKo { get; set; }
                public string nazev { get; set; }
                public DateTime? platnostOd { get; set; }
                public string spravnost { get; set; }
            }

            public class Seznamovm
            {
                public int? id { get; set; }
                public string kodOvm { get; set; }
                public string nazevOvm { get; set; }
                public DateTime? platnostOd { get; set; }
                public DateTime? pusobnostOd { get; set; }
                public string spravnost { get; set; }
                public Stav stav { get; set; }
            }
        }


        public class aplikacnicleneni
        {
            //https://rpp-ais.egon.gov.cz/AISP/rest/verejne/isvs/20228/polozkystruktury
            public int? id { get; set; }
            public string identifikator { get; set; }
            public string nazev { get; set; }
            public int? poradi { get; set; }
            public Typstruktury typStruktury { get; set; }
            public string ucel { get; set; }

            public class Typstruktury
            {
                public int? id { get; set; }
                public string kod { get; set; }
                public string nazev { get; set; }
            }
        }

        public class pravnipredpisy
        {
            //https://rpp-ais.egon.gov.cz/AISP/rest/verejne/isvs/20293/pp
            public string cislo { get; set; }
            public bool? hlavni { get; set; }
            public int? id { get; set; }
            public string nazev { get; set; }
            public int? rok { get; set; }
            public string typPp { get; set; }
            public Ustanovenipravpred[] ustanoveniPravPred { get; set; }

            public class Ustanovenipravpred
            {
                public int? id { get; set; }
                public string odstavec { get; set; }
                public string paragraf { get; set; }
                public string pismeno { get; set; }
            }
        }


        public class milniky
        {
            //https://rpp-ais.egon.gov.cz/AISP/rest/verejne/isvs/20293/milniky
            public DateTime? datumPlanDo { get; set; }
            public DateTime? datumPlanOd { get; set; }
            public DateTime? datumSkutecnostDo { get; set; }
            public DateTime? datumSkutecnostOd { get; set; }
            public Etapa etapa { get; set; }
            public Faze faze { get; set; }
            public int? id { get; set; }

            public class Faze
            {
                public int? id { get; set; }
                public string kod { get; set; }
                public string nazev { get; set; }
                public int? poradi { get; set; }
            }
        }

        public class gdpr
        {
            //https://rpp-ais.egon.gov.cz/AISP/rest/verejne/isvs/20293/gdpr
            public int? id { get; set; }
            public Pravnipredpisy pravniPredpisy { get; set; }
            public bool? souhlas { get; set; }
            public Typ typ { get; set; }

            public class Pravnipredpisy
            {
                public object[] seznam { get; set; }
            }

            public class Typ
            {
                public int? id { get; set; }
                public string kod { get; set; }
                public string nazev { get; set; }
                public bool? jePp { get; set; }
                public int? poradi { get; set; }
                public string typOdpovedi { get; set; }
            }
        }


        public class finance
        {
            //https://rpp-ais.egon.gov.cz/AISP/rest/verejne/isvs/20228/finance
            public Celkem[] celkem { get; set; }
            public Polozky[] polozky { get; set; }

            public class Celkem
            {
                public Typ typ { get; set; }
                public float soucet { get; set; }
            }
            public class Typ
            {
                public int? id { get; set; }
                public string kod { get; set; }
                public string nazev { get; set; }
                public int? poradi { get; set; }
            }


            public class Polozky
            {
                public Etapa etapa { get; set; }
                public int? id { get; set; }
                public Polozka[] polozky { get; set; }
                public int? rok { get; set; }
                public class Etapa
                {
                    public int? id { get; set; }
                    public string kod { get; set; }
                    public string nazev { get; set; }
                    public bool? opakovat { get; set; }
                    public int? poradi { get; set; }
                }

                public class Polozka
                {
                    public float externiNaklad { get; set; }
                    public int? id { get; set; }
                    public float interniVydaj { get; set; }
                    public Typ typ { get; set; }
                }
            }
        }




    }
}



