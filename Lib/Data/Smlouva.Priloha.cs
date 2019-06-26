using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devmasters.Core;
using System.Text.RegularExpressions;
using HlidacStatu.Util;

namespace HlidacStatu.Lib.Data
{
    public partial class Smlouva
    {
        public class Priloha : XSD.tPrilohaOdkaz
        {
            public class KeyVal
            {
                [Keyword()]
                public string Key { get; set; }
                [Keyword()]
                public string Value { get; set; }
            }

            public class Classification
            {

                [Nest.Number]
                public int Version { get; set; } = 0;

                [ShowNiceDisplayName()]
                public enum ContractDocTypes
                {

                    Dodatek,
                    Smlouva,
                    [NiceDisplayName("Objednávka")]
                    Objednavka,
                    Faktura,
                    [NiceDisplayName("Ostatní")]
                    Ostatni,
                };

                [ShowNiceDisplayName()]
                public enum ContractTypes
                {
                    [NiceDisplayName("O smlouvě budoucí")]
                    BUDOU,

                    [NiceDisplayName("O postoupení pohledávky")]
                    POST_POHL,

                    [NiceDisplayName("O převzetí dluhu")]
                    DLUH,

                    [NiceDisplayName("O postoupení smlouvy")]
                    POST_SML,

                    [NiceDisplayName("Ručení")]
                    RUC,

                    [NiceDisplayName("Finanční záruka")]
                    FIN_ZAR,

                    [NiceDisplayName("Darování")]
                    DAR,

                    [NiceDisplayName("Koupě")]
                    KOUP,

                    [NiceDisplayName("Směna")]
                    SMEN,

                    [NiceDisplayName("Výprosa")]
                    VYPR,

                    [NiceDisplayName("Výpůjčka")]
                    VYPUJ,

                    [NiceDisplayName("Nájem")]
                    NAJ,

                    [NiceDisplayName("Pacht")]
                    PACHT,

                    [NiceDisplayName("Licence")]
                    LIC,

                    [NiceDisplayName("Zápůjčka")]
                    ZAPUJ,

                    [NiceDisplayName("Úvěr")]
                    UVER,

                    [NiceDisplayName("Úschova")]
                    USCHOV,

                    [NiceDisplayName("Skladování")]
                    SKLAD,

                    [NiceDisplayName("Příkaz")]
                    PRIK,

                    [NiceDisplayName("Zprostředkování")]
                    ZPROSTRED,

                    [NiceDisplayName("Komise")]
                    KOMIS,

                    [NiceDisplayName("Zasílatelství")]
                    ZASIL,

                    [NiceDisplayName("Obchodní zastoupení")]
                    OBCH_ZAST,

                    [NiceDisplayName("Zájezd")]
                    ZAJEZD,

                    [NiceDisplayName("O přepravě")]
                    PREPRA,

                    [NiceDisplayName("Dílo")]
                    DILO,

                    [NiceDisplayName("Péče o zdraví")]
                    ZDRAV,

                    [NiceDisplayName("Kontrolní činnost")]
                    KONTROL,

                    [NiceDisplayName("Účet")]
                    UCET,

                    [NiceDisplayName("Jednorázový vklad")]
                    VKLAD,

                    [NiceDisplayName("Akreditiv")]
                    AKRED,

                    [NiceDisplayName("Inkaso")]
                    INKAS,

                    [NiceDisplayName("Důchod")]
                    DUCHOD,

                    [NiceDisplayName("Výměnek")]
                    VYMENEK,

                    [NiceDisplayName("Společnost")]
                    SPOL,

                    [NiceDisplayName("Tichá společnost")]
                    TICH_SPOL,

                    [NiceDisplayName("Pojištění")]
                    POJ,

                    [NiceDisplayName("Sázka, hra a los")]
                    SAZKA,

                    [NiceDisplayName("Veřejný příslib")]
                    VER_PRISL,

                    [NiceDisplayName("Slib odškodnění")]
                    SLIB_ODSK,

                    [NiceDisplayName("Dotace")]
                    DOT,

                    [NiceDisplayName("Služebnost")]
                    SLUZ,

                    [NiceDisplayName("Ostatní")]
                    OSTATNI,
                }

                [ShowNiceDisplayName()]
                public enum ContractSubjectTypes
                {
                    #region DatLowe_types

                    [NiceDisplayName("Produkty zemědělství, hospodářské produkty, produkty akvakultury, lesnictví a související produkty")]
                    X03000000_1,

                    [NiceDisplayName("Ropné produkty, paliva, elektrická energie a ostatní zdroje energie")]
                    X09000000_3,

