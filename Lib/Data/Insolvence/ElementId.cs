namespace HlidacSmluv.Lib.Data.Insolvence
{
	public abstract class ElementId
	{
		[Nest.Keyword]
		public string SpisovaZnacka { get; set; }

		public string UrlId() => SpisovaZnacka.Replace(" ", "_").Replace("/", "-");
	}
}
