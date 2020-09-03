using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HlidacStatu.Lib.Data;
using HlidacStatu.Util;

using Nest;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{

    public partial class Calculator
    {
        const int minPocetSmluvKoncentraceDodavateluProZahajeniVypoctu = 1;

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

            this.urad = Firmy.Get(this.Ico);
            if (urad.Valid == false)
                throw new ArgumentOutOfRangeException("invalid ICO");

            kindex = KIndexData.GetDirect(ico);
        }

        object lockCalc = new object();
        public KIndexData GetData(bool refreshData = false, bool forceCalculateAllYears = false)
        {
            if (refreshData || forceCalculateAllYears)
                kindex = null;
            lock (lockCalc)
            {
                if (kindex == null)
                {
                    kindex = CalculateSourceData(forceCalculateAllYears);
                }
            }

            return kindex;
        }

        private KIndexData CalculateSourceData(bool forceCalculateAllYears)
        {
            this.InitData();
            foreach (var year in Consts.CalculationYears)
            {
                KIndexData.Annual data_rok = CalculateForYear(year, forceCalculateAllYears);
                kindex.roky.Add(data_rok);
            }
            return kindex;
        }

        private void InitData()
        {


            kindex = new KIndexData();
            kindex.Ico = urad.ICO;
            kindex.Jmeno = urad.Jmeno;
            kindex.UcetniJednotka = KIndexData.UcetniJednotkaInfo.Load(urad.ICO);


            _calc_SeZasadnimNedostatkem = AdvancedQuery.PerYear($"ico:{this.Ico} and chyby:zasadni");

            _calc_UzavrenoOVikendu = AdvancedQuery.PerYear($"ico:{this.Ico} AND (hint.denUzavreni:>0)");

            _calc_ULimitu = AdvancedQuery.PerYear($"ico:{this.Ico} AND ( hint.smlouvaULimitu:>0 )");

            _calc_NovaFirmaDodavatel = AdvancedQuery.PerYear($"ico:{this.Ico} AND ( hint.pocetDniOdZalozeniFirmy:>-50 AND hint.pocetDniOdZalozeniFirmy:<30 )");

        }


        static object _koncetraceDodavateluOboryLock = new object();
        private KIndexData.Annual CalculateForYear(int year, bool forceCalculateAllYears)
        {
            decimal smlouvyZaRok = (decimal)urad.Statistic().BasicStatPerYear[year].Pocet;

            KIndexData.Annual ret = new KIndexData.Annual(year);
            var fc = new FinanceDataCalculator(this.Ico, year);
            ret.FinancniUdaje = fc.GetData();


            ret.PercSeZasadnimNedostatkem = smlouvyZaRok == 0 ? 0m : (decimal)_calc_SeZasadnimNedostatkem[year].Pocet / smlouvyZaRok;
            ret.PercSmlouvySPolitickyAngazovanouFirmou = this.urad.Statistic().RatingPerYear[year].PercentSPolitiky;

            ret.PercNovaFirmaDodavatel = smlouvyZaRok == 0 ? 0m : (decimal)_calc_NovaFirmaDodavatel[year].Pocet / smlouvyZaRok;
            ret.PercSmluvUlimitu = smlouvyZaRok == 0 ? 0m : (decimal)_calc_ULimitu[year].Pocet / smlouvyZaRok;
            ret.PercUzavrenoOVikendu = smlouvyZaRok == 0 ? 0m : (decimal)_calc_UzavrenoOVikendu[year].Pocet / smlouvyZaRok;

            var stat = this.urad.Statistic().RatingPerYear[year];
            ret.Statistika = new KIndexData.StatistickeUdaje()
            {
                PocetSmluv = this.urad.Statistic().BasicStatPerYear[year].Pocet,
                CelkovaHodnotaSmluv = this.urad.Statistic().BasicStatPerYear[year].CelkemCena,
                PocetSmluvBezCeny = stat.NumBezCeny,
                PocetSmluvBezSmluvniStrany = stat.NumBezSmluvniStrany,
                PocetSmluvPolitiky = stat.NumSPolitiky,
                PercentSmluvBezCeny = stat.PercentBezCeny,
                PercentSmluvBezSmluvniStrany = stat.PercentBezSmluvniStrany,
                PercentKcBezSmluvniStrany = stat.PercentKcBezSmluvniStrany,
                PercentKcSmluvPolitiky = stat.PercentKcSPolitiky,
                PercentSmluvPolitiky = stat.PercentSPolitiky,
                SumKcSmluvBezSmluvniStrany = stat.SumKcBezSmluvniStrany,
                SumKcSmluvPolitiky = stat.SumKcSPolitiky,
                PocetSmluvULimitu = _calc_ULimitu[year].Pocet,
                PocetSmluvOVikendu = _calc_UzavrenoOVikendu[year].Pocet,
                PocetSmluvSeZasadnimNedostatkem = _calc_SeZasadnimNedostatkem[year].Pocet,
                PocetSmluvNovaFirma = _calc_NovaFirmaDodavatel[year].Pocet,
            };


            string queryPlatce = $"icoPlatce:{this.Ico} AND datumUzavreni:[{year}-01-01 TO {year + 1}-01-01}}";

            if (smlouvyZaRok >= minPocetSmluvKoncentraceDodavateluProZahajeniVypoctu)
            {
                IEnumerable<Calculator.SmlouvyForIndex> allSmlouvy = GetSmlouvy(queryPlatce);
                ret.Statistika.PocetSmluvSeSoukromymSubj = allSmlouvy.Count();
                ret.Statistika.PocetSmluvBezCenySeSoukrSubj = allSmlouvy.Where(m => m.HodnotaSmlouvy == 0).Count();
                ret.Statistika.CelkovaHodnotaSmluvSeSoukrSubj = allSmlouvy.Sum(m => m.HodnotaSmlouvy);
                if (allSmlouvy.Any(m => m.HodnotaSmlouvy > 0))
                    ret.Statistika.PrumernaHodnotaSmluvSeSoukrSubj = allSmlouvy.Where(m => m.HodnotaSmlouvy > 0).Average(m => m.HodnotaSmlouvy);
                else
                    ret.Statistika.PrumernaHodnotaSmluvSeSoukrSubj = 0;

                ret.CelkovaKoncentraceDodavatelu = KoncentraceDodavateluCalculator(allSmlouvy, queryPlatce, "Koncentrace soukromých dodavatelů");
                if (ret.CelkovaKoncentraceDodavatelu != null)
                {

                    if (ret.CelkovaKoncentraceDodavatelu != null)
                    {
                        //ma cenu koncentraci pocitat?
                        //musi byt vice ne 5 smluv a nebo jeden dodavatel musi mit vice nez 1/2 smluv a to vice nez 2
                        if (
                            (allSmlouvy.Where(m => m.HodnotaSmlouvy == 0).Count() > 0)
                            &&
                            (allSmlouvy.Where(m => m.HodnotaSmlouvy == 0).Count() > 5
                            || allSmlouvy.Where(m => m.HodnotaSmlouvy == 0)
                                    .GroupBy(k => k.Dodavatel, v => v, (k, v) => v.Count())
                                    .Max() > 2
                            )
                         )
                        {
                            ret.KoncentraceDodavateluBezUvedeneCeny
                                = KoncentraceDodavateluCalculator(allSmlouvy.Where(m => m.HodnotaSmlouvy == 0), queryPlatce + " AND cena:0",
                                "Koncentrace soukromých dodavatelů u smluv s utajenou cenou",
                                ret.Statistika.PrumernaHodnotaSmluvSeSoukrSubj, 2);

                            ret.KoncentraceDodavateluBezUvedeneCeny.Dodavatele = ret.KoncentraceDodavateluBezUvedeneCeny
                                .Dodavatele
                                //jde o smlouvy bez ceny, u souhrnu dodavatelu resetuj ceny na 0
                                .Select(m => new KoncentraceDodavateluIndexy.Souhrn() { HodnotaSmluv = 0, Ico = m.Ico, PocetSmluv = m.PocetSmluv, Poznamka = m.Poznamka })
                                .ToArray();
                        }
                    }
                    if (ret.PercSmluvUlimitu > 0)
                    {
                        if (
                            (allSmlouvy.Where(m => m.ULimitu > 0).Count() > 0)
                            && (
                                allSmlouvy.Where(m => m.ULimitu > 0).Count() > 5
                                || allSmlouvy.Where(m => m.ULimitu > 0)
                                    .GroupBy(k => k.Dodavatel, v => v, (k, v) => v.Count())
                                    .Max() > 2
                                )
                            )
                        {
                            ret.KoncentraceDodavateluCenyULimitu
                            = KoncentraceDodavateluCalculator(allSmlouvy.Where(m => m.ULimitu > 0), queryPlatce + " AND ( hint.smlouvaULimitu:>0 )",
                            "Koncentrace soukromých dodavatelů u smluv s cenou u limitu veřejných zakázek",
                            ret.Statistika.PrumernaHodnotaSmluvSeSoukrSubj, 3);
                        }
                    }
                    Dictionary<int, string> obory = Lib.Data.Smlouva.SClassification
                        .AllTypes
                        .Where(m => m.MainType)
                        .OrderBy(m => m.Value)
                        .ToDictionary(k => k.Value, v => v.SearchShortcut);

                    ret.KoncetraceDodavateluObory = new List<KoncentraceDodavateluObor>();

                    Devmasters.Core.Batch.Manager.DoActionForAll<int>(obory.Keys,
                        (oborid) =>
                        {

                            var queryPlatceObor = $"icoPlatce:{this.Ico} AND datumUzavreni:[{year}-01-01 TO {year + 1}-01-01}} AND oblast:{obory[oborid]}";
                            var allSmlouvyObory = allSmlouvy.Where(w => w.ContainsObor(oborid));
                            var k = KoncentraceDodavateluCalculator(allSmlouvyObory,
                                    queryPlatceObor, "Koncentrace soukromých dodavatelů u oboru " + obory[oborid],
                                    ret.Statistika.PrumernaHodnotaSmluvSeSoukrSubj);


                            //KoncentraceDodavateluIndexy kbezCeny = null;
                            lock (_koncetraceDodavateluOboryLock)
                            {
                                if (k != null)
                                {
                                    ret.KoncetraceDodavateluObory.Add(new KoncentraceDodavateluObor()
                                    {
                                        OborId = oborid,
                                        OborName = obory[oborid],
                                        Koncentrace = k,
                                        SmluvBezCenyMalusKoeficient = k.PocetSmluvProVypocet == 0 ?
                                              1 : (1m + (decimal)k.PocetSmluvBezCenyProVypocet / (decimal)k.PocetSmluvProVypocet)
                                        //KoncentraceBezUvedeneCeny = kbezCeny
                                    });
                                };
                            }
                            return new Devmasters.Core.Batch.ActionOutputData();
                        }, null, null, !System.Diagnostics.Debugger.IsAttached, maxDegreeOfParallelism: 10 // 
                        );
                } // if (ret.CelkovaKoncentraceDodavatelu != null)

            }
            ret.TotalAveragePercSmlouvyPod50k = TotalAveragePercSmlouvyPod50k(ret.Rok);

            ret.PercSmlouvyPod50k = AveragePercSmlouvyPod50k(this.Ico, ret.Rok, ret.Statistika.PocetSmluv);
            ret.PercSmlouvyPod50kBonus = SmlouvyPod50kBonus(ret.PercSmlouvyPod50k, ret.TotalAveragePercSmlouvyPod50k);


            ret = FinalCalculationKIdx(ret, forceCalculateAllYears);

            return ret;
        }

        public KIndexData.Annual FinalCalculationKIdx(KIndexData.Annual ret, bool forceCalculateAllYears)
        {
            decimal smlouvyZaRok = (decimal)urad.Statistic().BasicStatPerYear[ret.Rok].Pocet;

            ret.KIndex = CalculateKIndex(ret);
            ret.KIndexReady = true;
            if (
                !(
                    ret.Statistika.PocetSmluvSeSoukromymSubj >= Consts.MinSmluvPerYear
                    ||
                    (
                        ret.Statistika.CelkovaHodnotaSmluvSeSoukrSubj >= Consts.MinSumSmluvPerYear
                    || 
                        (ret.Statistika.CelkovaHodnotaSmluvSeSoukrSubj + ret.Statistika.PrumernaHodnotaSmluvSeSoukrSubj * ret.Statistika.PocetSmluvBezCenySeSoukrSubj) >= Consts.MinSumSmluvPerYear
                    )
                )
            )
            {
                if (forceCalculateAllYears == false)
                {

                    ret.KIndexReady = false;
                    ret.KIndexIssues = new string[] { $"K-Index nespočítán. Méně než {Consts.MinSmluvPerYear} smluv za rok nebo malý objem smluv." };
                }
                else
                    ret.KIndexIssues = new string[] { $"Organizace má méně smluv nebo objem smluv než určuje metodika. Pro výpočet a publikaci byla aplikována výjimka." };
            }


            ret.LastUpdated = DateTime.Now;

            return ret;
        }

        public decimal CalculateKIndex(KIndexData.Annual datayear)
        {

            //https://docs.google.com/spreadsheets/d/1FhaaXOszHORki7t5_YEACWFZUya5QtqUnNVPAvCArGQ/edit#gid=0
            KIndexData.VypocetDetail vypocet = new KIndexData.VypocetDetail();
            var vradky = new List<KIndexData.VypocetDetail.Radek>();


            decimal val =
            //r5
            datayear.Statistika.PercentSmluvBezCeny * KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.PercentBezCeny);   //=10   C > 1  F > 2,5
            vradky.Add(new KIndexData.VypocetDetail.Radek(
                KIndexData.KIndexParts.PercentBezCeny,
                datayear.Statistika.PercentSmluvBezCeny,
                KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.PercentBezCeny))
            );

            //r11
            val += datayear.PercSeZasadnimNedostatkem * KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.PercSeZasadnimNedostatkem);  //=20
            vradky.Add(new KIndexData.VypocetDetail.Radek(
                KIndexData.KIndexParts.PercSeZasadnimNedostatkem,
                datayear.PercSeZasadnimNedostatkem,
                KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.PercSeZasadnimNedostatkem))
            );


            //r13
            val += datayear.CelkovaKoncentraceDodavatelu?.Herfindahl_Hirschman_Modified * KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.CelkovaKoncentraceDodavatelu) ?? 0; //=40
            vradky.Add(new KIndexData.VypocetDetail.Radek(
                KIndexData.KIndexParts.CelkovaKoncentraceDodavatelu,
                datayear.CelkovaKoncentraceDodavatelu?.Herfindahl_Hirschman_Modified ?? 0,
                KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.CelkovaKoncentraceDodavatelu))
            );


            //r15
            decimal r15koef = KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.KoncentraceDodavateluBezUvedeneCeny);
            if (datayear.Statistika.PercentSmluvBezCeny < 0.05m)
                r15koef = r15koef / 2m;

            val += datayear.KoncentraceDodavateluBezUvedeneCeny?.Herfindahl_Hirschman_Modified * r15koef ?? 0; //60
            vradky.Add(new KIndexData.VypocetDetail.Radek(
                KIndexData.KIndexParts.KoncentraceDodavateluBezUvedeneCeny,
                datayear.KoncentraceDodavateluBezUvedeneCeny?.Herfindahl_Hirschman_Modified ?? 0,
                r15koef)
            );

            //r17
            val += datayear.PercSmluvUlimitu * KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.PercSmluvUlimitu);  //70
            vradky.Add(new KIndexData.VypocetDetail.Radek(
                KIndexData.KIndexParts.PercSmluvUlimitu,
                datayear.PercSmluvUlimitu,
                KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.PercSmluvUlimitu))
            );

            //r18
            val += datayear.KoncentraceDodavateluCenyULimitu?.Herfindahl_Hirschman_Modified * KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.KoncentraceDodavateluCenyULimitu) ?? 0; //80
            vradky.Add(
                new KIndexData.VypocetDetail.Radek(
                    KIndexData.KIndexParts.KoncentraceDodavateluCenyULimitu,
                    datayear.KoncentraceDodavateluCenyULimitu?.Herfindahl_Hirschman_Modified ?? 0,
                KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.KoncentraceDodavateluCenyULimitu))
            );

            //r19
            val += datayear.PercNovaFirmaDodavatel * KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.PercNovaFirmaDodavatel); //82
            vradky.Add(
                new KIndexData.VypocetDetail.Radek(KIndexData.KIndexParts.PercNovaFirmaDodavatel, datayear.PercNovaFirmaDodavatel, KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.PercNovaFirmaDodavatel))
                );


            //r21
            val += datayear.PercUzavrenoOVikendu * KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.PercUzavrenoOVikendu); //84
            vradky.Add(
                new KIndexData.VypocetDetail.Radek(KIndexData.KIndexParts.PercUzavrenoOVikendu, datayear.PercUzavrenoOVikendu, KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.PercUzavrenoOVikendu))
            );


            //r22
            val += datayear.PercSmlouvySPolitickyAngazovanouFirmou * KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.PercSmlouvySPolitickyAngazovanouFirmou); //86
            vradky.Add(new KIndexData.VypocetDetail.Radek(
                KIndexData.KIndexParts.PercSmlouvySPolitickyAngazovanouFirmou, datayear.PercSmlouvySPolitickyAngazovanouFirmou, KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.PercSmlouvySPolitickyAngazovanouFirmou)
                )
            );

            if (datayear.KoncetraceDodavateluObory != null)
            {
                
                //oborova koncentrace
                var oboryKoncentrace = datayear.KoncetraceDodavateluObory
                    //.Where(m=>m != null)
                    .Where(m =>
                            m.Koncentrace.HodnotaSmluvProVypocet > (datayear.Statistika.CelkovaHodnotaSmluv * 0.05m)
                            || m.Koncentrace.PocetSmluvProVypocet > (datayear.Statistika.PocetSmluv * 0.05m)
                            || (m.Koncentrace.PocetSmluvBezCenyProVypocet > datayear.Statistika.PocetSmluvBezCeny * 0.02m)
                            )
                    .ToArray(); //for better debug;

                decimal prumernaCenaSmluv = datayear.Statistika.PrumernaHodnotaSmluvSeSoukrSubj;
                var oboryVahy = oboryKoncentrace
                    .Select(m => new KIndexData.VypocetOboroveKoncentrace.RadekObor()
                    {
                        Obor = m.OborName,
                        Hodnota = m.Combined_Herfindahl_Hirschman_Modified(),
                        Vaha = m.Koncentrace.HodnotaSmluvProVypocet > 0 ? m.Koncentrace.HodnotaSmluvProVypocet : 1,  // prumerne cenu u nulobych cen uz pocitam u Koncentrace.HodnotaSmluvProVypocet //+ (prumernaCenaSmluv * (decimal)m.Koncentrace.PocetSmluvProVypocet * m.PodilSmluvBezCeny),
                        PodilSmluvBezCeny = m.PodilSmluvBezCeny,
                        CelkovaHodnotaSmluv = m.Koncentrace.HodnotaSmluvProVypocet,
                        PocetSmluvCelkem = m.Koncentrace.PocetSmluvProVypocet
                    })
                    .ToArray();
                decimal avg = oboryVahy.WeightedAverage(m => m.Hodnota, w => w.Vaha);
                val += avg * KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.KoncentraceDodavateluObory);
                vradky.Add(
                    new KIndexData.VypocetDetail.Radek(KIndexData.KIndexParts.KoncentraceDodavateluObory, avg, KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.KoncentraceDodavateluObory))
                    );
                vypocet.OboroveKoncentrace = new KIndexData.VypocetOboroveKoncentrace();
                vypocet.OboroveKoncentrace.PrumernaCenaSmluv = prumernaCenaSmluv;
                vypocet.OboroveKoncentrace.Radky = oboryVahy.ToArray();
            }
            //
            //r16 - bonus!
            val -= datayear.PercSmlouvyPod50kBonus * KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.PercSmlouvyPod50kBonus);

            vradky.Add(new KIndexData.VypocetDetail.Radek(
                KIndexData.KIndexParts.PercSmlouvyPod50kBonus, -1 * datayear.PercSmlouvyPod50kBonus, KIndexData.DefaultKIndexPartKoeficient(KIndexData.KIndexParts.PercSmlouvyPod50kBonus)
                )
            );
            vypocet.Radky = vradky.ToArray();

            if (val < 0)
                val = 0;

            var kontrolniVypocet = vypocet.Vypocet();
            if (val != kontrolniVypocet)
                throw new ApplicationException("Nesedi vypocet");

            vypocet.LastUpdated = DateTime.Now;
            datayear.KIndexVypocet = vypocet;


            return val;

        }



        public static decimal AveragePercSmlouvyPod50k(string ico, int year, long pocetSmluvCelkem)
        {
            if (pocetSmluvCelkem == 0)
                return 0;

            var res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch($"ico:{ico} cena:>0 AND cena:<=50000 AND datumUzavreni:[{year}-01-01 TO {year + 1}-01-01}}"
, 1, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, null, exactNumOfResults: true);

            decimal perc = (decimal)(res.Total) / (decimal)pocetSmluvCelkem;
            return perc;
        }


        static Dictionary<int, decimal> totalsAvg50k = new Dictionary<int, decimal>();
        static object totalsAvg50kLock = new object();
        public static decimal TotalAveragePercSmlouvyPod50k(int year)
        {
            if (!totalsAvg50k.ContainsKey(year))
            {
                lock (totalsAvg50kLock)
                {
                    if (!totalsAvg50k.ContainsKey(year))
                    {
                        decimal smlouvyAllCount = 0;
                        decimal smlouvyPod50kCount = 0;
                        var res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch($"datumUzavreni:[{year}-01-01 TO {year + 1}-01-01}}"
                            , 1, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, null, exactNumOfResults: true);
                        if (res.IsValid)
                            smlouvyAllCount = res.Total;
                        res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch($"cena:>0 AND cena:<=50000 AND datumUzavreni:[{year}-01-01 TO {year + 1}-01-01}}"
                            , 1, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, null, exactNumOfResults: true);
                        if (res.IsValid)
                            smlouvyPod50kCount = res.Total;


                        decimal smlouvyPod50kperc = 0;

                        if (smlouvyAllCount > 0)
                            smlouvyPod50kperc = smlouvyPod50kCount / smlouvyAllCount;

                        totalsAvg50k.Add(year, smlouvyPod50kperc);
                    }
                }
            }
            return totalsAvg50k[year];
        }
        public static decimal SmlouvyPod50kBonus(decimal icoPodil, decimal vsePodil)
        {
            if (icoPodil > vsePodil * 1.75m)
                return Consts.BonusPod50K_3;
            else if (icoPodil > vsePodil * 1.5m)
                return Consts.BonusPod50K_2;
            if (icoPodil > vsePodil * 1.25m)
                return Consts.BonusPod50K_1;
            return 0m;
        }

        class smlouvaStat
        {
            public string Id { get; set; }
            public string IcoDodavatele { get; set; }
            public decimal CastkaSDPH { get; set; }
            public int Rok { get; set; }
            public DateTime Podepsano { get; set; }
            public int ULimitu { get; set; }
            public int[] Obory { get; set; } = new int[] { };
        }
        public IEnumerable<SmlouvyForIndex> GetSmlouvy(string query)
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
                                dodavatel = "neznamy";

                            smlStat.Add(new smlouvaStat()
                            {
                                Id = s.Id,
                                IcoDodavatele = dodavatel,
                                CastkaSDPH = Math.Abs((decimal)s.CalculatedPriceWithVATinCZK / (decimal)s.Prijemce.Length),
                                Podepsano = s.datumUzavreni,
                                Rok = s.datumUzavreni.Year,
                                ULimitu = s.Hint?.SmlouvaULimitu ?? 0,
                                Obory = s.GetRelevantClassification()
                                    .OrderByDescending(oo => oo.ClassifProbability)
                                    .Select(m => m.TypeValue).ToArray()
                            }); ;

                        }
                    }

                    return new Devmasters.Core.Batch.ActionOutputData();
                }, null,
                null, null,
                false, blockSize: 100);

            IEnumerable<SmlouvyForIndex> smlouvy = smlStat
                .Select(m => new SmlouvyForIndex(m.IcoDodavatele, m.CastkaSDPH, m.ULimitu, m.Obory))
                .OrderByDescending(m => m.HodnotaSmlouvy) //just better debug
                .ToArray(); //just better debug

            return smlouvy;
        }

    }

}
