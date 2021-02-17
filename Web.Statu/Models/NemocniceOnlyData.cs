using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Nest;

namespace HlidacStatu.Web.Models
{
    public class NemocniceOnlyData
    {
        public NemocniceOnlyData() { }
        string _id = "";
        [CsvHelper.Configuration.Attributes.Ignore]
        public string id
        {
            get
            {
                return $"{datum:yyyy-MM-dd}-{zz_id}";
            }
        }

        [Description("Datum")]
        public DateTime datum { get; set; }

        public string zz_id
        {
            get
            {
                return $"{zz_kod}-{Devmasters.Crypto.Hash.ComputeHashToHex(zz_nazev).Substring(0, 8)}";
            }
        }

        [Description("Identifikátor zdravotnického zařízení (IČZ).")]
        public string zz_kod { get; set; }

        [Description("Název zdravotnického zařízení")]
        public string zz_nazev { get; set; }

        [Description("Identifikátor kraje podle klasifikace NUTS 3.")]
        public string kraj_nuts_kod { get; set; }

        [Description("Název kraje")]
        public string kraj_nazev { get; set; }

        [Description("Počet přístrojů ECMO na pracovišti ARO A JIP (ARO – zdravotní péče z těchto lůžek se vykazuje pod odborností 7I8 nebo 7T8; JIP – zdravotní péče z těchto lůžek se vykazuje pod odborností I nebo T, nejedná se o lůžka následné a dlouhodobé péče). Volná kapacita.")]
        public int ecmo_kapacita_volna { get; set; }

        [Description("Počet přístrojů ECMO na pracovišti ARO A JIP (ARO – zdravotní péče z těchto lůžek se vykazuje pod odborností 7I8 nebo 7T8; JIP – zdravotní péče z těchto lůžek se vykazuje pod odborností I nebo T, nejedná se o lůžka následné a dlouhodobé péče). Celková kapacita.")]
        public int ecmo_kapacita_celkem { get; set; }

        public decimal ecmo_kapacita_perc() => (ecmo_kapacita_celkem == 0 ? 0 : ((decimal)ecmo_kapacita_volna) / (decimal)ecmo_kapacita_celkem);


        [Description("Počet přístrojů UPV na pracovišti ARO A JIP (ARO – zdravotní péče z těchto lůžek se vykazuje pod odborností 7I8 nebo 7T8; JIP – zdravotní péče z těchto lůžek se vykazuje pod odborností I nebo T, nejedná se o lůžka následné a dlouhodobé péče). Volná kapacita.")]
        public int upv_kapacita_volna { get; set; }

        [Description("Počet přístrojů UPV na pracovišti ARO A JIP (ARO – zdravotní péče z těchto lůžek se vykazuje pod odborností 7I8 nebo 7T8; JIP – zdravotní péče z těchto lůžek se vykazuje pod odborností I nebo T, nejedná se o lůžka následné a dlouhodobé péče). Celková kapacita.")]
        public int upv_kapacita_celkem { get; set; }
        public decimal upv_kapacita_perc() => (upv_kapacita_celkem == 0 ? 0 : ((decimal)upv_kapacita_volna) / (decimal)upv_kapacita_celkem);


        [Description("Počet přístrojů pro kontinuální dialýzu (bez rozlišení, na kterém pracovišti se momentálně nachází). Volná kapacita.")]
        public int crrt_kapacita_volna { get; set; }

        [Description("Počet přístrojů pro kontinuální dialýzu (bez rozlišení, na kterém pracovišti se momentálně nachází). Celková kapacita.")]
        public int crrt_kapacita_celkem { get; set; }
        public decimal crrt_kapacita_perc() => (crrt_kapacita_celkem == 0 ? 0 : ((decimal)crrt_kapacita_volna) / (decimal)crrt_kapacita_celkem);


        [Description("Počet přístrojů pro intermitentní dialýzu (bez rozlišení, na kterém pracovišti se momentálně nachází). Nejedná se o přístroje pro léčbu pacientů s chronickým dialyzačním programem. Volná kapacita.")]
        public int ihd_kapacita_volna { get; set; }

        [Description("Počet přístrojů pro intermitentní dialýzu (bez rozlišení, na kterém pracovišti se momentálně nachází). Nejedná se o přístroje pro léčbu pacientů s chronickým dialyzačním programem. Celková kapacita.")]
        public int ihd_kapacita_celkem { get; set; }
        public decimal ihd_kapacita_perc() => (ihd_kapacita_celkem == 0 ? 0 : ((decimal)ihd_kapacita_volna) / (decimal)ihd_kapacita_celkem);


