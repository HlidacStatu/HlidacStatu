namespace HlidacStatu.Lib.ES
{
	public class InsolvenceSearchResult : SearchData<Data.Insolvence.Rizeni>
	{
		public Nest.ISearchResponse<Data.Insolvence.Rizeni> Result
		{
			get
			{
				return ElasticResults;
			}
			set
			{
				ElasticResults = value;
			}
		}


		public object ToRouteValues(int page)
		{
			return new
			{
				Q = string.IsNullOrEmpty(Q) ? OrigQuery : Q,
				Page = page,
			};
		}

		public bool ShowWatchdog { get; set; } = true;

	}
}
