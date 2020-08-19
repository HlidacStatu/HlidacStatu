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

        public static int[] KIndexLimits = { 0, 3, 6, 9, 12, 15 };


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

        [Devmasters.Core.ShowNiceDisplayName]
        public enum KIndexParts
        {
            [Devmasters.Core.NiceDisplayName("% smluv bez ceny")]
            PercentBezCeny = 0,

            [Devmasters.Core.NiceDisplayName("% smluv s nedostatkem")]
            PercSeZasadnimNedostatkem = 1,

            [Devmasters.Core.NiceDisplayName("Koncentrace dodavatelů")]
            CelkovaKoncentraceDodavatelu = 2,

            [Devmasters.Core.NiceDisplayName("Koncentr.dodav. u smluv se skrytou cenou")]
            KoncentraceDodavateluBezUvedeneCeny = 3,

            [Devmasters.Core.NiceDisplayName("% smluv u limitu veřejných zakázek")]
            PercSmluvUlimitu = 4,

            [Devmasters.Core.NiceDisplayName("Koncentr.dodav. u smluv u limitu VZ")]
            KoncentraceDodavateluCenyULimitu = 5,

            [Devmasters.Core.NiceDisplayName("% smluv s nově založenými firmami")]
            PercNovaFirmaDodavatel = 6,

            [Devmasters.Core.NiceDisplayName("% smluv uzavřených o víkendu")]
            PercUzavrenoOVikendu = 7,

            [Devmasters.Core.NiceDisplayName("% smluv s politicky angažovanou firmou")]
            PercSmlouvySPolitickyAngazovanouFirmou = 8,

            [Devmasters.Core.NiceDisplayName("Koncentr.dodavatelů podle oborů")]
            KoncentraceDodavateluObory = 9,

            [Devmasters.Core.NiceDisplayName("Bonus za transparentnost")]
            PercSmlouvyPod50kBonus = 10
        }



        public Annual LastKIndex()
        {
            return roky?.Where(m => m.KIndexAvailable)?.OrderByDescending(m => m.Rok)?.FirstOrDefault();
        }

        public decimal? LastKIndexValue()
        {
            return LastKIndex()?.KIndex;
        }
        public KIndexLabelValues LastKIndexLabel()
        {
            return LastKIndexLabel(out int? tmp);
        }
        public KIndexLabelValues LastKIndexLabel(out int? rok)
        {

            var val = LastKIndex();
            rok = null;
            if (val != null)
            {
                rok = val.Rok;
                return val.KIndexLabel;
            }
            else
                return KIndexLabelValues.None;
        }

        public List<Annual> roky { get; set; } = new List<Annual>();

        public Annual ForYear(int year)
        {
            return roky.Where(m => m != null && m.Rok == year).FirstOrDefault();
        }

        public string Ico { get; set; }
        public string Jmeno { get; set; }
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




        public static KIndexData GetDirect(string ico)
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


        public static string KIndexLabelColor(KIndexLabelValues label)
        {
            switch (label)
            {
                case KIndexLabelValues.None:
                    return "#000000";
                case KIndexLabelValues.A:
                    return "#00A5FF";
                case KIndexLabelValues.B:
                    return "#0064B4";
                case KIndexLabelValues.C:
                    return "#002D5A";
                case KIndexLabelValues.D:
                    return "#9099A3";
                case KIndexLabelValues.E:
                    return "#F2B41E";
                case KIndexLabelValues.F:
                    return "#D44820";
                default:
                    return "#000000";
            }
        }

        public static KIndexLabelValues KIndexLabelForPart(KIndexParts part, decimal value)
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
                    return KIndexLabelValues.None;
            }
        }

        public static decimal DefaultKIndexPartKoeficient(KIndexParts part)
        {
            switch (part)
            {
                case KIndexParts.PercentBezCeny:
                    return 10m;
                case KIndexParts.PercSeZasadnimNedostatkem:
                    return 10m;
                case KIndexParts.CelkovaKoncentraceDodavatelu:
                    return 10m;
                case KIndexParts.KoncentraceDodavateluBezUvedeneCeny:
                    return 10m;
                case KIndexParts.PercSmluvUlimitu:
                    return 10m;
                case KIndexParts.KoncentraceDodavateluCenyULimitu:
                    return 10m;
                case KIndexParts.PercNovaFirmaDodavatel:
                    return 10m;
                case KIndexParts.PercUzavrenoOVikendu:
                    return 10m;
                case KIndexParts.PercSmlouvySPolitickyAngazovanouFirmou:
                    return 10m;
                case KIndexParts.KoncentraceDodavateluObory:
                    return 10m;
                case KIndexParts.PercSmlouvyPod50kBonus:
                    return 1m;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string KIndexLabelIconUrl(KIndexLabelValues value, bool local = true)
        {
            string url = "";
            if (local == false)
                url = "https://www.hlidacstatu.cz";
            switch (value)
            {
                case KIndexLabelValues.None:
                    return url + "/Content/Img/1x1.png ";
                default:
                    return url + $"/Content/kindex/icon{value.ToString()}.svg";

            }
        }


    }
}
