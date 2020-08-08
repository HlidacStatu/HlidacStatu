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
        [Devmasters.Core.ShowNiceDisplayName()]
        public enum KIndexLabelValues
        {
            [Devmasters.Core.NiceDisplayName("-")]
            None = -1,
            A = 0,
            B = 1,
            C = 2,
            D = 3,
            E = 4,
            F = 5
        }

        public enum KIndexParts
        {
            PercentBezCeny = 0,
            PercSeZasadnimNedostatkem = 1,
            CelkovaKoncentraceDodavatelu = 2,
            KoncentraceDodavateluBezUvedeneCeny = 3,
            PercSmluvUlimitu = 4,
            KoncentraceDodavateluCenyULimitu = 5,
            PercNovaFirmaDodavatel = 6,
            PercUzavrenoOVikendu = 7,
            PercSmlouvySPolitickyAngazovanouFirmou = 8,
            KoncentraceDodavateluObory = 9,
            PercSmlouvyPod50kBonus = 10
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
