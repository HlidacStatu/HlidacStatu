using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HlidacStatu.Util.Cache;
using Nest;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class KIndex
    {
        private static ElasticClient _esClient = ES.Manager.GetESClient_KIndex();
        private static CouchbaseCacheManager<KIndexData, string> instanceByIco
       = CouchbaseCacheManager<KIndexData, string>.GetSafeInstance("kindexByICO", getDirect,
#if (!DEBUG)
                TimeSpan.FromMinutes(1200)
#else
                TimeSpan.FromSeconds(120)
#endif
           );
        static KIndexData notFoundKIdx = new KIndexData() { Ico = "-" };
        private static KIndexData getDirect(string ico)
        {
            var res = _esClient.Get<KIndexData>(ico);
            if (res.Found == false)
                return notFoundKIdx;
            else if (!res.IsValid)
            {
                throw new ApplicationException(res.ServerError?.ToString());
            }
            else
                return res.Source;
        }


        private static MemoryCacheManager<Tuple<int?, KIndexData.KIndexLabelValues>, string> instanceLabelByIco
       = MemoryCacheManager<Tuple<int?, KIndexData.KIndexLabelValues>, string>.GetSafeInstance("kindexLabelByICO", getDirectLabel,
#if (!DEBUG)
                TimeSpan.FromMinutes(1200)
#else
                TimeSpan.FromSeconds(120)
#endif
           );
        private static Tuple<int?, KIndexData.KIndexLabelValues> getDirectLabel(string ico)
        {
            var kidx = Get(ico);
            if (kidx != null)
            {
                var lbl = kidx.LastKIndexLabel(out int? rok);
                return new Tuple<int?, KIndexData.KIndexLabelValues>(rok, lbl);
            }

            return new Tuple<int?, KIndexData.KIndexLabelValues>(null, KIndexData.KIndexLabelValues.None);
        }


        public static KIndexData Get(string ico)
        {
            if (string.IsNullOrEmpty(ico))
                return null;
            var f = instanceByIco.Get(ico);
            if (f != null && f.Ico == "-")
                return null;
            
            return f;
        }

        public static IEnumerable<KIndexData> YieldExistingKindexes(string scrollTimeout = "2m", int scrollSize = 300)
        {
            ISearchResponse<KIndexData> initialResponse = _esClient.Search<KIndexData>
                (scr => scr.From(0)
                     .Take(scrollSize)
                     .MatchAll()
                     .Scroll(scrollTimeout));

            if (!initialResponse.IsValid || string.IsNullOrEmpty(initialResponse.ScrollId))
                throw new Exception(initialResponse.ServerError.Error.Reason);

            if (initialResponse.Documents.Any())
                foreach (var document in initialResponse.Documents)
                {
                    // filter to get only existing calculated Kindexes
                    if (document.roky.Any(m => m.KIndexAvailable))
                        yield return document;
                }

            string scrollid = initialResponse.ScrollId;
            bool isScrollSetHasData = true;
            while (isScrollSetHasData)
            {
                ISearchResponse<KIndexData> loopingResponse = _esClient.Scroll<KIndexData>(scrollTimeout, scrollid);
                if (loopingResponse.IsValid)
                {
                    foreach (var document in loopingResponse.Documents)
                    {
                        // filter to get only existing calculated Kindexes
                        if (document.roky.Any(m => m.KIndexAvailable))
                            yield return document;
                    }
                    scrollid = loopingResponse.ScrollId;
                }
                isScrollSetHasData = loopingResponse.Documents.Any();
            }

            _esClient.ClearScroll(new ClearScrollRequest(scrollid));

        }

        public static Tuple<int?, KIndexData.KIndexLabelValues> GetLastLabel(string ico)
        {
            return instanceLabelByIco.Get(ico);
        }


        public static decimal Average(int year)
        {
            var stat =  Statistics.KIndexStatTotal.Get().FirstOrDefault(m => m.Rok == year);
            if (stat == null)
                return 0;
            else
                return stat.AverageKindex;
        }
        public static decimal Average(int year, KIndexData.KIndexParts part)
        {
            var stat = Statistics.KIndexStatTotal.Get().FirstOrDefault(m => m.Rok == year);
            if (stat == null)
                return 0;
            else
                return stat.AverageParts.Radky.First(m => m.Velicina == (int)part).Hodnota;
        }


    }
}
