using SSLLabsApiWrapper.Models;

namespace SSLLabsApiWrapper.Domain
{
	class RequestModelFactory
	{
		public RequestModel NewInfoRequestModel(string apiBaseUrl, string action)
		{
			return new RequestModel() {ApiBaseUrl = apiBaseUrl, Action = action};
		}

		public RequestModel NewAnalyzeRequestModel(string apiBaseUrl, string action, string host, string publish, string startNew,
			string fromCache, int? maxHours, string all, string ignoreMismatch)
		{
			var requestModel = new RequestModel() { ApiBaseUrl = apiBaseUrl, Action = action};

			requestModel.Parameters.Add("host", host);
			requestModel.Parameters.Add("publish", publish);
			
			if (all.ToLower() != "ignore") { requestModel.Parameters.Add("all", all); }
			if (startNew.ToLower() != "ignore") { requestModel.Parameters.Add("startNew", startNew); }
			if (fromCache.ToLower() != "ignore") { requestModel.Parameters.Add("fromCache", fromCache); }
			if (maxHours != null) { requestModel.Parameters.Add("maxHours", maxHours.ToString()); }
			if (ignoreMismatch.ToLower() != "off") { requestModel.Parameters.Add("ignoreMismatch", ignoreMismatch); }

			return requestModel;
		}

		public RequestModel NewEndpointDataRequestModel(string apiBaseUrl, string action, string host, string s, string fromCache)
		{
			var requestModel = new RequestModel() {ApiBaseUrl = apiBaseUrl, Action = action};

			requestModel.Parameters.Add("host", host);
			requestModel.Parameters.Add("s", s);
			requestModel.Parameters.Add("fromCache", fromCache);

			return requestModel;
		}

		public RequestModel NewStatusCodesRequestModel(string apiBaseUrl, string action)
		{
			return new RequestModel() {ApiBaseUrl = apiBaseUrl, Action = action};
		}
	}
}