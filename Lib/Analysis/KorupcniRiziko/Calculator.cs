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

        public string Ico { get; set; }

        private CalculatedData kindex = null;

        public Calculator(string ico)
        {
            this.Ico = ico;
        }

        object lockCalc = new object();
        public CalculatedData GetData(bool refresh=false)
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

        private CalculatedData Calculate()
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

            _calc_SeZasadnimNedostatkem = AdvancedQuery.PerYear($"ico:{this.Ico} and chyby:zasadni");

            _calc_UzavrenoOVikendu = AdvancedQuery.PerYear($"ico:{this.Ico} AND (hint.denUzavreni:>0)");

            _calc_ULimitu = AdvancedQuery.PerYear($"ico:{this.Ico} AND ( hint.smlouvaULimitu:>0 )");


            _calc_NovaFirmaDodavatel = AdvancedQuery.PerYear($"ico:{this.Ico} AND ( hint.pocetDniOdZalozeniFirmy:<30 )");
        }

        private CalculatedData.Annual CalculateForYear(int year)
        {
            decimal smlouvyZaRok = (decimal)urad.Statistic().BasicStatPerYear[year].Pocet;
            CalculatedData.Annual ret = new CalculatedData.Annual();

            ret.PercBezUvedeneCeny = this.urad.Statistic().RatingPerYear[year].PercentBezCeny;
            ret.PercSeZasadnimNedostatkem = (decimal)_calc_SeZasadnimNedostatkem[year].Pocet / smlouvyZaRok;
            ret.PercSmlouvySPolitickyAngazovanouFirmou = this.urad.Statistic().RatingPerYear[year].PercentSPolitiky;

            ret.PercNovaFirmaDodavatel = (decimal)_calc_NovaFirmaDodavatel[year].Pocet / smlouvyZaRok;
            ret.PercSmluvUlimitu = (decimal)_calc_ULimitu[year].Pocet / smlouvyZaRok;
            ret.PercUzavrenoOVikendu = (decimal)_calc_UzavrenoOVikendu[year].Pocet / smlouvyZaRok;

            ret.BasicStat = this.urad.Statistic().BasicStatPerYear[year];
            ret.Rating = this.urad.Statistic().RatingPerYear[year];


            ret.CelkovaKoncentraceDodavatelu = 0;
            ret.KoncentraceDodavateluBezUvedeneCeny = 0;
            ret.KoncetraceDodavateluObory = null;


            return ret;
        }


    }
}
