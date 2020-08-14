using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public partial class KIndexData
    {
        public class Annual
        {
            protected Annual() { }

            [Nest.Date]
            public DateTime LastUpdated { get; set; }

            public Annual(int rok) { this.Rok = rok; }

            public decimal PodilSmluvNaCelkovychNakupech { get; set; } //Podíl smluv na celkových nákupech

            //r13
            public KoncentraceDodavateluIndexy CelkovaKoncentraceDodavatelu { get; set; } //Koncentrace dodavatelů
            //r15
            public KoncentraceDodavateluIndexy KoncentraceDodavateluBezUvedeneCeny { get; set; } //Koncentrace dodavatelů u smluv bez uvedených cen
            //r15
            public KoncentraceDodavateluIndexy KoncentraceDodavateluCenyULimitu { get; set; } //Koncentrace dodavatelů u smluv u limitu VZ


            //r16
            public decimal PercSmlouvyPod50k { get; set; } //% smluv s cenou pod 50000

            //r16
            public decimal PercSmlouvyPod50kBonus { get; set; }

            //r16
            public decimal TotalAveragePercSmlouvyPod50k { get; set; } //% smluv s cenou pod 50000 prumerny pres vsechny smlouvy v roce


            //r20
            public decimal PercNovaFirmaDodavatel { get; set; } //% smluv s dodavatelem mladším 2 měsíců

            //r11
            public decimal PercSeZasadnimNedostatkem { get; set; } //% smluv s zásadním nedostatkem 

            //r23
            public decimal PercSmlouvySPolitickyAngazovanouFirmou { get; set; } //% smluv uzavřených s firmou navazanou na politicky aktivní osobu v předchozích 5 letechs

            //r19
            public decimal PercSmluvUlimitu { get; set; } //% smluv těsně pod hranicí 2M Kč (zakázka malého rozsahu) a 6M (u stavebnictví)

            //r22
            public decimal PercUzavrenoOVikendu { get; set; } // % smluv uzavřených o víkendu či státním svátku

            public List<KoncentraceDodavateluObor> KoncetraceDodavateluObory { get; set; } //Koncentrace dodavatelů

            public KoncentraceDodavateluObor KoncetraceDodavateluProObor(int oborId)
            {
                return KoncetraceDodavateluObory.Where(m => m != null).FirstOrDefault(m => m.OborId == oborId);
            }
            public KoncentraceDodavateluObor KoncetraceDodavateluProObor(string searchShortcut)
            {
                return KoncetraceDodavateluObory.Where(m => m != null).FirstOrDefault(m => m.OborName == searchShortcut);
            }
            public KoncentraceDodavateluObor KoncetraceDodavateluProObor(Lib.Data.Smlouva.SClassification.ClassificationsTypes type)
            {
                return KoncetraceDodavateluProObor((int)type);
            }

            //radek 5
            //
            public StatistickeUdaje Statistika { get; set; }

            public int Rok { get; set; }
            //r12
            public FinanceData FinancniUdaje { get; set; }


            public decimal? KIndex { get; set; }

            public string[] KIndexIssues { get; set; }

            public VypocetDetail KIndexVypocet { get; set; }

            [Nest.Object(Ignore = true)]
            public bool KIndexAvailable
            { 
                get => this.KIndex.HasValue && this.KIndex != Consts.MinSmluvPerYearKIndexValue;
            }

            [Nest.Object(Ignore = true)]
            public KIndexLabelValues KIndexLabel
            {
                get
                {
                    KIndexLabelValues val = KIndexLabelValues.None;
                    for (int i = 0; i < KIndexLimits.Length; i++)
                    {
                        if (this.KIndex > KIndexLimits[i])
                            val = (KIndexLabelValues)i;
                    }

                    return val;
                }
            }
            [Nest.Object(Ignore = true)]
            public string KIndexLabelColor
            {
                get => KIndexData.KIndexLabelColor(this.KIndexLabel);
            }

            public string KIndexLabelIconUrl(bool local = true)
            {
                return KIndexData.KIndexLabelIconUrl(this.KIndexLabel, local);
            }
        }

    }
}
