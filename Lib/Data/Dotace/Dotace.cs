using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data.Dotace
{
    [ElasticsearchType(IdProperty = nameof(IdDotace))]
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
        public string ProjektKod { get; set; }
        [Nest.Date]
        public DateTime? PodpisDatum { get; set; }
        [Nest.Keyword]
        public string SubjektRozliseniKod { get; set; }
        [Nest.Date]
        public DateTime? UkonceniPlanovaneDatum { get; set; }
        [Nest.Date]
        public DateTime? UkonceniSkutecneDatum { get; set; }
        [Nest.Date]
        public DateTime? ZahajeniPlanovaneDatum { get; set; }
        [Nest.Date]
        public DateTime? ZahajeniSkutecneDatum { get; set; }
        [Nest.Boolean]
        public bool ZmenaSmlouvyIndikator { get; set; }
        [Nest.Keyword]
        public string ProjektIdentifikator { get; set; }
        [Nest.Text]
        public string ProjektNazev { get; set; }
        [Nest.Keyword]
        public string IriOperacniProgram { get; set; }
        [Nest.Keyword]
        public string IriPodprogram { get; set; }
        [Nest.Keyword]
        public string IriPriorita { get; set; }
        [Nest.Keyword]
        public string IriOpatreni { get; set; }
        [Nest.Keyword]
        public string IriPodopatreni { get; set; }
        [Nest.Keyword]
        public string IriGrantoveSchema { get; set; }
        [Nest.Keyword]
        public string IriProgramPodpora { get; set; }
        [Nest.Keyword]
        public string IriTypCinnosti { get; set; }
        [Nest.Keyword]
        public string IriProgram { get; set; }
        [Nest.Date]
        public DateTime? DPlatnost { get; set; }
        [Nest.Date]
        public DateTime? DTAktualizace { get; set; }
        [Nest.Keyword]
        public string IdOperacniProgram { get; set; }
        [Nest.Keyword]
        public string OperacniProgramKod { get; set; }
        [Nest.Text]
        public string OperacniProgramNazev { get; set; }
        [Nest.Number]
        public int? OperacniProgramCislo { get; set; }
        [Nest.Date]
        public DateTime? OperacniProgramZaznamPlatnostOdDatum { get; set; }
        [Nest.Date]
        public DateTime? OperacniProgramZaznamPlatnostDoDatum { get; set; }
        [Nest.Keyword]
        public string IdGrantoveSchema { get; set; }
        [Nest.Keyword]
        public string GrantoveSchemaKod { get; set; }
        [Nest.Text]
        public string GrantoveSchemaNazev { get; set; }
        [Nest.Number]
        public int? GrantoveSchemaCislo { get; set; }
        [Nest.Date]
        public DateTime? GrantoveSchemaZaznamPlatnostOdDatum { get; set; }
        [Nest.Date]
        public DateTime? GrantoveSchemaZaznamPlatnostDoDatum { get; set; }
        [Nest.Object]
        public List<Rozhodnuti> Rozhodnuti { get; set; }

        // calculated fields
        [Nest.Number]
        public float DotaceCelkem { get; set; }
        [Nest.Number]
        public float PujckaCelkem { get; set; }
        
        public string GetNazevDotace()
        {
            if (!string.IsNullOrWhiteSpace(ProjektNazev))
            {
                return ProjektNazev;
            }
            if (!string.IsNullOrWhiteSpace(ProjektIdentifikator))
            {
                return ProjektIdentifikator;
            }

            return "";
        }

        public void Save()
        {
            //calculate fields before saving
            CalculateTotals();
            var res = ES.Manager.GetESClient_Dotace().Index<Dotace>(this, o => o.Id(this.IdDotace)); //druhy parametr musi byt pole, ktere je unikatni
            if (!res.IsValid)
            {
                throw new ApplicationException(res.ServerError?.ToString());
            }
        }

        private void CalculateTotals()
        {
            DotaceCelkem = Rozhodnuti.Where(p => p.RefundaceIndikator == false)
                .Sum(p => p.CastkaRozhodnuta);
            PujckaCelkem = Rozhodnuti
                .Where(p => p.RefundaceIndikator == false && p.NavratnostIndikator)
                .Sum(p => p.CastkaRozhodnuta);
        }

    }
}
