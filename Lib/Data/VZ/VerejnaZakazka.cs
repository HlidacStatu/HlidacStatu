using Devmasters.Core;
using HlidacStatu.Util;
using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace HlidacStatu.Lib.Data.VZ

{
    [Description("Struktura verejne zakazky v Hlidaci Statu")]
    public partial class VerejnaZakazka
        : Bookmark.IBookmarkable, ISocialInfo
    {
        public const string Pre2016Dataset = "VVZ-2006";
        public const string Post2016Dataset = "VVZ-2016";
        public const string ProfileOnlyDataset = "VVZ-Profil";


        [Description("Formuláře spojené s touto zakázkou. Vychazi z XML na VVZ z www.isvz.cz/ISVZ/MetodickaPodpora/Napovedaopendata.pdf")]
        public class Formular
        {
            [Description("Číslo formuláře")]
            [Keyword]
            public string Cislo { get; set; } = string.Empty;

            [Description("Druh formuláře (F01-F31, CZ01-CZ02)")]
            [Keyword]
            public string Druh { get; set; } = string.Empty;

            [Keyword]
            [Description("Typ formuláře (řádný/opravný), nebo nevyplněno (pak je řádný)")]
            public string TypFormulare { get; set; } = string.Empty;

            [Keyword]
            [Description("Limit VZ (nadlimitní/podlimitní/VZMR)")]
            public string LimitVZ { get; set; } = string.Empty;

            [Keyword]
            [Description("Druh řízení dle ZVZ")]
            public string DruhRizeni { get; set; } = string.Empty;

            public VerejnaZakazka.DruhyFormularu DruhFormulare()
            {
                DruhyFormularu druh;
                if (Enum.TryParse<DruhyFormularu>(this.Druh, out druh))
                    return druh;
                else
                    return DruhyFormularu.Unknown;
            }
            public string DruhFormulareName()
            {
                if (DruhFormulare() == DruhyFormularu.Unknown)
                    return "";
                else
                    return DruhFormulare().ToNiceDisplayName().Trim();
            }

            //[Object(Ignore =true)]
            //public string P

            [Description("Datum zveřejnění.")]
            [Date]
            public DateTime? Zverejnen { get; set; }

            [Description("URL formuláře, může být prázdné")]
            [Keyword(Index = false)]
            public String URL { get; set; } = string.Empty;

            [Description("Je zakazka pouze na profilu zadavatele?")]
            [Boolean]
            public bool OnlyOnProfile { get; set; }

            //[Boolean]
            //public bool 
        }

        [Description("Je zakazka pouze na profilu zadavatele?")]
        [Boolean]
        public bool OnlyOnProfile { get; set; }

        [Description("Druh formuláře - mezinárodní (F*) i české (CZ*).")]
        [Devmasters.Core.ShowNiceDisplayName]
        public enum DruhyFormularu
        {
            [Devmasters.Core.NiceDisplayName("Předběžné oznámení")]
            F01,
            [Devmasters.Core.NiceDisplayName("Oznámení o zahájení zadávacího řízení")]
            F02,
            [Devmasters.Core.NiceDisplayName("Oznámení o výsledku zadávacího řízení")]
            F03,
            [Devmasters.Core.NiceDisplayName("Pravidelné předběžné oznámení")]
            F04,
            [Devmasters.Core.NiceDisplayName("Oznámení o zahájení zadávacího řízení veřejné služby")]
            F05,
            [Devmasters.Core.NiceDisplayName("Oznámení o výsledku zadávacího řízení veřejné služby")]
            F06,
            [Devmasters.Core.NiceDisplayName("Systém kvalifikace veřejné služby")]
            F07,
            [Devmasters.Core.NiceDisplayName("Oznámení na profilu kupujícího")]
            F08,
            [Devmasters.Core.NiceDisplayName("Oznámení o zahájení soutěže o návrh")]
            F12,
            [Devmasters.Core.NiceDisplayName("Oznámení o výsledcích soutěže o návrh")]
            F13,
            [Devmasters.Core.NiceDisplayName("Oprava ")]
            F14,
            [Devmasters.Core.NiceDisplayName("Oznámení o dobrovolné průhlednosti ex ante ")]
            F15,
            [Devmasters.Core.NiceDisplayName("Oznámení předběžných informací - obrana a bezpečnost ")]
            F16,
            [Devmasters.Core.NiceDisplayName("Oznámení o zakázce - obrana a bezpečnost")]
            F17,
            [Devmasters.Core.NiceDisplayName("Oznámení o zadání zakázky - obrana a bezpečnost")]
            F18,
            [Devmasters.Core.NiceDisplayName("Oznámení o subdodávce - obrana a bezpečnost")]
            F19,
            [Devmasters.Core.NiceDisplayName("Oznámení o změně ")]
            F20,
            [Devmasters.Core.NiceDisplayName("Sociální a jiné zvláštní služby – veřejné zakázky ")]
            F21,
            [Devmasters.Core.NiceDisplayName("Sociální a jiné zvláštní služby – veřejné služby")]
            F22,
            [Devmasters.Core.NiceDisplayName("Sociální a jiné zvláštní služby – koncese")]
            F23,
            [Devmasters.Core.NiceDisplayName("Oznámení o zahájení koncesního řízení ")]
            F24,
            [Devmasters.Core.NiceDisplayName("Oznámení o výsledku koncesního řízení")]
            F25,

            [Devmasters.Core.NiceDisplayName("Předběžné oznámení zadávacího řízení v podlimitním režimu")]
            CZ01,
            [Devmasters.Core.NiceDisplayName("Oznámení o zahájení zadávacího řízení v podlimitním režimu ")]
            CZ02,
            [Devmasters.Core.NiceDisplayName("Oznámení o výsledku zadávacího řízení v podlimitním režimu ")]
            CZ03,
            [Devmasters.Core.NiceDisplayName("Oprava národního formuláře ")]
            CZ04,
            [Devmasters.Core.NiceDisplayName("Oznámení profilu zadavatele")]
            CZ05,
            [Devmasters.Core.NiceDisplayName("Zrušení/zneaktivnění profilu zadavatele")]
            CZ06,
            [Devmasters.Core.NiceDisplayName("??")]
            CZ07,

            [Devmasters.Core.NiceDisplayName("Záznam na profilu zadavatele")]
            ProfilZadavatele,

            [Devmasters.Core.NiceDisplayName("Neznámý")]
            Unknown

        }

        [Description("Odvozené stavy zakázky pro Hlídače.")]
        [Devmasters.Core.ShowNiceDisplayName]
        public enum StavyZakazky
        {
            [Devmasters.Core.NiceDisplayName("Oznámen úmysl vyhlásit tendr")] Umysl = 1,
            [Devmasters.Core.NiceDisplayName("Tendr zahájen")] Zahajeno = 100,
            [Devmasters.Core.NiceDisplayName("Tendr se vyhodnocuje")] Vyhodnocovani = 200,
            [Devmasters.Core.NiceDisplayName("Tendr byl ukončen")] Ukonceno = 300,
            [Devmasters.Core.NiceDisplayName("Tendr byl zrušen")] Zruseno = 50,
            [Devmasters.Core.NiceDisplayName("Nejasný stav")] Jine = 0,
        }

        string _id = null;

        [Description("Unikátní ID zakázky. Nevyplňujte, ID vygeneruje Hlídač Státu.")]
        public string Id
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                    InitId();
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        [Keyword()]
        [Description("Evidenční číslo zakázky ve VVZ")]
        public string EvidencniCisloZakazky { get; set; } = null;

        [Description("ID profilu zadavatele, na kterém tato zakázka.")]
        [Keyword()]
        public string ZakazkaNaProfiluId { get; set; } = null;

        [Description("Všechny formuláře spojené se zakázkou")]
        public Formular[] Formulare { get; set; } = new Formular[] { };

        [Description("Hodnotící kritéria zakázky")]
        public HodnoticiKriteria[] Kriteria { get; set; } = new HodnoticiKriteria[] { };

        [Description("Nepouzito")]
        [Keyword()]
        public int DisplayId { get; set; }

        [Keyword()]
        [Description("Hodnota 'VVZ-2006' pro zakázky z VVZ od 2006-2016, 'VVZ-2016' pro nové dle zákona o VZ z 2016")]
        public string Dataset { get; set; } = "VVZ-2016";

        [Object()]
        [Description("Zadavatel")]
        public Subject Zadavatel { get; set; }

        [Object()]
        [Description("Dodavatele")]
        public Subject[] Dodavatele { get; set; } = new Subject[] { };

        [Description("Název zakázky")]
        public string NazevZakazky { get; set; }
        [Description("Popis zakázky")]
        public string PopisZakazky { get; set; }

        [Object()]
        [Description("Zadávací dokumentace a další dokumenty spojené se zakázkou")]
        public Document[] Dokumenty { get; set; } = new Document[] { };


        [Keyword()]
        [Description("CPV kódy určující oblast VZ")]
        public string[] CPV { get; set; } = new string[] { };

        [Date()]
        [Description("Datum uveřejnění")]
        public DateTime? DatumUverejneni { get; set; } = null;

        [Date()]
        [Description("Lhůta pro doručení nabídek")]
        public DateTime? LhutaDoruceni { get; set; } = null;

        [Date()]
        [Description("Datum uzavření smlouvy u ukončené zakázky")]
        public DateTime? DatumUzavreniSmlouvy { get; set; } = null;


        [Date()]
        [Description("Poslední změna VZ podle poslední změny formuláře.")]
        public DateTime? PosledniZmena { get; set; } = null;


        [Description("Číselná hodnota odvozeného stavu zakázky z enumerace 'StavyZakazky'.")]
        public int StavVZ { get; set; } = 0;
        [Object(Ignore = true)]
        public StavyZakazky StavZakazky
        {
            get
            {
                return (StavyZakazky)this.StavVZ;
            }
            set
            {
                this.StavVZ = (int)value;
            }
        }

        //return true if changed
        public bool SetStavZakazky()
        {
            var stav = CalculateStavVZ();
            if (stav.HasValue == false)
                return false;
            if (stav.Value != StavZakazky)
            {
                this.StavZakazky = stav.Value;
                return true;
            }
            else
                return false;
        }


        //podle http://portal-vz.cz/getmedia/0d646e5f-d960-4743-b5b3-e885dcab7b1c/Metodika-k-vyhlasce-o-uverejnovani-a-profilu-zadavatele_v4-bez-registrace_duben-2017.pdf
        //Zahajeny
        static string[,] umysl = new string[,] {
                { "F01","" },
                { "CZ01","" },
                { "F04","Toto oznámení je pouze pravidelným předběžným oznámením" },
                { "F16","" } // v oblasti obrany nebo bezpečnosti z
            };


        static string[,] Zahajeny = new string[,] {
                { "F02","" },
                { "CZ02","" },
                { "F05","" },
                { "F07","" },
                { "F12","" },
                { "F15","" },//dobrovolné oznámení o záměru uzavřít smlouvu v případě veřejné zakázky v nadlimitním režimu
                { "F24","" },
                { "F17","" },
            };

        //vyhodnocovani - je v Zahajeny, ale urcim to podle casu odevzdani nabidek


        //ukonceni
        static string[,] ukonceni = new string[,] {
                { "F03","" },
                { "CZ03","" },
                { "F06","" },
                { "F13","" },
                { "F18","" },
                { "F19","" },
                { "F25","" },
            };

        //jine - neda se zatim urcit
        static string[,] jine = new string[,] {
                { "F21","" },
                { "F22","" },
                { "F23","" },
            };
        public StavyZakazky? CalculateStavVZ()
        {

            if (HasForm(jine))
                if (this.LhutaDoruceni.HasValue && this.LhutaDoruceni.Value < DateTime.Now)
                    return StavyZakazky.Vyhodnocovani;
                else
                    return StavyZakazky.Jine;

            if (HasForm(ukonceni))
                return StavyZakazky.Ukonceno;

            if (HasForm(Zahajeny))
            {
                //vyhodnocovani - je v Zahajeny, ale urcim to podle casu odevzdani nabidek
                if (this.LhutaDoruceni.HasValue && this.LhutaDoruceni.Value < DateTime.Now)
                    return StavyZakazky.Vyhodnocovani;
                else
                    return StavyZakazky.Zahajeno;
            }

            if (HasForm(umysl))
                if (this.LhutaDoruceni.HasValue && this.LhutaDoruceni.Value < DateTime.Now)
                    return StavyZakazky.Vyhodnocovani;
                else
                    return StavyZakazky.Umysl;



            return null;
        }
        private bool HasForm(string[,] forms)
        {
            for (int i = 0; i < forms.GetLength(0); i++)
            {
                foreach (var f in this.Formulare.OrderBy(o => o.Zverejnen))
                {
                    if (f.Druh.ToUpper() == forms[i, 0])
                        return true;
                }
            }
            return false;
        }

        public void UpdatePosledniZmena(bool force = false, bool save = false)
        {
            DateTime? prevVal = this.PosledniZmena;
            bool changed = false;
            if (this.PosledniZmena.HasValue && force == false)
                return;
            else if (this.LastestFormular() != null)
            {
                this.PosledniZmena = this.LastestFormular().Zverejnen;
            }
            else if (this.DatumUverejneni.HasValue)
            {
                this.PosledniZmena = DatumUverejneni;
            }
            else
            {
                return;
            }
            if (this.PosledniZmena != prevVal && save)
                this.Save();
        }

        public string FormattedCena(bool html)
        {
            if (this.KonecnaHodnotaBezDPH.HasValue)
            {
                return HlidacStatu.Util.RenderData.NicePrice(this.KonecnaHodnotaBezDPH.Value, html: html);
            }
            else if (this.OdhadovanaHodnotaBezDPH.HasValue)
            {
                return HlidacStatu.Util.RenderData.NicePrice(this.OdhadovanaHodnotaBezDPH.Value, html: html);
            }
            else
                return String.Empty;
        }

        public DateTime? GetPosledniZmena()
        {
            if (this.LastestFormular() != null)
            {
                return this.LastestFormular().Zverejnen;
            }
            else if (this.DatumUverejneni.HasValue)
            {
                return DatumUverejneni;
            }
            else
            {
                return null;
            }

        }


        [Description("")]
        public DateTime? LastUpdated { get; set; } = DateTime.MinValue;

        [Description("Odhadovaná hodnota zakázky bez DPH.")]
        [Number]
        public decimal? OdhadovanaHodnotaBezDPH { get; set; } = null;
        [Description("Konečná (vysoutěžená) hodnota zakázky bez DPH.")]
        [Number]
        public decimal? KonecnaHodnotaBezDPH { get; set; } = null;

        [Description("Měna odhadovaná hodnoty zakázky.")]
        [Keyword]
        public string OdhadovanaHodnotaMena { get; set; }
        [Description("Měna konečné hodnoty zakázky.")]
        [Keyword]
        public string KonecnaHodnotaMena { get; set; }

        [Description("Dokumenty příslušející zakázky (typicky zadávací a smluvní dokumentace)")]
        public class Document
        {
            [Description("URL uvedené v profilu zadavatele")]
            [Keyword]
            public string OficialUrl { get; set; }
            [Description("Přímé URL na tento dokument.")]
            [Keyword]
            public string DirectUrl { get; set; }
            [Description("Popis obsahu dokumentu, z XML na profilu z pole dokument/typ_dokumentu.")]
            [Keyword]
            public string TypDokumentu { get; set; }

            [Description("Datum vložení, uveřejnění dokumentu.")]
            [Date]
            public DateTime? VlozenoNaProfil { get; set; }
            [Description("Číslo verze")]
            [Keyword]
            public string CisloVerze { get; set; }

            [Description("Neuvádět, obsah dokumentu v plain textu pro ftx vyhledávání")]
            [Text()]
            public string PlainText { get; set; }
            [Description("Neuvádět.")]
            public DataQualityEnum PlainTextContentQuality { get; set; } = DataQualityEnum.Unknown;

            [Description("Neuvádět")]
            [Date]
            public DateTime LastUpdate { get; set; } = DateTime.MinValue;

            [Description("Neuvádět")]
            [Date]
            public DateTime LastProcessed { get; set; } = DateTime.MinValue;

            [Description("Neuvádět")]
            [Keyword()]
            public string ContentType { get; set; }
            [Description("Neuvádět")]
            public int Lenght { get; set; }
            [Description("Neuvádět")]
            public int WordCount { get; set; }
            [Description("Neuvádět")]
            public int Pages { get; set; }

            [Keyword()]
            public string StorageId { get; set; }

            [Keyword()]
            public string PlainDocumentId { get; set; }

            public string GetDocumentUrlToDownload()
            {
                return $"http://bpapi.datlab.eu/document/{this.StorageId}";
            }

            [Keyword()]
            public string Name { get; set; }

            [Object(Ignore = true)]
            public bool EnoughExtractedText
            {
                get
                {
                    return !(this.Lenght <= 20 || this.WordCount <= 10);
                }
            }

            public void CalculateDocStats()
            {
                this.Lenght = this.PlainText.Length;
                this.WordCount = HlidacStatu.Util.ParseTools.CountWords(this.PlainText);
            }


        }

        [Description("Hodnotící kritéria veřejné zakázky")]
        public class HodnoticiKriteria
        {
            [Description("Pořadí.")]
            public int Poradi { get; set; }
            [Keyword]
            [Description("Popis kritéria")]
            public string Kriterium { get; set; }
            [Text]
            [Description("Název kritéria")]
            public string Nazev { get; set; }

            [Number]
            [Description("Váha v hodnocení")]
            public decimal Vaha { get; set; } = 0;

        }

        public class Subject
        {
            [Keyword()]
            [Description("IC subjektu")]
            public string ICO { get; set; }
            [Keyword()]
            [Description("Obchodní jméno")]
            public string Jmeno { get; set; }
        }


        [Text(Index = false)]
        [Description("HTML stránky zakázky, pokud bylo parsováno z HTML")]
        public string RawHtml { get; set; }

        public string CPVText(string cpv)
        {
            return VerejnaZakazka.CPVToText(cpv);
        }
        public static string CPVToText(string cpv)
        {
            if (string.IsNullOrEmpty(cpv))
                return "";
            if (StaticData.CPVKody.ContainsKey(cpv))
                return StaticData.CPVKody[cpv];

            if (cpv.Contains("-"))
            {
                int nalez = cpv.IndexOf("-");
                cpv = cpv.Substring(0, nalez);
            }
            var key = StaticData.CPVKody.Keys.FirstOrDefault(m => m.StartsWith(cpv));
            if (key != null)
                return StaticData.CPVKody[key];
            else
                return cpv;

        }

        public void InitId()
        {
            if (string.IsNullOrEmpty(Dataset) || string.IsNullOrEmpty(EvidencniCisloZakazky))
                throw new NullReferenceException();
            this.Id = Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(Dataset + "|" + EvidencniCisloZakazky);
        }

        public string GetUrl(bool local = true)
        {
            return GetUrl(local, string.Empty);
        }

        public string GetUrl(bool local, string foundWithQuery)
        {
            string url = "/verejnezakazky/zakazka/" + this.Id;// E49DE92B876B0C66C2F29457EB61C7B7

            if (!string.IsNullOrEmpty(foundWithQuery))
                url = url + "?qs=" + System.Net.WebUtility.UrlEncode(foundWithQuery);

            if (local == false)
                return "https://www.hlidacstatu.cz" + url;
            else
                return url;
        }

        public Formular LastestFormular()
        {
            if (this.Formulare != null)
                return Formulare.OrderByDescending(m => m.Zverejnen).FirstOrDefault();
            else
                return null;
        }

        public void Save(ElasticClient client = null, DateTime? posledniZmena = null)
        {
            try
            {
                if (string.IsNullOrEmpty(this.Id))
                    InitId();
                SetStavZakazky();
                this.LastUpdated = DateTime.Now;
                if (posledniZmena.HasValue)
                    this.PosledniZmena = posledniZmena;
                else
                    this.PosledniZmena = GetPosledniZmena();
                var es = client ?? ES.Manager.GetESClient_VZ();
                es.IndexDocument<VerejnaZakazka>(this);

            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error($"VZ ERROR Save ID:{this.Id} Size:{Newtonsoft.Json.JsonConvert.SerializeObject(this).Length}", e);
            }
        }

        public static VerejnaZakazka LoadFromES(string id, ElasticClient client = null)
        {
            var es = client ?? ES.Manager.GetESClient_VZ();
            var res = es.Get<VerejnaZakazka>(id);
            if (res.Found)
                return res.Source;
            else
                return null;
        }

        public static bool Exists(VerejnaZakazka vz, ElasticClient client = null)
        {
            return Exists(vz.Id, client);
        }
        public static bool Exists(string id, ElasticClient client = null)
        {
            var es = client ?? ES.Manager.GetESClient_VZ();
            var res = es.DocumentExists<VerejnaZakazka>(id);
            return res.Exists;
        }



        static string cisloZakazkyRegex = @"Evidenční \s* číslo \s* zakázky: \s*(?<zakazka>(Z?)\d{1,8}([-]*\d{1,7})?)\s+";
        static string cisloConnectedFormRegex = @"Evidenční \s* číslo \s* souvisejícího \s* formuláře: \s*(?<zakazka>(F?)\d{1,8}([-]*\d{1,7})?)\s+";
        static string cisloFormRegex = @"Evidenční \s* číslo \s* formuláře: \s*(?<zakazka>(F?)\d{1,8}([-]*\d{1,7})?)\s+";
        static string nazevZakazkyRegex = @"Název \s* zakázky: \s*(?<nazev>.*)\s+<div";
        static string zadavatelNazev = @"Název \s* zadavatele: \s*(?<zadavatel>.*)\s+<div";
        static string zadavatelICO = @"IČO \s* zadavatele: \s*(?<zadavatel>\d{2,8})\s+<div";

        static string datumUverejneniRegex = @"Datum\s*uveřejnění\s*ve\s*VVZ:\s*(?<datum>[0-9.: ]*)\s*";

        static string[] formsToSkip = new string[] { "F07", "F08", "CZ05", "CZ06" };



        public static T GetElemVal<T>(XDocument xd, string name)
        {

            if (typeof(T) == typeof(string))
            {
                return (T)HlidacStatu.Util.ParseTools.ChangeType(xd.Root.Element(name)?.Value, typeof(T));
            }
            else if (typeof(T) == typeof(decimal?))
            {
                return (T)HlidacStatu.Util.ParseTools.ChangeType(HlidacStatu.Util.ParseTools.ToDecimal(xd.Root.Element(name)?.Value), typeof(T));
            }
            else if (typeof(T) == typeof(int?))
            {
                if (xd.Root.Element(name) == null)
                    return default(T);
                else
                    return (T)HlidacStatu.Util.ParseTools.ChangeType((int?)HlidacStatu.Util.ParseTools.ToDecimal(xd.Root.Element(name)?.Value), typeof(T));
            }
            else if (typeof(T) == typeof(DateTime?))
            {
                if (xd.Root.Element(name) == null)
                    return default(T);
                else
                    return (T)HlidacStatu.Util.ParseTools.ChangeType(HlidacStatu.Util.ParseTools.ToDateTime(xd.Root.Element(name)?.Value, "yyyy-MM-ddThh:mm:ss", "dd.MM.yyyy"), typeof(T));
            }
            else
                throw new NotImplementedException();


        }


        public static bool IsElem(XDocument xd, string name)
        {
            return string.IsNullOrEmpty(xd.Root.Element(name).Value) == false;
        }

        public static VerejnaZakazka FromHtml(string html, int displayId)
        {
            throw new NotImplementedException("not finished yet. See XML parser");
            /*
                        string zakazkaId = ParseTools.GetRegexGroupValue(html, cisloZakazkyRegex, "zakazka");
                        if (!string.IsNullOrEmpty(zakazkaId))
                        {
                            VerejnaZakazka vz = new VerejnaZakazka();
                            vz.DisplayId = displayId;
                            vz.EvidencniCisloZakazky = zakazkaId;
                            vz.Zadavatel = new Subject()
                            {
                                Jmeno = ParseTools.GetRegexGroupValue(html, zadavatelNazev, "zadavatel"),
                                ICO = ParseTools.GetRegexGroupValue(html, zadavatelICO, "zadavatel")
                            };
                            vz.NazevZakazky = ParseTools.GetRegexGroupValue(html, nazevZakazkyRegex, "nazev");
                            vz.RawHtml = html;

                            XPath doc = new XPath(html);

                            string form = doc.GetNodeText("//h1");
                            // 
                            var rForm = Regex.Match(form, @"(?<form>((f|cj)\d{2})) (\s* - \s*) (?<txt>.*)", regexOption);
                            //vz.Formulare = rForm.Groups["form"].Value;
                            //vz.FormularPopis = rForm.Groups["txt"].Value;

                            if (formsToSkip.Contains(vz.Formular))
                                return null;


                            DateTime tmpDate = DateTime.MinValue;
                            var m = Regex.Match(html, datumUverejneniRegex, regexOption);
                            if (m.Success)
                            {
                                var dt = m.Groups["datum"].Value.Trim();
                                if (!DateTime.TryParseExact(dt, "d. M. yyyy H:m:ss", HlidacStatu.Util.Consts.czCulture, System.Globalization.DateTimeStyles.AssumeLocal, out tmpDate))
                                {
                                    DateTime.TryParseExact(dt, "dd. MM. yyyy HH:mm:ss", HlidacStatu.Util.Consts.czCulture, System.Globalization.DateTimeStyles.AssumeLocal, out tmpDate);
                                }
                            }
                            vz.DatumUverejneni = tmpDate;


                            vz.PopisZakazky = doc.GetBestNodeText(
                                "//span[contains(text(),'II.1.4)')]/parent::div/following-sibling::div[1]/span[@class='spanRight']",
                                "//span[@class='spanRight' and contains(text(),'Stručný popis')]/parent::div/following-sibling::div[1]/span[@class='spanRight']",
                                "//span[@class='spanRight' and contains(text(),'Popis zakázky')]/parent::div/following-sibling::div[1]/span[@class='spanRight']"
                                );
                            List<string> cpvs = new List<string>();
                            var cpvNodes = doc.GetNodes("//span[contains(text(),'CPV')]/parent::div/following-sibling::div[1]/span[@class='spanRight']");
                            foreach (var n in cpvNodes)
                            {
                                if (System.Text.RegularExpressions.Regex.Match(n.InnerText.Trim(), "\\d{8}").Success)
                                    cpvs.Add(n.InnerText.Trim());
                            }

                            //cena
                            //f1,f2,f4,f5,f24 - II.1.5)Předpokládaná celková hodnota
                            //f1,f2,f4,f5,f24 - II.2.6)Předpokládaná hodnota
                            //f3,f6,f15,f25 - II.1.7)Konečná hodnota veřejné zakázky
                            //f3,f6,f15,f25 - V.2.4)Údaje o hodnotě zakázky/části
                            //cz01,cz02,cz03 - II.2.1)Celková předpokládaná hodnota
                            //cz01,cz02 - II.5.4)Předpokládaná hodnota
                            //cz03 - II.2.2)Konečná hodnota veřejné zakázky
                            //cz03 - IV.3.4)Údaje o hodnotě zakázky / části

                            switch (vz.Formular)
                            {
                                case "F1":
                                case "F2":
                                case "F4":
                                case "F5":
                                case "F24":

                                    var sPrice = doc.GetBestNodeText(
                                        "//span[contains(text(),'II.1.5)')]/parent::div/following-sibling::div[1]/span[@class='spanRight']",
                                        "//span[@class='spanRight' and contains(text(),')Předpokládaná celková hodnota')]/parent::div/following-sibling::div[1]/span[@class='spanRight']",
                                        "//span[@class='spanRight' and contains(text(),')Předpokládaná hodnota')]/parent::div/following-sibling::div[1]/span[@class='spanRight']"
                                        );


                                    break;
                                case "F3":
                                case "F6":
                                case "F15":
                                case "F25":
                                    break;
                                case "CZ01":
                                case "CZ02":
                                    break;
                                case "CZ03":
                                default:
                                    break;
                            }



                            return vz;
                        }
                        */
            return null;
        }

        public string ToAuditJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string ToAuditObjectTypeName()
        {
            return "Veřejná zakázka";
        }

        public string ToAuditObjectId()
        {
            return this.Id;
        }

        public string BookmarkName()
        {
            return this.NazevZakazky;
        }
        public bool NotInterestingToShow() { return false; }

        public string SocialInfoTitle()
        {
            return Devmasters.Core.TextUtil.ShortenText(this.NazevZakazky, 50);
        }

        public string SocialInfoSubTitle()
        {
            if ((this.CPV?.Length ?? 0) == 0)
            {
                return "";
            }
            else if (this.CPV?.Length == 1)
            {
                return $"{this.CPVText(this.CPV[0])} ({this.CPV[0]})";
            }
            else if (this.CPV?.Length == 2)
            {
                return $"{this.CPVText(this.CPV[0])} ({this.CPV[0]})"
                    + $"{this.CPVText(this.CPV[1])} ({this.CPV[1]})";
            }
            else
                return $"{this.CPVText(this.CPV[0])} ({this.CPV[0]})"
                    + $"{this.CPVText(this.CPV[1])} ({this.CPV[1]}) "
                    + Devmasters.Core.Lang.Plural.Get(this.CPV.Length - 2, "a další obor", "+ {0} obory", "+ {0} oborů")
                    ;

        }


        public string SocialInfoBody()
        {
            return "<ul>" +
    HlidacStatu.Util.InfoFact.RenderInfoFacts(this.InfoFacts(), 4, true, true, "", "<li>{0}</li>", true)
    + "</ul>";
        }


        public string SocialInfoFooter()
        {
            return this.DatumUverejneni.HasValue
                ? "Poslední změna " + this.DatumUverejneni.Value.ToShortDateString()
                : (this.DatumUzavreniSmlouvy.HasValue ? "Datum&nbsp;uzavření&nbsp;smlouvy " + this.DatumUzavreniSmlouvy.Value.ToShortDateString()
                        : "");
        }

        public string SocialInfoImageUrl()
        {
            return string.Empty;
        }

        InfoFact[] _infofacts = null;
        object lockInfoObj = new object();
        public InfoFact[] InfoFacts()
        {
            lock (lockInfoObj)
            {
                if (_infofacts == null)
                {

                    List<InfoFact> f = new List<InfoFact>();

                    string hlavni = $"Veřejná zakázka od <b>{Devmasters.Core.TextUtil.ShortenText(this.Zadavatel?.Jmeno, 60)}</b>"
                        + (this.DatumUverejneni.HasValue
                                ? " vyhlášena dne " + DatumUverejneni.Value.ToShortDateString()
                                : ". "
                                );
                    if (this.Dodavatele.Count() == 0)
                        hlavni += $"";
                    else if (this.Dodavatele.Count() == 1)
                        hlavni += $"Zakázku vyhrál <b>{this.Dodavatele[0].Jmeno}</b>. ";
                    else if (this.Dodavatele.Count() > 1)
                        hlavni += $"Zakázku vyhrál <b>{this.Dodavatele[0].Jmeno}</b> a další. ";
                    if (this.KonecnaHodnotaBezDPH.HasValue && this.KonecnaHodnotaBezDPH != 0)
                    {
                        hlavni += $"Konečná hodnota zakázky je <b>{HlidacStatu.Util.RenderData.NicePrice(this.KonecnaHodnotaBezDPH.Value, mena: this.KonecnaHodnotaMena ?? "Kč")}</b>.";
                    }
                    else if (this.OdhadovanaHodnotaBezDPH.HasValue && this.OdhadovanaHodnotaBezDPH != 0)
                    {
                        hlavni += $"Odhadovaná hodnota zakázky je <b>{HlidacStatu.Util.RenderData.NicePrice(this.OdhadovanaHodnotaBezDPH.Value, mena: this.OdhadovanaHodnotaMena ?? "Kč")}</b>.";
                    }

                    f.Add(new InfoFact(hlavni, InfoFact.ImportanceLevel.Summary));

                    //sponzori
                    foreach (var subj in this.Dodavatele.Union(new HlidacStatu.Lib.Data.VZ.VerejnaZakazka.Subject[] { this.Zadavatel }))
                    {
                        if (subj != null)
                        {
                            var firma = HlidacStatu.Lib.Data.Firmy.Get(subj.ICO);
                            if (firma.Valid && firma.IsSponzor() && firma.JsemSoukromaFirma())
                            {
                                f.Add(new InfoFact(
                                    $"{firma.Jmeno}: " +
                                    firma.Description(true, m => m.Type == (int)HlidacStatu.Lib.Data.FirmaEvent.Types.Sponzor, itemDelimeter: ", ", numOfRecords: 2)
                                    , InfoFact.ImportanceLevel.Medium)
                                    );
                            }
                        }
                    }


                    //politici
                    foreach (var ss in this.Dodavatele)
                    {
                        if (!string.IsNullOrEmpty(ss.ICO) && HlidacStatu.Lib.StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get().SoukromeFirmy.ContainsKey(ss.ICO))
                        {
                            var politici = StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get().SoukromeFirmy[ss.ICO];
                            if (politici.Count > 0)
                            {
                                var sPolitici = Osoby.GetById.Get(politici[0]).FullNameWithYear();
                                if (politici.Count == 2)
                                {
                                    sPolitici = sPolitici + " a " + Osoby.GetById.Get(politici[1]).FullNameWithYear();
                                }
                                else if (politici.Count == 3)
                                {
                                    sPolitici = sPolitici
                                        + ", "
                                        + Osoby.GetById.Get(politici[1]).FullNameWithYear()
                                        + " a "
                                        + Osoby.GetById.Get(politici[2]).FullNameWithYear();

                                }
                                else if (politici.Count > 3)
                                {
                                    sPolitici = sPolitici
                                        + ", "
                                        + Osoby.GetById.Get(politici[1]).FullNameWithYear()
                                        + ", "
                                        + Osoby.GetById.Get(politici[2]).FullNameWithYear()
                                        + " a další";

                                }
                                f.Add(new InfoFact($"V dodavateli {Firmy.GetJmeno(ss.ICO)} se "
                                    + Devmasters.Core.Lang.Plural.Get(politici.Count()
                                                                        , " angažuje jedna politicky angažovaná osoba - "
                                                                        , " angažují {0} politicky angažované osoby - "
                                                                        , " angažuje {0} politicky angažovaných osob - ")
                                    + sPolitici + "."
                                    , InfoFact.ImportanceLevel.Medium)
                                    );
                            }


                        }

                    }

                    _infofacts = f.OrderByDescending(o => o.Level).ToArray();
                }
            }
            return _infofacts;
        }

        public ZakazkaSource ZdrojZakazkyUrl()
        {

            //2006 https://old.vestnikverejnychzakazek.cz/cs/Searching/SearchContractNumber?cococode=847422
            //2016 https://www.vestnikverejnychzakazek.cz/SearchForm/SearchContract?contractNumber=




            string searchUrl = null;
            if (!string.IsNullOrEmpty(this.EvidencniCisloZakazky))
            {
                string profilUrl = "";
                if (this.Dataset == HlidacStatu.Lib.Data.VZ.VerejnaZakazka.Post2016Dataset)
                    profilUrl = ProfilZadavatele.GetById(this.ZakazkaNaProfiluId)?.Url?.Trim();
                else if (this.Dataset == HlidacStatu.Lib.Data.VZ.VerejnaZakazka.Pre2016Dataset)
                    profilUrl = ProfilZadavatele.GetById(this.ZakazkaNaProfiluId)?.Url?.Trim();
                else if (!this.Dataset.StartsWith("DatLab-"))
                    profilUrl = this.Dataset ?? ProfilZadavatele.GetById(this.ZakazkaNaProfiluId)?.Url?.Trim();
                if (System.Uri.TryCreate(profilUrl, UriKind.Absolute, out var profilUri))
                {
                    string googlQ = this.EvidencniCisloZakazky + " site:" + profilUri.Host;
                    searchUrl = $"https://www.google.cz/search?client=safari&rls=en&q={(System.Net.WebUtility.UrlEncode(googlQ))}&ie=UTF-8&oe=UTF-8";
                }
            }

            if (!string.IsNullOrEmpty(this.EvidencniCisloZakazky))
            {
                if (this.Dataset == HlidacStatu.Lib.Data.VZ.VerejnaZakazka.Post2016Dataset)
                    return
                        new ZakazkaSource()
                        {
                            ZakazkaURL = $"https://www.vestnikverejnychzakazek.cz/SearchForm/SearchContract?contractNumber={this.EvidencniCisloZakazky}",
                            ProfilZadavatelUrl = ProfilZadavatele.GetById(this.ZakazkaNaProfiluId)?.Url?.Trim(),
                            SearchZakazkaUrl = searchUrl

                        };
                else if (this.Dataset == HlidacStatu.Lib.Data.VZ.VerejnaZakazka.Pre2016Dataset)
                    return new ZakazkaSource()
                    {
                        ZakazkaURL = $"https://old.vestnikverejnychzakazek.cz/cs/Searching/SearchContractNumber?cococode={this.EvidencniCisloZakazky}",
                        ProfilZadavatelUrl = ProfilZadavatele.GetById(this.ZakazkaNaProfiluId)?.Url?.Trim(),
                        SearchZakazkaUrl = searchUrl
                    };
                else if (!this.Dataset.StartsWith("DatLab-"))
                {

                        return new ZakazkaSource()
                        {
                            SearchZakazkaUrl = searchUrl,
                            ProfilZadavatelUrl = this.Dataset ?? ProfilZadavatele.GetById(this.ZakazkaNaProfiluId)?.Url?.Trim()
                        };

                }
            }
            return null;
        }



        public class ZakazkaSource
        {
            public string ZakazkaURL { get; set; }
            public string ProfilZadavatelUrl { get; set; }
            public string SearchZakazkaUrl { get; set; }

        }
        public class ExportedVZ
        {
            public class SubjectExport 
            {
                public SubjectExport() { }

                public SubjectExport(Subject s)
                {
                    if (s != null)
                    {
                        this.ICO = s.ICO;
                        this.Jmeno = s.Jmeno;
                    }
                    if (!string.IsNullOrEmpty(s?.ICO))
                    {
                        var f = Firma.FromIco(s.ICO);
                        if (f != null && f.Valid)
                        {
                            this.KrajId = f.KrajId;
                            this.OkresId = f.OkresId;
                        }
                    }
                }


                public string ICO { get; set; }
                public string Jmeno { get; set; }
                public string KrajId { get; set; }
                public string OkresId { get; set; }
            }
            public string Id { get; set; }
            public string EvidencniCisloZakazky { get; set; }
            public SubjectExport Zadavatel { get; set; }
            public Subject[] Dodavatele { get; set; }
            public string NazevZakazky { get; set; }
            public string PopisZakazky { get; set; }
            public string[] CPV { get; set; }
            public DateTime? DatumUverejneni { get; set; }
            public DateTime? LhutaDoruceni { get; set; }
            public DateTime? DatumUzavreniSmlouvy { get; set; }
            public DateTime? PosledniZmena { get; set; }
            public StavyZakazky StavZakazky { get; set; }
            public decimal? OdhadovanaHodnotaBezDPH { get; set; } = null;
            public decimal? KonecnaHodnotaBezDPH { get; set; } = null;
            public string OdhadovanaHodnotaMena { get; set; }
            public string KonecnaHodnotaMena { get; set; }
            public string UrlProfiluZadavatele { get; set; }
            public ZakazkaSource ZdrojZakazky { get; set; }
        }
        public VerejnaZakazka Export(bool allData = false)
        {
            VerejnaZakazka vz = (VerejnaZakazka) this.MemberwiseClone();
            if (allData == false)
            {
                if (vz.Dokumenty != null)
                {
                    foreach (var vzd in vz.Dokumenty)
                    {
                        vzd.DirectUrl = "";
                        vzd.PlainDocumentId = "";
                        vzd.PlainText = "-- Tato data jsou dostupná pouze v komerční nebo speciální licenci. Kontaktujte nás. --";
                    }
                }
            }
            vz.RawHtml = "";
            return vz;
        }
    }
}
