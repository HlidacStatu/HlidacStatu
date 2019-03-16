using SSLLabsApiWrapper.Models.Response.EndpointSubModels;

namespace SSLLabsApiWrapper.Models.Response
{
	public class Endpoint : BaseModel
	{
		public string ipAddress { get; set; }
		public string serverName { get; set; }
		public string statusMessage { get; set; }
		public string statusDetails { get; set; }
		public string statusDetailsMessage { get; set; }
		public int progress { get; set; }
		public int eta { get; set; }
		public int delegation { get; set; }

		// Two groups of poperities can be returned. Just seperating them out for my own reference.
		public int duration { get; set; }
		public string grade { get; set; }
		public string gradeTrustIgnored { get; set; }
		public bool hasWarnings { get; set; }
		public bool isExceptional { get; set; }

		public Details Details { get; set; }

		public Endpoint()
		{
			Details = new Details();
		}

	}
}
