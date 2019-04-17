using HlidacStatu.Lib.Data.Insolvence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HlidacStatu.Web.Models
{

	public class InsolvenceIndexViewModel
	{
		public Rizeni[] NoveFirmyVInsolvenci { get; set; }
		public Rizeni[] NoveOsobyVInsolvenci { get; set; }

		public InsolvenceIndexViewModel()
		{
			NoveFirmyVInsolvenci = new Rizeni[0];
			NoveOsobyVInsolvenci = new Rizeni[0];
		}
	}

	public class OsobaViewModel
	{
		public Osoba Osoba { get; set; }
		public string SpisovaZnacka { get; set; }
		public string UrlId { get; set; }
		public bool DisplayLinkToRizeni { get; set; }
	}

	public class PeopleListViewModel
	{
		public IEnumerable<OsobaViewModel> Osoby { get; set; }
		public bool ShowAsDataTable { get; set; }
		public string Typ { get; set; }
	}

	public class DokumentListViewModel
	{
		public string Oddil { get; set; }
		public Dokument[] Dokumenty { get; set; }
        public Nest.HighlightFieldDictionary HighlightingData{ get; set; }
    }

	public class SoudViewModel
	{
		public string Soud { get; set; }
	}

	public class StavRizeniViewModel
	{
		public string Stav { get; set; }
	}

	public class DokumentyViewModel
	{
		public string SpisovaZnacka { get; set; }
		public string UrlId { get; set; }
		public Dokument[] Dokumenty { get; set; }
        public Nest.HighlightFieldDictionary HighlightingData { get; set; }

        public DokumentyViewModel()
		{
			Dokumenty = new Dokument[0];
		}
	}
}