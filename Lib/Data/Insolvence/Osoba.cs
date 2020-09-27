using Devmasters;
using System;
using System.Linq;

namespace HlidacStatu.Lib.Data.Insolvence
{
	public class Osoba
	{
		[Nest.Keyword]
		public string IdPuvodce { get; set; }
		[Nest.Keyword]
		public string IdOsoby { get; set; }
		[Nest.Text]
		public string PlneJmeno { get; set; }

        [Nest.Keyword]
		public string Role { get; set; }
		[Nest.Keyword]
		public string Typ { get; set; }
        /*
         "F","doc_count": 188652
         "PODNIKATEL","doc_count": 62398
        "P","doc_count": 35596
        "ADVOKÁT","doc_count": 19
        "OST_OVM","doc_count": 9
         "EXEKUTOR","doc_count": 4
        "PAT_ZAST","doc_count": 3
        "DAN_PORAD", "doc_count": 1
        "SPRÁV_INS","doc_count": 1
        "U","doc_count": 1
             */


        string _jmeno = null;
        public string Jmeno()
        {
            if (_jmeno == null)
            {
                var foundO = Validators.JmenoInText(this.PlneJmeno);
                _jmeno = foundO?.Jmeno ?? this.PlneJmeno;
                _prijmeni = foundO?.Prijmeni ?? this.PlneJmeno;
            }
            return _jmeno;
        }
        string _prijmeni = null;
        public string Prijmeni()
        {
            if (_prijmeni == null)
            {
                var foundO = Validators.JmenoInText(this.PlneJmeno);
                _jmeno = foundO?.Jmeno ?? this.PlneJmeno;
                _prijmeni = foundO?.Prijmeni ?? this.PlneJmeno;
            }
            return _prijmeni;
        }


        [Nest.Keyword]
		public string ICO { get; set; }
		[Nest.Keyword]
		public string Rc { get; set; }

        [Nest.Date]
        public DateTime? Zalozen { get; set; }
        [Nest.Date]
        public DateTime? Odstranen { get; set; }


        [Nest.Date]
		public DateTime? DatumNarozeni { get; set; }
		[Nest.Keyword]
		public string Mesto { get; set; }
		[Nest.Keyword]
		public string Okres { get; set; }
		[Nest.Keyword]
		public string Zeme { get; set; }
		[Nest.Keyword]
		public string Psc { get; set; }

        [Nest.Keyword]
        public string OsobaId { get; set; }

        public string ToHtml()
        {
            string dn = GetDatumNarozeni() != null ? $" (*{GetDatumNarozeni()?.Year})" : "";
            if (!string.IsNullOrEmpty(ICO) && Util.DataValidators.CheckCZICO(ICO))
            {
                return $"<a href='/subjekt/{ICO}'>{TextUtil.ShortenText(PlneJmeno, 30, "...")}</a> IČ:{ICO}";
            }
            else if (!string.IsNullOrEmpty(OsobaId))
            {
                return $"<a href='/osoba/{OsobaId}'>{TextUtil.ShortenText(PlneJmeno, 30, "...")}</a>{dn}";
            }
            return $"{PlneJmeno}{dn}";
        }

        

		public DateTime? GetDatumNarozeni()
		{
			if (DatumNarozeni.HasValue)
			{
				return DatumNarozeni;
			}
			else if (!string.IsNullOrEmpty(Rc))
			{
                return Devmasters.DT.Util.RodneCisloToDate(Rc);
            }
            return null;
		}

        public string FullNameWithYear()
        {
            return PlneJmeno + GetDatumNarozeni()?.Year ?? "";
        }
    }
}
