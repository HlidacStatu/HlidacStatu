using System.Collections.Generic;

namespace SSLLabsApiWrapper.Models.Response
{
	public class Info : BaseModel
	{
		public string engineVersion { get; set; }
		public string criteriaVersion { get; set; }
		public int clientMaxAssessments { get; set; }
		public int currentAssessments { get; set; }
		public List<string> messages { get; set; }
		public bool Online { get; set; }

		public Info()
		{
			// Assigning default online status
			this.Online = false;
		}
	}
}