        [Description("Počet lůžek anesteziologicko–resuscitačního oddělení (zdravotní péče z těchto lůžek se vykazuje pod odborností 7I8 nebo 7T8) a počet lůžek na jednotkách intenzivní péče pro dospělé (zdravotní péče z těchto lůžek se vykazuje pod odborností I nebo T, nejedná se o lůžka následné a dlouhodobé péče). Celková kapacita.")]
        public int luzka_aro_jip_kapacita_celkem { get; set; }

        [Description("Počet lůžek anesteziologicko–resuscitačního oddělení (zdravotní péče z těchto lůžek se vykazuje pod odborností 7I8 nebo 7T8) a počet lůžek na jednotkách intenzivní péče pro dospělé (zdravotní péče z těchto lůžek se vykazuje pod odborností I nebo T, nejedná se o lůžka následné a dlouhodobé péče). Volná kapacita pro pacienty COVID-19 pozitivní.")]
        public int luzka_aro_jip_kapacita_volna_covid_pozitivni { get; set; }

        [Description("Počet lůžek anesteziologicko–resuscitačního oddělení (zdravotní péče z těchto lůžek se vykazuje pod odborností 7I8 nebo 7T8) a počet lůžek na jednotkách intenzivní péče pro dospělé (zdravotní péče z těchto lůžek se vykazuje pod odborností I nebo T, nejedná se o lůžka následné a dlouhodobé péče). Volná kapacita pro pacienty COVID-19 negativní.")]
        public int luzka_aro_jip_kapacita_volna_covid_negativni { get; set; }
        public decimal luzka_aro_jip_kapacita_perc() => (luzka_aro_jip_kapacita_celkem == 0 ? 0 : ((decimal)luzka_aro_jip_kapacita_volna_covid_negativni + (decimal)luzka_aro_jip_kapacita_volna_covid_pozitivni) / (decimal)luzka_aro_jip_kapacita_celkem);


        [Description("Z celkového počtu standardních akutních lůžek se uvádí počet lůžek, ke kterým je přiveden kyslík (zdravotní péče z těchto lůžek se vykazuje pod odborností H nebo F, nejedná se o lůžka následné a dlouhodobé péče). Celková kapacita.")]
        public int luzka_standard_kyslik_kapacita_celkem { get; set; }

        [Description("Z celkového počtu standardních akutních lůžek se uvádí počet lůžek, ke kterým je přiveden kyslík (zdravotní péče z těchto lůžek se vykazuje pod odborností H nebo F, nejedná se o lůžka následné a dlouhodobé péče). Volná kapacita pro pacienty COVID-19 pozitivní.")]
        public int luzka_standard_kyslik_kapacita_volna_covid_pozitivni { get; set; }

        [Description("Z celkového počtu standardních akutních lůžek se uvádí počet lůžek, ke kterým je přiveden kyslík (zdravotní péče z těchto lůžek se vykazuje pod odborností H nebo F, nejedná se o lůžka následné a dlouhodobé péče). Volná kapacita pro pacienty COVID-19 negativní.")]
        public int luzka_standard_kyslik_kapacita_volna_covid_negativni { get; set; }
        public decimal luzka_standard_kyslik_kapacita_perc() => (luzka_standard_kyslik_kapacita_celkem == 0 ? 0 : ((decimal)luzka_standard_kyslik_kapacita_volna_covid_negativni + (decimal)luzka_standard_kyslik_kapacita_volna_covid_pozitivni) / (decimal)luzka_standard_kyslik_kapacita_celkem);


        [Description("Počet přenosných/transportních ventilátorů. Volná kapacita.")]
        public int ventilatory_prenosne_kapacita_volna { get; set; }

        [Description("Počet přenosných/transportních ventilátorů. Celková kapacita.")]
        public int ventilatory_prenosne_kapacita_celkem { get; set; }
        public decimal ventilatory_prenosne_kapacita_perc() => (ventilatory_prenosne_kapacita_celkem == 0 ? 0 : ((decimal)ventilatory_prenosne_kapacita_volna) / (decimal)ventilatory_prenosne_kapacita_celkem);


