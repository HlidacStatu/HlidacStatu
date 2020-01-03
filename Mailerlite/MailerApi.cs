using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Mailerlite
{
    public class MailerApi
    {
        private HttpClient _httpClient = new HttpClient();
        public MailerApi(string apiKey)
        {
            _httpClient.BaseAddress = new Uri("https://api.mailerlite.com/api/v2/");
            _httpClient.DefaultRequestHeaders.Add("x-mailerlite-apikey", apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        public async Task AddSubscriberToGroup(string email, string groupId)
        {

            string json = $"{{\"email\":\"{email}\"}}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"groups/{groupId}/subscribers", content);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Request ended with response status code = [{response.StatusCode}]");

        }

        // finalizer
        ~MailerApi()
        {
            _httpClient.Dispose();
        }
    }
}
