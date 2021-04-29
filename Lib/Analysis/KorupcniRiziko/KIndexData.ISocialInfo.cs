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
            var txt = data.Info(part).ShortComment;
            if (html)
                return txt;
            else
                return Devmasters.TextUtil.RemoveHTML(txt);
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
            return InfoFacts(kidx?.Rok ?? Consts.AvailableCalculationYears.Max());
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
            return this.LastKIndexLabel().ToString() + " index klíčových rizik";
        }

        public string SocialInfoTitle()
        {
            return this.Jmeno;
        }
    }
}
