namespace SSLLabsApiWrapper.Models.Response.EndpointSubModels
{
	public class Key
	{
		public int size { get; set; }
		public string alg { get; set; }
		public bool debianFlaw { get; set; } //**Deprecated - To be removed in new release 
		public int strength { get; set; }
	}
}
