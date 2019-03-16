using System.Collections.Generic;
using SSLLabsApiWrapper.Models.Response.BaseSubModels;

namespace SSLLabsApiWrapper.Interfaces
{
	public interface IBaseResponse
	{
		Header Header { get; set; }
		bool HasErrorOccurred { get; set; }
		List<Error> Errors { get; set; }
		Wrapper Wrapper { get; set; }
	}
}