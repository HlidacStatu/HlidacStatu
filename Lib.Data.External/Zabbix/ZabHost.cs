using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HlidacStatu.Lib;
using Devmasters.Core;

using Newtonsoft.Json;
using HlidacStatu.Util;
using Devmasters.Enums;

namespace HlidacStatu.Lib.Data.External.Zabbix
{

    public enum Statuses
    {
        OK = 0,
        Pomalé = 1,
        Nedostupné = 2,
        TimeOuted = 98,
        BadHttpCode = 99,
        Unknown = 1000,
    }

    [ShowNiceDisplayName()]
    public enum SSLLabsGrades
    {
        [NiceDisplayName("A+")]
        Aplus = 1,
        A = 2,
        [NiceDisplayName("A-")]
        Aminus = 3,
        B = 5,
        C = 6,
        D = 7,
        E = 8,
        F = 9,
        [NiceDisplayName("T")]
        T = 20,
        [NiceDisplayName("M")]
        M = 50,
        [NiceDisplayName("X")]
        X = 100

    }

    public class ZabHost
    {
        private string _hash = null;
        public ZabHost(string hostId, string host, string url, string description, string[] mainGroup = null)
        {
            this.hostid = hostId;
            this.host = host;
            this.url = url;
            this.urad = Devmasters.Core.TextUtil.NormalizeToBlockText(ParseTools.GetRegexGroupValue(description, @"Urad:\s?(?<txt>[^\x0a\x0d]*)", "txt"));
            this.popis = Devmasters.Core.TextUtil.ShortenHTML(ParseTools.GetRegexGroupValue(description, @"Popis:\s?(?<txt>[^\x0a\x0d]*)", "txt"), 10000, new string[] {"a","b"} );
            this.publicname = Devmasters.Core.TextUtil.NormalizeToBlockText(ParseTools.GetRegexGroupValue(description, @"Nazev:\s?(?<txt>[^\x0a\x0d]*)", "txt"));            
            string sgroup = Devmasters.Core.TextUtil.NormalizeToBlockText(ParseTools.GetRegexGroupValue(description, @"Poznamka:\s?(?<txt>[^\x0a\x0d]*)", "txt"));

            this.customUrl = Devmasters.Core.TextUtil.NormalizeToBlockText(ParseTools.GetRegexGroupValue(description, @"URL:\s?(?<txt>[^\x0a\x0d]*)", "txt"));

            this.groups.Clear();
            if (!string.IsNullOrEmpty(sgroup))
            {
                var agroups = sgroup.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                this.groups.AddRange(agroups);
            }
            if (mainGroup != null && mainGroup.Length > 0)
                this.groups.AddRange(mainGroup);

            if (string.IsNullOrEmpty(publicname))
                publicname = this.host;
            _hash = Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(hostid + "xxttxx" + hostid);
        }
        public string hostid { get; set; }
        public string host { get; set; }
        public string url { get; set; }

        public string opendataUrl { get { return "https://www.hlidacstatu.cz/api/v2/Weby/" + this.hostid ; } }
        public string pageUrl { get { return "https://www.hlidacstatu.cz/StatniWeby/Info/" + this.hostid + "?h=" + hash; } }

        [JsonIgnore()]
        public string customUrl { get; set; }
        public string urad { get; set; }
        public string publicname { get; set; }
        public string popis { get; set; }
        [JsonIgnore()]
        public string itemIdResponseTime { get; set; }
        [JsonIgnore()]
        public string itemIdResponseCode { get; set; }

        [JsonIgnore()]
        public string itemIdSsl { get; set; }
        [JsonIgnore()]
        public List<string> groups { get; set; } = new List<string>();

        public string UriHost()
        {
            string s = "";
            if (!string.IsNullOrEmpty(this.customUrl))
                s = this.customUrl;
            else
                s = this.url;
            Uri uri = null;
            Uri.TryCreate(s, UriKind.Absolute, out uri);
            return uri?.Host;

        }

        public string hash { get { return _hash; } }
        public bool ValidHash(string h)
        {
            return h == _hash;
        }
    }
}