using org.omg.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public partial class Statistics
    {

        static int[] Percentiles = new int[] { 1, 5, 10, 33, 50, 66, 90, 95, 99 };

        public static Devmasters.Cache.V20.File.FileCache<Analysis.KorupcniRiziko.Statistics[]> KIndexStatTotal = new Devmasters.Cache.V20.File.FileCache<Analysis.KorupcniRiziko.Statistics[]>(
                Lib.Init.WebAppDataPath, TimeSpan.Zero, "KIndexStat",
                    (o) =>
                    {
                        return Lib.Analysis.KorupcniRiziko.Statistics.Calculate(2019).ToArray();
                    });



        public decimal AverageKindex { get; set; }
        public Dictionary<int, decimal> PercentileKIndex { get; set; } = new Dictionary<int, decimal>();
        public List<(string ico, decimal kindex)> SubjektOrderedListKIndexAsc { get; set; }
        public Dictionary<KIndexData.KIndexParts, List<(string ico, decimal kindex)>> SubjektOrderedListPartsAsc { get; set; } = new Dictionary<KIndexData.KIndexParts, List<(string ico, decimal kindex)>>();


        public KIndexData.VypocetDetail AverageParts { get; set; }
        public Dictionary<int, KIndexData.VypocetDetail> PercentileParts { get; set; } = new Dictionary<int, KIndexData.VypocetDetail>();
        public int Rok { get; set; }


        //private 
        protected Statistics() { }
        public static Statistics GetStatistics(int year)
        {
            var stat = KIndexStatTotal.Get().FirstOrDefault(m => m.Rok == year);
            if (stat == null)
                return null;
            else
                return stat;
        }

        /// <summary>
        /// Select companies and how much they improved since year - 1
        /// </summary>
        /// <param name="year">Minimum year = 2018</param>
        /// <returns>Positive number means improvement. Negative number means worsening.</returns>
        public static IEnumerable<Company> GetJumpers(int year)
        {
            if(year < 2017 || year >= DateTime.Now.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(year), $"Year must be bigger than 2017 and smaler than {DateTime.Now.Year}.");
            }


            var statChosenYear = KIndexStatTotal.Get().FirstOrDefault(m => m.Rok == year).SubjektOrderedListKIndexAsc;
            var statYearBefore = KIndexStatTotal.Get().FirstOrDefault(m => m.Rok == year - 1).SubjektOrderedListKIndexAsc;

            var result = statChosenYear.Join(statYearBefore,
                cy => cy.ico,
                yb => yb.ico,
                (cy, yb) => {
                    Company.GetCompanies().TryGetValue(cy.ico, out Company comp);
                    comp.Value4Sort = yb.kindex - cy.kindex;
                    return comp;
                });

            if (statChosenYear == null || statYearBefore == null)
                return new List<Company>();
            else
                return result;
        }

        public decimal Average()
        {
            return this.AverageKindex;
        }
        public decimal Average(KIndexData.KIndexParts part)
        {
            return this.AverageParts.Radky.First(m => m.Velicina == (int)part).Hodnota;
        }

        public IEnumerable<(string ico, decimal kindex)> Filter(IEnumerable<(string ico, decimal kindex)> source, IEnumerable<string> filterIco = null, bool showNone = false)
        {
            if (showNone)
            {
                var data = source
                    .Where(m => (filterIco == null) || filterIco.Contains(m.ico));
                if (filterIco != null && filterIco.Count() > 0)
                {
                    IEnumerable<(string ico, decimal kindex)> missing_data = filterIco
                        .Except(data.Select(m => m.ico))
                        .Select(m => (m,Consts.MinSmluvPerYearKIndexValue));
                    return data.Concat(missing_data);
                }
                else return data;
            }
            else
                return source
                    .Where(m => (filterIco == null) || filterIco.Contains(m.ico));
        }

        public IEnumerable<Company> SubjektOrderedListKIndexCompanyAsc(IEnumerable<string> filterIco = null, bool showNone = false)
        {
            return Filter(SubjektOrderedListKIndexAsc,filterIco, showNone)
                .OrderBy(m => m.kindex)
                .Select(m => new Company(
                    Company.GetCompanies().ContainsKey(m.ico)
                        ? Company.GetCompanies()[m.ico].Name
                        : Lib.Data.Firmy.GetJmeno(m.ico)
                    , m.ico, m.kindex)
                );
        }

        public IEnumerable<Company> SubjektOrderedListPartsCompanyAsc(KIndexData.KIndexParts part, IEnumerable<string> filterIco = null, bool showNone = false)
        {
            return Filter(SubjektOrderedListPartsAsc[part],filterIco,showNone)
                .OrderBy(m => m.kindex)
                .Select(m => new Company(
                    Company.GetCompanies().ContainsKey(m.ico)
                        ? Company.GetCompanies()[m.ico].Name
                        : Lib.Data.Firmy.GetJmeno(m.ico)
                    , m.ico, m.kindex)
                )
                .ToList();
        }

        public int? SubjektRank(string ico)
        {
            var res = this.SubjektOrderedListKIndexAsc.FindIndex(m => m.ico == ico);
            if (res == -1)
                return null;
            else
                return res + 1;
        }
        public int? SubjektRank(string ico, KIndexData.KIndexParts part)
        {
            var res = this.SubjektOrderedListPartsAsc[part].FindIndex(m => m.ico == ico);
            if (res == -1)
                return null;
            else
                return res + 1;
        }

        public string SubjektRankText(string ico)
        {
            var rank = SubjektRank(ico);
            if (rank == null)
                return "";
            else
                return RankText(rank.Value, this.SubjektOrderedListKIndexAsc.Count);
        }

        public string SubjektRankText(string ico, KIndexData.KIndexParts part)
        {
            var rank = SubjektRank(ico, part);
            if (rank == null)
                return "";
            else
                return RankText(rank.Value, this.SubjektOrderedListPartsAsc[part].Count);
        }

        public string RankText(int rank, int count)
        {
            if (rank == 1)
            {
                return $"nejlepší";
            }
            else if (rank == count)
            {
                return $"nejhorší";
            }
            else if (rank <= 100)
            {
                return $"{rank}. nejlepší";
            }
            else if (rank >= count - 100)
            {
                return $"{count - rank}. z nejhorších";
            }
            else
            {
                return $"{rank}. z {count}";
            }

        }

        public decimal Percentil(int perc, KIndexData.KIndexParts part)
        {
            return this.PercentileParts[perc].Radky.First(m => m.Velicina == (int)part).Hodnota;
        }


        public decimal Percentil(int perc)
        {
            return this.PercentileKIndex[perc];
        }

        public PercInterval GetKIndexPercentile(decimal value)
        {
            int prefValue = 0;
            foreach (var perc in this.PercentileKIndex)
            {
                if (value <= perc.Value)
                    return new PercInterval(prefValue, perc.Key);

                prefValue = perc.Key;
            }
            return new PercInterval(prefValue, 100);
        }
        public PercInterval GetPartPercentil(KIndexData.KIndexParts part, decimal value)
        {
            int prefValue = 0;
            foreach (var perc in this.PercentileParts)
            {
                var percValue = perc.Value.Radky.First(m => m.Velicina == (int)part).Hodnota;
                if (value <= percValue)
                    return new PercInterval(prefValue, perc.Key);

                prefValue = perc.Key;
            }
            return new PercInterval(prefValue, 100);
        }

        public string PercIntervalShortText(KIndexData.KIndexParts part, decimal value)
        {
            return PercIntervalShortText(GetPartPercentil(part, value));
        }
        public string PercIntervalShortText(decimal value)
        {
            return PercIntervalShortText(GetKIndexPercentile(value));
        }

    }
}
