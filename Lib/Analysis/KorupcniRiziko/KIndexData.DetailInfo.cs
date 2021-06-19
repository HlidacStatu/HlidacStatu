using System;
using System.Linq;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public partial class KIndexData
    {
        public class DetailInfo
        {

            public Annual AnnualData { get; private set; }
            public KIndexParts Part { get; private set; }
            public string Ico { get; private set; }



            public decimal? RawValue { get; private set; }
            public decimal? Value { get => this.Koeficient * this.RawValue; }
            public KIndexLabelValues Label { get; private set; }
            public string Description { get; private set; }
            public string ShortComment { get; private set; }
            public decimal Koeficient { get; private set; }

            public DetailInfo(string ico, KIndexData.Annual data, KIndexParts part)
            {
                this.AnnualData = data;
                this.Part = part;
                this.Ico = ico;
                Init();
            }
            public void Init()
            {
                this.RawValue = FindValue();
                this.Description = PartsDescription(this.Part);
                this.Label = KIndexLabelForPart(this.Part, this.RawValue);
                this.Koeficient = DefaultKIndexPartKoeficient(this.Part);
                this.ShortComment = _kIndexCommentForPart(this.Part, this.Label);
            }

            public string Query(string oborName = null)
            {
                return SmlouvyQueryForPart(this.Ico, this.Part, this.AnnualData, oborName);
            }


            private string _kIndexCommentForPart(KIndexParts part, KIndexLabelValues lbl)
            {
                switch (part)
                {
                    case KIndexParts.PercentBezCeny:
                        switch (lbl)
                        {
                            case KIndexLabelValues.A:
                                return "U smluv neskrývá ceny.";
                            case KIndexLabelValues.B:
                                return "U většiny smluv netají ceny.";
                            case KIndexLabelValues.C:
                            case KIndexLabelValues.D:
                            case KIndexLabelValues.E:
                                return $"Skrývá ceny u {Devmasters.Lang.Plural.Get(this.AnnualData.Statistika.PocetSmluvBezCeny, "{0} smlouvy;{0} smluv;{0} smluv")}.";
                            case KIndexLabelValues.F:
                                return $"Skrývá ceny u varujícího počtu {Devmasters.Lang.Plural.Get(this.AnnualData.Statistika.PocetSmluvBezCeny, "{0} smlouvy;{0} smluv;{0} smluv")} z {this.AnnualData.Statistika.PocetSmluv} smluv.";
                            default:
                                return "";
                        }
                    case KIndexParts.PercSeZasadnimNedostatkem:
                        switch (lbl)
                        {
                            case KIndexLabelValues.A:
                                return "Smlouvy bez zásadních nedostatků.";
                            case KIndexLabelValues.B:
                            case KIndexLabelValues.C:
                                return $"Zásadní nedostatky u {Devmasters.Lang.Plural.Get(this.AnnualData.Statistika.PocetSmluvSeZasadnimNedostatkem, "{0} smlouvy;{0} smluv;{0} smluv")}.";
                            case KIndexLabelValues.D:
                            case KIndexLabelValues.E:
                            case KIndexLabelValues.F:
                                return $"Zásadní nedostatky u <b>{Devmasters.Lang.Plural.Get(this.AnnualData.Statistika.PocetSmluvSeZasadnimNedostatkem, "{0} smlouvy;{0} smluv;{0} smluv")}</b> z {this.AnnualData.Statistika.PocetSmluv} smluv.";
                            default:
                                return "";
                        }
                    case KIndexParts.CelkovaKoncentraceDodavatelu:
                        switch (lbl)
                        {
                            case KIndexLabelValues.A:
                                return "Zakázky se nekoncentrují u žádných dodavatelů.";
                            case KIndexLabelValues.B:
                            case KIndexLabelValues.C:
                                return "Žádný dodavatel nedominuje nad ostatními.";
                            case KIndexLabelValues.D:
                            case KIndexLabelValues.E:
                                return "Velké zakázky se koncentrují u malého počtu dodavatelů.";
                            case KIndexLabelValues.F:
                                return $"Většinu peněz {Devmasters.Lang.Plural.Get(this.AnnualData.CelkovaKoncentraceDodavatelu.TopDodavatele().Count(), "dostal {0} dodavatel;dostali {0} dodavatelé;dostalo {0} dodavatelů")} z {this.AnnualData.CelkovaKoncentraceDodavatelu.Dodavatele.Count()}.";
                            default:
                                return "";
                        }
                    case KIndexParts.KoncentraceDodavateluBezUvedeneCeny:
                        switch (lbl)
                        {
                            case KIndexLabelValues.A:
                                return "Zakázky se skrytou cenou se nekoncentrují u žádných dodavatelů.";
                            case KIndexLabelValues.B:
                            case KIndexLabelValues.C:
                                return "Zakázky se skrytou cenou uzavřelo více dodavatelů, žádný nedominuje.";
                            case KIndexLabelValues.D:
                            case KIndexLabelValues.E:
                                return $"Zakázky se skrytou cenou se koncentrují u {Devmasters.Lang.Plural.Get(this.AnnualData.KoncentraceDodavateluBezUvedeneCeny.TopDodavatele().Count(), "{0} dodavatele;{0} dodavatelů;{0} dodavatelů")}.";
                            case KIndexLabelValues.F:
                                return $"Většinu zakázek se skrytou cenou {Devmasters.Lang.Plural.Get(this.AnnualData.KoncentraceDodavateluBezUvedeneCeny.TopDodavatele().Count(), "dostal {0} dodavatel;dostali {0} dodavatelé;dostalo {0} dodavatelů")}.";
                            default:
                                return "";
                        }
                    case KIndexParts.KoncentraceDodavateluCenyULimitu:
                        switch (lbl)
                        {
                            case KIndexLabelValues.A:
                                return "Zakázky s podezřelou cenou se nekoncentrují u žádných dodavatelů.";
                            case KIndexLabelValues.B:
                            case KIndexLabelValues.C:
                                return "Zakázky s podezřelou cenou uzavřelo více dodavatelů, žádný nedominuje.";
                            case KIndexLabelValues.D:
                            case KIndexLabelValues.E:
                                return "Zakázky s podezřelou cenou se se koncentrují u malého počtu dodavatelů.";
                            case KIndexLabelValues.F:
                                return $"Většinu zakázek s cenou u limitu VZ {Devmasters.Lang.Plural.Get(this.AnnualData.KoncentraceDodavateluCenyULimitu.TopDodavatele().Count(), "dostal {0} dodavatel;dostali {0} dodavatelé;dostalo {0} dodavatelů")}.";
                            default:
                                return "";
                        }
                    case KIndexParts.KoncentraceDodavateluObory:
                        switch (lbl)
                        {
                            case KIndexLabelValues.A:
                                return "Zakázky z jednotlivých oborů se u dodavatelů nekoncentrují.";
                            case KIndexLabelValues.B:
                            case KIndexLabelValues.C:
                                return "Zakázky z jednotlivých oborů uzavřelo více dodavatelů, žádný výrazně nedominuje.";
                            case KIndexLabelValues.D:
                            case KIndexLabelValues.E:
                                var oboryE = this.AnnualData.KIndexVypocet.OboroveKoncentrace?
                                    .Radky
                                    .Where(m => KIndexLabelForPart(KIndexParts.KoncentraceDodavateluObory, m.Hodnota) >= KIndexLabelValues.D);

                                return $"Dominance zakázek u dodavatelů se objevuje v {Devmasters.Lang.Plural.Get(oboryE.Count(), "{0} oboru;{0} oborech;{0} oborech")}.";
                            case KIndexLabelValues.F:
                                var oboryF = this.AnnualData.KIndexVypocet.OboroveKoncentrace?
                                    .Radky
                                    .Where(m => KIndexLabelForPart(KIndexParts.KoncentraceDodavateluObory, m.Hodnota) >= KIndexLabelValues.D);

                                return $"Dominance zakázek u dodavatelů je výrazná v {Devmasters.Lang.Plural.Get(oboryF.Count(), "{0} oboru;{0} oborech;{0} oborech")}.";
                            default:
                                return "";
                        }
                    case KIndexParts.PercSmluvUlimitu:
                        switch (lbl)
                        {
                            case KIndexLabelValues.A:
                                return "Neobchází limity zákona o veřejných zakázkách.";
                            case KIndexLabelValues.B:
                                return "Velmi málo smluv má hodnotu blízkou limitům pro veřejné zakázky.";
                            case KIndexLabelValues.C:
                            case KIndexLabelValues.D:
                            case KIndexLabelValues.E:
                            case KIndexLabelValues.F:
                                return $"{Devmasters.Lang.Plural.Get(this.AnnualData.Statistika.PocetSmluvULimitu, "{0} smlouva má;{0} smlouvy mají;{0} smluv má")} hodnotu blízkou limitům veřejných zakázek, což může naznačovat snahu se vyhnout řádné veřejné soutěži.";
                            //return $"Zásadní nedostatky evidujeme u {Devmasters.Lang.Plural.Get(data.StatistikaRegistruSmluv.PocetSmluvSeZasadnimNedostatkem, "{0} smlouva;{0} smlouvy;{0} smluv")} z {data.StatistikaRegistruSmluv.PocetSmluv}";
                            default:
                                return "";
                        }
                    case KIndexParts.PercNovaFirmaDodavatel:
                        switch (lbl)
                        {
                            case KIndexLabelValues.A:
                                return "Nemá smlouvy s nově založenými firmami.";
                            case KIndexLabelValues.B:
                            case KIndexLabelValues.C:
                            case KIndexLabelValues.D:
                            case KIndexLabelValues.E:
                            case KIndexLabelValues.F:
                                return $"Uzavřeli {Devmasters.Lang.Plural.Get(this.AnnualData.Statistika.PocetSmluvNovaFirma, "{0} smlouvu;{0} smlouvy;{0} smluv")} s nově založenými firmami.";
                            default:
                                return "";
                        }
                    case KIndexParts.PercUzavrenoOVikendu:
                        switch (lbl)
                        {
                            case KIndexLabelValues.A:
                                return "Neuzavřeli žádné smlouvy o víkendu či svátku.";
                            case KIndexLabelValues.B:
                            case KIndexLabelValues.C:
                            case KIndexLabelValues.D:
                            case KIndexLabelValues.E:
                            case KIndexLabelValues.F:
                                return $"Uzavřeli {Devmasters.Lang.Plural.Get(this.AnnualData.Statistika.PocetSmluvOVikendu, "{0} smlouvu;{0} smlouvy;{0} smluv")} o víkendu nebo svátku.";
                            default:
                                return "";
                        }
                    case KIndexParts.PercSmlouvySPolitickyAngazovanouFirmou:
                        switch (lbl)
                        {
                            case KIndexLabelValues.A:
                                return "Nemá uzavřené smlouvy s firmami, jejichž majitelé či ony samy sponzorovali politické strany.";
                            case KIndexLabelValues.B:
                            case KIndexLabelValues.C:
                            case KIndexLabelValues.D:
                            case KIndexLabelValues.E:
                            case KIndexLabelValues.F:
                                return $"Uzavřeli {Devmasters.Lang.Plural.Get(this.AnnualData.Statistika.PocetSmluvPolitiky, "{0} smlouvu;{0} smlouvy;{0} smluv")} s firmami, jejichž majitelé či ony samy sponzorovali politické strany.";
                            default:
                                return "";
                        }
                    case KIndexParts.PercSmlouvyPod50kBonus:
                        var bonusR = this.AnnualData.KIndexVypocet.Radky.FirstOrDefault(m => m.VelicinaPart == part);
                        if (bonusR != null)
                        {
                            if (bonusR.Hodnota == -1 * Consts.BonusPod50K_3)
                                return "Dobrovolně zveřejňuje velké množství smluv pod 50 000 Kč.";
                            if (bonusR.Hodnota == -1 * Consts.BonusPod50K_2)
                                return "Dobrovolně zveřejňuje smlouvy pod 50 000 Kč.";
                            if (bonusR.Hodnota == -1 * Consts.BonusPod50K_1)
                                return "Dobrovolně zveřejňuje smlouvy pod 50 000 Kč.";
                        }
                        return "Nesplňuje podmínku pro udělení bonusu za transparentnost.";
                    default:
                        return "";
                }
            }

            public decimal? FindValue()
            {
                return this.AnnualData.KIndexVypocet.Radky.FirstOrDefault(m => m.VelicinaPart == this.Part)?.Hodnota;

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

            public static KIndexLabelValues KIndexLabelForPart(KIndexParts part, decimal? value, object addValue = null)
            {
                if (value.HasValue == false)
                    return KIndexLabelValues.None;

                switch (part)
                {
                    case KIndexParts.PercSmlouvySPolitickyAngazovanouFirmou:
                    case KIndexParts.PercentBezCeny:
                        int pocetSmluv = Devmasters.ParseText.ToInt(addValue?.ToString()) ?? 1000; //default hodne smluv

                        if (value == 0m)
                            return KIndexLabelValues.A;
                        else if (value < 0.05m)
                            return KIndexLabelValues.B;
                        else if (value < 0.1m)
                            return KIndexLabelValues.C;
                        else if (value < 0.15m || pocetSmluv <= 5) //pokud je malo smluv, nedavej vetsi riziko
                            return KIndexLabelValues.D;
                        else if (value < 0.2m)
                            return KIndexLabelValues.E;
                        else
                            return KIndexLabelValues.F;

                    case KIndexParts.PercSeZasadnimNedostatkem:
                        int pocetSmluv1 = Devmasters.ParseText.ToInt(addValue?.ToString()) ?? 1000; //default hodne smluv
                        if (value == 0m)
                            return KIndexLabelValues.A;
                        else if (value < 0.01m)
                            return KIndexLabelValues.C;
                        else if (value < 0.015m || pocetSmluv1 <= 5) //pokud je malo smluv, nedavej vetsi riziko)
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
                        int pocetSmluv2 = Devmasters.ParseText.ToInt(addValue?.ToString()) ?? 1000; //default hodne smluv
                        if (value == 0)
                            return KIndexLabelValues.A;
                        else if (value < 0.04m)
                            return KIndexLabelValues.B;
                        else if (value < 0.06m)
                            return KIndexLabelValues.C;
                        else if (value < 0.08m || pocetSmluv2 <= 5) //pokud je malo smluv, nedavej vetsi riziko
                            return KIndexLabelValues.D;
                        else if (value < 0.1m)
                            return KIndexLabelValues.E;
                        else
                            return KIndexLabelValues.F;

                    case KIndexParts.PercNovaFirmaDodavatel:
                    case KIndexParts.PercUzavrenoOVikendu:
                        int pocetSmluv3 = Devmasters.ParseText.ToInt(addValue?.ToString()) ?? 1000; //default hodne smluv
                        if (value == 0)
                            return KIndexLabelValues.A;
                        else if (value < 0.02m)
                            return KIndexLabelValues.B;
                        else if (value < 0.03m)
                            return KIndexLabelValues.C;
                        else if (value < 0.04m || pocetSmluv3 <= 5) //pokud je malo smluv, nedavej vetsi riziko
                            return KIndexLabelValues.D;
                        else if (value < 0.5m)
                            return KIndexLabelValues.E;
                        else
                            return KIndexLabelValues.F;

                    case KIndexParts.PercSmlouvyPod50kBonus:
                        if (value > 0)
                            return KIndexLabelValues.A;
                        else
                            return KIndexLabelValues.None;
                    default:
                        return KIndexLabelValues.None;
                }
            }

            public static string SmlouvyQueryForPart(string ico, KIndexData.KIndexParts part, KIndexData.Annual annual, string oborName = null)
            {
                string q = "";
                if (annual is null)
                    return null;
                string baseQ = $"ico:{ico} AND datumUzavreni:[{annual.Rok}-01-01 TO {annual.Rok + 1}-01-01}}";
                switch (part)
                {
                    case KIndexParts.PercentBezCeny:
                        return baseQ + " AND " + "hint.skrytaCena:1";
                    case KIndexParts.PercSeZasadnimNedostatkem:
                        return baseQ + " AND " + "chyby:zasadni";
                    case KIndexParts.PercSmlouvySPolitickyAngazovanouFirmou:
                        return baseQ + " AND " + "hint.smlouvaSPolitickyAngazovanymSubjektem:>0";
                    case KIndexParts.CelkovaKoncentraceDodavatelu:
                        q = annual?.CelkovaKoncentraceDodavatelu?.Query ?? "";
                        if (!string.IsNullOrEmpty(q))
                            return " hint.vztahSeSoukromymSubjektem:>0 AND " + q;
                        return q;
                    case KIndexParts.KoncentraceDodavateluBezUvedeneCeny:
                        q = annual?.KoncentraceDodavateluBezUvedeneCeny?.Query ?? "";
                        if (!string.IsNullOrEmpty(q))
                            return " hint.vztahSeSoukromymSubjektem:>0 AND " + q;
                        return q;
                    case KIndexParts.KoncentraceDodavateluObory:
                        q = annual?.KoncetraceDodavateluObory.FirstOrDefault(m => m.OborName == oborName)?.Koncentrace?.Query ?? "";
                        if (!string.IsNullOrEmpty(q))
                            return " hint.vztahSeSoukromymSubjektem:>0 AND " + q;
                        return q;
                    case KIndexParts.KoncentraceDodavateluCenyULimitu:
                        q = annual?.KoncentraceDodavateluCenyULimitu?.Query ?? "";
                        if (!string.IsNullOrEmpty(q))
                            return " hint.vztahSeSoukromymSubjektem:>0 AND " + q;
                        return q;
                    case KIndexParts.PercSmluvUlimitu:
                        return baseQ + " AND " + "hint.smlouvaULimitu:>0";
                    case KIndexParts.PercNovaFirmaDodavatel:
                        return baseQ + "  AND " + "( hint.pocetDniOdZalozeniFirmy:>-50 AND hint.pocetDniOdZalozeniFirmy:<30 )";
                    case KIndexParts.PercUzavrenoOVikendu:
                        return baseQ + "  AND " + "hint.denUzavreni:>0";
                    case KIndexParts.PercSmlouvyPod50kBonus:
                        return baseQ + "  AND " + " cena:>0 AND cena:<50000";
                    default:
                        return baseQ;
                }
            }



            public static string PartsDescription(KIndexData.KIndexParts part)
            {
                switch (part)
                {
                    case KIndexData.KIndexParts.PercentBezCeny:
                        return "Procentní podíl smluv uzavřených se soukromými společnostmi za kalendářní rok, které mají skrytou cenu. Cenu je možné skrýt pouze z důvodu bankovního a obchodního tajemství" +
                            "a tato výjimka je velmi často zneužívána pro utajení hodnoty smlouvy. " +
                            "Hodnota <b>0</b> znamená, že žádná smlouva nemá skrytou cenu, hodnota <b>0.5</b> znamená polovinu smluv se skrytou cenou.";
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
                        return "Procentní podíl smluv, jejichž hodnota je pouze o malou částku nižší než je zákonný limit, nad kterým by se zakázka musela soutěžit podle zákona o zadávání veřejných zakázek. " +
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


        }


    }
}
