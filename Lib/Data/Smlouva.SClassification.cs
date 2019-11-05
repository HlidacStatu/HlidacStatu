using Devmasters.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class Smlouva
    {
        public class SClassification
        {

            public SClassification() { }

            public SClassification(Classification[] types, int version = 0)
            {
                this.LastUpdate = DateTime.Now;
                this.Version = version;
                this.Types = types;
            }

            public class Classification
            {
                public int TypeValue { get; set; }
                public decimal ClassifProbability { get; set; }

                public ClassificationsTypes ClassifType() { return (ClassificationsTypes)TypeValue; }
            }
            [ShowNiceDisplayName()]
            public enum ClassificationsTypes
            {

     
                [NiceDisplayName("Ostatní")]
                OSTATNI=0,

                [NiceDisplayName("IT")]
                it_generic = 10000,

                [NiceDisplayName("IT HW")]
                it_hw=10001,
                [NiceDisplayName("IT SW")]
                it_sw=10002,
                [NiceDisplayName("Informační systémy a servery")]
                it_servery = 10003,
                [NiceDisplayName("Opravy a údržba osobních počítačů")]
                it_opravy = 10004,
                [NiceDisplayName("Informační technologie: poradenství, vývoj programového vybavení, internet a podpora")]
                it_vyvoj = 10005,
                [NiceDisplayName("SW Systémové poradenské služby")]
                it_system = 10006,
                [NiceDisplayName("SW sluzby")]
                it_sw_sluzby = 10007,
                [NiceDisplayName("Internetové služby a počítačové sítě")]
                it_site = 10008,

                [NiceDisplayName("Stavebnictví")]
                stav_generic = 10100,
                [NiceDisplayName("Stavební konstrukce a materiály")]
                stav_materialy = 10101,
                [NiceDisplayName("Stavební práce")]
                stav_prace = 10102,
                [NiceDisplayName("Bytová výstavba")]
                stav_byty = 10103,
                [NiceDisplayName("Konstrukční a stavební práce")]
                stav_konstr = 10104,
                [NiceDisplayName("Stavební úpravy mostů a tunelů")]
                stav_mosty = 10105,
                [NiceDisplayName("Stavební práce pro potrubní, telekomunikační a elektrické vedení")]
                stav_vedeni = 10106,
                [NiceDisplayName("Výstavba, zakládání a povrchové práce pro silnice")]
                stav_silnice = 10107,
                [NiceDisplayName("Stavební úpravy pro železnici")]
                stav_zeleznice = 10108,
                [NiceDisplayName("Výstavba vodních děl")]
                stav_voda = 10109,
                [NiceDisplayName("Stavební montážní práce")]
                stav_montaz = 10110,
                [NiceDisplayName("Práce při dokončování budov")]
                stav_dokonceni = 10111,
                [NiceDisplayName("Opravy a údržba technických stavebních zařízení")]
                stav_technik = 10112,
                [NiceDisplayName("Instalační a montážní služby")]
                stav_instal = 10113,
                [NiceDisplayName("Architektonické, stavební, technické a inspekční služby")]
                stav_inspekce = 10114,
                [NiceDisplayName("Technicko_inženýrské služby")]
                stav_inzenyr = 10115,

                [NiceDisplayName("Doprava")]
                doprava_generic = 10200,
                [NiceDisplayName("Osobní vozidla")]
                doprava_osobni = 10201,
                [NiceDisplayName("Specializovaná vozidla")]
                doprava_special = 10202,
                [NiceDisplayName("Motorová vozidla pro přepravu více než 10 lidí")]
                doprava_lidi = 10203,
                [NiceDisplayName("Nakladní vozy")]
                doprava_nakladni = 10203,
                [NiceDisplayName("Vozidla silniční údržby")]
                doprava_udrzba = 10205,
                [NiceDisplayName("Nákladní vozidla pro pohotovostní služby")]
                doprava_pohotovost = 10206,
                [NiceDisplayName("Díly a příslušenství k motorovým vozidlům")]
                doprava_dily = 10207,
                [NiceDisplayName("Železniční a tramvajové lokomotivy a vozidla")]
                doprava_koleje = 10208,
                [NiceDisplayName("Silniční zařízení")]
                doprava_silnice = 10209,
                [NiceDisplayName("Opravy a údržba vozidel a příslušenství k nim a související služby")]
                doprava_opravy = 10210,
                [NiceDisplayName("Služby silniční dopravy")]
                doprava_sluzby = 10211,
                [NiceDisplayName("Poštovní a telekomunikační služby")]
                doprava_posta = 10212,

                [NiceDisplayName("Stroje a zařízení")]
                stroje_generic = 10300,
                [NiceDisplayName("Elektricke stroje")]
                stroje_elektricke = 10301,
                [NiceDisplayName("Laboratorní přístroje a zařízení")]
                stroje_laborator = 10302,
                [NiceDisplayName("Průmyslové stroje")]
                stroje_prumysl = 10303,

                [NiceDisplayName("Telco")]
                telco_generic = 10400,
                [NiceDisplayName("TV")]
                telco_tv = 10401,
                [NiceDisplayName("Sítě a přenos dat")]
                telco_site = 10402,
                [NiceDisplayName("Telekomunikační služby")]
                telco_sluzby = 10403,

                [NiceDisplayName("Zdravotnictví")]
                zdrav_generic = 10500,
                [NiceDisplayName("Zdravotnické přístroje")]
                zdrav_pristroje = 10501,
                [NiceDisplayName("Leciva")]
                zdrav_leciva = 10502,
                [NiceDisplayName("Kosmetika")]
                zdrav_kosmetika = 10503,
                [NiceDisplayName("Opravy a údržba zdravotnických přístrojů")]
                zdrav_opravy = 10504,

                [NiceDisplayName("Voda a potraviny")]
                jidlo_generic = 10600,
                [NiceDisplayName("Potraviny")]
                jidlo_potrava = 10601,
                [NiceDisplayName("Pitná voda, nápoje, tabák atd.")]
                jidlo_voda = 10602,

                [NiceDisplayName("Bezpečnostní a ochranné vybavení a údržba")]
                bezpecnost_generic = 10700,
                bezpecnost = 10701,

                [NiceDisplayName("Přírodní zdroje")]
                prirodnizdroj_generic = 10800,
                [NiceDisplayName("Písky a jíly")]
                prirodnizdroj_pisky = 10801,
                [NiceDisplayName("Chemické výrobky")]
                prirodnizdroj_chemie = 10802,
                [NiceDisplayName("Jiné přírodní zdroje")]
                prirodnizdroj_vse = 10803,

                [NiceDisplayName("Energie")]
                energie_generic = 10900,
                [NiceDisplayName("Paliva a oleje")]
                energie_paliva = 10901,
                [NiceDisplayName("Elektricka energie")]
                energie_elektrina = 10902,
                [NiceDisplayName("Jiná energie")]
                energie_jina = 10903,
                [NiceDisplayName("Veřejné služby pro energie")]
                energie_sluzby = 10904,

                [NiceDisplayName("Zemědělství")]
                agro_generic = 11000,
                [NiceDisplayName("Lesnictví")]
                agro_les = 11001,
                [NiceDisplayName("Těžba dřeva")]
                agro_tezba = 11002,
                [NiceDisplayName("Zahradnické služby")]
                agro_zahrada = 11003,

                [NiceDisplayName("Kancelář")]
                kancelar_generic = 11100,
                [NiceDisplayName("Tisk")]
                kancelar_tisk = 11101,
                [NiceDisplayName("Kancelářské stroje (mimo počítače a nábytek)")]
                kancelar_stroje = 11102,
                [NiceDisplayName("Nábytek")]
                kancelar_nabytek = 11103,
                [NiceDisplayName("Domácí spotřebiče")]
                kancelar_spotrebice = 11104,
                [NiceDisplayName("Čisticí výrobky")]
                kancelar_cisteni = 11105,
                [NiceDisplayName("Nábor zaměstnanců")]
                kancelar_nabor = 11106,
                [NiceDisplayName("Vyhledávací a bezpečnostní služby")]
                kancelar_bezpecnost = 11107,

                [NiceDisplayName("Řemesla")]
                remeslo_generic = 11200,
                [NiceDisplayName("Oděvy")]
                remeslo_odevy = 11201,
                [NiceDisplayName("Textilie")]
                remeslo_textil = 11202,
                [NiceDisplayName("Hudební nástroje")]
                remeslo_hudba = 11203,

                [NiceDisplayName("Sociální služby")]
                social_generic = 11300,
                [NiceDisplayName("Vzdělávání")]
                social_vzdelavani = 11301,
                [NiceDisplayName("Školení")]
                social_skoleni = 11302,
                [NiceDisplayName("Zdravotní péče")]
                social_zdravotni = 11303,
                [NiceDisplayName("Sociální péče")]
                social_pece = 11304,
                [NiceDisplayName("Rekreační, kulturní akce")]
                social_kultura = 11305,
                [NiceDisplayName("Knihovny, archivy, muzea a jiné")]
                social_knihovny = 11306,
                [NiceDisplayName("Sportovní služby")]
                social_sport = 11307,

                [NiceDisplayName("Finance")]
                finance_generic = 11400,
                [NiceDisplayName("Finanční a pojišťovací služby")]
                finance_sluzby = 11401,
                [NiceDisplayName("Účetní, revizní a peněžní služby")]
                finance_ucetni = 11402,
                [NiceDisplayName("Podnikatelské a manažerské poradenství a související služby")]
                finance_poradenstvi = 11403,

                legal_generic = 11500,
                [NiceDisplayName("Realitní služby")]
                legal_reality = 11501,
                [NiceDisplayName("Právní služby")]
                legal_pravni =  11502,
                [NiceDisplayName("Nájemní smlouvy")]
                legal_najem = 11503,
                [NiceDisplayName("Pronájem pozemků")]
                legal_pozemky = 11504,


                techsluzby_generic = 11600,
                [NiceDisplayName("Kanalizace")]
                techsluzby_kanaly = 11601,
                [NiceDisplayName("Čistící a hygienické služby")]
                techsluzby_cisteni = 11602,
                [NiceDisplayName("Úklidové služby")]
                techsluzby_uklid = 11603,


                [NiceDisplayName("Výzkum a vývoj a související služby")]
                vyzkum_generic = 11700,
                [NiceDisplayName("Výzkum a vývoj a související služby")]
                vyzkum = 11701,

                [NiceDisplayName("Reklamní a marketingové služby")]
                marketing_generic = 11800,
                marketing = 11801,

                jine_generic = 11900,
                [NiceDisplayName("Pohostinství a ubytovací služby a maloobchodní služby")]
                jine_pohostinstvi = 11901,
                [NiceDisplayName("Služby závodních jídelen")]
                jine_jidelny = 11902,
                [NiceDisplayName("Administrativní služby")]
                jine_admin = 11903,
                [NiceDisplayName("Zajišťování služeb pro veřejnost")]
                jine_verejnost = 11904,
                [NiceDisplayName("Průzkum veřejného mínění a statistiky")]
                jine_pruzkum = 11905,
                [NiceDisplayName("Opravy a údržba")]
                jine_opravy = 11906,
            }

            public DateTime? LastUpdate { get; set; } = null;

            public int Version { get; set; } = 1;

            public Classification[] Types { get; set; } = null;

            public override string ToString()
            {
                if (this.Types != null)
                {
                    return $"Types:{Types.Select(m=>m.ClassifType().ToString() + " ("+ m.ClassifProbability.ToString("P2") + ")").Aggregate((f,s)=>f+"; "+s)}"
                        + $" updated:{LastUpdate.ToString()}";
                }
                else
                {
                    return  $"Types:snull updated:{LastUpdate.ToString()}";
                }
                //return base.ToString();
            }

        }
    }
}
