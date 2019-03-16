using System.Collections.Generic;

namespace SSLLabsApiWrapper.Models.Response.EndpointSubModels
{
	public class Cert
	{
		public string subject { get; set; }
		public List<string> commonNames { get; set; }
		public List<string> altNames { get; set; }
		public long notBefore { get; set; }
		public long notAfter { get; set; }
		public string issuerSubject { get; set; }
		public string sigAlg { get; set; }
		public string issuerLabel { get; set; }
		public int revocationInfo { get; set; }
		public List<string> crlURIs { get; set; }
		public List<string> ocspURIs { get; set; }
		public int revocationStatus { get; set; }
		public int crlRevocationStatus { get; set; }
		public int ocspRevocationStatus { get; set; }
		public int sgc { get; set; }
		public string validationType { get; set; }
		public int issues { get; set; }
		public bool sct { get; set; }
	}
}
