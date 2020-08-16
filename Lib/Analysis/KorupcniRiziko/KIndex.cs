using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HlidacStatu.Util.Cache;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class KIndex
    {
        private static CouchbaseCacheManager<KIndexData, string> instanceByIco
       = CouchbaseCacheManager<KIndexData, string>.GetSafeInstance("kindexByICO", getDirect,
#if (!DEBUG)
                TimeSpan.FromMinutes(1200)
#else
                TimeSpan.FromSeconds(120)
#endif
           );

        private static MemoryCacheManager<Tuple<int?, KIndexData.KIndexLabelValues>, string> instanceLabelByIco
       = MemoryCacheManager<Tuple<int?, KIndexData.KIndexLabelValues>, string>.GetSafeInstance("kindexLabelByICO", getDirectLabel,
#if (!DEBUG)
                TimeSpan.FromMinutes(1200)
#else
                TimeSpan.FromSeconds(120)
#endif
           );


        public static KIndexData Get(string ico)
        {
            if (string.IsNullOrEmpty(ico))
                return null;
            var f = instanceByIco.Get(ico);
            if (f != null && f.Ico == "-")
                return null;
            
            return f;
        }

        public static Tuple<int?, KIndexData.KIndexLabelValues> GetLastLabel(string ico)
        {
            return instanceLabelByIco.Get(ico);
        }

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

        static KIndexData notFoundKIdx = new KIndexData() { Ico = "-" };
        private static KIndexData getDirect(string ico)
        {
            var res = ES.Manager.GetESClient_KIndex().Get<KIndexData>(ico);
            if (res.Found == false)
                return notFoundKIdx;
            else if (!res.IsValid)
            {
                throw new ApplicationException(res.ServerError?.ToString());
            }
            else
                return res.Source;
        }


    }
}
