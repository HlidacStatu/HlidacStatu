using System.Net;
using SSLLabsApiWrapper.Models;

namespace SSLLabsApiWrapper.Interfaces
{
	interface IApiProvider
	{
		WebResponseModel MakeGetRequest(RequestModel requestModel);
	}
}
