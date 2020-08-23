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
            var lbl = KIndexLabelForPart(part, data.KIndexVypocet.Radky.Where(m => m.VelicinaPart == part).First().Hodnota);
            switch (part)
            {
                case KIndexParts.PercentBezCeny:
                    switch (lbl)
                    {
                        case KIndexLabelValues.A:
                            return "Neskrývá ceny u žádných smluv.";
                        case KIndexLabelValues.B:
                            return "Skrývá ceny u minima smluv.";
                        case KIndexLabelValues.C:
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                            return $"Skrývá ceny u {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvBezCeny, "{0} smlouva;{0} smlouvy;{0} smluv")}.";
                        case KIndexLabelValues.F:
                            return $"Skrývá ceny u varujícího počtu {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvBezCeny, "{0} smlouva;{0} smlouvy;{0} smluv")}.";
                        default:
                            return "";
                    }
                case KIndexParts.PercSeZasadnimNedostatkem:
                    switch (lbl)
                    {
                        case KIndexLabelValues.A:
                            return "Nemá žádné smlouvy se zásadními nedostatky.";
                        case KIndexLabelValues.B:
                        case KIndexLabelValues.C:
                            return $"Zásadní nedostatky evidujeme u {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvSeZasadnimNedostatkem, "{0} smlouvy;{0} smluv;{0} smluv")} smluv.";
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                        case KIndexLabelValues.F:
                            return $"Zásadní nedostatky evidujeme u <b>{HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvSeZasadnimNedostatkem, "{0} smlouvy;{0} smlouv;{0} smluv")}</b> z celkem {data.Statistika.PocetSmluv} smluv.";
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
                            return "Rozdíly mezi velikostí zakázek dodavatelů jsou malé.";
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                            return "Velké zakázky se koncentrují u malého počtu dodavatelů.";
                        case KIndexLabelValues.F:
                            return $"Většinu peněz {HlidacStatu.Util.PluralForm.Get(data.CelkovaKoncentraceDodavatelu.TopDodavatele().Count(), "dostal {0} dodavatel;dostali {0} dodavatelé;dostalo {0} dodavatelů")} z celkem {data.CelkovaKoncentraceDodavatelu.Dodavatele.Count()} dodavatelů.";
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
                            return "Zakázky se skrytou cenou uzavřelo více dodavatelů a žádný nedominuje.";
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                            return "Zakázky se skrytou cenou se koncentrují u malého počtu dodavatelů.";
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
                            return "Zakázky s podezřelou cenou uzavřelo více dodavatelů a žádný nedominuje.";
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
                            return "Zakázky v jednotlivých oborech se nekoncentrují u žádných dodavatelů.";
                        case KIndexLabelValues.B:
                        case KIndexLabelValues.C:
                            return "Zakázky v jednotlivých oborech uzavřelo více dodavatelů a žádný výrazně nedominuje.";
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                            var oboryE = data.KoncetraceDodavateluObory
                                .Where(m => KIndexData.KIndexLabelForPart(KIndexParts.KoncentraceDodavateluObory, m.Koncentrace.Herfindahl_Hirschman_Modified) >= KIndexLabelValues.D);
                            var soboryE = oboryE.Select(m => m.OborName)
                                .Aggregate((f, s) => f + ", " + s);

                            return $"Dominance zakázek u dodavatelů je objevuje v {HlidacStatu.Util.PluralForm.Get(oboryE.Count(), "oboru;oborech;oborech")} {soboryE}.";
                        case KIndexLabelValues.F:
                            var oboryF = data.KoncetraceDodavateluObory
                                .Where(m => KIndexData.KIndexLabelForPart(KIndexParts.KoncentraceDodavateluObory, m.Koncentrace.Herfindahl_Hirschman_Modified) >= KIndexLabelValues.D);
                            var soboryF = oboryF.Select(m => m.OborName)
                                .Aggregate((f, s) => f + ", " + s);

                            return $"Dominance zakázek u dodavatelů je v {HlidacStatu.Util.PluralForm.Get(oboryF.Count(), "oboru;oborech;oborech")} {soboryF}.";
                        default:
                            return "";
                    }
                case KIndexParts.PercSmluvUlimitu:
                    switch (lbl)
                    {
                        case KIndexLabelValues.A:
                            return "Organizace neobhází limity zákona o veřejných zakázkách.";
                        case KIndexLabelValues.B:
                            return "Velmi málo smluv má hodnotu blízkou limitům pro veřejné zakázky.";
                        case KIndexLabelValues.C:
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                        case KIndexLabelValues.F:
                            return $"{HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvULimitu, "{0} smlouva;{0} smlouvy;{0} smluv")} má hodnotu blízkou limitům veřejných zakázek, což může naznačovat snahu se vyhnout řádné veřejné soutěži.";
                        //return $"Zásadní nedostatky evidujeme u {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvSeZasadnimNedostatkem, "{0} smlouva;{0} smlouvy;{0} smluv")} z {data.Statistika.PocetSmluv}";
                        default:
                            return "";
                    }
                case KIndexParts.PercNovaFirmaDodavatel:
                    switch (lbl)
                    {
                        case KIndexLabelValues.A:
                            return "Nemá žádné smlouvy s nově založenými firmami.";
                        case KIndexLabelValues.B:
                        case KIndexLabelValues.C:
                            return $"Uzavřela {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvNovaFirma, "{0} smlouvu;{0} smlouvy;{0} smluv")} s nově založenými firmami.";
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                        case KIndexLabelValues.F:
                            return $"Uzavřela {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvNovaFirma, "{0} smlouvu;{0} smlouvy;{0} smluv")} s nově založenými firmami.";
                        default:
                            return "";
                    }
                case KIndexParts.PercUzavrenoOVikendu:
                    switch (lbl)
                    {
                        case KIndexLabelValues.A:
                            return "Nemá žádné smlouvy uzavřené o víkendu nebo svátku.";
                        case KIndexLabelValues.B:
                        case KIndexLabelValues.C:
                            return $"Uzavřela {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvOVikendu, "{0} smlouvu;{0} smlouvy;{0} smluv")} o víkendu nebo svátku.";
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                        case KIndexLabelValues.F:
                            return $"Uzavřela {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvOVikendu, "{0} smlouvu;{0} smlouvy;{0} smluv")} o víkendu nebo svátku.";
                        default:
                            return "";
                    }
                case KIndexParts.PercSmlouvySPolitickyAngazovanouFirmou:
                    switch (lbl)
                    {
                        case KIndexLabelValues.A:
                            return "Nemá žádné smlouvy uzavřené s firmami, jejichž majitelé či ony sami sponzorovali politické strany.";
                        case KIndexLabelValues.B:
                        case KIndexLabelValues.C:
                            return $"Uzavřela {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvPolitiky, "{0} smlouvu;{0} smlouvy;{0} smluv")} s firmami, jejichž majitelé či ony sami sponzorovali politické strany.";
                        case KIndexLabelValues.D:
                        case KIndexLabelValues.E:
                        case KIndexLabelValues.F:
                            return $"Uzavřela {HlidacStatu.Util.PluralForm.Get((int)data.Statistika.PocetSmluvPolitiky, "{0} smlouvu;{0} smlouvy;{0} smluv")} ({Util.RenderData.NicePercent(data.Statistika.PercentKcSmluvPolitiky)}) s firmami, jejichž majitelé či ony sami sponzorovali politické strany.";
                        default:
                            return "";
                    }
                case KIndexParts.PercSmlouvyPod50kBonus:
                    var bonusR = data.KIndexVypocet.Radky.FirstOrDefault(m => m.VelicinaPart == part);
                    if (bonusR != null)
                    {
                        if (bonusR.Hodnota == Consts.BonusPod50K_3)
                            return "Dobrovolně zveřejňuje velké množství smluv pod 50.000 Kč a výrazně zvyšuje svou transparentnost hospodaření.";
                        if (bonusR.Hodnota == Consts.BonusPod50K_2)
                            return "Dobrovolně zveřejňuje smluvy pod 50.000 Kč a výrazně zvyšuje svou transparentnost hospodaření.";
                        if (bonusR.Hodnota == Consts.BonusPod50K_1)
                            return "Dobrovolně zveřejňuje smluvy pod 50.000 Kč a zvyšuje svou transparentnost hospodaření.";
                    }
                    return "Nesplněna podmínka pro udělení bonusu za transparentnost.";
                default:
                    return "";
            }
        }



        private string Best(Annual data, int year, string ico, out KIndexParts? usedPart)
        {
            Statistics stat = Statistics.GetStatistics(year);
            var bestRarr = data.KIndexVypocet.Radky
                .Select(m => new { r = m, rank = stat.SubjektRank(ico, m.VelicinaPart) })
                .Where(m => m.rank.HasValue)
                .Where(m =>
                    m.r.VelicinaPart != KIndexParts.PercNovaFirmaDodavatel //nezajimava oblast
                                                                           //&& m.r.VelicinaPart != KIndexParts.
                )
                .OrderBy(m => m.rank)
                .ThenBy(o => o.r.Hodnota)
                .ToArray(); //better debug
            var bestR = bestRarr.FirstOrDefault();
            usedPart = bestR?.r?.VelicinaPart;
            if (bestR != null)
            {
                return KIndexData.KIndexCommentForPart(bestR.r.VelicinaPart, data);
            }
            return null;
        }
        private string Worst(Annual data, int year, string ico, out KIndexParts? usedPart)
        {
            Statistics stat = Statistics.GetStatistics(year);
            var worstRarr = data.KIndexVypocet.Radky
                .Select(m => new { r = m, rank = stat.SubjektRank(ico, m.VelicinaPart) })
                .Where(m => m.rank.HasValue)
                .Where(m =>
                    m.r.VelicinaPart != KIndexParts.PercNovaFirmaDodavatel //nezajimava oblast
                                                                           //&& m.r.VelicinaPart != KIndexParts.
                )
                .OrderByDescending(m => m.rank)
                .ThenByDescending(o => o.r.Hodnota)
                .ToArray(); //better debug
            var worstR = worstRarr.FirstOrDefault();
            usedPart = worstR?.r?.VelicinaPart;
            if (worstR != null)
            {
                return KIndexData.KIndexCommentForPart(worstR.r.VelicinaPart, data);
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
                    facts.Add(new InfoFact("Velmi málo rizikových faktorů.", InfoFact.ImportanceLevel.Summary));
                    break;
                case KIndexLabelValues.B:
                    facts.Add(new InfoFact("Málo rizikových faktorů.", InfoFact.ImportanceLevel.Summary));
                    break;
                case KIndexLabelValues.C:
                    break;
                case KIndexLabelValues.D:
                    break;
                case KIndexLabelValues.E:
                    break;
                case KIndexLabelValues.F:
                    break;
                default:
                    break;
            }
            KIndexParts? bestPart = null;
            KIndexParts? worstPart = null;
            var sBest = Best(ann, year, this.Ico, out bestPart);
            if (!string.IsNullOrEmpty(sBest))
                facts.Add(new InfoFact(sBest, InfoFact.ImportanceLevel.Summary));
            var sworst = Worst(ann, year, this.Ico, out worstPart);
            if (!string.IsNullOrEmpty(sworst))
                facts.Add(new InfoFact(sworst, InfoFact.ImportanceLevel.Summary));

            foreach (var part in Devmasters.Enums.EnumTools.EnumToEnumerable<KIndexParts>().Select(m => m.Value))
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
            return InfoFacts(kidx?.Rok ?? 2019);
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
