﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HlidacStatu.Lib.Data;

using Nest;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{

    public partial class Calculator
    {
        public static int[] CalculationYears = Enumerable.Range(2017, DateTime.Now.Year - 2017).ToArray();
        const int minPocetSmluvKoncentraceDodavatelu = 1;

        private Firma urad = null;
        Dictionary<int, Lib.Analysis.BasicData> _calc_SeZasadnimNedostatkem = null;
        Dictionary<int, Lib.Analysis.BasicData> _calc_UzavrenoOVikendu = null;
        Dictionary<int, Lib.Analysis.BasicData> _calc_ULimitu = null;
        Dictionary<int, Lib.Analysis.BasicData> _calc_NovaFirmaDodavatel = null;

        public string Ico { get; private set; }

        private KIndexData kindex = null;

        public Calculator(string ico)
        {
            this.Ico = ico;
            kindex = KIndexData.Get(ico);
        }

        object lockCalc = new object();
        public KIndexData GetData(bool refresh = false)
        {
            if (refresh)
                kindex = null;
            lock (lockCalc)
            {
                if (kindex == null)
                {
                    kindex = CalculateSourceData();
                }
            }

            return kindex;
        }

        private KIndexData CalculateSourceData()
        {
            this.InitData();
            foreach (var year in CalculationYears)
            {
                KIndexData.Annual data_rok = CalculateForYear(year);
                kindex.roky.Add(data_rok);
            }
            return kindex;
        }

        private void InitData()
        {

            this.urad = Firmy.Get(this.Ico);
            if (urad.Valid == false)
                throw new ArgumentOutOfRangeException("invalid ICO");

            kindex = new KIndexData();
            kindex.Ico = urad.ICO;
            kindex.UcetniJednotka = KIndexData.UcetniJednotkaInfo.Load(urad.ICO);


            _calc_SeZasadnimNedostatkem = AdvancedQuery.PerYear($"ico:{this.Ico} and chyby:zasadni");

            _calc_UzavrenoOVikendu = AdvancedQuery.PerYear($"ico:{this.Ico} AND (hint.denUzavreni:>0)");

            _calc_ULimitu = AdvancedQuery.PerYear($"ico:{this.Ico} AND ( hint.smlouvaULimitu:>0 )");


            _calc_NovaFirmaDodavatel = AdvancedQuery.PerYear($"ico:{this.Ico} AND ( hint.pocetDniOdZalozeniFirmy:>-50 AND hint.pocetDniOdZalozeniFirmy:<30 )");
        }

        private KIndexData.Annual CalculateForYear(int year)
        {
            decimal smlouvyZaRok = (decimal)urad.Statistic().BasicStatPerYear[year].Pocet;
            if (smlouvyZaRok < 100)
                return null;

            KIndexData.Annual ret = new KIndexData.Annual(year);
            var fc = new FinanceDataCalculator(this.Ico, year);
            ret.FinancniUdaje = fc.GetData();

            ret.PercSeZasadnimNedostatkem = smlouvyZaRok == 0 ? 0m : (decimal)_calc_SeZasadnimNedostatkem[year].Pocet / smlouvyZaRok;
            ret.PercSmlouvySPolitickyAngazovanouFirmou = this.urad.Statistic().RatingPerYear[year].PercentSPolitiky;

            ret.PercNovaFirmaDodavatel = smlouvyZaRok == 0 ? 0m : (decimal)_calc_NovaFirmaDodavatel[year].Pocet / smlouvyZaRok;
            ret.PercSmluvUlimitu = smlouvyZaRok == 0 ? 0m : (decimal)_calc_ULimitu[year].Pocet / smlouvyZaRok;
            ret.PercUzavrenoOVikendu = smlouvyZaRok == 0 ? 0m : (decimal)_calc_UzavrenoOVikendu[year].Pocet / smlouvyZaRok;

            ret.Smlouvy = this.urad.Statistic().BasicStatPerYear[year];
            ret.Statistika = this.urad.Statistic().RatingPerYear[year];


            string query = $"icoPlatce:{this.Ico} AND datumUzavreni:[{year}-01-01 TO {year + 1}-01-01}}";
            ret.CelkovaKoncentraceDodavatelu = KoncentraceDodavateluCalculator(query);
            if (ret.CelkovaKoncentraceDodavatelu != null)
                ret.KoncentraceDodavateluBezUvedeneCeny
                    = KoncentraceDodavateluCalculator(query + " AND cena:0", ret.CelkovaKoncentraceDodavatelu.PrumernaHodnotaSmluv);

            if (ret.PercSmluvUlimitu>0)
                ret.KoncentraceDodavateluCenyULimitu
                    = KoncentraceDodavateluCalculator(query + " AND ( hint.smlouvaULimitu:>0 )", ret.CelkovaKoncentraceDodavatelu.PrumernaHodnotaSmluv);

            Dictionary<int, string> obory = Lib.Data.Smlouva.SClassification
                .AllTypes
                .Where(m => m.MainType)
                .OrderBy(m => m.Value)
                .ToDictionary(k => k.Value, v => v.SearchShortcut);

            ret.KoncetraceDodavateluObory = new List<KoncentraceDodavateluObor>();

            Devmasters.Core.Batch.Manager.DoActionForAll<int>(obory.Keys,
                (oborid) =>
                {

                    query = $"icoPlatce:{this.Ico} AND datumUzavreni:[{year}-01-01 TO {year + 1}-01-01}} AND oblast:{obory[oborid]}";
                    var k = KoncentraceDodavateluCalculator(query);
                    if (k != null)
                        ret.KoncetraceDodavateluObory.Add(new KoncentraceDodavateluObor()
                        {
                            OborId = oborid,
                            OborName = obory[oborid],
                            Koncentrace = k
                        });
                    return new Devmasters.Core.Batch.ActionOutputData();
                }, null, null, true, maxDegreeOfParallelism: 10
                );



            decimal smlouvyPod50kprumer = 0;
            decimal smlouvyAllCount = 0;
            decimal smlouvyPod50kCount = 0;
            var res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch($"datumUzavreni:[{ret.Rok}-01-01 TO {ret.Rok + 1}-01-01}}"
                , 1, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, null, exactNumOfResults: true);
            if (res.IsValid)
                smlouvyAllCount = res.Total;
            res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch($"cena:>0 AND cena:<=50000 AND datumUzavreni:[{ret.Rok}-01-01 TO {ret.Rok + 1}-01-01}}"
                , 1, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, null, exactNumOfResults: true);
            if (res.IsValid)
                smlouvyPod50kCount = res.Total;

            if (smlouvyAllCount > 0)
                smlouvyPod50kprumer = smlouvyPod50kCount / smlouvyAllCount;

            ret.TotalAveragePercSmlouvyPod50k = smlouvyPod50kprumer;

            res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch($"ico:{this.Ico} cena:>0 AND cena:<=50000 AND datumUzavreni:[{ret.Rok}-01-01 TO {ret.Rok + 1}-01-01}}"
    , 1, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, null, exactNumOfResults: true);
            if (res.IsValid && ret.Smlouvy.Pocet > 0)
                ret.PercSmlouvyPod50k = (decimal)(res.Total) / ret.Smlouvy.Pocet;

            decimal bonus = SmlouvyPod50kBonus(ret.PercSmlouvyPod50k, ret.TotalAveragePercSmlouvyPod50k);



            ret.KIndex = CalculateKIndex(bonus, ref ret);


            return ret;
        }

        public decimal CalculateKIndex(decimal bonus, ref KIndexData.Annual datayear)
        {

            //https://docs.google.com/spreadsheets/d/1FhaaXOszHORki7t5_YEACWFZUya5QtqUnNVPAvCArGQ/edit#gid=0


            decimal val =
            //r5
            datayear.Statistika.PercentBezCeny * 0.1m
            //r11
            + datayear.PercSeZasadnimNedostatkem * 0.02m
            //r13
            + datayear.CelkovaKoncentraceDodavatelu.Herfindahl_Hirschman_Modified * 0.2m
            //r15
            + datayear.KoncentraceDodavateluBezUvedeneCeny?.Herfindahl_Hirschman_Modified * 0.1m
            //r17
            + datayear.PercSmluvUlimitu * 0.1m
            //r18 - bonus!
            - bonus* 0.1m
            //r19
            //TODO
            + datayear.KoncentraceDodavateluCenyULimitu?.Herfindahl_Hirschman_Modified * 0.1m ?? 0
            //r20
            + datayear.PercNovaFirmaDodavatel * 0.02m
            //r22
            + datayear.PercUzavrenoOVikendu * 0.02m
            //r23
            + datayear.PercSmlouvySPolitickyAngazovanouFirmou * 0.02m
            ;
            //

            return val;

        }

        public static decimal SmlouvyPod50kBonus(decimal icoPodil, decimal vsePodil)
        {
            if (icoPodil > vsePodil * 1.75m)
                return 0.75m;
            else if (icoPodil > vsePodil * 1.5m)
                return 0.5m;
            if (icoPodil > vsePodil * 1.25m)
                return 0.25m;
            return 0m;
        }

        class smlouvaStat
        {
            public string Id { get; set; }
            public string IcoDodavatele { get; set; }
            public decimal CastkaSDPH { get; set; }
            public int Rok { get; set; }
            public DateTime Podepsano { get; set; }
        }
        public KoncentraceDodavateluIndexy KoncentraceDodavateluCalculator(string query, decimal? prumHodnotaSmlouvy = null)
        {
            Func<int, int, Nest.ISearchResponse<Lib.Data.Smlouva>> searchFunc = (size, page) =>
                {
                    return Lib.ES.Manager.GetESClient().Search<Lib.Data.Smlouva>(a => a
                                .Size(size)
                                .Source(ss => ss.Excludes(sml => sml.Field(ff => ff.Prilohy)))
                                .From(page * size)
                                .Query(q => Lib.Data.Smlouva.Search.GetSimpleQuery(query))
                                .Scroll("1m")
                                );
                };

            List<smlouvaStat> smlStat = new List<smlouvaStat>();
            Searching.Tools.DoActionForQuery<Lib.Data.Smlouva>(Lib.ES.Manager.GetESClient(),
                searchFunc,
                (h, o) =>
                {
                    Smlouva s = h.Source;
                    if (s != null)
                    {
                        foreach (var prij in s.Prijemce)
                        {
                            if (prij.ico == s.Platce.ico)
                                continue;
                            Firma f = Firmy.Get(prij.ico);
                            if (f.Valid && f.PatrimStatu())
                                continue;

                            string dodavatel = prij.ico;
                            if (string.IsNullOrWhiteSpace(dodavatel))
                                dodavatel = prij.nazev;
                            if (string.IsNullOrWhiteSpace(dodavatel))
                                dodavatel = "neznami";

                            smlStat.Add(new smlouvaStat()
                            {
                                Id = s.Id,
                                IcoDodavatele = dodavatel,
                                CastkaSDPH = (decimal)s.CalculatedPriceWithVATinCZK / (decimal)s.Prijemce.Length,
                                Podepsano = s.datumUzavreni,
                                Rok = s.datumUzavreni.Year
                            });

                        }
                    }

                    return new Devmasters.Core.Batch.ActionOutputData();
                }, null,
                null, null,
                false, blockSize: 100);

            IEnumerable<SmlouvyForIndex> smlouvy = smlStat
                .Select(m => new SmlouvyForIndex(m.IcoDodavatele, prumHodnotaSmlouvy ?? m.CastkaSDPH))
                .OrderByDescending(m => m.HodnotaSmlouvy) //just better debug
                .ToArray(); //just better debug


            if (smlouvy.Count() == 0)
                return null;
            if (smlouvy.Count() < minPocetSmluvKoncentraceDodavatelu)
                return null;

            var ret = new KoncentraceDodavateluIndexy();
            ret.PocetSmluv = smlouvy.Count();
            ret.PocetSmluvBezCeny = smlouvy.Count(m => m.HodnotaSmlouvy == 0);
            ret.PrumernaHodnotaSmluv = smlouvy
                                .Where(m => m.HodnotaSmlouvy != 0)
                                .Count() == 0 ? 0 : smlouvy
                                                        .Where(m => m.HodnotaSmlouvy != 0)
                                                        .Select(m => Math.Abs(m.HodnotaSmlouvy))
                                                        .Average();
            ret.CelkovaHodnotaSmluv = smlouvy.Sum(m => m.HodnotaSmlouvy);
            ret.Query = query;


            ret.Herfindahl_Hirschman_Index = Herfindahl_Hirschman_Index(smlouvy);
            ret.Herfindahl_Hirschman_Normalized = Herfindahl_Hirschman_IndexNormalized(smlouvy);
            ret.Herfindahl_Hirschman_Modified = Herfindahl_Hirschman_Modified(smlouvy);

            ret.Comprehensive_Industrial_Concentration_Index = Comprehensive_Industrial_Concentration_Index(smlouvy);
            ret.Hall_Tideman_Index = Hall_Tideman_Index(smlouvy);
            ret.Kwoka_Dominance_Index = Kwoka_Dominance_Index(smlouvy);
            return ret;
        }


    }

}
