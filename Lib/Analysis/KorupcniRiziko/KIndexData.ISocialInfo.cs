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

        private string Best(VypocetDetail vypocet, int year, string ico)
        {
            Statistics stat = Statistics.GetStatistics(year);
            var bestR = vypocet.Radky
                .Select(m => new { r = m, rank = stat.SubjektRank(ico, m.VelicinaPart) })
                .Where(m=>m.rank.HasValue)
                .OrderBy(m => m.rank)
                .FirstOrDefault();
            if (bestR != null)
            { 

            }
                //return new Tuple<VypocetDetail.Radek, string>(bestR.r, stat.SubjektRankText(ico, bestR.r.VelicinaPart));

            return null;
        }
        public InfoFact[] InfoFacts(int year)
        {
            var ann = this.roky[year];
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
                    facts.Add(new InfoFact("Velmi málo rizikových faktorů.", InfoFact.ImportanceLevel.Summary));
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
            throw new NotImplementedException();
        }
        public InfoFact[] InfoFacts()
        {
            return InfoFacts(this.roky.OrderByDescending(o => o.Rok).FirstOrDefault(m => m.KIndexReady)?.Rok ?? 2019);
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
