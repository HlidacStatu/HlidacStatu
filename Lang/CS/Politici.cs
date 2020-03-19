using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lang.CS
{
    public class Politici
    {
        public static Random Rnd = new Random();

        public static List<Tuple<string, string[]>> PoliticiStems = new List<Tuple<string, string[]>>();

        static Politici()
        {
            PoliticiStems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Tuple<string, string[]>>>(
                   System.IO.File.ReadAllText(@"politiciStem.json")
                   );

        }


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

        /// <summary>
        /// returns Osoba.NameId[]
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string[] FindCitations(string text)
        {
            var stopw = new Devmasters.Core.StopWatchEx();
            stopw.Start();
            string[] sText = HlidacStatu.Lang.CS.Politici.Stems(text);
            stopw.Stop();
            //Console.WriteLine($"stemmer {stopw.ExactElapsedMiliseconds} ");
            stopw.Restart();
            List<string> found = new List<string>();
            foreach (var kv in PoliticiStems)
            {
                string zkratka = kv.Item1;
                string[] politik = kv.Item2;

                for (int i = 0; i < sText.Length - (politik.Length - 1); i++)
                {
                    bool same = true;
                    for (int j = 0; j < politik.Length; j++)
                    {
                        if (sText[i + j] == politik[j])
                            same = same & true;
                        else
                        {
                            same = false;
                            break;
                        }
                    }
                    if (same)
                    {
                        if (!found.Contains(zkratka))
                            found.Add(zkratka);
                        break;
                    }

                }

            }
            stopw.Stop();
            //Console.WriteLine($"location {stopw.ExactElapsedMiliseconds} ");
            return found.ToArray();

        }


    }
}
