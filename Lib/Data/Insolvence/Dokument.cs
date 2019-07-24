using System;

namespace HlidacStatu.Lib.Data.Insolvence
{
	public class Dokument
	{
		[Nest.Keyword]
        public string Id { get; set; }

        //ciselnik typu udalsoti https://isir.justice.cz/isir/help/Cis_udalosti.xls
        [Nest.Number]
		public int TypUdalosti { get; set; }
		[Nest.Date]
		public DateTime DatumVlozeni { get; set; }
		[Nest.Text]
		public string Popis { get; set; }
		[Nest.Keyword]
		public string Url { get; set; }
		[Nest.Keyword]
		public string Oddil { get; set; }
		[Nest.Text]
		public string PlainText { get; set; }

        [Nest.Number]
        public long Lenght { get; set; }

        [Nest.Number]
        public long WordCount { get; set; }

        [Nest.Date]
        public DateTime? LastUpdate { get; set; }


        [Nest.Object(Ignore = true)]
        public bool EnoughExtractedText
        {
            get
            {
                return !(this.Lenght <= 20 || this.WordCount <= 10);
            }
        }

    }

    public class DokumentSeSpisovouZnackou
	{
		public Dokument Dokument { get; set; }
		public string SpisovaZnacka { get; set; }
		public string UrlId { get; set; }
        public Rizeni Rizeni { get; set; }
    }
}
