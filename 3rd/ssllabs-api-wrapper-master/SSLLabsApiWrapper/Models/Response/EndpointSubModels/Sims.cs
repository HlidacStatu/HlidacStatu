using System.Collections.Generic;

namespace SSLLabsApiWrapper.Models.Response.EndpointSubModels
{
	public class Sims
	{
		public List<Result> results { get; set; }

		public Sims()
		{
			results = new List<Result>();
		}
	}
}
