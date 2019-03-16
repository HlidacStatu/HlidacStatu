using System;
using Newtonsoft.Json;
using SSLLabsApiWrapper.Models;
using SSLLabsApiWrapper.Models.Response;
using SSLLabsApiWrapper.Models.Response.BaseSubModels;

namespace SSLLabsApiWrapper.Domain
{
	class ResponsePopulation
	{
		public JsonSerializerSettings JsonSerializerSettings;

		public ResponsePopulation()
		{
			// Ignoring null values when serializing json objects
			JsonSerializerSettings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
		}

		public Info InfoModel(WebResponseModel webResponse, Info infoModel)
		{
			if (webResponse.Payloay == null) { throw new Exception("webResponse payload is null. Unable to bind null."); }

			infoModel = JsonConvert.DeserializeObject<Info>(webResponse.Payloay, JsonSerializerSettings);
			infoModel.Header = PopulateHeader(infoModel.Header, webResponse);
			infoModel.Wrapper = PopulateWrapper(infoModel.Wrapper, webResponse);

			return infoModel;
		}

		public Analyze AnalyzeModel(WebResponseModel webResponse, Analyze analyzeModel)
		{
			analyzeModel = JsonConvert.DeserializeObject<Analyze>(webResponse.Payloay, JsonSerializerSettings);
			analyzeModel.Header = PopulateHeader(analyzeModel.Header, webResponse);
			analyzeModel.Wrapper = PopulateWrapper(analyzeModel.Wrapper, webResponse);

			if (analyzeModel.status == "ERROR") { analyzeModel.Errors.Add(new Error() { message = analyzeModel.statusMessage }); }

			return analyzeModel;
		}

		public Endpoint EndpointModel(WebResponseModel webResponse, Endpoint endpointModel)
		{
			endpointModel = JsonConvert.DeserializeObject<Endpoint>(webResponse.Payloay, JsonSerializerSettings);
			endpointModel.Header = PopulateHeader(endpointModel.Header, webResponse);
			endpointModel.Wrapper = PopulateWrapper(endpointModel.Wrapper, webResponse);

			return endpointModel;
		}

		public StatusCodes StatusCodesModel(WebResponseModel webResponse, StatusCodes statusCodes)
		{
			statusCodes = JsonConvert.DeserializeObject<StatusCodes>(webResponse.Payloay, JsonSerializerSettings);
			statusCodes.Header = PopulateHeader(statusCodes.Header, webResponse);
			statusCodes.Wrapper = PopulateWrapper(statusCodes.Wrapper, webResponse);

			return statusCodes;
		}

		public Header PopulateHeader(Header header, WebResponseModel webResponse)
		{
			header.statusCode = webResponse.StatusCode;
			header.statusDescription = webResponse.StatusDescription;

			return header;
		}

		public Wrapper PopulateWrapper(Wrapper wrapper, WebResponseModel webResponse)
		{
			wrapper.ApiCommandUrl = webResponse.Url;
			wrapper.ApiRawResponse = webResponse.Payloay;

			return wrapper;
		}
	}
}