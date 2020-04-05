using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data;
using Nest;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{

    public class Calculator
    {
        public static int[] CalculationYears = Enumerable.Range(2017, DateTime.Now.Year - 2017).ToArray();


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
                    kindex = Calculate();
                }
            }

            return kindex;
        }

        private KIndexData Calculate()
        {
            this.InitData();
            foreach (var year in CalculationYears)
            {
                kindex.roky.Add(year, CalculateForYear(year));
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
            KIndexData.Annual ret = new KIndexData.Annual();
            var fc = new FinanceDataCalculator(this.Ico, year);
            ret.FinancniUdaje = fc.GetData();

            ret.PercSeZasadnimNedostatkem = (decimal)_calc_SeZasadnimNedostatkem[year].Pocet / smlouvyZaRok;
            ret.PercSmlouvySPolitickyAngazovanouFirmou = this.urad.Statistic().RatingPerYear[year].PercentSPolitiky;

            ret.PercNovaFirmaDodavatel = (decimal)_calc_NovaFirmaDodavatel[year].Pocet / smlouvyZaRok;
            ret.PercSmluvUlimitu = (decimal)_calc_ULimitu[year].Pocet / smlouvyZaRok;
            ret.PercUzavrenoOVikendu = (decimal)_calc_UzavrenoOVikendu[year].Pocet / smlouvyZaRok;

            ret.Smlouvy = this.urad.Statistic().BasicStatPerYear[year];
            ret.Statistika = this.urad.Statistic().RatingPerYear[year];


            ret.CelkovaKoncentraceDodavatelu = KoncentraceDodavatelu($"icoPlatce:{this.Ico} AND datumUzavreni:[{year}-01-01 TO {year+1}-01-01}}");
            ret.KoncentraceDodavateluBezUvedeneCeny = 0;
            ret.KoncetraceDodavateluObory = null;


            return ret;
        }


        class smlouvaStat
        {
            public string Id { get; set; }
            public string IcoDodavatele { get; set; }
            public decimal CastkaSDPH { get; set; }
            public int Rok { get; set; }
            public DateTime Podepsano { get; set; }
        }
        private decimal KoncentraceDodavatelu(string query)
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
                Util.Consts.outputWriter.OutputWriter, Util.Consts.progressWriter.ProgressWriter,
                false, 100);

            Dictionary<string, decimal> kontraktyPerIco = smlStat
                .GroupBy(k => k.IcoDodavatele, v => v, (k, v) => new { ico = k, sum = v.Sum(s => s.CastkaSDPH) })
                .OrderByDescending(k=>k.sum)   //just better debug
                .ToDictionary(k => k.ico, v => v.sum);

            IEnumerable<Tuple<string, decimal>> kontrakty = smlStat
                .Select(m => new Tuple<string, decimal>(m.IcoDodavatele, m.CastkaSDPH))
                .OrderByDescending(m => m.Item2) //just better debug
                .ToArray(); //just better debug


            var herfindahlIndex = HerfindahlIndex(kontraktyPerIco.Values);
            var herfindahlIndex_z = Herfindahl_ZIndex(kontrakty);
            return herfindahlIndex;
        }

        public static decimal HerfindahlIndex(IEnumerable<Tuple<string, decimal>> individualContractDodavatelCena)
        {
            var groupedPerDodavatel = individualContractDodavatelCena
        .GroupBy(k => k.Item1, m => m, (k, v) => new { k = k, sumVal = v.Sum(m => m.Item2) })
        .ToDictionary(k => k.k, v => v.sumVal);

            return HerfindahlIndex(groupedPerDodavatel.Values);
        }

        public static decimal HerfindahlIndex(IEnumerable<decimal> valuesGroupedByCompany)
        {
            decimal total = valuesGroupedByCompany.Sum();
            decimal hindex = valuesGroupedByCompany
                .Select(v => v / total) //podil na trhu
                .Select(v => v * v) // ^2
                .Sum(); //SUM
            return hindex;
        }
        public static decimal Herfindahl_ZIndex(
            IEnumerable<Tuple<string,decimal>> individualContractDodavatelCena)
        {
            decimal idealHI = HerfindahlIndex(individualContractDodavatelCena.Select(m=>m.Item2));

            var groupedPerDodavatel = individualContractDodavatelCena
                .GroupBy(k => k.Item1, m => m, (k, v) => new { k = k, sumVal = v.Sum(m => m.Item2) })
                .ToDictionary(k => k.k, v => v.sumVal);

            decimal HI = HerfindahlIndex(groupedPerDodavatel.Values);


            return HI/idealHI;
        }




    }

}
