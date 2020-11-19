using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public static partial class Firmy
    {
        public class GlobalStatistics
        {
            public static Devmasters.Cache.Elastic.ElasticCache<Lib.Analytics.GlobalStatisticsPerYear<Lib.Data.Firma.Statistics.RegistrSmluv>> FirmySmlouvyGlobal =
                new Devmasters.Cache.Elastic.ElasticCache<Analytics.GlobalStatisticsPerYear<Firma.Statistics.RegistrSmluv>>(
                    Devmasters.Config.GetWebConfigValue("ESConnection").Split(';'),
                    "DevmastersCache",
                    TimeSpan.Zero, "FirmySmlouvyGlobal",
                    o =>
                    {
                        //fill from Lib.Data.AnalysisCalculation.CalculateGlobalRankPerYearFirmaSmlouvy && Tasks.UpdateWebCache
                        return null;
                    }, providerId: "HlidacStatu.Lib"
                    );

        }
    }
}
