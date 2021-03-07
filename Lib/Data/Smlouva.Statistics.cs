using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class Smlouva
    {
        public partial class Statistics
        {

            static Util.Cache.CouchbaseCacheManager<Analytics.StatisticsPerYear<Smlouva.Statistics.Data>, string> _cache
                = Util.Cache.CouchbaseCacheManager<Analytics.StatisticsPerYear<Smlouva.Statistics.Data>, string>
                                .GetSafeInstance("SmlouvyStatistics_Query_v3_",
                                    (query) => Calculate(query),
                                    TimeSpan.FromHours(12),
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseServers"].Split(','),
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseBucket"],
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbaseUsername"],
                                    System.Configuration.ConfigurationManager.AppSettings["CouchbasePassword"]);
                                
            static object _cachesLock = new object();


            public static Analytics.StatisticsPerYear<Smlouva.Statistics.Data> CachedStatisticsForQuery(string query)
            {
                return _cache.Get(query); 
            }


            public static Analytics.StatisticsPerYear<Data> Calculate(string query)
            {

                Dictionary<int, Lib.Analysis.BasicData> _calc_SeZasadnimNedostatkem =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND chyby:zasadni", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_UzavrenoOVikendu =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND (hint.denUzavreni:>0)", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_ULimitu =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( hint.smlouvaULimitu:>0 )", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_NovaFirmaDodavatel =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( hint.pocetDniOdZalozeniFirmy:>-50 AND hint.pocetDniOdZalozeniFirmy:<30 )", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_smlouvy =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_bezCeny =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( issues.issueTypeId:100 ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_bezSmlStran =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( issues.issueTypeId:18 OR issues.issueTypeId:12 ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_sVazbouNaPolitikyNedavne =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( sVazbouNaPolitikyNedavne:true ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);
                Dictionary<int, Lib.Analysis.BasicData> _calc_sVazbouNaPolitikyBezCenyNedavne =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( issues.issueTypeId:100 ) AND ( sVazbouNaPolitikyNedavne:true ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_soukrome =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( hint.vztahSeSoukromymSubjektem:>0 ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);
                Dictionary<int, Lib.Analysis.BasicData> _calc_soukromeBezCeny =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( issues.issueTypeId:100 ) AND ( hint.vztahSeSoukromymSubjektem:>0 ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                var _calc_poOblastech = Lib.ES.QueryGrouped.OblastiPerYear($"( {query} )", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Data> data = new Dictionary<int, Data>();
                foreach (var year in Lib.Analytics.Consts.RegistrSmluvYearsList)
                {
                    data.Add(year, new Data()
                    {
                        PocetSmluv = _calc_smlouvy[year].Pocet,
                        CelkovaHodnotaSmluv = _calc_smlouvy[year].CelkemCena,
                        PocetSmluvBezCeny = _calc_bezCeny[year].Pocet,
                        PocetSmluvBezSmluvniStrany = _calc_bezSmlStran[year].Pocet,
                        SumKcSmluvBezSmluvniStrany = _calc_bezSmlStran[year].CelkemCena,
                        PocetSmluvSeSoukromymSubj = _calc_soukrome[year].Pocet,
                        CelkovaHodnotaSmluvSeSoukrSubj = _calc_soukrome[year].CelkemCena,
                        PocetSmluvBezCenySeSoukrSubj = _calc_soukromeBezCeny[year].Pocet,
                        
                        PocetSmluvSponzorujiciFirmy = _calc_sVazbouNaPolitikyNedavne[year].Pocet,
                        PocetSmluvBezCenySponzorujiciFirmy = _calc_sVazbouNaPolitikyBezCenyNedavne[year].Pocet,
                        SumKcSmluvSponzorujiciFirmy = _calc_sVazbouNaPolitikyNedavne[year].CelkemCena,
                        PocetSmluvULimitu = _calc_ULimitu[year].Pocet,
                        PocetSmluvOVikendu = _calc_UzavrenoOVikendu[year].Pocet,
                        PocetSmluvSeZasadnimNedostatkem = _calc_SeZasadnimNedostatkem[year].Pocet,
                        PocetSmluvNovaFirma = _calc_NovaFirmaDodavatel[year].Pocet,
                        PoOblastech = _calc_poOblastech[year]
                    }
                    ) ;
                }
                return new Analytics.StatisticsPerYear<Statistics.Data>(data);

            }
        }
    }
}