        [Description("Počet ventilátorů na operačních sálech (přístroje, které jsou fyzicky umístěny na sálech). Volná kapacita.")]
        public int ventilatory_operacni_sal_kapacita_volna { get; set; }

        [Description("Počet ventilátorů na operačních sálech (přístroje, které jsou fyzicky umístěny na sálech). Celková kapacita.")]
        public int ventilatory_operacni_sal_kapacita_celkem { get; set; }
        public decimal ventilatory_operacni_sal_kapacita_perc() => (ventilatory_operacni_sal_kapacita_celkem == 0 ? 0 : ((decimal)ventilatory_operacni_sal_kapacita_volna) / (decimal)ventilatory_operacni_sal_kapacita_celkem);


        [Description("Lůžka IP (UPV +/- ) nově vyhrazená pro COVID pozitivní pacienty, která byla původně určena k poskytování jiného typu péče nebo péče jiného oboru/odbornosti. Volná kapacita.")]
        public int reprofilizovana_kapacita_luzka_aro_jip_kapacita_volna { get; set; }

        [Description("Lůžka IP (UPV +/- ) nově vyhrazená pro COVID pozitivní pacienty, která byla původně určena k poskytování jiného typu péče nebo péče jiného oboru/odbornosti. Celková kapacita.")]
        public int reprofilizovana_kapacita_luzka_aro_jip_kapacita_celkem { get; set; }

        [Description("Lůžka IP (UPV +/- ) nově vyhrazená pro COVID pozitivní pacienty, která byla původně určena k poskytování jiného typu péče nebo péče jiného oboru/odbornosti. Plánovaná kapacita je pevně stanovená MZ ČR, kterou nelze v systému DIP uživatelsky měnit.")]
        public int reprofilizovana_kapacita_luzka_aro_jip_kapacita_planovana { get; set; }

        [Description("Lůžka standardní s kyslíkem nově vyhrazená pro COVID pozitivní pacienty, která byla původně určena k poskytování jiného typu péče nebo péče jiného oboru/odbornosti. Volná kapacita.")]
        public int reprofilizovana_kapacita_luzka_standard_kyslik_kapacita_volna { get; set; }

        [Description("Lůžka standardní s kyslíkem nově vyhrazená pro COVID pozitivní pacienty, která byla původně určena k poskytování jiného typu péče nebo péče jiného oboru/odbornosti. Celková kapacita.")]
        public int reprofilizovana_kapacita_luzka_standard_kyslik_kapacita_celkem { get; set; }

        [Description("Lůžka standardní s kyslíkem nově vyhrazená pro COVID pozitivní pacienty, která byla původně určena k poskytování jiného typu péče nebo péče jiného oboru/odbornosti. Plánovaná kapacita je pevně stanovená MZ ČR, kterou nelze v systému DIP uživatelsky měnit.")]
        public int reprofilizovana_kapacita_luzka_standard_kyslik_kapacita_planovana { get; set; }



