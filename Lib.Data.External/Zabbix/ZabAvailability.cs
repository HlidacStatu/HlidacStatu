using Newtonsoft.Json;
using System;

namespace HlidacStatu.Lib.Data.External.Zabbix
{

    public class ZabAvailability
    {
        public static decimal OKLimit = 1.000m;
        public static decimal SlowLimit = 2.000m;
        public static decimal TimeOuted = 99m;
        public static decimal BadHttpCode = 99.1m;

        public static Statuses GetStatus(decimal responseInMs)
        {
            if (responseInMs < OKLimit)
                return Statuses.OK;
            else if (responseInMs < SlowLimit)
                return Statuses.Pomalé;
            else
                return Statuses.Nedostupné;

        }

        public static string GetStatusChartColor(Statuses status)
        {
            switch (status)
            {
                case Statuses.OK:
                    return "#3c763d";
                case Statuses.Pomalé:
                    return "#ff9600";
                case Statuses.Nedostupné:
                    return "#db3330";
                case Statuses.TimeOuted:
                    return "#961b19";
                case Statuses.BadHttpCode:
                    return "#4c0908";
                default:
                    return "#ddd";
            }
        }

        public DateTime Time { get; set; }
        public decimal? Response { get; set; } = null;

        public int? HttpStatusCode { get; set; } = null;
        [JsonIgnore()]
        public decimal? DownloadSpeed { get; set; } = null;

        public Statuses Status()
        {
            if (this.Response.HasValue == false)
            {
                if (this.HttpStatusCode.HasValue)
                {
                    if (this.HttpStatusCode.Value >= 300)
                        return Statuses.Nedostupné;
                    if (this.HttpStatusCode.Value >= 200)
                        return Statuses.OK;
                }
                return Statuses.Unknown;
            }
            else
                return GetStatus(this.Response.Value);
        }


    }
}