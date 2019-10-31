using Nest;
using System;

namespace HlidacStatu.Lib.Data.Dotace
{
    [ElasticsearchType(IdProperty = nameof(IdRozhodnuti))]
    public partial class Dotace
    {

        [Nest.Keyword]
        public string IdPrijemce { get; set; }
        [Nest.Keyword]
        public string PrijemceIco { get; set; }
        [Nest.Text]
        public string PrijemceJmenoPrijemce { get; set; }
        [Nest.Text]
        public string PrijemceObchodniJmeno { get; set; }
        [Nest.Text]
        public string PrijemcePrijmeni { get; set; }
        [Nest.Text]
        public string PrijemceJmeno { get; set; }
        [Nest.Number]
        public int? PrijemceRokNarozeni { get; set; }
        [Nest.Keyword]
        public string PrijemceObecKod { get; set; }
        [Nest.Keyword]
        public string PrijemceIriObec { get; set; }
        [Nest.Keyword]
        public string PrijemceObecNazev { get; set; }
        [Nest.Keyword]
        public string PrijemceObecNutsKod { get; set; }
        [Nest.Keyword]
        public string PrijemceIriOkres { get; set; }
        [Nest.Keyword]
        public string PrijemceOkresNazev { get; set; }
        [Nest.Keyword]
        public string PrijemceOkresNutsKod { get; set; }
        [Nest.Keyword]
        public string IdDotace { get; set; }
        [Nest.Keyword]
        public string DotaceProjektKod { get; set; }
        [Nest.Date]
        public DateTime? DotacePodpisDatum { get; set; }
        [Nest.Keyword]
        public string DotaceSubjektRozliseniKod { get; set; }
        [Nest.Date]
        public DateTime? DotaceUkonceniPlanovaneDatum { get; set; }
        [Nest.Date]
        public DateTime? DotaceUkonceniSkutecneDatum { get; set; }
        [Nest.Date]
        public DateTime? DotaceZahajeniPlanovaneDatum { get; set; }
        [Nest.Date]
        public DateTime? DotaceZahajeniSkutecneDatum { get; set; }
        [Nest.Boolean]
        public bool DotaceZmenaSmlouvyIndikator { get; set; }
        [Nest.Keyword]
        public string DotaceProjektIdentifikator { get; set; }
        [Nest.Text]
        public string DotaceProjektNazev { get; set; }
        [Nest.Keyword]
        public string DotaceIriOperacniProgram { get; set; }
        [Nest.Keyword]
        public string DotaceIriPodprogram { get; set; }
        [Nest.Keyword]
        public string DotaceIriPriorita { get; set; }
        [Nest.Keyword]
        public string DotaceIriOpatreni { get; set; }
        [Nest.Keyword]
        public string DotaceIriPodopatreni { get; set; }
        [Nest.Keyword]
        public string DotaceIriGrantoveSchema { get; set; }
        [Nest.Keyword]
        public string DotaceIriProgramPodpora { get; set; }
        [Nest.Keyword]
        public string DotaceIriTypCinnosti { get; set; }
        [Nest.Keyword]
        public string DotaceIriProgram { get; set; }
        [Nest.Date]
        public DateTime? DotaceDPlatnost { get; set; }
        [Nest.Date]
        public DateTime? DotaceDTAktualizace { get; set; }
        [Nest.Keyword]
        public string IdOperacniProgram { get; set; }
        [Nest.Keyword]
        public string DotaceOperacniProgramKod { get; set; }
        [Nest.Text]
        public string DotaceOperacniProgramNazev { get; set; }
        [Nest.Number]
        public int? DotaceOperacniProgramCislo { get; set; }
        [Nest.Date]
        public DateTime? DotaceOperacniProgramZaznamPlatnostOdDatum { get; set; }
        [Nest.Date]
        public DateTime? DotaceOperacniProgramZaznamPlatnostDoDatum { get; set; }
        [Nest.Keyword]
        public string IdGrantoveSchema { get; set; }
        [Nest.Keyword]
        public string DotaceGrantoveSchemaKod { get; set; }
        [Nest.Text]
        public string DotaceGrantoveSchemaNazev { get; set; }
        [Nest.Number]
        public int? DotaceGrantoveSchemaCislo { get; set; }
        [Nest.Date]
        public DateTime? DotaceGrantoveSchemaZaznamPlatnostOdDatum { get; set; }
        [Nest.Date]
        public DateTime? DotaceGrantoveSchemaZaznamPlatnostDoDatum { get; set; }
        [Nest.Keyword]
        public string IdRozhodnuti { get; set; }
        [Nest.Number]
        public float? RozhodnutiCastkaPozadovana { get; set; }
        [Nest.Number]
        public float? RozhodnutiCastkaRozhodnuta { get; set; }
        [Nest.Number]
        public int? RozhodnutiRokRozhodnuti { get; set; }
        [Nest.Boolean]
        public bool RozhodnutiInvesticeIndikator { get; set; }
        [Nest.Boolean]
        public bool RozhodnutiNavratnostIndikator { get; set; }
        [Nest.Boolean]
        public bool RozhodnutiRefundaceIndikator { get; set; }
        [Nest.Boolean]
        public bool RozhodnutiTuzemskyZdroj { get; set; }
        [Nest.Keyword]
        public string RozhodnutiFinancniZdrojKod { get; set; }
        [Nest.Text]
        public string RozhodnutiFinancniZdrojNazev { get; set; }
        [Nest.Keyword]
        public string RozhodnutiDotacePoskytovatelKod { get; set; }
        [Nest.Text]
        public string RozhodnutiDotacePoskytovatelNazev { get; set; }
        [Nest.Keyword]
        public string IdObdobi { get; set; }
        [Nest.Number]
        public float? ObdobiCastkaCerpana { get; set; }
        [Nest.Number]
        public float? ObdobiCastkaUvolnena { get; set; }
        [Nest.Number]
        public float? ObdobiCastkaVracena { get; set; }
        [Nest.Number]
        public float? ObdobiCastkaSpotrebovana { get; set; }
        [Nest.Number]
        public int? ObdobiRozpoctoveObdobi { get; set; }
        [Nest.Keyword]
        public string ObdobiIriDotacniTitul { get; set; }
        [Nest.Keyword]
        public string ObdobiIriUcelovyZnak { get; set; }
        [Nest.Date]
        public DateTime? ObdobiDPlatnost { get; set; }
        [Nest.Date]
        public DateTime? ObdobiDTAktualizace { get; set; }
        [Nest.Keyword]
        public string ObdobiDotaceTitulKod { get; set; }
        [Nest.Text]
        public string ObdobiDotaceTitulNazev { get; set; }
        [Nest.Keyword]
        public string ObdobiUcelZnakKod { get; set; }
        [Nest.Text]
        public string ObdobiUcelZnakNazev { get; set; }

        public void Save()
        {
            var res = ES.Manager.GetESClient_Dotace().Index<Dotace>(this, o => o.Id(this.IdRozhodnuti)); //druhy parametr musi byt pole, ktere je unikatni
            if (!res.IsValid)
            {
                throw new ApplicationException(res.ServerError?.ToString());
            }
        }
    }
}
