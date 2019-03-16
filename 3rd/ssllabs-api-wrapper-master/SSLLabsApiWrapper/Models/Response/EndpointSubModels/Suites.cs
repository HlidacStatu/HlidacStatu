using System.Collections.Generic;

namespace SSLLabsApiWrapper.Models.Response.EndpointSubModels
{
	public class Suites
	{
		public List<List> list { get; set; }
		public bool preference { get; set; }

		public Suites()
		{
			list = new List<List>();
		}
	}
}