        public static NemocniceOnlyData Diff(NemocniceOnlyData fh, NemocniceOnlyData lh)
        {
            NemocniceOnlyData h = new NemocniceOnlyData();
            h.datum = new DateTime((lh.datum - fh.datum).Ticks);
            h.zz_kod = fh.zz_kod;
            h.zz_nazev = fh.zz_nazev;
            h.kraj_nazev = fh.kraj_nazev;
            h.kraj_nuts_kod = fh.kraj_nuts_kod;
            h.luzka_aro_jip_kapacita_celkem = lh.luzka_aro_jip_kapacita_celkem - fh.luzka_aro_jip_kapacita_celkem;
            h.luzka_aro_jip_kapacita_volna_covid_pozitivni = lh.luzka_aro_jip_kapacita_volna_covid_pozitivni - fh.luzka_aro_jip_kapacita_volna_covid_pozitivni;
            h.luzka_aro_jip_kapacita_volna_covid_negativni = lh.luzka_aro_jip_kapacita_volna_covid_negativni - fh.luzka_aro_jip_kapacita_volna_covid_negativni;
            h.crrt_kapacita_celkem = lh.crrt_kapacita_celkem - fh.crrt_kapacita_celkem;
            h.crrt_kapacita_volna = lh.crrt_kapacita_volna - fh.crrt_kapacita_volna;
            h.ecmo_kapacita_celkem = lh.ecmo_kapacita_celkem - fh.ecmo_kapacita_celkem;
            h.ecmo_kapacita_volna = lh.ecmo_kapacita_volna - fh.ecmo_kapacita_volna;

            h.ihd_kapacita_celkem = lh.ihd_kapacita_celkem - fh.ihd_kapacita_celkem;
            h.ihd_kapacita_volna = lh.ihd_kapacita_volna - fh.ihd_kapacita_volna;
            h.reprofilizovana_kapacita_luzka_aro_jip_kapacita_celkem = lh.reprofilizovana_kapacita_luzka_aro_jip_kapacita_celkem - fh.reprofilizovana_kapacita_luzka_aro_jip_kapacita_celkem;
            h.reprofilizovana_kapacita_luzka_aro_jip_kapacita_planovana = lh.reprofilizovana_kapacita_luzka_aro_jip_kapacita_planovana - fh.reprofilizovana_kapacita_luzka_aro_jip_kapacita_planovana;
            h.reprofilizovana_kapacita_luzka_aro_jip_kapacita_volna = lh.reprofilizovana_kapacita_luzka_aro_jip_kapacita_volna - fh.reprofilizovana_kapacita_luzka_aro_jip_kapacita_volna;

            h.reprofilizovana_kapacita_luzka_standard_kyslik_kapacita_celkem = lh.reprofilizovana_kapacita_luzka_standard_kyslik_kapacita_celkem - fh.reprofilizovana_kapacita_luzka_standard_kyslik_kapacita_celkem;
            h.reprofilizovana_kapacita_luzka_standard_kyslik_kapacita_planovana = lh.reprofilizovana_kapacita_luzka_standard_kyslik_kapacita_planovana - fh.reprofilizovana_kapacita_luzka_standard_kyslik_kapacita_planovana;
            h.reprofilizovana_kapacita_luzka_standard_kyslik_kapacita_volna = lh.reprofilizovana_kapacita_luzka_standard_kyslik_kapacita_volna - fh.reprofilizovana_kapacita_luzka_standard_kyslik_kapacita_volna;

            h.luzka_standard_kyslik_kapacita_celkem = lh.luzka_standard_kyslik_kapacita_celkem - fh.luzka_standard_kyslik_kapacita_celkem;
            h.luzka_standard_kyslik_kapacita_volna_covid_negativni = lh.luzka_standard_kyslik_kapacita_volna_covid_negativni - fh.luzka_standard_kyslik_kapacita_volna_covid_negativni;
            h.luzka_standard_kyslik_kapacita_volna_covid_pozitivni = lh.luzka_standard_kyslik_kapacita_volna_covid_pozitivni - fh.luzka_standard_kyslik_kapacita_volna_covid_pozitivni;

            h.upv_kapacita_celkem = lh.upv_kapacita_celkem - fh.upv_kapacita_celkem;
            h.upv_kapacita_volna = lh.upv_kapacita_volna - fh.upv_kapacita_volna;
            h.ventilatory_operacni_sal_kapacita_celkem = lh.ventilatory_operacni_sal_kapacita_celkem - fh.ventilatory_operacni_sal_kapacita_celkem;
            h.ventilatory_operacni_sal_kapacita_volna = lh.ventilatory_operacni_sal_kapacita_volna - fh.ventilatory_operacni_sal_kapacita_volna;
            h.ventilatory_prenosne_kapacita_celkem = lh.ventilatory_prenosne_kapacita_celkem - fh.ventilatory_prenosne_kapacita_celkem;
            h.ventilatory_prenosne_kapacita_volna = lh.ventilatory_prenosne_kapacita_volna - fh.ventilatory_prenosne_kapacita_volna;



            return h;
        }


        public static DateTime? ToDateTime(string value, params string[] formats)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            foreach (var f in formats)
            {
                var dt = ToDateTime(value, f);
                if (dt.HasValue)
                    return dt;
            }
            return null;
        }

        public static DateTime? ToDateTime(string value, string format)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            DateTime tmp;
            if (DateTime.TryParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out tmp))
                return new DateTime?(tmp);
            else
                return null;
        }

        static ElasticClient _client = null;
        static object _clientLockObj = new object();
        public static ElasticClient Client()
        {
            if (_client == null)
            {
                lock (_clientLockObj)
                {
                    if (_client == null)
                    {
                        ConnectionSettings sett = Lib.ES.Manager.GetElasticSearchConnectionSettings("data_kapacity-jedn-nemocnic");
                        _client = new Nest.ElasticClient(sett);
                    }
                }

            }
            return _client;

        }


    }
}
