using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lang.CS
{
    public class Stemmer2
    {
        public static Random Rnd = new Random();

        public static string[] Stems(string text)
        {
            //HttpClientHandler httpClientHandler = new HttpClientHandler()
            //{
            //    Proxy = new WebProxy(string.Format("{0}:{1}", "127.0.0.1", 8888), false)
            //};
            var wc = new System.Net.Http.HttpClient();
            try
            {

                var data = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.ToString(text));
                var res = wc.PostAsync(classificationBaseUrl() + "/text_stemmer?ngrams=1", data).Result;

                return Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                wc.Dispose();
            }

        }

        private static string classificationBaseUrl()
        {
            string[] baseUrl = Devmasters.Core.Util.Config.GetConfigValue("Classification.Service.Url")
                .Split(',', ';');
            //Dictionary<string, DateTime> liveEndpoints = new Dictionary<string, DateTime>();

            return baseUrl[ Rnd.Next(baseUrl.Length)];

        }
    }
}
