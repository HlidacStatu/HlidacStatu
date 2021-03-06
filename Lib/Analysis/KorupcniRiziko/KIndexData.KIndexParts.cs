﻿using Devmasters.Enums;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{

    public partial class KIndexData
    {
        [Groupable]
        [Sortable( SortableAttribute.SortAlgorithm.BySortValue)]
        [Devmasters.Enums.ShowNiceDisplayName()]
        public enum KIndexParts
        {
            [SortValue(1)]
            [GroupValue("Hlavni")]
            [Devmasters.Enums.NiceDisplayName("Podíl smluv bez ceny")]
            PercentBezCeny = 0,

            [SortValue(2)]
            [GroupValue("Hlavni")]
            [Devmasters.Enums.NiceDisplayName("Podíl smluv s nedostatkem")]
            PercSeZasadnimNedostatkem = 1,

            [GroupValue("Hlavni")]
            [SortValue(3)]
            [Devmasters.Enums.NiceDisplayName("Podíl smluv uzavřených s firmami, jejichž majitelé či ony samy sponzorovali politické strany")]
            PercSmlouvySPolitickyAngazovanouFirmou = 8,

            [SortValue(4)]
            [GroupValue("Doplnkove")]
            [Devmasters.Enums.NiceDisplayName("Podíl smluv s cenou u limitu veřejných zakázek")]
            PercSmluvUlimitu = 4,

            [SortValue(6)]
            [GroupValue("Doplnkove")]
            [Devmasters.Enums.NiceDisplayName("Podíl smluv uzavřených o víkendu či svátku")]
            PercUzavrenoOVikendu = 7,

            [SortValue(5)]
            [GroupValue("Doplnkove")]
            [Devmasters.Enums.NiceDisplayName("Podíl smluv uzavřených s nově založenými firmami")]
            PercNovaFirmaDodavatel = 6,


            [SortValue(10)]
            [GroupValue("Koncentrace")]
            [Devmasters.Enums.NiceDisplayName("Koncentrace zakázek do rukou malého počtu dodavatelů")]
            CelkovaKoncentraceDodavatelu = 2,

            [SortValue(11)]
            [GroupValue("Koncentrace")]
            [Devmasters.Enums.NiceDisplayName("Koncentrace dodavatelů u smluv se skrytou cenou")]
            KoncentraceDodavateluBezUvedeneCeny = 3,

            [SortValue(12)]
            [GroupValue("Koncentrace")]
            [Devmasters.Enums.NiceDisplayName("Koncentrace dodavatelů u smluv blízké u limitu VZ")]
            KoncentraceDodavateluCenyULimitu = 5,

            [SortValue(13)]
            [GroupValue("Koncentrace")]
            [Devmasters.Enums.NiceDisplayName("Koncentrace zakázek do rukou malého počtu dodavatelů u hlavních oborů činnosti")]
            KoncentraceDodavateluObory = 9,




            [SortValue(100)]
            [GroupValue("Bonus")]
            [Devmasters.Enums.NiceDisplayName("Bonus za transparentnost")]
            PercSmlouvyPod50kBonus = 10
        }


    }
}