                    [NiceDisplayName("Produkty těžebního průmyslu, kovové suroviny a související produkty")]
                    X14000000_1,

                    [NiceDisplayName("Potraviny, nápoje, tabák a související produkty")]
                    X15000000_8,

                    [NiceDisplayName("Zemědělské stroje")]
                    X16000000_5,

                    [NiceDisplayName("Oděvy, obuv, brašnářské výrobky a doplňky")]
                    X18000000_9,

                    [NiceDisplayName("Usně a textilie, plastové a pryžové materiály")]
                    X19000000_6,

                    [NiceDisplayName("Tiskařské výrobky a související produkty")]
                    X22000000_0,

                    [NiceDisplayName("Chemické výrobky")]
                    X24000000_4,

                    [NiceDisplayName("Kancelářské a počítací stroje, zařízení a potřeby mimo nábytek a balíky programů")]
                    X30000000_9,

                    [NiceDisplayName("Elektrické strojní zařízení, přístroje, zařízení a spotřební materiál, osvětlení")]
                    X31000000_6,

                    [NiceDisplayName("Rozhlas, televize, komunikace, telekomunikace a související zařízení")]
                    X32000000_3,

                    [NiceDisplayName("Zdravotnické přístroje, farmaceutika a prostředky pro osobní péči")]
                    X33000000_0,

                    [NiceDisplayName("Přepravní zařízení a pomocné výrobky pro přepravu")]
                    X34000000_7,

                    [NiceDisplayName("Bezpečnostní, hasičské, policejní a ochranné vybavení")]
                    X35000000_4,

                    [NiceDisplayName("Hudební nástroje, sportovní zboží, hry, hračky, materiál pro řemeslné a umělecké práce a příslušenství")]
                    X37000000_8,

                    [NiceDisplayName("Laboratorní, optické a přesné přístroje a zařízení (mimo skel)")]
                    X38000000_5,

                    [NiceDisplayName("Nábytek (včetně kancelářského), zařízení interiéru, domácí spotřebiče (mimo osvětlení) a čisticí prostředky")]
                    X39000000_2,

                    [NiceDisplayName("Shromážděná a upravená voda")]
                    X41000000_9,

                    [NiceDisplayName("Průmyslové stroje")]
                    X42000000_6,

                    [NiceDisplayName("Stroje pro hlubinné a povrchové dobývání a stavební stroje")]
                    X43000000_3,

                    [NiceDisplayName("Stavební konstrukce a materiály; pomocné výrobky pro konstrukce (mimo elektrické přístroje)")]
                    X44000000_0,

                    [NiceDisplayName("Stavební práce")]
                    X45000000_7,

                    [NiceDisplayName("Balíky programů a informační systémy")]
                    X48000000_8,

                    [NiceDisplayName("Opravy a údržba")]
                    X50000000_5,

                    [NiceDisplayName("Instalační a montážní služby (mimo programového vybavení)")]
                    X51000000_9,

                    [NiceDisplayName("Pohostinství a ubytovací služby a maloobchodní služby")]
                    X55000000_0,

                    [NiceDisplayName("Přepravní služby (mimo přepravu odpadů)")]
                    X60000000_8,

                    [NiceDisplayName("Pomocné a doplňkové dopravní služby; provozování cestovních agentur")]
                    X63000000_9,

                    [NiceDisplayName("Poštovní a telekomunikační služby")]
                    X64000000_6,

                    [NiceDisplayName("Veřejné služby")]
                    X65000000_3,

                    [NiceDisplayName("Finanční a pojišťovací služby")]
                    X66000000_0,

                    [NiceDisplayName("Realitní služby")]
                    X70000000_1,

                    [NiceDisplayName("Architektonické, stavební, technické a inspekční služby")]
                    X71000000_8,

                    [NiceDisplayName("Informační technologie: poradenství, vývoj programového vybavení, internet a podpora")]
                    X72000000_5,

                    [NiceDisplayName("Výzkum a vývoj a související služby")]
                    X73000000_2,

                    [NiceDisplayName("Administrativa, ochrana a sociální zabezpečení")]
                    X75000000_6,

                    [NiceDisplayName("Služby vztahující se k ropnému a plynárenskému průmyslu")]
                    X76000000_3,

                    [NiceDisplayName("Zemědělské, lesnické, zahradnické služby a služby v oblasti akvakultury a včelařství")]
                    X77000000_0,

                    [NiceDisplayName("Obchodní služby: právní, marketingové, poradenské služby, nábor zaměstnanců, tiskařské a bezpečnostní služby")]
                    X79000000_4,

                    [NiceDisplayName("Vzdělávání a školení")]
                    X80000000_4,

