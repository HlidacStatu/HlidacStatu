namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public partial class KIndexData
    {
        [Devmasters.Core.ShowNiceDisplayName]
        public enum KIndexParts
        {
            [Devmasters.Core.NiceDisplayName("Podíl smluv bez ceny")]
            PercentBezCeny = 0,

            [Devmasters.Core.NiceDisplayName("Podíl smluv s nedostatkem")]
            PercSeZasadnimNedostatkem = 1,

            [Devmasters.Core.NiceDisplayName("Koncentrace zakázek do rukou malého počtu dodavatelů")]
            CelkovaKoncentraceDodavatelu = 2,

            [Devmasters.Core.NiceDisplayName("Opakování dodavatelů u smluv se skrytou cenou")]
            KoncentraceDodavateluBezUvedeneCeny = 3,

            [Devmasters.Core.NiceDisplayName("Podíl smluv s cenou u limitu veřejných zakázek")]
            PercSmluvUlimitu = 4,

            [Devmasters.Core.NiceDisplayName("Koncentrace dodavatelů u smluv blízské u limitu VZ")]
            KoncentraceDodavateluCenyULimitu = 5,

            [Devmasters.Core.NiceDisplayName("Podíl smluv uzavřených s nově založenými firmami")]
            PercNovaFirmaDodavatel = 6,

            [Devmasters.Core.NiceDisplayName("Podíl smluv uzavřených o víkendu či svátku")]
            PercUzavrenoOVikendu = 7,

            [Devmasters.Core.NiceDisplayName("Podíl smluv uzavřených s firmami, jejichž majitelé či ony sami sponzorovali politické strany")]
            PercSmlouvySPolitickyAngazovanouFirmou = 8,

            [Devmasters.Core.NiceDisplayName("Koncentrace zakázek do rukou malého počtu dodavatelů u hlavních oborů činnosti")]
            KoncentraceDodavateluObory = 9,

            [Devmasters.Core.NiceDisplayName("Bonus za transparentnost")]
            PercSmlouvyPod50kBonus = 10
        }


    }
}
