using Devmasters.Core;
using HlidacStatu.Util.Cache;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            public const decimal MinAcceptableProbability = 0.5m;

            public SClassification() { }

            public SClassification(Classification[] types)
            {
                this.LastUpdate = DateTime.Now;
                this.Types = types;
            }
            


            public class Classification
            {
                [Nest.Number]
                public int TypeValue { get; set; }
                [Nest.Number]
                public decimal ClassifProbability { get; set; }

                public ClassificationsTypes ClassifType() { return (ClassificationsTypes)TypeValue; }

                public string ClassifTypeName()
                {
                    return ClassifTypeName(this.TypeValue);
                }

                public static string ClassifTypeName(int value)
                {
                    ClassificationsTypes t;
                    if (Enum.TryParse(value.ToString(), out t))
                    { 
                        if (Devmasters.Core.TextUtil.IsNumeric(t.ToString()))
                        {
                            Util.Consts.Logger.Warning("Missing Classification value" + value);
                            return "(neznámý)";
                        }
                        return t.ToNiceDisplayName();
                    }
                    else
                    {
                        Util.Consts.Logger.Warning("Missing Classification value" + value);
                        return "(neznámý)";
                    }
                }
                public static ClassificationsTypes? ToClassifType(int value)
                {
                    ClassificationsTypes t;
                    if (Enum.TryParse(value.ToString(), out t))
                    {
                        if (Devmasters.Core.TextUtil.IsNumeric(t.ToString()))
                            return null;
                        else
                            return t;
                    }
                    else
                        return null;
                }
                public static ClassificationsTypes? ToClassifType(string value)
                {
                    ClassificationsTypes t;
                    if (Enum.TryParse(value, out t))
                    {
                        if (Devmasters.Core.TextUtil.IsNumeric(t.ToString()))
                            return null;
                        else
                            return t;
                    }
                    else
                        return null;
                }

                public string ClassifSearchQuery()
                {
                    return ClassifSearchQuery(ClassifType());
                }
                public static string ClassifSearchQuery(ClassificationsTypes t)
                {
                    var val = t.ToString();
                    if (val.EndsWith("_obecne"))
                        val = val.Replace("_obecne", "");
                    return val;
                }

                public static string GetSearchUrl(ClassificationsTypes t, bool local = true)
                {
                    string url = "/HledatSmlouvy?Q=oblast:" + ClassifSearchQuery(t);
                    if (local == false)
                        return "https://www.hlidacstatu.cz" + url;
                    else
                        return url;
                }


            }
            [ShowNiceDisplayName()]
            public enum ClassificationsTypes
            {


                [NiceDisplayName("Ostatní")]
                OSTATNI = 0,

                [NiceDisplayName("IT")]
                it_obecne = 10000,

                [NiceDisplayName("IT Hardware")]
                it_hw = 10001,
                [NiceDisplayName("IT Software")]
                it_sw = 10002,
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
                stav_obecne = 10100,
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
                doprava_obecne = 10200,
                [NiceDisplayName("Osobní vozidla")]
                doprava_osobni = 10201,
                [NiceDisplayName("Specializovaná vozidla")]
                doprava_special = 10202,
                [NiceDisplayName("Motorová vozidla pro přepravu více než 10 lidí")]
                doprava_lidi = 10203,
                [NiceDisplayName("Nakladní vozy")]
                doprava_nakladni = 10204,
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
                stroje_obecne = 10300,
                [NiceDisplayName("Elektricke stroje")]
                stroje_elektricke = 10301,
                [NiceDisplayName("Laboratorní přístroje a zařízení")]
                stroje_laborator = 10302,
                [NiceDisplayName("Průmyslové stroje")]
                stroje_prumysl = 10303,

                [NiceDisplayName("Telco")]
                telco_obecne = 10400,
                [NiceDisplayName("TV")]
                telco_tv = 10401,
                [NiceDisplayName("Sítě a přenos dat")]
                telco_site = 10402,
                [NiceDisplayName("Telekomunikační služby")]
                telco_sluzby = 10403,

                [NiceDisplayName("Zdravotnictví")]
                zdrav_obecne = 10500,
                [NiceDisplayName("Zdravotnické přístroje")]
                zdrav_pristroje = 10501,
                [NiceDisplayName("Leciva")]
                zdrav_leciva = 10502,
                [NiceDisplayName("Kosmetika")]
                zdrav_kosmetika = 10503,
                [NiceDisplayName("Opravy a údržba zdravotnických přístrojů")]
                zdrav_opravy = 10504,
                [NiceDisplayName("Zdravotnický materiál")]
                zdrav_material = 10505,
                [NiceDisplayName("Zdravotnický hygienický materiál")]
                zdrav_hygiena = 10506,

                [NiceDisplayName("Voda a potraviny")]
                jidlo_obecne = 10600,
                [NiceDisplayName("Potraviny")]
                jidlo_potrava = 10601,
                [NiceDisplayName("Pitná voda, nápoje, tabák atd.")]
                jidlo_voda = 10602,

                [NiceDisplayName("Bezpečnostní a ochranné vybavení a údržba")]
                bezpecnost_obecne = 10700,
                [NiceDisplayName("Kamerové systémy")]
                bezpecnost_kamery = 10701,
                [NiceDisplayName("Hasičské vybavení, požární ochrana")] 
                bezpecnost_hasici = 10702,
                [NiceDisplayName("Zbraně")]
                bezpecnost_zbrane = 10703,
                [NiceDisplayName("Ostraha objektů")]
                bezpecnost_ostraha = 10704,

                //bezpecnost;bezpecnost-generic;Bezpečnost generická;35;506;5155;519
                //bezpecnost; bezpecnost; Bezpečnostní a ochranné vybavení a údržba;35;506;5155;519

                //


                [NiceDisplayName("Přírodní zdroje")]
                prirodnizdroj_obecne = 10800,
                [NiceDisplayName("Písky a jíly")]
                prirodnizdroj_pisky = 10801,
                [NiceDisplayName("Chemické výrobky")]
                prirodnizdroj_chemie = 10802,
                [NiceDisplayName("Jiné přírodní zdroje")]
                prirodnizdroj_vse = 10803,

                [NiceDisplayName("Energie")]
                energie_obecne = 10900,
                [NiceDisplayName("Paliva a oleje")]
                energie_paliva = 10901,
                [NiceDisplayName("Elektricka energie")]
                energie_elektrina = 10902,
                [NiceDisplayName("Jiná energie")]
                energie_jina = 10903,
                [NiceDisplayName("Veřejné služby pro energie")]
                energie_sluzby = 10904,

                [NiceDisplayName("Zemědělství")] //mozna 
                agro_obecne = 11000,
                [NiceDisplayName("Lesnictví")]
                agro_les = 11001,
                [NiceDisplayName("Těžba dřeva")]
                agro_tezba = 11002,
                [NiceDisplayName("Zahradnické služby")]
                agro_zahrada = 11003,

                [NiceDisplayName("Kancelář")]
                kancelar_obecne = 11100,
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

                [NiceDisplayName("Řemesla")]
                remeslo_obecne = 11200,
                [NiceDisplayName("Oděvy")]
                remeslo_odevy = 11201,
                [NiceDisplayName("Textilie")]
                remeslo_textil = 11202,
                [NiceDisplayName("Hudební nástroje")]
                remeslo_hudba = 11203,
                [NiceDisplayName("Sport a sportoviště")]
                remeslo_sport = 11204,

                [NiceDisplayName("Sociální služby")]
                social_obecne = 11300,
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


                [NiceDisplayName("Finance")]
                finance_obecne = 11400,
                [NiceDisplayName("Finanční a pojišťovací služby")]
                finance_sluzby = 11401,
                [NiceDisplayName("Účetní, revizní a peněžní služby")]
                finance_ucetni = 11402,
                [NiceDisplayName("Podnikatelské a manažerské poradenství a související služby")]
                finance_poradenstvi = 11403,
                [NiceDisplayName("Dotace")]
                finance_dotace = 11404,

                [NiceDisplayName("Právní a realitní služby")]
                legal_obecne = 11500,
                [NiceDisplayName("Realitní služby")]
                legal_reality = 11501,
                [NiceDisplayName("Právní služby")]
                legal_pravni = 11502,
                [NiceDisplayName("Nájemní smlouvy")]
                legal_najem = 11503,
                [NiceDisplayName("Pronájem pozemků")]
                legal_pozemky = 11504,


                [NiceDisplayName("Kanalizace a odpady")]
                techsluzby_obecne = 11600,
                [NiceDisplayName("Odpady")]
                techsluzby_odpady = 11601,
                [NiceDisplayName("Čistící a hygienické služby")]
                techsluzby_cisteni = 11602,
                [NiceDisplayName("Úklidové služby")]
                techsluzby_uklid = 11603,


                [NiceDisplayName("Výzkum a vývoj a související služby")]
                vyzkum_obecne = 11700,
                [NiceDisplayName("Výzkum a vývoj")]
                vyzkum_vyvoj = 11701,

                [NiceDisplayName("Reklamní a marketingové služby")]
                marketing_obecne = 11800,
                [NiceDisplayName("Marketing a reklama")]
                marketing_reklama = 11801,

                [NiceDisplayName("Jiné služby")]
                jine_obecne = 11900,
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




            [Nest.Date]
            public DateTime? LastUpdate { get; set; } = null;

            [Nest.Number]
            public int Version { get; set; } = 1;

            public Classification[] Types { get; set; } = null;

            public override string ToString()
            {
                if (this.Types != null)
                {
                    return $"Types:{Types.Select(m => m.ClassifType().ToString() + " (" + m.ClassifProbability.ToString("P2") + ")").Aggregate((f, s) => f + "; " + s)}"
                        + $" updated:{LastUpdate.ToString()}";
                }
                else
                {
                    return $"Types:snull updated:{LastUpdate.ToString()}";
                }
                //return base.ToString();
            }



            private static string classificationBaseUrl()
            {
                string[] baseUrl = Devmasters.Core.Util.Config.GetConfigValue("Classification.Service.Url")
                    .Split(',',';');
                //Dictionary<string, DateTime> liveEndpoints = new Dictionary<string, DateTime>();

                return baseUrl[Util.Consts.Rnd.Next(baseUrl.Length)];
            
            }

            private static volatile FileCacheManager stemCacheManager
                = FileCacheManager.GetSafeInstance("SmlouvyStems",
                    smlouvaKeyId => getRawStemsFromServer(smlouvaKeyId),
                    TimeSpan.FromDays(365*10)); //10 years

            private static byte[] getRawStemsFromServer(KeyAndId smlouvaKeyId)
            {
                Smlouva s = Smlouva.Load(smlouvaKeyId.ValueForData);

                if (s == null)
                    return null;

                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new HlidacStatu.Util.FirstCaseLowercaseContractResolver();


                using (Devmasters.Net.Web.URLContent stem = new Devmasters.Net.Web.URLContent(classificationBaseUrl() + "/stemmer"))
                {
                    stem.Method = Devmasters.Net.Web.MethodEnum.POST;
                    stem.Tries = 3;
                    stem.TimeInMsBetweenTries = 5000;
                    stem.Timeout = 1000*60*30; //30min
                    stem.ContentType = "application/json; charset=utf-8";
                    stem.RequestParams.RawContent = Newtonsoft.Json.JsonConvert.SerializeObject(s, settings);
                    Devmasters.Net.Web.BinaryContentResult stems = null;
                    try
                    {
                        Util.Consts.Logger.Debug("Getting stems from " + stem.Url);

                        stems = stem.GetBinary();
                        return stems.Binary;
                    }
                    catch (Exception e)
                    {
                        Util.Consts.Logger.Error("Classification Stemmer API error " + stem.Url, e);
                        throw;
                    }
                }
            }

            public static string GetRawStems(Smlouva s, bool rewriteStems = false)
            {
                if (s == null)
                    return null;
                var key = new KeyAndId() { ValueForData = s.Id, CacheNameOnDisk = $"stem_smlouva_{s.Id}" };
                if (rewriteStems)
                    stemCacheManager.Delete(key);
                var data = stemCacheManager.Get(key);
                if (data == null)
                    return null;

                return System.Text.Encoding.UTF8.GetString(data);

            }

            public static Dictionary<Lib.Data.Smlouva.SClassification.ClassificationsTypes, decimal> GetClassificationFromServer(Smlouva s, bool rewriteStems = false)
            {
                Dictionary<Lib.Data.Smlouva.SClassification.ClassificationsTypes, decimal> data = new Dictionary<Smlouva.SClassification.ClassificationsTypes, decimal>();
                if (s.Prilohy.All(m => m.EnoughExtractedText) == false)
                    return null;

                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new HlidacStatu.Util.FirstCaseLowercaseContractResolver();

                var stems = GetRawStems(s, rewriteStems);
                if (string.IsNullOrEmpty(stems))
                {
                    return data;
                }
                    

                using (Devmasters.Net.Web.URLContent classif = new Devmasters.Net.Web.URLContent(classificationBaseUrl() + "/classifier"))
                {
                    classif.Method = Devmasters.Net.Web.MethodEnum.POST;
                    classif.Tries = 3;
                    classif.TimeInMsBetweenTries = 5000;
                    classif.Timeout = 180000;
                    classif.ContentType = "application/json; charset=utf-8";
                    classif.RequestParams.RawContent = stems;
                    Devmasters.Net.Web.TextContentResult classifier = null;
                    try
                    {
                        Util.Consts.Logger.Debug("Getting classification from " + classif.Url);
                        classifier = classif.GetContent();
                    }
                    catch (Exception e)
                    {
                        Util.Consts.Logger.Error("Classification Classifier API " + classif.Url, e);
                        throw;
                    }

                    using (Devmasters.Net.Web.URLContent fin = new Devmasters.Net.Web.URLContent(classificationBaseUrl() + "/finalizer"))
                    {
                        fin.Method = Devmasters.Net.Web.MethodEnum.POST;
                        fin.Tries = 3;
                        fin.TimeInMsBetweenTries = 5000;
                        fin.Timeout = 30000;
                        fin.ContentType = "application/json; charset=utf-8";
                        fin.RequestParams.RawContent = classifier.Text;
                        Devmasters.Net.Web.TextContentResult res = null;
                        try
                        {
                            Util.Consts.Logger.Debug("Getting classification finalizer from " + classif.Url);
                            res = fin.GetContent();
                        }
                        catch (Exception e)
                        {
                            Util.Consts.Logger.Error("Classification finalizer API " + classif.Url, e);
                            throw;
                        }


                        var jsonData = Newtonsoft.Json.Linq.JObject.Parse(res.Text);

                        foreach (JProperty item in jsonData.Children())
                        {

                            string key = item.Name.Replace("-", "_").Replace("_generic","_obecne");// jsonData[i][0].Value<string>().Replace("-", "_");
                            decimal prob = HlidacStatu.Util.ParseTools.ToDecimal(item.Value.ToString()) ?? 0;
                            if (Enum.TryParse<Smlouva.SClassification.ClassificationsTypes>(key, out var typ))
                            {
                                if (!data.ContainsKey(typ))
                                    data.Add(typ, prob);
                                else if (typ == SClassification.ClassificationsTypes.OSTATNI)
                                    Util.Consts.Logger.Warning($"Classification type lookup failure : { key }");


                            }
                            else
                            {
                                Util.Consts.Logger.Warning("Classification type lookup failure - Invalid key " + key);
                                data.Add(Smlouva.SClassification.ClassificationsTypes.OSTATNI, prob);
                            }
                        }

                    }

                }
                return data;
            }



        }
    }
}
