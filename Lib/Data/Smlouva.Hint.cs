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
        public enum DenUzavreniTyp
        {
            Pracovni = 0,
            Vikend = 1,
            Svatek = 2
        }
        public enum PolitickaAngazovanostTyp
        {
            Neni = 0,
            PrimoSubjekt = 1,
            AngazovanyMajitel = 2
        }

        [Nest.Number]
        public int SmlouvaULimitu { get; set; } = 0;
        [Nest.Number]
        public int DenUzavreni { get; set; } = 0;
        [Nest.Number]
        public int SmlouvaSPolitickyAngazovanymSubjektem { get; set; } = 0;
    }
}
