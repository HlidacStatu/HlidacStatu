using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{

    [Nest.ElasticsearchType(IdProperty = nameof(Ico))]
    public partial class KIndexData
    {
        public class Annual
        {
            public decimal CelkovaKoncentraceDodavatelu { get; set; } //Koncentrace dodavatelů
            public decimal KoncentraceDodavateluBezUvedeneCeny { get; set; } //Koncentrace dodavatelů u smluv bez uvedených cen
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

        public Dictionary<int, Annual> roky { get; set; } = new Dictionary<int, Annual>();

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

    }
}
