using System.Collections.Generic;

namespace SSLLabsApiWrapper.Models.Response.EndpointSubModels
{
	public class Chain
	{
		public List<Cert2> certs { get; set; }
		public int issues { get; set; }

		public Chain()
		{
			certs = new List<Cert2>();
		}
	}
}
