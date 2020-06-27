using Nest;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace HlidacStatu.Lib.Data.Dotace
{
    [ElasticsearchType(IdProperty = nameof(IdDotace))]
    public partial class Dotace : Bookmark.IBookmarkable, IFlattenedExport
    {
        [Nest.Keyword]
        public string IdDotace { get; set; }
        [Nest.Date]
        public DateTime? DatumPodpisu { get; set; }
        [Nest.Date]
        public DateTime? DatumAktualizace { get; set; }
        [Nest.Keyword]
        public string KodProjektu { get; set; }
        [Nest.Text]
        public string NazevProjektu { get; set; }
        [Nest.Object]
        public Zdroj Zdroj { get; set; }
        
        [Nest.Object]
        public Prijemce Prijemce { get; set; }

        [Nest.Object]
        public DotacniProgram Program { get; set; }
        // calculated fields
        // Celková výše dotace včetně půjčky
        [Nest.Number]
        public decimal? DotaceCelkem { get; set; }
        // Půjčené peníze
        [Nest.Number]
        public decimal? PujckaCelkem { get; set; }

        // Rozhodnutí je neuvěřitelné hausnumero a může se v něm skrývat cokoliv.
        // Například to může být hodnota, která odpovídá nákladům na realizování dotovaného projektu společnosti, 
        // kde dotace samotná může dosahovat třeba jen 40 % z rozhodnuté částky
        [Nest.Object]
        public List<Rozhodnuti> Rozhodnuti { get; set; }
        [Nest.Keyword]
        public string Duplicita { get; set; }

        [Nest.Text]
        public List<string> Chyba { get; set; }



        public string BookmarkName()
        {
            return this.NazevProjektu;
        }

        public string GetNazevDotace()
        {
            if (!string.IsNullOrWhiteSpace(NazevProjektu))
            {
                return NazevProjektu;
            }
            if (!string.IsNullOrWhiteSpace(KodProjektu))
            {
                return KodProjektu;
            }

            return "";
        }

        public string GetUrl(bool local = true)
        {
            return GetUrl(local, string.Empty);
        }

        public string GetUrl(bool local, string foundWithQuery)
        {
            string url = "/Dotace/Detail/" + this.IdDotace;
            if (!string.IsNullOrEmpty(foundWithQuery))
                url = url + "?qs=" + System.Net.WebUtility.UrlEncode(foundWithQuery);

            if (local == false)
                return "https://www.hlidacstatu.cz" + url;
            else
                return url;
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

        public string ToAuditJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string ToAuditObjectId()
        {
            return this.IdDotace;
        }

        public string ToAuditObjectTypeName()
        {
            return "Dotace";
        }


        public void CalculateTotals()
        {
            if (Rozhodnuti is null || Rozhodnuti.Count == 0)
            {
                DotaceCelkem = null;
                PujckaCelkem = null;
            }
            else
            {
                // we need to refresh values (in case something was changed)
                Rozhodnuti.ForEach(r => r.RecalculateCerpano());

                DotaceCelkem = Rozhodnuti.Sum(r => r.CerpanoCelkem ?? 0);
                PujckaCelkem = Rozhodnuti.Where(r => r.JePujcka.HasValue && r.JePujcka.Value).Sum(r => r.CerpanoCelkem ?? 0);
            }
         
        }

        public ExpandoObject FlatExport()
        {
            dynamic v = new System.Dynamic.ExpandoObject();
            v.Url = this.GetUrl(false);
            v.idDotace = this.IdDotace;
            v.prijmenceJmeno = this.Prijemce?.Jmeno;
            v.prijmenceIco = this.Prijemce?.Ico;
            v.prijmenceJmeno = this.Prijemce.Jmeno;
            v.prijmenceJmenoAktualni = this.Prijemce.HlidacJmeno;
            v.obec = this.Prijemce.Obec;
            v.nazevProjektu = this.GetNazevDotace();
            v.dotacniProgram = this.Program?.Kod ?? "neuveden";
            v.datumPodpisu = this.DatumPodpisu;
            v.dotace = this.DotaceCelkem;
            v.vratnaDotace = this.PujckaCelkem;

            return v;
        }
    }
}
