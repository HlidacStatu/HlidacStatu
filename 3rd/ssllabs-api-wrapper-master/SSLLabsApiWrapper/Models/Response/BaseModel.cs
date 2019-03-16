using System.Collections.Generic;
using SSLLabsApiWrapper.Interfaces;
using SSLLabsApiWrapper.Models.Response.BaseSubModels;

namespace SSLLabsApiWrapper.Models.Response
{
	public class BaseModel : IBaseResponse
	{
		public Header Header { get; set; }
		public bool HasErrorOccurred { get; set; }
		public List<Error> Errors { get; set; }
		public Wrapper Wrapper { get; set; }

		public BaseModel()
		{
			Header = new Header();
			Errors = new List<Error>();
			Wrapper = new Wrapper();
			this.HasErrorOccurred = false;
		}
	}
}
