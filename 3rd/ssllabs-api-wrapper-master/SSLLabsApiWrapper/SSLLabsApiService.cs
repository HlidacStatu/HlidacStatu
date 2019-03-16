using System;
using System.Threading;
using SSLLabsApiWrapper.Domain;
using SSLLabsApiWrapper.External;
using SSLLabsApiWrapper.Interfaces;
using SSLLabsApiWrapper.Models.Response;
using SSLLabsApiWrapper.Models.Response.BaseSubModels;

namespace SSLLabsApiWrapper
{
	public class SSLLabsApiService
	{
		#region construction

		private readonly IApiProvider _apiProvider;
		private readonly RequestModelFactory _requestModelFactory;
		private readonly ResponsePopulation _responsePopulation;
		private readonly UrlValidation _urlValidation;
		private string ApiUrl { get; set; }

		public enum Publish
		{
			On,
			Off
		}

		public enum StartNew
		{
			On,
			Ignore
		}

		public enum FromCache
		{
			On,
			Off,
			Ignore
		}

		public enum All
		{
			On,
			Done,
			Ignore
		}

		public enum IgnoreMismatch
		{
			On,
			Off
		}

		public SSLLabsApiService(string apiUrl)
			: this(apiUrl, new SSLLabsApi())
		{
		}

		internal SSLLabsApiService(string apiUrl, IApiProvider apiProvider)
		{
			_apiProvider = apiProvider;
			_requestModelFactory = new RequestModelFactory();
			_urlValidation = new UrlValidation();
			_responsePopulation = new ResponsePopulation();
			ApiUrl = _urlValidation.Format(apiUrl);
		}

		#endregion

		public Info Info()
		{
			var infoModel = new Info() { };
			var requestModel = _requestModelFactory.NewInfoRequestModel(ApiUrl, "info");

			try
			{
				// Making Api request and binding result to model
				var webResponse = _apiProvider.MakeGetRequest(requestModel);
				infoModel = _responsePopulation.InfoModel(webResponse, infoModel);

				if (infoModel.engineVersion != null)
				{
					infoModel.Online = true;
				}
			}
			catch (Exception ex)
			{
				infoModel.HasErrorOccurred = true;
				infoModel.Errors.Add(new Error { message = ex.ToString() });
			}

			if (infoModel.Errors.Count != 0 && !infoModel.HasErrorOccurred) { infoModel.HasErrorOccurred = true; }

			return infoModel;
		}

		public Analyze Analyze(string host)
		{
			// overloaded method to provide a default set of options
			return Analyze(host, Publish.Off, StartNew.On, FromCache.Ignore, null, All.On, IgnoreMismatch.Off);
		}

		public Analyze Analyze(string host, Publish publish, StartNew startNew, FromCache fromCache, int? maxHours, All all, IgnoreMismatch ignoreMismatch)
		{
			var analyzeModel = new Analyze();

			// Checking host is valid before continuing
			if (!_urlValidation.IsValid(host))
			{
				analyzeModel.HasErrorOccurred = true;
				analyzeModel.Errors.Add(new Error { message = "Host does not pass preflight validation. No Api call has been made." });
				return analyzeModel;
			}

			// Building request model
			var requestModel = _requestModelFactory.NewAnalyzeRequestModel(ApiUrl, "analyze", host, publish.ToString().ToLower(), startNew.ToString().ToLower(),
				fromCache.ToString().ToLower(), maxHours, all.ToString().ToLower(), ignoreMismatch.ToString().ToLower());

			try
			{
				var webResponse = _apiProvider.MakeGetRequest(requestModel);
				analyzeModel = _responsePopulation.AnalyzeModel(webResponse, analyzeModel);
			}
			catch (Exception ex)
			{
				analyzeModel.HasErrorOccurred = true;
				analyzeModel.Errors.Add(new Error { message = ex.ToString() });
			}

			// Checking if errors have occoured either from ethier api or wrapper
			if (analyzeModel.Errors.Count != 0 && !analyzeModel.HasErrorOccurred) { analyzeModel.HasErrorOccurred = true; }

			return analyzeModel;
		}

		public Analyze AutomaticAnalyze(string host)
		{
			return AutomaticAnalyze(host, 300, 15);
		}

		public Analyze AutomaticAnalyze(string host, int maxWaitInterval, int sleepInterval)
		{
			return AutomaticAnalyze(host, Publish.Off, StartNew.On, FromCache.Ignore, null, All.On, IgnoreMismatch.Off, maxWaitInterval, sleepInterval);
		}

		public Analyze AutomaticAnalyze(string host, Publish publish, StartNew startNew, FromCache fromCache, int? maxHours, All all, IgnoreMismatch ignoreMismatch,
			int maxWaitInterval, int sleepInterval)
		{
			var startTime = DateTime.UtcNow;
			var sleepIntervalMilliseconds = sleepInterval * 1000;
			var apiPassCount = 1;
			var analyzeModel = Analyze(host, publish, startNew, fromCache, maxHours, all, ignoreMismatch);

			// Ignoring cache settings after first request to prevent loop
			startNew = StartNew.Ignore;

			// Shouldn't have to check status header as HasErrorOccurred should be enough
			while (analyzeModel.HasErrorOccurred == false && analyzeModel.status != "READY" && (DateTime.UtcNow - startTime).TotalSeconds < maxWaitInterval)
			{
				Thread.Sleep(sleepIntervalMilliseconds);
				apiPassCount++;
				analyzeModel = Analyze(host, publish, startNew, fromCache, null, all, ignoreMismatch);
			}

			analyzeModel.Wrapper.ApiPassCount = apiPassCount;

			return analyzeModel;
		}

		public Endpoint GetEndpointData(string host, string s)
		{
			return GetEndpointData(host, s, FromCache.Off);
		}

		public Endpoint GetEndpointData(string host, string s, FromCache fromCache)
		{
			var endpointModel = new Endpoint();

			// Checking host is valid before continuing
			if (!_urlValidation.IsValid(host))
			{
				endpointModel.HasErrorOccurred = true;
				endpointModel.Errors.Add(new Error { message = "Host does not pass preflight validation. No Api call has been made." });
				return endpointModel;
			}

			// Building request model
			var requestModel = _requestModelFactory.NewEndpointDataRequestModel(ApiUrl, "getEndpointData", host, s,
				fromCache.ToString());

			try
			{
				var webResponse = _apiProvider.MakeGetRequest(requestModel);
				endpointModel = _responsePopulation.EndpointModel(webResponse, endpointModel);
			}
			catch (Exception ex)
			{
				endpointModel.HasErrorOccurred = true;
				endpointModel.Errors.Add(new Error { message = ex.ToString() });
			}

			// Checking if errors have occoured either from ethier api or wrapper
			if (endpointModel.Errors.Count != 0 && !endpointModel.HasErrorOccurred) { endpointModel.HasErrorOccurred = true; }

			return endpointModel;
		}

		public StatusCodes GetStatusCodes()
		{
			var statusCodesModel = new StatusCodes();
			var requestModel = _requestModelFactory.NewStatusCodesRequestModel(ApiUrl, "getStatusCodes");

			try
			{
				var webResponse = _apiProvider.MakeGetRequest(requestModel);
				statusCodesModel = _responsePopulation.StatusCodesModel(webResponse, statusCodesModel);
			}
			catch (Exception ex)
			{
				statusCodesModel.HasErrorOccurred = true;
				statusCodesModel.Errors.Add(new Error { message = ex.ToString() });
			}

			return statusCodesModel;
		}
	}
}
