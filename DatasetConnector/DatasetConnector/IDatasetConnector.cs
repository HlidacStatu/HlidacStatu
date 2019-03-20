using System.Threading.Tasks;

namespace HlidacStatu.Api.Dataset.Connector
{
	public interface IDatasetConnector
	{
		Task<string> AddItemToDataset<TData>(Dataset<TData> dataset, TData item) where TData : IDatasetItem;
        Task<string> AddItemToDataset<TData>(string datasetId, TData item) where TData : IDatasetItem;

        Task<TData> GetItemFromDataset<TData>(string datasetId, string id) where TData : IDatasetItem;
        Task<SearchResult<TData>> SearchItemsInDataset<TData>(string datasetId, string query, int page) where TData : IDatasetItem;

        Task<bool> DatasetExists<TData>(Dataset<TData> dataset) where TData : IDatasetItem;
		Task DeleteDataset<TData>(Dataset<TData> dataset) where TData : IDatasetItem;
		Task<string> CreateDataset<TData>(Dataset<TData> dataset) where TData : IDatasetItem;
		Task<string> UpdateDataset<TData>(Dataset<TData> dataset) where TData : IDatasetItem;


	}
}