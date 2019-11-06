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
        [Nest.Object]
        public List<Rozhodnuti> Rozhodnuti { get; set; }

        // calculated fields
        [Nest.Number]
        public float DotaceCelkem { get; set; }
        [Nest.Number]
        public float PujckaCelkem { get; set; }
        
        public string GetNazevDotace()
        {
            if (!string.IsNullOrWhiteSpace(DotaceProjektNazev))
            {
                return DotaceProjektNazev;
            }
            if (!string.IsNullOrWhiteSpace(DotaceProjektIdentifikator))
            {
                return DotaceProjektIdentifikator;
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
            DotaceCelkem = Rozhodnuti.Where(p => p.RozhodnutiRefundaceIndikator == false)
                .Sum(p => p.RozhodnutiCastkaRozhodnuta);
            PujckaCelkem = Rozhodnuti
                .Where(p => p.RozhodnutiRefundaceIndikator == false && p.RozhodnutiNavratnostIndikator)
                .Sum(p => p.RozhodnutiCastkaRozhodnuta);
        }

        public Dictionary<string,string> GetDetail()
        {
            var result = new Dictionary<string, string>()
            {
                ["Jméno příjemce"] = PrijemceJmenoPrijemce,
                ["IČO"] = PrijemceIco,
                ["Kód projektu"] = DotaceProjektKod,
                ["Identifikátor projektu"] = DotaceProjektIdentifikator,
                ["Název projektu"] = DotaceProjektNazev,
                ["Datum podpisu"] = DotacePodpisDatum?.ToString("dd.MM.yyyy"),
                ["Kód operačního programu"] = DotaceOperacniProgramKod,
                ["Název operačního programu"] = DotaceOperacniProgramNazev,
                ["Kód grantového schematu"] = DotaceGrantoveSchemaKod,
                ["Název grantového schematu"] = DotaceGrantoveSchemaNazev

            };

            return result;
        }


    }
}
