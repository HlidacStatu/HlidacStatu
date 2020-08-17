using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class Statistics
    {
        public class PercInterval
        {
            public PercInterval() { }
            public PercInterval(int value) { From = value; To = value; }
            public PercInterval(int from, int to) { From = from; To = to; }
            public int From { get; set; }
            public int To { get; set; }
        }


        public static Devmasters.Cache.V20.File.FileCache<Analysis.KorupcniRiziko.Statistics[]> KIndexStat = new Devmasters.Cache.V20.File.FileCache<Analysis.KorupcniRiziko.Statistics[]>(
                Lib.Init.WebAppDataPath, TimeSpan.Zero, "KIndexStat",
                    (o) =>
                    {
                        return Lib.Analysis.KorupcniRiziko.Statistics.Calculate(2019).ToArray();
                    });


        static int[] Percentiles = new int[] { 1, 5, 10, 33, 50, 66, 90, 95, 99 };

        public decimal AverageKindex { get; set; }
        public Dictionary<int, decimal> PercentileKIndex { get; set; } = new Dictionary<int, decimal>();
        public List<string> SubjectOrderedListKIndex { get; set; }
        public Dictionary<KIndexData.KIndexParts, List<string>> SubjectOrderedListParts { get; set; } = new Dictionary<KIndexData.KIndexParts, List<string>>();


        public KIndexData.VypocetDetail AverageParts { get; set; }
        public Dictionary<int, KIndexData.VypocetDetail> PercentileParts { get; set; } = new Dictionary<int, KIndexData.VypocetDetail>();
        public int Rok { get; set; }


        public static IEnumerable<Statistics> Calculate(int untilYear, string[] forIcos = null)
        {
            int[] calculationYears = Enumerable.Range(2017, untilYear - 2017 + 1).ToArray();

            Func<int, int, Nest.ISearchResponse<Lib.Analysis.KorupcniRiziko.KIndexData>> searchfnc = (size, page) =>
            {
                return Lib.ES.Manager.GetESClient_KIndex().Search<Lib.Analysis.KorupcniRiziko.KIndexData>(a => a
                            .Size(size)
                            .From(page * size)
                            .Query(q => q.MatchAll())
                            .Scroll("10m")
                            );
            };

            List<Lib.Analysis.KorupcniRiziko.KIndexData> data = new List<Analysis.KorupcniRiziko.KIndexData>();
            Lib.Searching.Tools.DoActionForQuery<Lib.Analysis.KorupcniRiziko.KIndexData>(Lib.ES.Manager.GetESClient_KIndex(),
                searchfnc,
                (hit, param) =>
                {
                    if (hit.Source.roky.Any(m => m.KIndexAvailable))
                        data.Add(hit.Source);


                    return new Devmasters.Core.Batch.ActionOutputData() { CancelRunning = false, Log = null };
                }, null, Devmasters.Core.Batch.Manager.DefaultOutputWriter, Devmasters.Core.Batch.Manager.DefaultProgressWriter, 
                false, prefix: "GET data ");


            List<Statistics> stats = new List<Statistics>();

            foreach (var year in calculationYears)
            {
                var datayear = data
                    .Where(m => (forIcos == null || forIcos?.Contains(m.Ico) == true))
                    .Select(m => new { ic = m.Ico, data = m.ForYear(year) })
                    .Where(y => y != null && y.data != null && y.data.KIndexAvailable)
                    .ToDictionary(k => k.ic, v => v.data);

                var stat = new Statistics() { Rok = year };
                //poradi
                stat.SubjectOrderedListKIndex = datayear
                    .Where(m => m.Value.KIndex != Consts.MinSmluvPerYearKIndexValue)
                    .OrderBy(m => m.Value.KIndex)
                    .Select(m => m.Key)
                    .ToList();
                foreach (KIndexData.KIndexParts part in Enum.GetValues(typeof(KIndexData.KIndexParts)))
                {
                    stat.SubjectOrderedListParts.Add(part,datayear
                        .Where(m => m.Value.KIndex != Consts.MinSmluvPerYearKIndexValue)
                        .OrderBy(m => m.Value.KIndexVypocet.Radky.First(r => r.VelicinaPart == part).Hodnota)
                        .Select(m => m.Key)
                        .ToList()
                        );
                }

                //prumery
                stat.AverageKindex = datayear.Average(m => m.Value.KIndex.Value);

                stat.AverageParts = new KIndexData.VypocetDetail();
                List<KIndexData.VypocetDetail.Radek> radky = new List<KIndexData.VypocetDetail.Radek>();
                foreach (KIndexData.KIndexParts part in Enum.GetValues(typeof(KIndexData.KIndexParts)))
                {
                    decimal val = datayear.Select(m => m.Value.KIndexVypocet.Radky.FirstOrDefault(r => r.Velicina == (int)part))
                            .Average(a => a.Hodnota);

                    KIndexData.VypocetDetail.Radek radek = new KIndexData.VypocetDetail.Radek(part, val, KIndexData.DefaultKIndexPartKoeficient(part));
                    radky.Add(radek);
                }
                stat.AverageParts = new KIndexData.VypocetDetail() { Radky = radky.ToArray() };


                //percentily
                stat.PercentileKIndex = new Dictionary<int, decimal>();
                stat.PercentileParts = new Dictionary<int, KIndexData.VypocetDetail>();


                foreach (var perc in Percentiles)
                {


                    stat.PercentileKIndex.Add(perc,
                        HlidacStatu.Util.MathTools.PercentileCont(perc / 100m, datayear.Select(m => m.Value.KIndex.Value))
                        );

                    radky = new List<KIndexData.VypocetDetail.Radek>();
                    foreach (KIndexData.KIndexParts part in Enum.GetValues(typeof(KIndexData.KIndexParts)))
                    {
                        decimal val = HlidacStatu.Util.MathTools.PercentileCont(perc / 100m,
                                datayear.Select(m => m.Value.KIndexVypocet.Radky.FirstOrDefault(r => r.Velicina == (int)part).Hodnota)
                            );
                        KIndexData.VypocetDetail.Radek radek = new KIndexData.VypocetDetail.Radek(part, val, KIndexData.DefaultKIndexPartKoeficient(part));
                        radky.Add(radek);
                    }
                    stat.PercentileParts.Add(perc, new KIndexData.VypocetDetail() { Radky = radky.ToArray() });
                }

                stats.Add(stat);
            } //year

            return stats;
        }

        public static decimal Average(int year)
        {
            var stat = KIndexStat.Get().FirstOrDefault(m => m.Rok == year);
            if (stat == null)
                return 0;
            else
                return stat.AverageKindex;
        }
        public static decimal Average(int year, KIndexData.KIndexParts part)
        {
            var stat = KIndexStat.Get().FirstOrDefault(m => m.Rok == year);
            if (stat == null)
                return 0;
            else
                return stat.AverageParts.Radky.First(m => m.Velicina == (int)part).Hodnota;
        }

        public static decimal Percentil(int perc, int year)
        {
            var stat = KIndexStat.Get().FirstOrDefault(m => m.Rok == year);
            if (stat == null)
                return 0;
            else
                return stat.PercentileKIndex[perc];
        }
        public static decimal Percentil(int perc, int year, KIndexData.KIndexParts part)
        {
            var stat = KIndexStat.Get().FirstOrDefault(m => m.Rok == year);
            if (stat == null)
                return 0;
            else
                return stat.PercentileParts[perc].Radky.First(m => m.Velicina == (int)part).Hodnota;
        }

        public static PercInterval GetKIndexPercentile(int year, decimal value)
        {
            var stat = KIndexStat.Get().FirstOrDefault(m => m.Rok == year);
            if (stat == null)
                return new PercInterval(0);
            int prefValue = 0;
            foreach (var perc in stat.PercentileKIndex)
            {
                if (value <= perc.Value)
                    return new PercInterval(prefValue, perc.Key);

                prefValue = perc.Key;
            }
            return new PercInterval(prefValue, 100);
        }
        public static PercInterval GetPartPercentil(int year, KIndexData.KIndexParts part, decimal value)
        {
            var stat = KIndexStat.Get().FirstOrDefault(m => m.Rok == year);
            if (stat == null)
                return new PercInterval(0);
            int prefValue = 0;
            foreach (var perc in stat.PercentileParts)
            {
                var percValue = perc.Value.Radky.First(m => m.Velicina == (int)part).Hodnota;
                if (value <= percValue)
                    return new PercInterval(prefValue, perc.Key);

                prefValue = perc.Key;
            }
            return new PercInterval(prefValue, 100);
        }

        public static string PercIntervalShortText(int year, KIndexData.KIndexParts part, decimal value)
        {
            return PercIntervalShortText(GetPartPercentil(year, part, value));
        }
        public static string PercIntervalShortText(int year, decimal value)
        {
            return PercIntervalShortText(GetKIndexPercentile(year, value));
        }

        public static string PercIntervalShortText(PercInterval val)
        {
            if (val.To <= 50)
            {
                if (val.To <= 10)
                    return $"je mezi {val.To} % nejlepších";
                else if (val.To <= 33)
                    return $"patří mezi první třetinu nejlepších";
                else
                    return "je v lepší polovině";
            }
            else
            {
                if (val.From >= 90)
                    return $"je mezi {100 - val.From} % nejhorších";
                else if (val.From >= 66)
                    return $"patří do třetiny nejhorších";
                else
                    return "průměrné, v horší polovině";

            }
        }
    }
}
