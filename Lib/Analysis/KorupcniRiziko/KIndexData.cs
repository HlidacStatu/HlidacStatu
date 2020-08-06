using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Nest;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{

    [Nest.ElasticsearchType(IdProperty = nameof(Ico))]
    public partial class KIndexData
    {
        public enum KIndexLabelValues
        {
            A, B, C, D, E, F
        }

        public enum KIndexParts
        {
            PercentBezCeny,
            PercSeZasadnimNedostatkem,
            CelkovaKoncentraceDodavatelu,
            KoncentraceDodavateluBezUvedeneCeny,
            PercSmluvUlimitu,
            KoncentraceDodavateluCenyULimitu,
            PercNovaFirmaDodavatel,
            PercUzavrenoOVikendu,
            PercSmlouvySPolitickyAngazovanouFirmou,
            KoncentraceDodavateluObory
        }


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


            public Lib.Analysis.BasicData Smlouvy { get; set; }
            //radek 5
            //
            public Lib.Analysis.RatingData Statistika { get; set; }

            public int Rok { get; set; }
            //r12
            public FinanceData FinancniUdaje { get; set; }

            public decimal? KIndex { get; set; }

            public string[] KIndexIssues { get; set; }

            [Nest.Object( Ignore =true )]
            public KIndexLabelValues KIndexLabel
            {
                get {
                    return KIndexLabelValues.C; //TODO
                }
            }

        }


        public KIndexLabelValues KIndexLabelForPart(KIndexParts part, decimal value)
        {
            switch (part)
            {
                case KIndexParts.PercentBezCeny:
                    if (value < 0.1m)
                        return KIndexLabelValues.A;
                    else if (value > 0.25m)
                        return KIndexLabelValues.F;
                    else
                        return KIndexLabelValues.D;
                case KIndexParts.PercSeZasadnimNedostatkem:
                    if (value < 0.02m)
                        return KIndexLabelValues.A;
                    else if (value > 0.05m)
                        return KIndexLabelValues.F;
                    else
                        return KIndexLabelValues.D;
                case KIndexParts.CelkovaKoncentraceDodavatelu:
                case KIndexParts.KoncentraceDodavateluCenyULimitu:
                case KIndexParts.KoncentraceDodavateluBezUvedeneCeny:
                case KIndexParts.KoncentraceDodavateluObory:
                    if (value < 0.15m)
                        return KIndexLabelValues.A;
                    else if (value > 0.25m)
                        return KIndexLabelValues.F;
                    else
                        return KIndexLabelValues.D;
                case KIndexParts.PercSmluvUlimitu:
                    if (value < 0.1m)
                        return KIndexLabelValues.A;
                    else if (value > 0.2m)
                        return KIndexLabelValues.F;
                    else
                        return KIndexLabelValues.D;
                case KIndexParts.PercNovaFirmaDodavatel:
                    if (value < 0.05m)
                        return KIndexLabelValues.A;
                    else if (value > 0.15m)
                        return KIndexLabelValues.F;
                    else
                        return KIndexLabelValues.D;
                case KIndexParts.PercUzavrenoOVikendu:
                    if (value < 0.01m)
                        return KIndexLabelValues.A;
                    else if (value > 0.1m)
                        return KIndexLabelValues.F;
                    else
                        return KIndexLabelValues.D;
                case KIndexParts.PercSmlouvySPolitickyAngazovanouFirmou:
                    if (value < 0.1m)
                        return KIndexLabelValues.A;
                    else if (value > 0.2m)
                        return KIndexLabelValues.F;
                    else
                        return KIndexLabelValues.D;
                default:
                        throw new ArgumentOutOfRangeException();
            }
        }



        public List<Annual> roky { get; set; } = new List<Annual>();

        public Annual ForYear(int year)
        {
            return roky.Where(m => m != null && m.Rok == year).FirstOrDefault();
        }

        public string Ico { get; set; }
        public UcetniJednotkaInfo UcetniJednotka { get; set; } = new UcetniJednotkaInfo();

        [Nest.Date]
        public DateTime LastSaved { get; set; }

        public void Save()
        {
            //calculate fields before saving
            this.LastSaved = DateTime.Now;
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
