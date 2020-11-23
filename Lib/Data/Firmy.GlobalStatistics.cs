using Devmasters.Batch;

using Nest;

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

            static List<string> _vsechnyUrady = null;
            static object _vsechnyUradyLock = new object();
            public static List<string> VsechnyUrady(
                Action<string> logOutputFunc = null, 
                Action<ActionProgressData> progressOutputFunc = null,
                int? threads = null)
            {
                if (_vsechnyUrady != null)
                    return _vsechnyUrady;
                else
                {
                    lock (_vsechnyUradyLock)
                    {
                        if (_vsechnyUrady != null)
                            return _vsechnyUrady;


                        var icos = new List<string>();

                        HlidacStatu.Util.Consts.Logger.Info($"Loading ALL ICOS");

                        icos.AddRange(StaticData.KrajskeUradyCache.Get().Select(m => m.ICO));
                        icos.AddRange(StaticData.MinisterstvaCache.Get().Select(m => m.ICO));
                        // krajske technicke sluzby
                        icos.AddRange("00066001,00090450,70947023,70946078,72053119,00080837,70971641,27502988,00085031,70932581,70960399,00095711,26913453,03447286,25396544,60733098".Split(','));
                        //velke nemocnice
                        icos.AddRange("00064165,00064173,00064203,00098892,00159816,00179906,00669806,00843989,25488627,26365804,27283933,27661989,65269705,27283518,26000202,00023736,00023884,27256391,61383082,27256537,00023001,27520536,26068877,47813750,00064211,00209805,27660915,00635162,27256456,00090638,00092584,00064190".Split(','));
                        //fakultni nemocnice
                        icos.AddRange("00064165,00064173,00064203,00098892,00159816,00179906,00669806,00843989,25488627,26365804,27283933,27661989,65269705,27283518,26000202,00023736,00023884,27256391,61383082,27256537,00023001,27520536,26068877,47813750,00064211,00209805,27660915,00635162,27256456,00090638,00092584,00064190".Split(','));

                        //CEZ, CPost, CD, 
                        icos.AddRange("45274649,47114983,70994226".Split(','));
                        icos.AddRange(Firma.StatniFirmyICO);

                        icos.AddRange(Lib.DirectDB
                            .GetList<string>("select distinct ico from firma where status =1 and Kod_PF in (301,302,312,313,314,325,331,352,353,361,362,381,382,521,771,801,804,805)")
                                );

                        icos.AddRange(StaticData.StatutarniMestaAllCache.Get().Select(m => m.ICO));
                        //velka mesta
                        string velkamesta = "00064581,00081531,00266094,00254657,00262978,44992785,00845451,00274046,00075370,00262978,00299308,00244732,00283924";
                        icos.AddRange(velkamesta.Split(','));

                        icos.AddRange(Firma.Zatrideni.Subjekty(Firma.Zatrideni.StatniOrganizaceObor.Vse).Select(m => m.Ico));

                        //nejvice utajujici smluvni strany
                        HlidacStatu.Util.Consts.Logger.Info($"Loading ICOS utajujici smluvni strany");
                        AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggs = new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                            .Terms("perIco", m => m
                                .Field("platce.ico")
                                .Size(2500)
                                .Order(o => o.Descending("_count"))
                                );

                        var res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch("(issues.issueTypeId:18 OR issues.issueTypeId:12)", 1, 0,
                            HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, aggs, platnyZaznam: true);

                        foreach (Nest.KeyedBucket<object> val in ((BucketAggregate)res.ElasticResults.Aggregations["perIco"]).Items)
                        {
                            var ico = (string)val.Key;
                            var f = Firmy.Get(ico);
                            if (f.PatrimStatu())
                            {
                                if (HlidacStatu.Lib.Analysis.ACore.GetBasicStatisticForICO(ico)
                                       .Data.Any(m => m.Value.Pocet > Lib.Analysis.KorupcniRiziko.Consts.MinSmluvPerYear)
                                   )
                                    icos.Add(ico);
                            }
                            else
                            {
                            }
                        }

                        //nejvice utajujici ceny
                        HlidacStatu.Util.Consts.Logger.Info($"Loading ICOS utajujici ceny");
                        aggs = new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                            .Terms("perIco", m => m
                                .Field("platce.ico")
                                .Size(2500)
                                .Order(o => o.Descending("_count"))
                                );

                        res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch("(issues.issueTypeId:100)", 1, 0,
                            HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, aggs, platnyZaznam: true);

                        foreach (Nest.KeyedBucket<object> val in ((BucketAggregate)res.ElasticResults.Aggregations["perIco"]).Items)
                        {
                            var ico = (string)val.Key;
                            var f = Firmy.Get(ico);
                            if (f.PatrimStatu())
                            {
                                if (HlidacStatu.Lib.Analysis.ACore.GetBasicStatisticForICO(ico)
                                   .Data.Any(m => m.Value.Pocet > Lib.Analysis.KorupcniRiziko.Consts.MinSmluvPerYear)
                               )
                                    icos.Add(ico);
                            }
                            else
                            {
                                if (System.Diagnostics.Debugger.IsAttached)
                                {
                                    //System.Diagnostics.Debugger.Break();
                                    Console.WriteLine("excl:" + f.Jmeno);
                                }
                            }
                        }



                        HlidacStatu.Util.Consts.Logger.Info("Dohledani podrizenych organizaci");
                        //podrizene organizace
                        var allIcos = new System.Collections.Concurrent.ConcurrentBag<string>();

                        Devmasters.Batch.Manager.DoActionForAll<string>(icos.Distinct().ToArray(),
                            (i) =>
                            {
                                var fk = Firmy.Get(i);
                                allIcos.Add(i);
                                foreach (var pic in fk.IcosInHolding(Relation.AktualnostType.Aktualni))
                                    allIcos.Add(pic); ;

                                return new Devmasters.Batch.ActionOutputData();
                            },
                        logOutputFunc,
                        progressOutputFunc,
                        true, maxDegreeOfParallelism: threads);


                        icos = allIcos.ToList();
                        _vsechnyUrady = icos
                            .Select(i => HlidacStatu.Util.ParseTools.NormalizeIco(i))
                            .Distinct()
                            .ToList();
                    }
                }
                return _vsechnyUrady;
            }

            private static
                Dictionary<int,
                Devmasters.Cache.Elastic.ElasticCache<Lib.Analytics.GlobalStatisticsPerYear<Lib.Data.Firma.Statistics.RegistrSmluv>>>
                _uradySmlouvyGlobal =
                    new Dictionary<int, Devmasters.Cache.Elastic.ElasticCache<Analytics.GlobalStatisticsPerYear<Firma.Statistics.RegistrSmluv>>>();


            public static Lib.Analytics.GlobalStatisticsPerYear<Lib.Data.Firma.Statistics.RegistrSmluv> UradySmlouvyGlobal(int? obor = null,
                Analytics.GlobalStatisticsPerYear<Firma.Statistics.RegistrSmluv> newData = null)
            {
                obor = obor ?? 0;

                if (!_uradySmlouvyGlobal.ContainsKey(obor.Value))
                    _uradySmlouvyGlobal[obor.Value] = new Devmasters.Cache.Elastic.ElasticCache<Analytics.GlobalStatisticsPerYear<Firma.Statistics.RegistrSmluv>>(
                            Devmasters.Config.GetWebConfigValue("ESConnection").Split(';'),
                            "DevmastersCache",
                            TimeSpan.Zero, $"UradySmlouvyGlobal_{obor.Value}",
                            o =>
                            {
                            //fill from Lib.Data.AnalysisCalculation.CalculateGlobalRankPerYear_UradySmlouvy && Tasks.UpdateWebCache
                            return null;
                            }, providerId: "HlidacStatu.Lib"
                            );

                if (newData != null)
                {
                    _uradySmlouvyGlobal[obor.Value].ForceRefreshCache(newData);
                    return newData;
                }
                else
                    return _uradySmlouvyGlobal[obor.Value].Get();

            }

            public static void CalculateGlobalRangeCaches_UradySmlouvy(int? threads = null,
         Action<string> logOutputFunc = null, Action<ActionProgressData> progressOutputFunc = null)
            {
                UradySmlouvyGlobal(null,CalculateGlobalRankPerYear_UradySmlouvy(null,threads,logOutputFunc,progressOutputFunc));
                foreach (var main in Devmasters.Enums.EnumTools
                    .EnumToEnumerable(typeof(HlidacStatu.Lib.Data.Smlouva.SClassification.ClassificationsTypes))
                    .Select(m => new { value = Convert.ToInt32(m.Value), key = m.Key })
                    .Where(m => m.value % 100 == 0)
                    )
                {
                    UradySmlouvyGlobal(main.value, CalculateGlobalRankPerYear_UradySmlouvy(null, threads, logOutputFunc, progressOutputFunc));
                }
            }

            private static Analytics.GlobalStatisticsPerYear<Firma.Statistics.RegistrSmluv> CalculateGlobalRankPerYear_UradySmlouvy(
                 int? obor = null,
                 int? threads = null,
                 Action<string> logOutputFunc = null, Action<ActionProgressData> progressOutputFunc = null
                 )
            {
                obor = obor ?? 0;
                var icos = Firmy.GlobalStatistics.VsechnyUrady(logOutputFunc,progressOutputFunc);
                object lockObj = new object();
                List<Analytics.StatisticsSubjectPerYear<Firma.Statistics.RegistrSmluv>> data =
                    new List<Analytics.StatisticsSubjectPerYear<Firma.Statistics.RegistrSmluv>>();
                Devmasters.Batch.Manager.DoActionForAll<string>(icos,
                    (Func<string, ActionOutputData>)(
                    ico =>
                    {
                        var stat = Firmy.Get(ico)?.Statistika(obor.Value);
                        if (stat != null)
                            lock (lockObj)
                            {
                                data.Add(stat);
                            }
                        return new Devmasters.Batch.ActionOutputData();
                    }), logOutputFunc, progressOutputFunc, true, maxDegreeOfParallelism: threads, prefix: $"CalculateGlobalRankPerYear_UradySmlouvy_{obor.Value} ");

                return new Analytics.GlobalStatisticsPerYear<Firma.Statistics.RegistrSmluv>(
                    Analytics.Consts.RegistrSmluvYearsList,
                    data,
                     m => m.PocetSmluv >= 10
                     );
            }

        }
    }
}
