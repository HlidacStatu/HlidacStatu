using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HlidacStatu.Util.Cache;

using Nest;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public static class KIndex
    {

        private static MemoryCacheManager<KIndexData, (string ico, bool useTemp)> instanceByIco
       = MemoryCacheManager<KIndexData, (string ico, bool useTemp)>.GetSafeInstance("kindexByICOv2", KIndexData.GetDirect,
#if (!DEBUG)
                TimeSpan.FromHours(1)
#else
                TimeSpan.FromSeconds(10)
#endif
           );
        static KIndexData notFoundKIdx = new KIndexData() { Ico = "-" };


        private static MemoryCacheManager<Tuple<int?, KIndexData.KIndexLabelValues>, (string ico, bool useTemp)> instanceLabelByIco
       = MemoryCacheManager<Tuple<int?, KIndexData.KIndexLabelValues>, (string ico, bool useTemp)>.GetSafeInstance("kindexLabelByICO", getDirectLabel,
#if (!DEBUG)
                TimeSpan.FromMinutes(1200)
#else
                TimeSpan.FromSeconds(120)
#endif
           );
        private static Tuple<int?, KIndexData.KIndexLabelValues> getDirectLabel((string ico, bool useTemp) param)
        {
            if (Consts.KIndexExceptions.Contains(param.ico) && param.useTemp ==false)
                return new Tuple<int?, KIndexData.KIndexLabelValues>(null, KIndexData.KIndexLabelValues.None);

            var kidx = Get(param.ico, param.useTemp);
            if (kidx != null)
            {
                var lbl = kidx.LastKIndexLabel(out int? rok);
                return new Tuple<int?, KIndexData.KIndexLabelValues>(rok, lbl);
            }

            return new Tuple<int?, KIndexData.KIndexLabelValues>(null, KIndexData.KIndexLabelValues.None);
        }


        public static KIndexData Get(string ico, bool useTemp=false)
        {
            if (string.IsNullOrEmpty(ico))
                return null;
            var f = instanceByIco.Get((ico, useTemp));
            if (f == null || f.Ico == "-")
                return null;
            //fill Annual
            foreach (var r in f.roky)
            {
                if (r != null)
                    r.Ico = ico;
            }
            return f;
        }

        public static string PlannedKIndexHash(string ico, int rok)
        {
            string salt = string.Format(Devmasters.Config.GetWebConfigValue("KIndexSaltTemplate"),ico,rok);
            string hash=Devmasters.Crypto.Hash.ComputeHashToHex(salt);
            return Devmasters.Core.Right(hash, 15);
        }

        public static IEnumerable<KIndexData> YieldExistingKindexes(string scrollTimeout = "2m", int scrollSize = 300, bool? useTempDb = null)
        {
            useTempDb = useTempDb ?? !string.IsNullOrEmpty(Devmasters.Config.GetWebConfigValue("UseKindexTemp"));

            var client = ES.Manager.GetESClient_KIndex();
            if (useTempDb.Value)
                client = ES.Manager.GetESClient_KIndexTemp();


            ISearchResponse<KIndexData> initialResponse = client.Search<KIndexData>
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
                    if (document.roky.Any(m => m.KIndexReady))
                        yield return document;
                }

            string scrollid = initialResponse.ScrollId;
            bool isScrollSetHasData = true;
            while (isScrollSetHasData)
            {
                ISearchResponse<KIndexData> loopingResponse = client.Scroll<KIndexData>(scrollTimeout, scrollid);
                if (loopingResponse.IsValid)
                {
                    foreach (var document in loopingResponse.Documents)
                    {
                        // filter to get only existing calculated Kindexes
                        if (document.roky.Any(m => m.KIndexReady))
                            yield return document;
                    }
                    scrollid = loopingResponse.ScrollId;
                }
                isScrollSetHasData = loopingResponse.Documents.Any();
            }

            client.ClearScroll(new ClearScrollRequest(scrollid));

        }

        public static bool HasKIndexValue(string ico, bool useTemp)
        {
            var kidx = Get(ico,useTemp);
            if (kidx == null)
                return false;
            else
            {
                return kidx.roky.Any(m => m.KIndexReady);
            }
        }

        public static Tuple<int?, KIndexData.KIndexLabelValues> GetLastLabel(string ico, bool useTemp)
        {
            return instanceLabelByIco.Get((ico,useTemp));
        }


        public static decimal Average(int year)
        {
            var stat = Statistics.KIndexStatTotal.Get().FirstOrDefault(m => m.Rok == year);
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
        public static Riziko.RizikoValues ToRiziko(KIndexData.KIndexLabelValues val)
        {
            switch (val)
            {
                case KIndexData.KIndexLabelValues.None:
                    return Riziko.RizikoValues.None;
                case KIndexData.KIndexLabelValues.A:
                    return Riziko.RizikoValues.A;
                case KIndexData.KIndexLabelValues.B:
                    return Riziko.RizikoValues.B;
                case KIndexData.KIndexLabelValues.C:
                    return Riziko.RizikoValues.C;
                case KIndexData.KIndexLabelValues.D:
                    return Riziko.RizikoValues.D;
                case KIndexData.KIndexLabelValues.E:
                    return Riziko.RizikoValues.E;
                case KIndexData.KIndexLabelValues.F:
                    return Riziko.RizikoValues.F;
                default:
                    return Riziko.RizikoValues.None;
            }
        }
        public static Riziko.RizikoValues AsRiziko(this KIndexData.KIndexLabelValues val)
        {
            return ToRiziko(val);
        }

    }
}
