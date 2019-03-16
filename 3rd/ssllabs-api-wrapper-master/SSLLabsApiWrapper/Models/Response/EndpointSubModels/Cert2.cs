namespace SSLLabsApiWrapper.Models.Response.EndpointSubModels
{
	public class Cert2
	{
		public string subject { get; set; }
		public string label { get; set; }
		public object notBefore { get; set; }
		public object notAfter { get; set; }
		public string issuerSubject { get; set; }
		public string issuerLabel { get; set; }
		public string sigAlg { get; set; }
		public int issues { get; set; }
		public string keyAlg { get; set; }
		public int keySize { get; set; }
		public int keyStrength { get; set; }
		public int revocationStatus { get; set; }
		public int crlRevocationStatus { get; set; }
		public int ocspRevocationStatus { get; set; }
		public string raw { get; set; }
	}
}
