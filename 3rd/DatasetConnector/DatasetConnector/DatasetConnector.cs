using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HlidacStatu.Api.Dataset.Connector
{
    public class DatasetConnector : IDatasetConnector
    {
        private static string apiRoot = "https://www.hlidacstatu.cz/api/v1";

        private readonly string ApiToken;
        private readonly HttpClient HttpClient;

        public DatasetConnector(string apiToken)
        {
            ApiToken = apiToken;

            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("Authorization", ApiToken);
        }

        public async Task<string> AddItemToDataset<TData>(Dataset<TData> dataset, TData item)
            where TData : IDatasetItem
        {
            return await AddItemToDataset<TData>(dataset.DatasetId, item);
        }

        public async Task<string> AddItemToDataset<TData>(string datasetId, TData item)
			where TData : IDatasetItem
		{
			var content = new StringContent(JsonConvert.SerializeObject(item));
			var response = await HttpClient.PostAsync(apiRoot + $"/DatasetItem/{datasetId}/{item.Id}", content);
			var result = JObject.Parse(await response.Content.ReadAsStringAsync());
			if (result["error"] == null)
			{
				return result["id"].Value<string>();
			}
			else
			{
				throw new DatasetConnectorException(result["error"]["description"].Value<string>());
			}
		}
        public async Task<bool> DatasetExists<TData>(Dataset<TData> dataset)
			where TData : IDatasetItem
		{
			var response = await HttpClient.GetAsync(apiRoot + "/Datasets/" + dataset.DatasetId);
			var content = await response.Content.ReadAsStringAsync();

			return JContainer.Parse(content).HasValues;
		}

		public async Task<string> CreateDataset<TData>(Dataset<TData> dataset)
			where TData : IDatasetItem
		{
			if (await DatasetExists(dataset))
			{
				throw new DatasetConnectorException($"Dataset {dataset.DatasetId} already exists");
			}

			var content = new StringContent(JsonConvert.SerializeObject(dataset));
			var response = await HttpClient.PostAsync(apiRoot + "/Datasets", content);
			var result = JObject.Parse(await response.Content.ReadAsStringAsync());
			if (result["error"] == null)
			{
				return result["datasetId"].Value<string>();
			}
			else
			{
				throw new DatasetConnectorException(result["error"]["description"].ToString());
			}
		}

		public async Task<string> UpdateDataset<TData>(Dataset<TData> dataset)
			where TData : IDatasetItem
		{
			if (!await DatasetExists(dataset))
			{
				throw new DatasetConnectorException($"Dataset {dataset.DatasetId} not found");
			}

			var content = new StringContent(JsonConvert.SerializeObject(dataset));
			var response = await HttpClient.PutAsync(apiRoot + "/Datasets/" + dataset.DatasetId, content);
			var result = JObject.Parse(await response.Content.ReadAsStringAsync());

			if (!result["error"].HasValues)
			{
				return "Ok";
			}
			else
			{
				throw new DatasetConnectorException(result["error"]["description"].ToString());
			}
		}

		public async Task DeleteDataset<TData>(Dataset<TData> dataset)
			where TData : IDatasetItem
		{
			if (!await DatasetExists(dataset))
			{
				throw new DatasetConnectorException($"Dataset {dataset.DatasetId} not found");
			}

			var deleteResponse = await HttpClient.DeleteAsync(apiRoot + "/Datasets/" + dataset.DatasetId);
			await deleteResponse.Content.ReadAsStringAsync();
			return;
		}

        public async Task<TData> GetItemFromDataset<TData>(string datasetId, string id) where TData : IDatasetItem
        {
            //var content = new StringContent(JsonConvert.SerializeObject(item));
            var response = await HttpClient.GetAsync(apiRoot + $"/DatasetItem/{datasetId}/{id}");
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (result["error"] == null)
            {
                return result.ToObject<TData>();
            }
            else
            {
                throw new DatasetConnectorException(result["error"]["description"].Value<string>());
            }
        }


        public async Task<SearchResult<TData>> SearchItemsInDataset<TData>(string datasetId, string query, int page) where TData : IDatasetItem
        {
            //var content = new StringContent(JsonConvert.SerializeObject(item));
            var response = await HttpClient.GetAsync(apiRoot + $"/DatasetSearch/{datasetId}?q={System.Net.WebUtility.UrlEncode(query)}&page={page}");
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (result["error"] == null)
            {
                return result.ToObject<SearchResult<TData>>();
            }
            else
            {
                throw new DatasetConnectorException(result["error"]["description"].Value<string>());
            }
        }
    }
}
