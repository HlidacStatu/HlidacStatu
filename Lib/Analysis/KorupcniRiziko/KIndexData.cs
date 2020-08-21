using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using com.sun.org.apache.regexp.@internal;

using Nest;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{

    [Nest.ElasticsearchType(IdProperty = nameof(Ico))]
    public partial class KIndexData
        : Util.ISocialInfo
    {

        public static int[] KIndexLimits = { 0, 3, 6, 9, 12, 15 };

        public static string PartsDescription(KIndexData.KIndexParts part)
        {
            switch (part)
            {
                case KIndexData.KIndexParts.PercentBezCeny:
                    return "Procentní podíl smluv uzavřených se soukromými společnostmi za kalendářní rok, které mají skrytou cenu. Cenu je možné skrýt pouze z důvodu bankovního a obchodního tajemství" +
                        "a tato výjimka je velmi často zneužívána pro utajení hodnoty smlouvy. " +
                        "Hodnota <b>0</b> znamená, že žádná smlouva nemá skrytou cenu, hodnota <b>0.5</b> znamená polovinu smlouv se skrytou cenou.";
                case KIndexData.KIndexParts.PercSeZasadnimNedostatkem:
                    return "Procentní podíl smluv, u kterých jsme při analýze našli zásadní nedostatky. " +
                        "Hodnota <b>0</b> znamená, že žádná smlouva nemá zásadní nedostatky, hodnota <b>0.5</b> znamená polovinu smluv se zásadními nedostatky.";
                case KIndexData.KIndexParts.CelkovaKoncentraceDodavatelu:
                    return "Koncentrace dodavatelů ukazuje míru nerovnoměrného rozdělení peněz mezi dodavatele. Neboli - zda malé množství dodavatelů dostává většinu peněz." +
                        $"Koncentraci dodavatelů počítáme pouze pro soukromé dodavatele, tzn. pouze pro smlouvy mezi organizací a soukromými dodavateli. " +
                        "Pro výpočet používáme upravený <b>Herfindahl Hirschman Index</b> a způsob výpočtu podrobně popisujeme v metodice. " +
                        "Hodnota <b>0</b> znamená  ideální stav, ideální konkurenci mezi dodavateli, neboli že všichni dodavatelé mají pouze 1 smlouvu ve stejné výši." +
                        "Hodnota <b>od 0.25 do 1.0</b> znamená vysokou koncentraci dodavatelů až monopolní postavení jednoho z nich.";
                case KIndexData.KIndexParts.KoncentraceDodavateluBezUvedeneCeny:
                    return "Koncentrace dodavatelů ukazuje míru nerovnoměrného rozdělení peněz mezi dodavatele. Neboli - zda malé množství dodavatelů dostává většinu peněz." +
                        $"V tomto parametru zkoumáme koncentraci dodavatelů u smluv, které mají utajenou cenu. Jinak řečeno - jestli k utajování ceny nedochází pouze u některý vybraných dodavatelů. " +
                        "Hodnota <b>0</b> znamená ideální stav." +
                        "Hodnota <b>od 0.25 do 1.0</b> znamená, že utajování ceny se týká jen vybraného okruhu dodavatelů.";
                case KIndexData.KIndexParts.PercSmluvUlimitu:
                    return "Procentní podíl smluv, jejichž hodnota pouze malou hodnotu nižší než je zákonný limit, podle které by se zakázka musela soutěžit podle zákona o zadávání veřejných zakázek. " +
                        "Typicky se jedná o limit <b>6 miliónů</b> korun pro stavební zakázky a <b>2 miliony</b> pro ostatní." +
                        "Hodnota <b>0</b> znamená, že žádná smlouva není u limitu, hodnota <b>1.0</b> znamená všechny smluvy u limitu.";
                case KIndexData.KIndexParts.KoncentraceDodavateluCenyULimitu:
                    return "Koncentrace dodavatelů ukazuje míru nerovnoměrného rozdělení peněz mezi dodavatele. Neboli - zda malé množství dodavatelů dostává většinu peněz." +
                        $"V tomto parametru zkoumáme koncentraci dodavatelů u smluv, které mají cenu u limitu veřejných zakázek. " +
                        $"Jinak řečeno - jestli se obcházení zákona o zadávání veřejných zakázek netýka pouze některých vybraných dodavatelů. " +
                        "Hodnota <b>0</b> znamená  ideální stav." +
                        "Hodnota <b>od 0.25 do 1.0</b> znamená, že ceny smluv u limitu VZ se týká jen vybraného okruhu dodavatelů.";
                case KIndexData.KIndexParts.PercNovaFirmaDodavatel:
                    return "Procentní podíl smluv, které jsou uzavřeny z nově založenou firmou. " +
                        "Hodnota <b>0</b> znamená, že žádná smlouva takto uzavřená není, hodnota <b>0.5</b> znamená polovinu smluv uzavřenou s novými firmami.";
                case KIndexData.KIndexParts.PercUzavrenoOVikendu:
                    return "Procentní podíl smluv, které jsou podepsány o víkendu nebo státním svátku. Státní úřady obvykle neuzavírají smlouvy mimo pracovní dny. Historicky se u veřejně známých kauz stávalo, " +
                        "že právě ty problematické smlouvy byly uzavřeny mimo pracovní den, aby se snížila možnost třetích stran na to reagovat. " +
                        "Takto Ministerstvo dopravy obešlo konkurenci a <a href='https://www.idnes.cz/ekonomika/doprava/stat-obesel-skytoll-podpisem-smlouvy-s-kapschem-v-nedeli.A160830_175428_eko-doprava_suj' onclick=\"return trackOutLink(this, 'kindex-detail'); \">uzavřelo smlouvu za 5 miliard Kč v neděli</a>. " +
                        "Hodnota <b>0</b> znamená, že žádná smlouva takto uzavřená není, hodnota <b>0.5</b> znamená polovinu smluv uzavřenou mimo pracovní den.";
                case KIndexData.KIndexParts.PercSmlouvySPolitickyAngazovanouFirmou:
                    return "Procentní podíl smluv, které jsou uzavřeny s firmou, která má vazbu na politiku, neboli sponzorovala politickou stranu, " +
                        "nebo je přímo či nepřímo vlastněna sponzorem politické strany anebo ji přímo či nepřímo vlastní politik. " +
                        "Hodnota <b>0</b> znamená, že žádná smlouva s takovou firmou uzavřená není, hodnota <b>0.5</b> znamená, že polovina smluv je uzavřena s takovými firmami.";
                case KIndexData.KIndexParts.KoncentraceDodavateluObory:
                    return "Koncentrace dodavatelů ukazuje míru nerovnoměrného rozdělení peněz mezi dodavatele. Neboli - zda malé množství dodavatelů dostává většinu peněz." +
                        "Celkovou koncentraci dodavatelů počítáme v jiném parametru. Zde analyzujeme, zda nedochází ke koncentraci dodavatelů pouze v některých oblastech. " +
                        "Např. pouze v oblasti IT, právní služeb apod. Způsob výpočtu podrobně popisujeme v metodice K-Indexu." +
                        "Hodnota <b>0</b> znamená  ideální stav, ideální konkurenci mezi dodavateli, neboli že všichni dodavatelé mají pouze 1 smlouvu ve stejné výši." +
                        "Hodnota <b>od 0.25 do 1.0</b> znamená vysokou koncentraci dodavatelů až monopolní postavení jednoho z nich v nejdůležitějších oborech.";
                case KIndexData.KIndexParts.PercSmlouvyPod50kBonus:
                    return "Bonus (proto má zápornou hodnotu) pro organizace, které jsou transparetní nad rámec zákona. Pokud vkládají do registru smluv smlouvy pod hodnotu 50.000 Kč (což nemusí), mohou dostat bonus. " +
                        "Pokud je podíl smluv pod 50.000 Kč o 25% vyšší než je obvyklé (průměr celého registru smluv), pak mají bonus 0.25 bodů. Pokud je podíl vyšší o více než 50%, pak je bonus 0.5 bodů. Pokud je podíl vyšší o více než 75%, pak je bonus 0.75 bodů.";
                default:
                    return "";
            }
        }

        public Annual LastKIndex()
        {
            return roky?.Where(m => m.KIndexReady)?.OrderByDescending(m => m.Rok)?.FirstOrDefault();
        }

        public decimal? LastKIndexValue()
        {
            return LastKIndex()?.KIndex;
        }
        public KIndexLabelValues LastKIndexLabel()
        {
            return LastKIndexLabel(out int? tmp);
        }
        public KIndexLabelValues LastKIndexLabel(out int? rok)
        {

            var val = LastKIndex();
            rok = null;
            if (val != null)
            {
                rok = val.Rok;
                return val.KIndexLabel;
            }
            else
                return KIndexLabelValues.None;
        }

        public List<Annual> roky { get; set; } = new List<Annual>();

        public Annual ForYear(int year)
        {
            return roky.Where(m => m != null && m.Rok == year).FirstOrDefault();
        }

        public string Ico { get; set; }
        public string Jmeno { get; set; }
        public UcetniJednotkaInfo UcetniJednotka { get; set; } = new UcetniJednotkaInfo();

        [Nest.Date]
        public DateTime LastSaved { get; set; }

        public void Save()
        {
            //calculate fields before saving
            this.LastSaved = DateTime.Now;
            var res = ES.Manager.GetESClient_KIndex().Index<KIndexData>(this, o => o.Id(this.Ico)); //druhy parametr musi byt pole, ktere je unikatni
            if (!res.IsValid)
            {
                throw new ApplicationException(res.ServerError?.ToString());
            }
        }

        public static KIndexLabelValues CalculateLabel(decimal? value)
        {
            KIndexLabelValues label = KIndexLabelValues.None;
            for (int i = 0; i < KIndexLimits.Length; i++)
            {
                if (value > KIndexLimits[i])
                    label = (KIndexLabelValues)i;
            }

            return label;
        }

        public static KIndexData GetDirect(string ico)
        {
            var res = ES.Manager.GetESClient_KIndex().Get<KIndexData>(ico);
            if (res.Found == false)
                return null;
            else if (!res.IsValid)
            {
                throw new ApplicationException(res.ServerError?.ToString());
            }
            else
                return res.Source;
        }


        public static string KIndexLabelColor(KIndexLabelValues label)
        {
            switch (label)
            {
                case KIndexLabelValues.None:
                    return "#000000";
                case KIndexLabelValues.A:
                    return "#00A5FF";
                case KIndexLabelValues.B:
                    return "#0064B4";
                case KIndexLabelValues.C:
                    return "#002D5A";
                case KIndexLabelValues.D:
                    return "#9099A3";
                case KIndexLabelValues.E:
                    return "#F2B41E";
                case KIndexLabelValues.F:
                    return "#D44820";
                default:
                    return "#000000";
            }
        }
        public static string KIndexCommentForPart(KIndexParts part, KIndexData.Annual data)
        {
            var lbl = KIndexLabelForPart(part, data.KIndexVypocet.Radky.Where(m=>m.VelicinaPart == part).First().Hodnota);
            switch (part)
            {
                case KIndexParts.PercentBezCeny:
                    switch (lbl)
                    {
                        case KIndexLabelValues.A:
                            return "Neskrývá ceny u žádných smluv";
                        case KIndexLabelValues.B:
                            return "Skrývá ceny u minima smluv";
                        case KIndexLabelValues.C:
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                            return $"Skrývá ceny u {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetBezCeny, "{0} smlouva;{0} smlouvy;{0} smluv")}";
                        case KIndexLabelValues.F:
                            return $"Skrývá ceny u varujícího počtu {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetBezCeny, "{0} smlouva;{0} smlouvy;{0} smluv")}";
                        default:
                            return "";
                    }
                case KIndexParts.PercSeZasadnimNedostatkem:
                    switch (lbl)
                    {
                        case KIndexLabelValues.A:
                            return "Nemá žádné smlouvy se zásadními nedostatky";
                        case KIndexLabelValues.B:
                        case KIndexLabelValues.C:
                            return $"Zásadní nedostatky evidujeme pouze u {data.PercSeZasadnimNedostatkem:P2} smluv";
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                        case KIndexLabelValues.F:
                            return $"Zásadní nedostatky evidujeme u {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvSeZasadnimNedostatkem, "{0} smlouva;{0} smlouvy;{0} smluv")} z {data.Statistika.PocetSmluv}";
                        default:
                            return "";
                    }
                case KIndexParts.CelkovaKoncentraceDodavatelu:
                    switch (lbl)
                    {
                        case KIndexLabelValues.A:
                            return "Zakázky se nekoncentrují u žádných dodavatelů";
                        case KIndexLabelValues.B:
                        case KIndexLabelValues.C:
                            return "Rozdíly mezi velikostí zakázek dodavatelů jsou malé";
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                            return "Zakázky se koncentrují u malého počtu dodavatelů";
                        case KIndexLabelValues.F:
                            return $"Koncentrace velkých zakázek u vybraných dodavatelů je vysoká";
                        default:
                            return "";
                    }
                case KIndexParts.KoncentraceDodavateluBezUvedeneCeny:
                    switch (lbl)
                    {
                        case KIndexLabelValues.A:
                            return "Zakázky se nekoncentrují u žádných dodavatelů";
                        case KIndexLabelValues.B:
                        case KIndexLabelValues.C:
                            return "Rozdíly mezi velikostí zakázek dodavatelů jsou malé";
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                            return "Zakázky se koncentrují u malého počtu dodavatelů";
                        case KIndexLabelValues.F:
                            return $"Koncentrace zakázek u vybraných dodavatelů je vysoká";
                        default:
                            return "";
                    }
                case KIndexParts.KoncentraceDodavateluCenyULimitu:
                    switch (lbl)
                    {
                        case KIndexLabelValues.A:
                            return "Zakázky s podezřelou cenou se nekoncentrují u žádných dodavatelů";
                        case KIndexLabelValues.B:
                        case KIndexLabelValues.C:
                            return "Zakázky s podezřelou cenou se nekoncentrují u vybraných dodavatelů";
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                            return "Zakázky s podezřelou cenou se se koncentrují u malého počtu dodavatelů";
                        case KIndexLabelValues.F:
                            return $"Koncentrace zakázek s podezřelou cenou u vybraných dodavatelů je vysoká";
                        default:
                            return "";
                    }
                case KIndexParts.KoncentraceDodavateluObory:
                    switch (lbl)
                    {
                        case KIndexLabelValues.A:
                            return "Zakázky s podezřelou cenou se nekoncentrují u žádných dodavatelů";
                        case KIndexLabelValues.B:
                        case KIndexLabelValues.C:
                            return "Zakázky s podezřelou cenou se nekoncentrují u vybraných dodavatelů";
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                            return "Zakázky s podezřelou cenou se se koncentrují u malého počtu dodavatelů";
                        case KIndexLabelValues.F:
                            return $"Koncentrace zakázek s podezřelou cenou u vybraných dodavatelů je vysoká";
                        default:
                            return "";
                    }
                case KIndexParts.PercSmluvUlimitu:
                    switch (lbl)
                    {
                        case KIndexLabelValues.A:
                            return "Organizace neobhází limity zákona o veřejných zakázkách";
                        case KIndexLabelValues.B:
                            return "Velmi málo smluv má hodnotu blízkou limitům pro ";
                        case KIndexLabelValues.C:
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                        case KIndexLabelValues.F:
                            return $"{HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvULimitu, "{0} smlouva;{0} smlouvy;{0} smluv")} má hodnotu blízkou limitům veřejných zakázek, což může naznačovat snahu se vyhnout řádné veřejné soutěži";
                        default:
                            return "";
                    }
                case KIndexParts.PercNovaFirmaDodavatel:
                    break;
                case KIndexParts.PercUzavrenoOVikendu:
                    break;
                case KIndexParts.PercSmlouvySPolitickyAngazovanouFirmou:
                    break;
                case KIndexParts.PercSmlouvyPod50kBonus:
                    break;
                default:
                    break;
            }
        }

        public static KIndexLabelValues KIndexLabelForPart(KIndexParts part, decimal value)
        {
            switch (part)
            {
                case KIndexParts.PercSmlouvySPolitickyAngazovanouFirmou:
                case KIndexParts.PercentBezCeny:
                    if (value == 0m)
                        return KIndexLabelValues.A;
                    else if (value < 0.05m)
                        return KIndexLabelValues.B;
                    else if (value < 0.1m)
                        return KIndexLabelValues.C;
                    else if (value < 0.15m)
                        return KIndexLabelValues.D;
                    else if (value < 0.2m)
                        return KIndexLabelValues.E;
                    else
                        return KIndexLabelValues.F;

                case KIndexParts.PercSeZasadnimNedostatkem:
                    if (value == 0m)
                        return KIndexLabelValues.A;
                    else if (value < 0.01m)
                        return KIndexLabelValues.C;
                    else if (value < 0.015m)
                        return KIndexLabelValues.D;
                    else
                        return KIndexLabelValues.F;

                case KIndexParts.CelkovaKoncentraceDodavatelu:
                case KIndexParts.KoncentraceDodavateluCenyULimitu:
                case KIndexParts.KoncentraceDodavateluBezUvedeneCeny:
                case KIndexParts.KoncentraceDodavateluObory:
                    if (value < 0.05m)
                        return KIndexLabelValues.A;
                    else if (value < 0.1m)
                        return KIndexLabelValues.B;
                    else if (value < 0.15m)
                        return KIndexLabelValues.C;
                    else if (value < 0.2m)
                        return KIndexLabelValues.D;
                    else if (value < 0.25m)
                        return KIndexLabelValues.E;
                    else
                        return KIndexLabelValues.F;

                case KIndexParts.PercSmluvUlimitu:
                    if (value == 0)
                        return KIndexLabelValues.A;
                    else if (value < 0.04m)
                        return KIndexLabelValues.B;
                    else if (value < 0.06m)
                        return KIndexLabelValues.C;
                    else if (value < 0.08m)
                        return KIndexLabelValues.D;
                    else if (value < 0.1m)
                        return KIndexLabelValues.E;
                    else
                        return KIndexLabelValues.F;

                case KIndexParts.PercNovaFirmaDodavatel:
                case KIndexParts.PercUzavrenoOVikendu:
                    if (value == 0)
                        return KIndexLabelValues.A;
                    else if (value < 0.02m)
                        return KIndexLabelValues.B;
                    else if (value < 0.03m)
                        return KIndexLabelValues.C;
                    else if (value < 0.04m)
                        return KIndexLabelValues.D;
                    else if (value < 0.5m)
                        return KIndexLabelValues.E;
                    else
                        return KIndexLabelValues.F;

                default:
                    return KIndexLabelValues.None;
            }
        }

        public static decimal DefaultKIndexPartKoeficient(KIndexParts part)
        {
            switch (part)
            {
                case KIndexParts.PercentBezCeny:
                    return 10m;
                case KIndexParts.PercSeZasadnimNedostatkem:
                    return 10m;
                case KIndexParts.CelkovaKoncentraceDodavatelu:
                    return 10m;
                case KIndexParts.KoncentraceDodavateluBezUvedeneCeny:
                    return 10m;
                case KIndexParts.PercSmluvUlimitu:
                    return 10m;
                case KIndexParts.KoncentraceDodavateluCenyULimitu:
                    return 10m;
                case KIndexParts.PercNovaFirmaDodavatel:
                    return 10m;
                case KIndexParts.PercUzavrenoOVikendu:
                    return 10m;
                case KIndexParts.PercSmlouvySPolitickyAngazovanouFirmou:
                    return 10m;
                case KIndexParts.KoncentraceDodavateluObory:
                    return 10m;
                case KIndexParts.PercSmlouvyPod50kBonus:
                    return 1m;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string KIndexLabelIconUrl(KIndexLabelValues value, bool local = true)
        {
            string url = "";
            if (local == false)
                url = "https://www.hlidacstatu.cz";

            bool hranate = Devmasters.Core.Util.Config.GetConfigValue("KIdxIconStyle") == "hranate";
            switch (value)
            {
                case KIndexLabelValues.None:
                    return url + "/Content/Img/1x1.png ";
                default:
                    return url + $"/Content/kindex/{(hranate ? "hranate" : "kulate")}/icon{value.ToString()}.svg";

            }
        }


    }
}
