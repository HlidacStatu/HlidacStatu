using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HlidacStatu.ClassificationRepair
{
    public interface IStemmerService
    {
        Task<IEnumerable<Explanation>> ExplainCategories(string text, CancellationToken cancellationToken);

        Task<IEnumerable<string>> GetBullshitStems();

        Task<IEnumerable<string>> GetAllStems();

        Task<IEnumerable<string>> Stem(string text, CancellationToken cancellationToken);
    }

    public class Explanation
    {
        public int Prediction { get; set; }
        public string Tag { get; set; }
        public IEnumerable<string> Words { get; set; }
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

        public async Task<IEnumerable<Explanation>> ExplainCategories(string text, CancellationToken cancellationToken)
        {
            var uri = new Uri("/explain_text_json", UriKind.Relative);
            string jsonText = JsonSerializer.Serialize<string>(text);
            using HttpContent content = new StringContent(jsonText, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await _httpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var result = JsonSerializer.Deserialize<IEnumerable<Explanation>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result;
            }
            _logger.LogError($"Failed getting response from: {response.RequestMessage.RequestUri}. Klasifikator responded with statusCode=[{response.StatusCode}].");
            throw new HttpRequestException($"Klasifikator responded with statusCode=[{response.StatusCode}].");
        }

        public async Task<IEnumerable<string>> Stem(string text, CancellationToken cancellationToken)
        {
            var uri = new Uri("/text_stemmer_ngrams?ngrams=3", UriKind.Relative);
            string jsonText = JsonSerializer.Serialize<string>(text);
            using HttpContent content = new StringContent(jsonText, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await _httpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonSerializer.Deserialize<IEnumerable<string>>(json);
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
                return JsonSerializer.Deserialize<IEnumerable<string>>(json);
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
                return JsonSerializer.Deserialize<IEnumerable<string>>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed getting response from: {new Uri(_httpClient.BaseAddress, uri)}.");
                throw;
            }
        }
    }
}