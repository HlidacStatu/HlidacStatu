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
            var ret = string.Empty;
            if (!string.IsNullOrEmpty(this.ICO) && HlidacStatu.Lib.Validators.CheckCZICO(this.ICO))
            {
                ret = $"<a href='/subjekt/{this.ICO}'>{Devmasters.Core.TextUtil.ShortenText(this.PlneJmeno,30,"...")}</a>";
                ret += $" ({this.ICO})";
            }
            else if (!string.IsNullOrEmpty(this.OsobaId))
            {
                ret =  $"<a href='/osoba/{this.OsobaId}'>{Devmasters.Core.TextUtil.ShortenText(this.PlneJmeno, 30, "...")}</a>";
                if (this.DatumNarozeni.HasValue)
                    ret += $" (* {this.DatumNarozeni.Value.Year})";
            }
            else 
            {
                ret = this.PlneJmeno;
                if (this.DatumNarozeni.HasValue)
                    ret += $" (* {this.DatumNarozeni.Value.Year})";
            }
            return ret;
        }
    }
}
