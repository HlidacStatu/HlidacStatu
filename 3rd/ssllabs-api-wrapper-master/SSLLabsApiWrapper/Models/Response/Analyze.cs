using System.Collections.Generic;

namespace SSLLabsApiWrapper.Models.Response
{
	public class Analyze : BaseModel
	{
		public string host { get; set; }
		public int port { get; set; }
		public string protocol { get; set; }
		public bool isPublic { get; set; }
		public string status { get; set; }
		public string statusMessage { get; set; }
		public long startTime { get; set; }
		public long testTime { get; set; }
		public string engineVersion { get; set; }
		public string criteriaVersion { get; set; }
		public List<Endpoint> endpoints { get; set; }
	}
}
