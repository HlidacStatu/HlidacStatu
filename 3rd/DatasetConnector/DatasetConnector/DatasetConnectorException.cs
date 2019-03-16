using System;

namespace HlidacStatu.Api.Dataset.Connector
{
	public class DatasetConnectorException : Exception {
		public DatasetConnectorException(string message)
			: base(message)
		{

		}
	}
}
