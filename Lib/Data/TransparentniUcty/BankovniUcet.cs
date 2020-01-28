using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devmasters.Core;

namespace HlidacStatu.Lib.Data.TransparentniUcty
{
    public class BankovniUcet
        : Plugin.TransparetniUcty.IBankovniUcet
    {
        private static External.DataSets.DataSet _client =
            External.DataSets.DataSet.CachedDatasets.Get("transparentni-ucty");

        public string Id
        {
            get
            {
                return TextUtil.NormalizeToURL(this.CisloUctu);
            }
        }

        public string Subjekt { get; set; }

        public string Nazev { get; set; }

        public string TypSubjektu { get; set; }

        public string Url { get; set; }

        public string CisloUctu { get; set; }

        public string Mena { get; set; }

        public DateTime LastSuccessfullParsing { get; set; }

        public string GetUrl(bool local = true)
        {
            return GetUrl(local, string.Empty);
        }

        public string GetUrl(bool local, string foundWithQuery)
        {
            string url = "/data/Detail/transparentni-ucty/" + System.Net.WebUtility.UrlEncode(this.Id);

            if (!string.IsNullOrEmpty(foundWithQuery))
                url = "/Data/hledat/transparentni-ucty?Q=" + System.Net.WebUtility.UrlEncode(foundWithQuery);

            if (!local)
                return "https://www.hlidacstatu.cz" + url;


            return url;
        }
        public string GetSubjektUrl(bool local = true)
        {
            string pref = "";
            if (!local)
                pref = "https://www.hlidacstatu.cz";
            return pref + "/data/Hledat/transparentni-ucty?Q=Subjekt.keyword%3A\"" + System.Net.WebUtility.UrlEncode(this.Subjekt) + "\"";
        }

        public int Active { get; set; } = 1;

        /* 
         * Může nabývat 3 hodnot:
         * 0 Neznámý typ účtu
         * 1 Provozní účet
         * 2 Volební účet
        */
        public string TypUctu { get; set; }
        
        public void Save(string user)
        {
            _client.AddData(this, this.Id, user);
        }

        public static BankovniUcet Get(string cislo)
        {
            BankovniUcet bu = _client.GetData<BankovniUcet>(TextUtil.NormalizeToURL(cislo));
            return bu;
        }

        public static bool DeleteUcet(BankovniUcet bu)
        {
            return _client.DeleteData(bu.Id);
        }

        public static IEnumerable<BankovniUcet> GetAll()
        {
            return _client.GetAllData<BankovniUcet>();
        }

        public static string NormalizeCisloUctu(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            var ucet = System.Text.RegularExpressions.Regex.Replace(s, "[^0-9-/]", "").Trim();

            string prefix = "";
            string number = "";
            string bankNumber = "";

            string[] partBank = ucet.Split('/');
            if (partBank.Length == 2)
                bankNumber = partBank[1];

            string[] partNumber = partBank[0].Split('-');
            if (partBank[0].StartsWith("-")) //fix pro "minusove" ucty CSSD -9/2010
            {
                prefix = "";
                number = partBank[0];
            }
            else if (partNumber.Length == 2)
            {
                prefix = partNumber[0];
                number = partNumber[1];
            }
            else
                number = partNumber[0];

            var prefix1 = prefix.TrimStart('0');
            var number1 = number.TrimStart('0');

            if (!string.IsNullOrEmpty(prefix1))
                return prefix1 + "-" + number1 + "/" + bankNumber;
            else
                return number1 + "/" + bankNumber;

        }
    }

}
