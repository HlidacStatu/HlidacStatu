using System;
using System.Collections.Generic;

namespace HlidacStatu.Lib.Data.Insolvence
{
	public class Rizeni
        : Bookmark.IBookmarkable
    {
		public Rizeni()
		{
			Dokumenty = new List<Dokument>();
			Dluznici = new List<Osoba>();
			Veritele = new List<Osoba>();
			Spravci = new List<Osoba>();
		}

		[Nest.Keyword]
		public string SpisovaZnacka { get; set; }
		[Nest.Keyword]
		public string Stav { get; set; }
		[Nest.Date]
		public DateTime? Vyskrtnuto { get; set; }
		[Nest.Keyword]
		public string Url { get; set; }
		[Nest.Date]
		public DateTime? DatumZalozeni { get; set; }
		[Nest.Date]
		public DateTime PosledniZmena { get; set; }
		[Nest.Keyword]
		public string Soud { get; set; }
		[Nest.Object]
		public List<Dokument> Dokumenty { get; set; }
		[Nest.Object]
		public List<Osoba> Dluznici { get; set; }
		[Nest.Object]
		public List<Osoba> Veritele { get; set; }
		[Nest.Object]
		public List<Osoba> Spravci { get; set; }

		public string UrlId() => SpisovaZnacka.Replace(" ", "_").Replace("/", "-");




        public HlidacStatu.Lib.OCR.Api.CallbackData CallbackDataForOCRReq(int prilohaindex)
        {
            var url = Devmasters.Core.Util.Config.GetConfigValue("ESConnection");

                url = url + $"/{Lib.ES.Manager.defaultIndexName_Insolvence}/rizeni/{System.Net.WebUtility.UrlEncode(this.SpisovaZnacka)}/_update";

            string callback = HlidacStatu.Lib.OCR.Api.CallbackData.PrepareElasticCallbackDataForOCRReq($"dokumenty[{prilohaindex}].plainText", true);
            callback = callback.Replace("#ADDMORE#", $"ctx._source.dokumenty[{prilohaindex}].lastUpdate = '#NOW#';"
                + $"ctx._source.dokumenty[{prilohaindex}].lenght = #LENGTH#;"
                + $"ctx._source.dokumenty[{prilohaindex}].wordCount=#WORDCOUNT#;");

            return new HlidacStatu.Lib.OCR.Api.CallbackData(new Uri(url), callback, HlidacStatu.Lib.OCR.Api.CallbackData.CallbackType.LocalElastic);
        }

        public string GetUrl(bool local)
        {
            string url= $"/Rizeni/{this.UrlId()}";
            if (local == false)
                url = "https://www.hlidacstatu.cz" + url;

            return url;
        }

        public string BookmarkName()
        {
            return "Insolvence " + this.SpisovaZnacka;
        }

        public string ToAuditJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string ToAuditObjectTypeName()
        {
            return "Insolvence";
        }

        public string ToAuditObjectId()
        {
            return this.SpisovaZnacka;
        }
    }
}
