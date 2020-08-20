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
        public List<string> SubjektOrderedListKIndexAsc { get; set; }
        public Dictionary<KIndexData.KIndexParts, List<string>> SubjektOrderedListPartsAsc { get; set; } = new Dictionary<KIndexData.KIndexParts, List<string>>();


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

        public decimal Average()
        {
            return this.AverageKindex;
        }
        public decimal Average(KIndexData.KIndexParts part)
        {
            return this.AverageParts.Radky.First(m => m.Velicina == (int)part).Hodnota;
        }

        public int? SubjektRank(string ico)
        {
            var res = this.SubjektOrderedListKIndexAsc.IndexOf(ico);
            if (res == -1)
                return null;
            else
                return res;
        }
        public int? SubjektRank(string ico, KIndexData.KIndexParts part)
        {
            var res = this.SubjektOrderedListPartsAsc[part].IndexOf(ico);
            if (res == -1)
                return null;
            else
                return res;
        }

        public string SubjektRankText(string ico)
        {
            var rank = SubjektRank(ico);
            if (rank == null)
                return "";
            else
            {
                int count = this.SubjektOrderedListPartsAsc.Count;
                if (rank <= 100)
                {
                    return $"je {rank}. nejlepší";
                }
                else
                if (rank >= count*2/3)
                {
                    return $"je {rank}. z ";
                }


            }

        }

        public int? SubjektRankText(string ico, KIndexData.KIndexParts part)
        {
            var res = this.SubjektOrderedListPartsAsc[part].IndexOf(ico);
            if (res == -1)
                return null;
            else
                return res;
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
