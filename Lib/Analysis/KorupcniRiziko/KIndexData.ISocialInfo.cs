using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HlidacStatu.Util;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public partial class KIndexData
        : Util.ISocialInfo
    {

        public static string KIndexCommentForPart(KIndexParts part, KIndexData.Annual data, bool html = false)
        {
            var txt = _kIndexCommentForPart(part, data);
            if (html)
                return txt;
            else
                return Devmasters.Core.TextUtil.RemoveHTML(txt);
        }
        private static string _kIndexCommentForPart(KIndexParts part, KIndexData.Annual data)
        {
            var v = data.KIndexVypocet.Radky.Where(m => m.VelicinaPart == part).FirstOrDefault()?.Hodnota;
            if (v.HasValue == false)
                return "";

            var lbl = KIndexLabelForPart(part, v.Value);
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
                            return $"Skrývá ceny u {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvBezCeny, "{0} smlouvy;{0} smluv;{0} smluv")}.";
                        case KIndexLabelValues.F:
                            return $"Skrývá ceny u varujícího počtu {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvBezCeny, "{0} smlouvy;{0} smluv;{0} smluv")} z {data.Statistika.PocetSmluv} smluv.";
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
                            return $"Zásadní nedostatky u {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvSeZasadnimNedostatkem, "{0} smlouvy;{0} smluv;{0} smluv")}.";
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                        case KIndexLabelValues.F:
                            return $"Zásadní nedostatky u <b>{HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvSeZasadnimNedostatkem, "{0} smlouvy;{0} smluv;{0} smluv")}</b> z {data.Statistika.PocetSmluv} smluv.";
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
                            return $"Většinu peněz {HlidacStatu.Util.PluralForm.Get(data.CelkovaKoncentraceDodavatelu.TopDodavatele().Count(), "dostal {0} dodavatel;dostali {0} dodavatelé;dostalo {0} dodavatelů")} z {data.CelkovaKoncentraceDodavatelu.Dodavatele.Count()}.";
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
                            return $"Zakázky se skrytou cenou se koncentrují u {HlidacStatu.Util.PluralForm.Get(data.KoncentraceDodavateluBezUvedeneCeny.TopDodavatele().Count(), "{0} dodavatele;{0} dodavatelů;{0} dodavatelů")}.";
                        case KIndexLabelValues.F:
                            return $"Většinu zakázek se skrytou cenou {HlidacStatu.Util.PluralForm.Get(data.KoncentraceDodavateluBezUvedeneCeny.TopDodavatele().Count(), "dostal {0} dodavatel;dostali {0} dodavatelé;dostalo {0} dodavatelů")}.";
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
                            return $"Většinu zakázek se skrytou cenou {HlidacStatu.Util.PluralForm.Get(data.KoncentraceDodavateluCenyULimitu.TopDodavatele().Count(), "dostal {0} dodavatel;dostali {0} dodavatelé;dostalo {0} dodavatelů")}.";
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
                            var oboryE = data.KIndexVypocet.OboroveKoncentrace?
                                .Radky
                                .Where(m => KIndexData.KIndexLabelForPart(KIndexParts.KoncentraceDodavateluObory, m.Hodnota) >= KIndexLabelValues.D);

                            return $"Dominance zakázek u dodavatelů se objevuje v {HlidacStatu.Util.PluralForm.Get(oboryE.Count(), "{0} oboru;{0} oborech;{0} oborech")}.";
                        case KIndexLabelValues.F:
                            var oboryF = data.KIndexVypocet.OboroveKoncentrace?
                                .Radky
                                .Where(m => KIndexData.KIndexLabelForPart(KIndexParts.KoncentraceDodavateluObory, m.Hodnota) >= KIndexLabelValues.D);

                            return $"Dominance zakázek u dodavatelů je výrazná v {HlidacStatu.Util.PluralForm.Get(oboryF.Count(), "{0} oboru;{0} oborech;{0} oborech")}.";
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
                            return $"{HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvULimitu, "{0} smlouva má;{0} smlouvy mají;{0} smluv má")} hodnotu blízkou limitům veřejných zakázek, což může naznačovat snahu se vyhnout řádné veřejné soutěži.";
                        //return $"Zásadní nedostatky evidujeme u {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvSeZasadnimNedostatkem, "{0} smlouva;{0} smlouvy;{0} smluv")} z {data.Statistika.PocetSmluv}";
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
                            return $"Uzavřel {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvNovaFirma, "{0} smlouvu;{0} smlouvy;{0} smluv")} s nově založenými firmami.";
                        default:
                            return "";
                    }
                case KIndexParts.PercUzavrenoOVikendu:
                    switch (lbl)
                    {
                        case KIndexLabelValues.A:
                            return "Neuzavřel žádné smlouvy o víkendu či svátku.";
                        case KIndexLabelValues.B:
                        case KIndexLabelValues.C:
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                        case KIndexLabelValues.F:
                            return $"Uzavřel {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvOVikendu, "{0} smlouvu;{0} smlouvy;{0} smluv")} o víkendu nebo svátku.";
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
                            return $"Uzavřel {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvPolitiky, "{0} smlouvu;{0} smlouvy;{0} smluv")} s firmami, jejichž majitelé či ony sami sponzorovali politické strany.";
                        default:
                            return "";
                    }
                case KIndexParts.PercSmlouvyPod50kBonus:
                    var bonusR = data.KIndexVypocet.Radky.FirstOrDefault(m => m.VelicinaPart == part);
                    if (bonusR != null)
                    {
                        if (bonusR.Hodnota == -1*Consts.BonusPod50K_3)
                            return "Dobrovolně zveřejňuje velké množství smluv pod 50.000 Kč.";
                        if (bonusR.Hodnota == -1 * Consts.BonusPod50K_2)
                            return "Dobrovolně zveřejňuje smlouvy pod 50.000 Kč.";
                        if (bonusR.Hodnota == -1 * Consts.BonusPod50K_1)
                            return "Dobrovolně zveřejňuje smlouvy pod 50.000 Kč.";
                    }
                    return "Nesplňuje podmínku pro udělení bonusu za transparentnost.";
                default:
                    return "";
            }
        }


        private string Best(Annual data, int year, string ico, out KIndexParts? usedPart)
        {
            Statistics stat = Statistics.GetStatistics(year);
            
            usedPart = data.OrderedValuesFromBestForInfofacts(ico).FirstOrDefault();
            if (usedPart != null)
            {
                return KIndexData.KIndexCommentForPart(usedPart.Value, data);
            }
            return null;
        }
        private string Worst(Annual data, int year, string ico, out KIndexParts? usedPart)
        {
            usedPart = data.OrderedValuesFromBestForInfofacts(ico)?.Reverse()?.FirstOrDefault();
            if (usedPart != null)
            {
                return KIndexData.KIndexCommentForPart(usedPart.Value, data);
            }
            return null;
        }

        public InfoFact[] InfoFacts(int year)
        {
            var ann = this.roky.Where(m => m.Rok == year).FirstOrDefault();

            List<InfoFact> facts = new List<InfoFact>();
            if (ann == null || ann.KIndexReady == false)
            {
                facts.Add(new InfoFact($"K-Index nespočítán. Organizace má méně než {Consts.MinSmluvPerYear} smluv za rok nebo malý objem smluv.", InfoFact.ImportanceLevel.Summary));
                return facts.ToArray();
            }
            switch (ann.KIndexLabel)
            {
                case KIndexLabelValues.None:
                    facts.Add(new InfoFact($"K-Index nespočítán. Organizace má méně než {Consts.MinSmluvPerYear} smluv za rok nebo malý objem smluv.", InfoFact.ImportanceLevel.Summary));
                    return facts.ToArray();

                case KIndexLabelValues.A:
                    facts.Add(new InfoFact("Nevykazuje téměř žádné rizikové faktory.", InfoFact.ImportanceLevel.Summary));
                    break;
                case KIndexLabelValues.B:
                    facts.Add(new InfoFact("Chování s malou mírou rizikových faktorů.", InfoFact.ImportanceLevel.Summary));
                    break;
                case KIndexLabelValues.C:
                    facts.Add(new InfoFact("Částečně umožňuje rizikové jednání.", InfoFact.ImportanceLevel.Summary));
                    break;
                case KIndexLabelValues.D:
                    facts.Add(new InfoFact("Vyšší míra rizikových faktorů.", InfoFact.ImportanceLevel.Summary));
                    break;
                case KIndexLabelValues.E:
                    facts.Add(new InfoFact("Vysoký výskyt rizikových faktorů.", InfoFact.ImportanceLevel.Summary));
                    break;
                case KIndexLabelValues.F:
                    facts.Add(new InfoFact("Velmi vysoká míra rizikových faktorů.", InfoFact.ImportanceLevel.Summary));
                    break;
                default:
                    break;
            }
            KIndexParts? bestPart = null;
            KIndexParts? worstPart = null;
            var sBest = Best(ann, year, this.Ico, out bestPart);
            var sworst = Worst(ann, year, this.Ico, out worstPart);

            //A-C dej pozitivni prvni
            if (ann.KIndexLabel == KIndexLabelValues.A
                || ann.KIndexLabel == KIndexLabelValues.B
                || ann.KIndexLabel == KIndexLabelValues.C
                )
            {
                if (!string.IsNullOrEmpty(sBest))
                    facts.Add(new InfoFact(sBest, InfoFact.ImportanceLevel.Stat));
                if (!string.IsNullOrEmpty(sworst))
                    facts.Add(new InfoFact(sworst, InfoFact.ImportanceLevel.Stat));
            }
            else
            {
                if (!string.IsNullOrEmpty(sworst))
                    facts.Add(new InfoFact(sworst, InfoFact.ImportanceLevel.Stat));
                if (!string.IsNullOrEmpty(sBest))
                    facts.Add(new InfoFact(sBest, InfoFact.ImportanceLevel.Stat));
            }
            foreach (var part in ann.OrderedValuesFromBestForInfofacts(this.Ico).Reverse())
            {
                if (part != bestPart && part != worstPart)
                {
                    var sFacts = KIndexData.KIndexCommentForPart(part, ann);
                    if (!string.IsNullOrEmpty(sFacts))
                        facts.Add(new InfoFact(sFacts, InfoFact.ImportanceLevel.High));
                }
            }

            return facts.ToArray();
        }

        public InfoFact[] InfoFacts()
        {
            var kidx = LastKIndex();
            return InfoFacts(kidx?.Rok ?? Consts.CalculationYears.Max());
        }

        public bool NotInterestingToShow()
        {
            return false;
        }

        public string SocialInfoBody()
        {
            return ""; //TODO
        }

        public string SocialInfoFooter()
        {
            return ""; //TODO
        }

        public string SocialInfoImageUrl()
        {
            return ""; //TODO
        }

        public string SocialInfoSubTitle()
        {
            return this.LastKIndexLabel().ToString() + " index korupčního rizika";
        }

        public string SocialInfoTitle()
        {
            return this.Jmeno;
        }
    }
}
