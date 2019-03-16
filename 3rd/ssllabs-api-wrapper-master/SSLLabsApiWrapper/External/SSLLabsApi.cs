using System.IO;
using System.Linq;
using System.Net;
using SSLLabsApiWrapper.Interfaces;
using SSLLabsApiWrapper.Models;

namespace SSLLabsApiWrapper.External
{
	class SSLLabsApi : IApiProvider
	{
		public WebResponseModel MakeGetRequest(RequestModel requestModel)
		{
			var url = requestModel.ApiBaseUrl + "/" + requestModel.Action;

			if (requestModel.Parameters.Count >= 1)
			{
				url = string.Format("{0}{1}{2}", url, "?", string.Join("&", (from parameter in requestModel.Parameters 
									  where parameter.Value != null select string.Format("{0}={1}", parameter.Key, parameter.Value))));
			}

			var webResponseModel = new WebResponseModel() {Url = url};

			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "GET";

			var response = (HttpWebResponse)request.GetResponse();
			var streamReader = new StreamReader(response.GetResponseStream());

			webResponseModel.Payloay = streamReader.ReadToEnd();
			webResponseModel.StatusCode = (int)response.StatusCode;
			webResponseModel.StatusDescription = response.StatusDescription;
			webResponseModel.Url = url;

			return webResponseModel;
		}
	}
}
