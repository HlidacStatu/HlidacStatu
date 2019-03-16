namespace SSLLabsApiWrapper.Models.Response.EndpointSubModels
{
	public class Result
	{
		public Client client { get; set; }
		public int errorCode { get; set; }
		public int attempts { get; set; }
		public int? protocolId { get; set; }
		public int? suiteId { get; set; }

		public Result()
		{
			client = new Client();
		}
	}
}
