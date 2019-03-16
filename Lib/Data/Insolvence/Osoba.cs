using Devmasters.Core;
using System;

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
		[Nest.Keyword]
		public string ICO { get; set; }
		[Nest.Keyword]
		public string Rc { get; set; }
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
            if (!string.IsNullOrEmpty(ICO) && Validators.CheckCZICO(ICO))
            {
                return $"<a href='/subjekt/{ICO}'>{TextUtil.ShortenText(PlneJmeno,30,"...")}</a> ({ICO})";
            }
            else if (!string.IsNullOrEmpty(OsobaId))
            {
                return $"<a href='/osoba/{OsobaId}'>{TextUtil.ShortenText(PlneJmeno, 30, "...")}</a>{GetDatumNarozeni()}";
            }
            return PlneJmeno + GetDatumNarozeni();
        }

		private string GetDatumNarozeni()
		{
			if (DatumNarozeni.HasValue)
			{
				return $" (* {DatumNarozeni.Value.Year})";
			}
			else if (!string.IsNullOrEmpty(Rc))
			{
				var suffix = Rc.Substring(0, 2);
				var year = (Convert.ToInt32((DateTime.Now.Year - 18).ToString().Substring(2)) > Convert.ToInt32(suffix) ? "20" : "19") + suffix;
				return $" (* {year})";
			}
			return string.Empty;
		}
    }
}
