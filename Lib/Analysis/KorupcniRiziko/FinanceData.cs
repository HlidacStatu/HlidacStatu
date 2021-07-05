using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class FinanceData
    {
        //r12
        public decimal? OvlivnitelneNakladyCinnosti { get; set; } = null;
        public Dictionary<string, decimal> Ukazatele { get; set; } = new Dictionary<string, decimal>();

        public bool NeuplnyRok { get; set; }

        [Nest.Date]
        public DateTime LastUpdated { get; set; }


    }
    public class FinanceDataCalculator
    {
        public int Rok { get; set; }

        public string Ico { get; set; }


        static object lockObj = new object();
        static Dictionary<int, int> obdobi = null;
        static FinanceDataCalculator()
        {
            lock (lockObj)
            {
                if (obdobi != null)
                    return;
                try
                {
                    using (Devmasters.Net.HttpClient.URLContent url = new Devmasters.Net.HttpClient.URLContent("https://monitor.statnipokladna.cz/api/obdobi"))
                    {
                        var html = url.GetContent();
                        JArray data = JArray.Parse(html.Text);
                        obdobi = data
                            .Where(m => m.Value<bool>("isYear") == true)
                            .Select(m => new { k = m.Value<int>("year"), v = m.Value<int>("loadID") })
                            .ToArray()
                            .ToDictionary(k => k.k, v => v.v);
                    }

                }
                catch (Exception e)
                {
                    obdobi = new Dictionary<int, int>();
                }
            }
        }

        private JObject rozvaha = null;
        private JObject vykaz_zisku_ztrat = null;
        private JObject penezni_toky = null;


        public FinanceDataCalculator(string ico, int rok)
        {
            this.Ico = ico;
            this.Rok = rok;
            // https://monitor.mfcr.cz/api/ucetni-zaverka/2?obdobi=1909&ic=44992785
        }

        public FinanceData GetData()
        {
            var data = new FinanceData();
            try
            {

                int loadId = obdobi[this.Rok];
                this.rozvaha = GetData($"https://monitor.statnipokladna.cz/api/ucetni-zaverka/1?obdobi={loadId}&ic={Ico}");
                this.vykaz_zisku_ztrat = GetData($"https://monitor.statnipokladna.cz/api/ucetni-zaverka/2?obdobi={loadId}&ic={Ico}");
                this.penezni_toky = GetData($"https://monitor.statnipokladna.cz/api/ucetni-zaverka/3?obdobi={loadId}&ic={Ico}");

                data.NeuplnyRok = this.rozvaha == null || this.vykaz_zisku_ztrat == null || this.penezni_toky == null;


                //Ovlivnitelné náklady činnosti dle metodiky MF
                // (https://www.mfcr.cz/assets/cs/media/Ucetnictvi_Prezentace_2014-02-13_Aplikace-klicovych-analytickych-ukazatelu-a-jejich-vyuziti-v-rizeni-verejnych-financi.pdf ), 
                // tzn. položky A.I.1-A.I.12; A.I.35, A.I.36 z výkazu zisku a ztrát + položka B.I. z přehledu o peněžních toků 

                decimal BI = 0;
                if (this.penezni_toky != null)
                    BI = this.penezni_toky["items"]
                        .Where(m => m.Value<string>("code") == "B.I.")
                        .FirstOrDefault()
                        ?.Value<decimal>("value") ?? 0;

                data.Ukazatele.Add("Cashflow.B.I", BI);

                decimal A = 0;
                if (this.vykaz_zisku_ztrat != null)
                {
                    foreach (var i in new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 35, 36 })
                    {
                        var item = this.vykaz_zisku_ztrat["expenses"]
                                 .Where(m => m.Value<string>("code") == $"A.I.{i}.")
                                 .FirstOrDefault();
                        if (item != null)
                        {
                            A = A + item.Value<decimal>("mainActivity")
                                   + item.Value<decimal>("economicActivity");
                            data.Ukazatele.Add("VykazZiskuZtrat.HlavniCinnost." + $"A.I.{i}", item.Value<decimal>("mainActivity"));
                            data.Ukazatele.Add("VykazZiskuZtrat.HospodarskaCinnost." + $"A.I.{i}", item.Value<decimal>("economicActivity"));
                        }
                        else
                        {
                            data.Ukazatele.Add("VykazZiskuZtrat.HlavniCinnost." + $"A.I.{i}", 0);
                            data.Ukazatele.Add("VykazZiskuZtrat.HospodarskaCinnost." + $"A.I.{i}", 0);
                        }
                    }
                }
                data.OvlivnitelneNakladyCinnosti = A + BI;
            }
            catch (Exception e)
            {

            }
            data.LastUpdated = DateTime.Now;
            return data;
        }

        //https://monitor.statnipokladna.cz/api/prispevkove-organizace?obdobi=1512&ic=00006947

        public static JObject GetData(string url)
        {
            try
            {
                using (Devmasters.Net.HttpClient.URLContent net = new Devmasters.Net.HttpClient.URLContent(url))
                {
                    net.Timeout = 1000 * 180;
                    var json = net.GetContent();
                    JObject data = JObject.Parse(json.Text);
                    return data;
                }

            }
            catch
            {
                return null;
            }

        }

    }
}
