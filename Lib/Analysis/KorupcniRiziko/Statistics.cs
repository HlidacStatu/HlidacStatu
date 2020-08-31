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

        static int[] Percentiles = new int[] { 1, 5, 10, 25,33, 50, 66, 75, 90, 95, 99 };

        public static Devmasters.Cache.V20.File.FileCache<Analysis.KorupcniRiziko.Statistics[]> KIndexStatTotal = new Devmasters.Cache.V20.File.FileCache<Analysis.KorupcniRiziko.Statistics[]>(
                Lib.Init.WebAppDataPath, TimeSpan.Zero, "KIndexStat",
                    (o) =>
                    {
                        return Lib.Analysis.KorupcniRiziko.Statistics.Calculate().ToArray();
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
        public static IEnumerable<SubjectWithKIndexTrend> GetJumpersFromBest(int year)
        {
            if (year < Consts.CalculationYears.Min() + 1)
                year = Consts.CalculationYears.Min() + 1;
            if (year > Consts.CalculationYears.Max() )
                year = Consts.CalculationYears.Max();

            var statChosenYear = KIndexStatTotal.Get().FirstOrDefault(m => m.Rok == year).SubjektOrderedListKIndexAsc;
            var statYearBefore = KIndexStatTotal.Get().FirstOrDefault(m => m.Rok == year - 1).SubjektOrderedListKIndexAsc;

            IEnumerable<SubjectWithKIndexTrend> result = statChosenYear.Join(statYearBefore,
                cy => cy.ico,
                yb => yb.ico,
                (cy, yb) => {
                    SubjectNameCache.GetCompanies().TryGetValue(cy.ico, out SubjectNameCache comp);
                    var r = new SubjectWithKIndexTrend()
                    {
                        Ico = cy.ico,
                        Jmeno = comp?.Name,
                        KIndex = Math.Abs(yb.kindex - cy.kindex),
                        Group = yb.kindex - cy.kindex < 0 ? "Zhoršení ratingu" : "Zlepšení ratingu",
                        Roky = new Dictionary<int, decimal> { { year - 1, yb.kindex }, { year, cy.kindex } }
                    };
                    return r;
                })
                .Where(m=>(m.Roky.First().Value - m.Roky.Last().Value )!= 0)
                .OrderByDescending(c => c.KIndex);

            if (statChosenYear == null || statYearBefore == null)
                return new List<SubjectWithKIndexTrend>();
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

        public IEnumerable<SubjectWithKIndex> Filter(IEnumerable<(string ico, decimal kindex)> source, IEnumerable<Lib.Data.Firma.Zatrideni.Item> filterIco = null, bool showNone = false)
        {
            IEnumerable<SubjectWithKIndex> data;
            if (filterIco != null && filterIco.Count() > 0)
            {
                data = source.Join(
                    filterIco,
                    cy => cy.ico,
                    yb => yb.Ico,
                    (cy, yb) =>
                        new SubjectWithKIndex()
                        {
                            Ico = cy.ico,
                            KIndex = cy.kindex,
                            Jmeno = yb.Jmeno,
                            Group = yb.Group,
                            KrajId = yb.KrajId,
                            Kraj = yb.Kraj
                        }
                    );
            }
            else
            {
                data = source.
                    Select(m => new SubjectWithKIndex()
                    {
                        Ico = m.ico,
                        Jmeno = SubjectNameCache.CachedCompanies.Get()[m.ico].Name,
                        KrajId = "",
                        Group="",
                        KIndex = m.kindex
                    }
                    ) ;
            }
            if (showNone)
            {
                if (filterIco != null && filterIco.Count() > 0)
                {
                    var missing_ico = filterIco.Select(m => m.Ico).Except(data.Select(m => m.Ico));

                    IEnumerable<SubjectWithKIndex> missing_data = missing_ico
                        .Join(filterIco,mi=>mi, fi=>fi.Ico, (mi,fi)=> 
                            new SubjectWithKIndex()
                            {
                                Ico = fi.Ico,
                                KIndex = Consts.MinSmluvPerYearKIndexValue,
                                Jmeno = fi.Jmeno,
                                Group = fi.Group,
                                KrajId = fi.KrajId,
                                Kraj = fi.Kraj
                            });
                    data = data.Concat(missing_data);
                }
            }
            return data;
        }

        public IEnumerable<SubjectWithKIndex> SubjektOrderedListKIndexCompanyAsc(IEnumerable<Lib.Data.Firma.Zatrideni.Item> filterIco = null, bool showNone = false)
        {
            return Filter(SubjektOrderedListKIndexAsc,filterIco, showNone)
                .OrderBy(m => m.KIndex);
        }

        public IEnumerable<SubjectWithKIndex> SubjektOrderedListPartsCompanyAsc(KIndexData.KIndexParts part, IEnumerable<Lib.Data.Firma.Zatrideni.Item> filterIco = null, bool showNone = false)
        {
            return Filter(SubjektOrderedListPartsAsc[part],filterIco,showNone)
                .OrderBy(m => m.KIndex);
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
