using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.ClassificationRepair
{
    public interface IStemmerService
    {
        Task<string> ExplainCategories(string text);

        Task<IEnumerable<string>> GetBullshitStems();

        Task<IEnumerable<string>> GetAllStems();

        Task<IEnumerable<string>> Stem(string text);
    }

    public class StemmerService : IStemmerService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public StemmerService(HttpClient httpClient, ILogger<StemmerService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> ExplainCategories(string text)
        {
            var uri = new Uri("explain_json", UriKind.Relative);
            string jsonText = System.Text.Json.JsonSerializer.Serialize<string>(text);
            using HttpContent content = new StringContent(jsonText, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await _httpClient.PostAsync(uri, content).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return json;
            }
            _logger.LogError($"Failed getting response from: {response.RequestMessage.RequestUri}. Klasifikator responded with statusCode=[{response.StatusCode}].");
            throw new HttpRequestException($"Klasifikator responded with statusCode=[{response.StatusCode}].");
        }

        public async Task<IEnumerable<string>> Stem(string text)
        {
            var uri = new Uri("/text_stemmer_ngrams?ngrams=3", UriKind.Relative);
            string jsonText = System.Text.Json.JsonSerializer.Serialize<string>(text);
            using HttpContent content = new StringContent(jsonText, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await _httpClient.PostAsync(uri, content).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<string>>(json);
            }
            _logger.LogError($"Failed getting response from: {response.RequestMessage.RequestUri}. Klasifikator responded with statusCode=[{response.StatusCode}].");
            throw new HttpRequestException($"Klasifikator responded with statusCode=[{response.StatusCode}].");
        }

        public async Task<IEnumerable<string>> GetBullshitStems()
        {
            var uri = new Uri("/bullshit", UriKind.Relative);
            try
            {
                var json = await _httpClient.GetStringAsync(uri).ConfigureAwait(false);
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<string>>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed getting response from: {new Uri(_httpClient.BaseAddress, uri)}.");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetAllStems()
        {
            var uri = new Uri("/all_words", UriKind.Relative);
            try
            {
                var json = await _httpClient.GetStringAsync(uri).ConfigureAwait(false);
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<string>>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed getting response from: {new Uri(_httpClient.BaseAddress, uri)}.");
                throw;
            }
        }
    }
}