                    [NiceDisplayName("Zdravotní a sociální péče")]
                    X85000000_9,

                    [NiceDisplayName("Kanalizace, odstraňování odpadu, čištění a ekologické služby")]
                    X90000000_7,

                    [NiceDisplayName("Rekreační, kulturní a sportovní služby")]
                    X92000000_1,

                    [NiceDisplayName("Jiné služby pro veřejnost, sociální služby a služby jednotlivcům")]
                    X98000000_3,
                    #endregion

                    [NiceDisplayName("Ostatní")]
                    XOSTATNI,
    
                }

                public static ContractDocTypes? GetDocType(string value)
                {
                    switch (value.ToLower())
                    {
                        case "ostat":
                            value = "Ostatni";
                            break;
                        case "other":
                            value = "Ostatni";
                            break;
                        default:
                            break;
                    };

                    ContractDocTypes outVal;
                    if (Enum.TryParse<ContractDocTypes>(Devmasters.Core.TextUtil.RemoveDiacritics(value), true, out outVal))
                    {
                        return outVal;
                    }
                    else
                        return null;
                }
                public ContractDocTypes? GetDocType()
                {
                    if (doctype != null && doctype.Length > 0)
                    {
                        var top = doctype.OrderByDescending(m => m.probability).First();
                        return GetDocType(top.value);
                    }
                    else
                        return null;
                }

                public static ContractTypes? GetContractType(string value)
                {
                    switch (value.ToLower())
                    {
                        case "ostat":
                            value = "Ostatni";
                            break;
                        case "other":
                            value = "Ostatni";
                            break;
                        default:
                            break;
                    };
                    ContractTypes outVal;
                    if (Enum.TryParse<ContractTypes>(Devmasters.Core.TextUtil.RemoveDiacritics(value), true, out outVal))
                    {
                        return outVal;
                    }
                    else
                        return null;
                }
                public ContractTypes? GetContractType()
                {
                    if (doctype != null && doctype.Length > 0)
                    {
                        var top = contracttype.OrderByDescending(m => m.probability).First();
                        return GetContractType(top.value);
                    }
                    else
                        return null;
                }

                public static ContractSubjectTypes? GetContractSubjectType(string value)
                {
                    switch (value.ToLower())
                    {
                        case "ostat":
                            value = "XOstatni";
                            break;
                        case "other":
                            value = "XOstatni";
                            break;
                        default:
                            if (!value.StartsWith("X"))
                            {
                                value = ConvertDatloweContractValueToMyValue(value);
                            }
                            break;
                    };
                    ContractSubjectTypes outVal;
                    if (Enum.TryParse<ContractSubjectTypes>(Devmasters.Core.TextUtil.RemoveDiacritics(value), true, out outVal))

                        return outVal;
                    else
                        return null;

                }
                public ContractSubjectTypes[] GetContractSubjectTypes()
                {
                    if (doctype != null && doctype.Length > 0)
                    {
                        var top = cpvcode
                            .OrderByDescending(m => m.probability)
                            .First();
                        var tops = cpvcode
                            .OrderByDescending(m => m.probability)
                            .Take(3)
                            .Where(m => m.probability > (top.probability * 0.4))
                            ;

                        List<ContractSubjectTypes> outVals = new List<ContractSubjectTypes>();

                        foreach (var t in tops)
                        {
                            var cstype = GetContractSubjectType(t.value);
                            if (cstype.HasValue)
                                outVals.Add(cstype.Value);
                        }
                        return outVals.ToArray();
                    }
                    else
                        return null;
                }
                public static string ConvertDatloweContractValueToMyValue(string s)
                {
                    if (string.IsNullOrEmpty(s))
                        return string.Empty;
                    return "X" + s.Replace("-", "_");
                }

                public ValueType[] doctype { get; set; }

                public ValueType[] contracttype { get; set; }
                //get { return _contracttype; }
                //set
                //{
                //    if (value != null)
                //    {
                //        foreach (var v in value)
                //        {
                //            var test = GetContractSubjectType(v.value);
                //            if (test.HasValue == false)
                //                throw new ArgumentOutOfRangeException("Invalid value " + v.value);
                //        }

                //    }
                //    else
                //        _cpvcode = value;
                //}



                public ValueType[] cpvcode { get; set; }
                //{
                //    get { return _cpvcode; }
                //    set
                //    {
                //        if (value != null)
                //        {
                //            foreach (var v in value)
                //            {
                //                var test = GetContractSubjectType(v.value);
                //                if (test.HasValue == false)
                //                    throw new ArgumentOutOfRangeException("Invalid value " + v.value);
                //            }

                //        }
                //        else
                //            _cpvcode = value;
                //    }
                //}

