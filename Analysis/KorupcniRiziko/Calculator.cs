using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data;
using Nest;

namespace HlidacStatu.Analysis.KorupcniRiziko
{

    public class Calculator
    {

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


        private Firma firma = null;
        Dictionary<int, Lib.Analysis.BasicData> _calc_PercSeZasadnimNedostatkem = null;

        public Calculator(string ico)
        {
            this.Ico = ico;
            _calc_PercSeZasadnimNedostatkem = Query.PerYear($"ico:{this.Ico} and chyby:zasadni");
            this.firma = Firmy.Get(this.Ico);
            if (firma.Valid == false)
                throw new ArgumentOutOfRangeException("invalid ICO");
        }

        public Data CalculateForYear(int year)
        {
            Data ret = new Data();
            ret.CelkovaKoncentraceDodavatelu = 0;
            ret.KoncentraceDodavatelůBezUvedeneCeny = 0;
            ret.KoncetraceDodavateluObory = null;

            ret.PercBezUvedeneCeny = this.firma.Statistic().RatingPerYear[year].PercentBezCeny;
            ret.PercSeZasadnimNedostatkem =
                _calc_PercSeZasadnimNedostatkem[year].Pocet / (decimal)firma.Statistic().BasicStatPerYear[year].Pocet;
            ret.PercSmlouvySPolitickyAngazovanouFirmou = this.firma.Statistic().RatingPerYear[year].PercentSPolitiky;

            ret.PercNovaFirmaDodavatel = 0;
            ret.PercSmluvUlimitu = 0;
            ret.PercUzavrenoOVikendu = 0;

            ret.BasicStat = this.firma.Statistic().BasicStatPerYear[year];
            ret.Rating = this.firma.Statistic().RatingPerYear[year];
            return ret;
        }


    }
}
