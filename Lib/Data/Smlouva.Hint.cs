using HlidacStatu.Lib.XSD;
using Nest;

namespace HlidacStatu.Lib.Data
{

    public partial class HintSmlouva
    {
        public enum ULimituTyp
        {
            OK = 0,
            Limit2M = 1,
            Limit6M = 2
        }
        public enum PolitickaAngazovanostTyp
        {
            Neni = 0,
            PrimoSubjekt = 1,
            AngazovanyMajitel = 2
        }
        public enum VztahSeSoukromymSubjektemTyp
        {
            PouzeSoukrSoukr = -1,
            Neznamy = -2,
            PouzeStatStat = 0,
            PouzeStatSoukr = 1,
            Kombinovane = 2
        }

        [Nest.Number]
        public int SmlouvaULimitu { get; set; } = 0;

        [Nest.Number]
        public int DenUzavreni { get; set; } = 0;

        [Nest.Number]
        public int SmlouvaSPolitickyAngazovanymSubjektem { get; set; } = 0;

        [Nest.Number]
        public int PocetDniOdZalozeniFirmy{ get; set; } = 999999;

        [Nest.Number]
        public int VztahSeSoukromymSubjektem { get; set; } = -2;

    }
}
