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

        public class Data
        {
            public decimal CelkovaKoncentraceDodavatelu { get; set; } //Koncentrace dodavatelů
            public decimal KoncentraceDodavatelůBezUvedeneCeny { get; set; } //Koncentrace dodavatelů u smluv bez uvedených cen
            public decimal PercBezUvedeneCeny { get; set; } //% smluv bez uvedené ceny
            public decimal PercNovaFirmaDodavatel { get; set; } //% smluv s dodavatelem mladším 2 měsíců
            public decimal PercSeZasadnimNedostatkem { get; set; } //% smluv s zásadním nedostatkem 
            public decimal PercSmlouvySPolitickyAngazovanouFirmou { get; set; } //% smluv uzavřených s firmou navazanou na politicky aktivní osobu v předchozích 5 letechs
            public decimal PercSmluvUlimitu { get; set; } //% smluv těsně pod hranicí 2M Kč (zakázka malého rozsahu) a 6M (u stavebnictví)
            public decimal PercUzavrenoOVikendu { get; set; } // % smluv uzavřených o víkendu či státním svátku
            public Dictionary<int, decimal> KoncetraceDodavateluObory { get; set; } //Koncentrace dodavatelů

            public Lib.Analysis.BasicData BasicStat { get; set; }
            public Lib.Analysis.RatingData Rating { get; set; }

            public int Rok { get; set; }
        }

        public Dictionary<int, Data> roky { get; set; } = new Dictionary<int, Data>();
        public string Ico { get; set; }


        private Firma urad = null;
        Dictionary<int, Lib.Analysis.BasicData> _calc_SeZasadnimNedostatkem = null;
        Dictionary<int, Lib.Analysis.BasicData> _calc_UzavrenoOVikendu = null;
        Dictionary<int, Lib.Analysis.BasicData> _calc_ULimitu = null;
        Dictionary<int, Lib.Analysis.BasicData> _calc_NovaFirmaDodavatel = null;
        
        public Calculator(string ico)
        {
            this.Ico = ico;
            InitData();
        }

        private void InitData()
        {
            this.urad = Firmy.Get(this.Ico);
            if (urad.Valid == false)
                throw new ArgumentOutOfRangeException("invalid ICO");

            _calc_SeZasadnimNedostatkem = AdvancedQuery.PerYear($"ico:{this.Ico} and chyby:zasadni");

            _calc_UzavrenoOVikendu= AdvancedQuery.PerYear($"ico:{this.Ico} AND (hint.denUzavreni:>0)");

            _calc_ULimitu = AdvancedQuery.PerYear($"ico:{this.Ico} AND ( hint.smlouvaULimitu:>0 )");


            _calc_NovaFirmaDodavatel = AdvancedQuery.PerYear($"ico:{this.Ico} AND ( hint.pocetDniOdZalozeniFirmy:<30 )");
        }

        public Data CalculateForYear(int year)
        {
            Data ret = new Data();
            ret.CelkovaKoncentraceDodavatelu = 0;
            ret.KoncentraceDodavatelůBezUvedeneCeny = 0;
            ret.KoncetraceDodavateluObory = null;

            ret.PercBezUvedeneCeny = this.urad.Statistic().RatingPerYear[year].PercentBezCeny;
            ret.PercSeZasadnimNedostatkem =
                (decimal)_calc_SeZasadnimNedostatkem[year].Pocet / (decimal)urad.Statistic().BasicStatPerYear[year].Pocet;
            ret.PercSmlouvySPolitickyAngazovanouFirmou = this.urad.Statistic().RatingPerYear[year].PercentSPolitiky;

            ret.PercNovaFirmaDodavatel = 0;
            ret.PercSmluvUlimitu =
                (decimal)_calc_ULimitu[year].Pocet / (decimal)urad.Statistic().BasicStatPerYear[year].Pocet;
            ret.PercUzavrenoOVikendu =
                (decimal)_calc_UzavrenoOVikendu[year].Pocet / (decimal)urad.Statistic().BasicStatPerYear[year].Pocet;

            ret.BasicStat = this.urad.Statistic().BasicStatPerYear[year];
            ret.Rating = this.urad.Statistic().RatingPerYear[year];
            return ret;
        }


    }
}
