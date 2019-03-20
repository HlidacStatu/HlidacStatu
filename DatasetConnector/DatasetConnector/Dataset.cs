using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace HlidacStatu.Api.Dataset.Connector
{
	public class Dataset<TData> where TData : IDatasetItem
	{


		public Dataset(string name, string datasetId, string origUrl, string description = "", string sourceCodeUrl = "", bool betaVersion = true, bool allowWriteAccess = false, string[,] orderList = null, Template searchResultTemplate = null, Template detailTemplate = null)
		{
			Name = name;
			DatasetId = datasetId;
			OrigUrl = origUrl;
			Description = description;
			SourceCodeUrl = sourceCodeUrl;
			BetaVersion = betaVersion;
			AllowWriteAccess = allowWriteAccess;
			OrderList = orderList;
			SearchResultTemplate = searchResultTemplate;
			DetailTemplate = detailTemplate;

			var jsonGen = new JSchemaGenerator
			{
				DefaultRequired = Required.Default
			};
			JsonSchema = jsonGen.Generate(typeof(TData));
		}

		public string Name { get; private set; }
		public string DatasetId { get; private set; }
		public string OrigUrl { get; private set; }
		public string Description { get; private set; }
		public string SourceCodeUrl { get; private set; }
		public JSchema JsonSchema { get; private set; }
		public bool BetaVersion { get; private set; }
		public bool AllowWriteAccess { get; private set; }
		public string[,] OrderList { get; private set; }
		public Template SearchResultTemplate { get; private set; }
		public Template DetailTemplate { get; private set; }
	}
}
