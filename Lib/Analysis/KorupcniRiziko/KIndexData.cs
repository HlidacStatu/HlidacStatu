using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{

    [Nest.ElasticsearchType(IdProperty = nameof(Ico))]
    public partial class KIndexData
    {
        public class KoncentraceDodavateluIndexy
        {
            public decimal Herfindahl_Hirschman_Index { get; set; }
            public decimal Herfindahl_Hirschman_Normalized { get; set; }
            public decimal Herfindahl_Hirschman_Modified { get; set; }
            public decimal Comprehensive_Industrial_Concentration_Index { get; set; }
            public decimal Hall_Tideman_Index { get; set; }
            public decimal Kwoka_Dominance_Index { get; set; }

            public decimal PrumernaHodnotaSmluv { get; set; } //pouze tech s hodnotou vyssi nez 0 Kc
        }
        public class Annual
        {
            protected Annual() { }
            public Annual(int rok) { this.Rok = rok; }

            public decimal PodilSmluvNaCelkovychNakupech { get; set; } //Podíl smluv na celkových nákupech
            public KoncentraceDodavateluIndexy CelkovaKoncentraceDodavatelu { get; set; } //Koncentrace dodavatelů
            public KoncentraceDodavateluIndexy KoncentraceDodavateluBezUvedeneCeny { get; set; } //Koncentrace dodavatelů u smluv bez uvedených cen
            public decimal PercNovaFirmaDodavatel { get; set; } //% smluv s dodavatelem mladším 2 měsíců
            public decimal PercSeZasadnimNedostatkem { get; set; } //% smluv s zásadním nedostatkem 
            public decimal PercSmlouvySPolitickyAngazovanouFirmou { get; set; } //% smluv uzavřených s firmou navazanou na politicky aktivní osobu v předchozích 5 letechs
            public decimal PercSmluvUlimitu { get; set; } //% smluv těsně pod hranicí 2M Kč (zakázka malého rozsahu) a 6M (u stavebnictví)
            public decimal PercUzavrenoOVikendu { get; set; } // % smluv uzavřených o víkendu či státním svátku
            public Dictionary<int, decimal> KoncetraceDodavateluObory { get; set; } //Koncentrace dodavatelů

            public Lib.Analysis.BasicData Smlouvy { get; set; }
            public Lib.Analysis.RatingData Statistika { get; set; }

            public int Rok { get; set; }

            public FinanceData FinancniUdaje { get; set; }
        }

        public List<Annual> roky { get; set; } = new List<Annual>();

        public string Ico { get; set; }
        public UcetniJednotkaInfo UcetniJednotka { get; set; } = new UcetniJednotkaInfo();

        public void Save()
        {
            //calculate fields before saving

            var res = ES.Manager.GetESClient_KIndex().Index<KIndexData>(this, o => o.Id(this.Ico)); //druhy parametr musi byt pole, ktere je unikatni
            if (!res.IsValid)
            {
                throw new ApplicationException(res.ServerError?.ToString());
            }
        }

        public static KIndexData Get(string ico)
        {
            var res = ES.Manager.GetESClient_KIndex().Get<KIndexData>(ico);
            if (res.Found == false)
                return null;
            else if (!res.IsValid)
            {
                throw new ApplicationException(res.ServerError?.ToString());
            }
            else
                return res.Source;
        }

    }
}
