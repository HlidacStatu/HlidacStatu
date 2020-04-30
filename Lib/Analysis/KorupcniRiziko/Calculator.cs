using System;
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
                kindex.roky.Add(CalculateForYear(year));
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
            ret.CelkovaKoncentraceDodavatelu = KoncentraceDodavatelu(query);
            if (ret.CelkovaKoncentraceDodavatelu != null)
                ret.KoncentraceDodavateluBezUvedeneCeny 
                    = KoncentraceDodavatelu(query + " AND cena:0",ret.CelkovaKoncentraceDodavatelu.PrumernaHodnotaSmluv);
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
        private KIndexData.KoncentraceDodavateluIndexy KoncentraceDodavatelu(string query, decimal? prumHodnotaSmlouvy = null)
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
                false, 100);

            IEnumerable<SmlouvyForIndex> smlouvy = smlStat
                .Select(m => new SmlouvyForIndex(m.IcoDodavatele, prumHodnotaSmlouvy ?? m.CastkaSDPH))
                .OrderByDescending(m => m.HodnotaSmlouvy) //just better debug
                .ToArray(); //just better debug


            if (smlouvy.Count() == 0)
                return null;

            var ret = new KIndexData.KoncentraceDodavateluIndexy();
            ret.PrumernaHodnotaSmluv = smlouvy
                                .Where(m => m.HodnotaSmlouvy != 0)
                                .Count() == 0 ? 0 : smlouvy
                                                        .Where(m => m.HodnotaSmlouvy != 0)
                                                        .Select(m => Math.Abs(m.HodnotaSmlouvy))
                                                        .Average();

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
