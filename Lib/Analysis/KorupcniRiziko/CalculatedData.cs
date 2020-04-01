using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{

    [Nest.ElasticsearchType(IdProperty = nameof(Ico))]
    public class CalculatedData
    {
        public class UcetniJednotkaInfo
        {
            //https://monitor.statnipokladna.cz/datovy-katalog/ciselniky/prohlizec/9
            public int DruhUcetniJednotky { get; set; }
            //https://monitor.statnipokladna.cz/datovy-katalog/ciselniky/prohlizec/24

            public int PodDruhUcetniJednotky { get; set; }

            //https://monitor.statnipokladna.cz/datovy-katalog/ciselniky/prohlizec/12
            public int FormaUcetniJednotky { get; set; }

            //https://monitor.statnipokladna.cz/datovy-katalog/ciselniky/prohlizec/17
            public int InstitucionalniSektor { get; set; }

            //KLASIFIKACE FUNKCÍ VLÁDNÍCH INSTITUCÍ
            //https://monitor.statnipokladna.cz/datovy-katalog/ciselniky/prohlizec/15
            //https://www.czso.cz/csu/czso/klasifikace_funkci_vladnich_instituci_-cz_cofog-
            public COFOG Cofog { get; set; }

        }
        public class COFOG
        {
            public int Oddil { get; set; }
            public int Skupina { get; set; } = 0;
            public int Trida { get; set; } = 0;

        }
        public class Annual
        {
            public decimal CelkovaKoncentraceDodavatelu { get; set; } //Koncentrace dodavatelů
            public decimal KoncentraceDodavateluBezUvedeneCeny { get; set; } //Koncentrace dodavatelů u smluv bez uvedených cen
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

        public Dictionary<int, Annual> roky { get; set; } = new Dictionary<int, Annual>();

        public string Ico { get; set; }
        public UcetniJednotkaInfo UcetniJednotka { get; set; } = new UcetniJednotkaInfo();

        public int PocetObyvatelObce { get; set; } = 0;

    }
}