                public DateTime Created { get; set; } = DateTime.Now;

                public class ValueType
                {
                    public ValueType()
                    { }
                    public ValueType(string value, string label, float probability)
                    {
                        this.value = value;
                        this.label = label;
                        this.probability = probability;
                    }

                    [Keyword()]
                    public string value { get; set; }
                    [Keyword()]
                    public string label { get; set; }

                    [Nest.Number]
                    public float probability { get; set; }
                }


            }

            public bool HasClassification()
            {
                return this.DatlClassification != null &&
                    (
                    this.DatlClassification.GetDocType() != null
                    ||
                    this.DatlClassification.GetDocType() != null
                    ||
                    this.DatlClassification.GetContractSubjectTypes() != null
                    );

            }

            public Classification DatlClassification { get; set; } = null;

            public KeyVal[] FileMetadata = new KeyVal[] { };


            string _plainTextContent = "";
            [Text()]
            public string PlainTextContent
            {
                get { return _plainTextContent; }
                set
                {
                    _plainTextContent = value;
                    this.Lenght = this.PlainTextContent.Length;
                    this.WordCount = ParseTools.CountWords(this.PlainTextContent);
                }
            }


            public DataQualityEnum PlainTextContentQuality { get; set; } = DataQualityEnum.Unknown;

            [Date]
            public DateTime LastUpdate { get; set; } = DateTime.MinValue;

            public byte[] LocalCopy { get; set; } = null;

            [Keyword()]
            public string ContentType { get; set; }
            public int Lenght { get; set; }
            public int WordCount { get; set; }
            public int Pages { get; set; }




            [Object(Ignore = true)]
            public bool EnoughExtractedText
            {
                get
                {
                    return !(this.Lenght <= 20 || this.WordCount <= 10);
                }
            }

            public Priloha()
            {
            }
            public Priloha(XSD.tPrilohaOdkaz tpriloha)
            {
                this.hash = tpriloha.hash;
                this.nazevSouboru = tpriloha.nazevSouboru;
                this.odkaz = tpriloha.odkaz;
                //if (tpriloha.data != null)
                //{
                //    //priloha je soucasti tpriloha, uloz


                //}
            }


            public string LimitedAccessSecret(string forEmail)
            {
                if (string.IsNullOrEmpty(forEmail))
                    throw new ArgumentNullException("forEmail");
                return Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(forEmail + this.hash + "__" + forEmail);
            }


            public static string GetFileFromPrilohaRepository(HlidacStatu.Lib.Data.Smlouva.Priloha att, Lib.Data.Smlouva smlouva)
            {
                var ext = ".pdf";
                try
                {
                    ext = new System.IO.FileInfo(att.nazevSouboru).Extension;

                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Warning("invalid file name " + (att?.nazevSouboru ?? "(null)"));
                }


                string localFile = Lib.Init.PrilohaLocalCopy.GetFullPath(smlouva, att);
                var tmpPath = System.IO.Path.GetTempPath();
                HlidacStatu.Util.IOTools.DeleteFile(tmpPath);
                if (!System.IO.Directory.Exists(tmpPath))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(tmpPath);

                    }
                    catch
                    {

                    }
                }
                string tmpFn = System.IO.Path.GetTempFileName() + HlidacStatu.Lib.OCR.DocTools.PrepareFilenameForOCR(att.nazevSouboru);
                //System.IO.File.Delete(fn);
                if (System.IO.File.Exists(localFile))
                {
                    //do local copy
                    System.IO.File.Copy(localFile, tmpFn, true);
                }
                else
                {

                    try
                    {
                        byte[] data = null;
                        using (Devmasters.Net.Web.URLContent web = new Devmasters.Net.Web.URLContent(att.odkaz))
                        {
                            web.Timeout = web.Timeout * 10;
                            data = web.GetBinary().Binary;
                            System.IO.File.WriteAllBytes(tmpFn, data);
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {
                            byte[] data = null;
                            using (Devmasters.Net.Web.URLContent web = new Devmasters.Net.Web.URLContent(att.odkaz))
                            {
                                web.Tries = 5;
                                web.IgnoreHttpErrors = true;
                                web.TimeInMsBetweenTries = 1000;
                                web.Timeout = web.Timeout * 20;
                                data = web.GetBinary().Binary;
                                System.IO.File.WriteAllBytes(tmpFn, data);
                            }
                            return tmpFn;
                        }
                        catch (Exception e)
                        {

                            HlidacStatu.Util.Consts.Logger.Error(att.odkaz, e);
                            return null;
                        }
                    }
                }
                return tmpFn;


            }



        }

    }
}
