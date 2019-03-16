using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HlidacStatu.Lib.Data.External.Zabbix
{
    public class ZabHostAvailability_PerMin
    {
        public ZabHostAvailability_PerMin(ZabHost host, IEnumerable<ZabHistoryItem> measures)
        {
            this.Host = host;
            List<ZabAvailability> avail = new List<ZabAvailability>();

            ZabHistoryItem first = measures.First();
            ZabAvailability prev = new ZabAvailability() { Time = RoundToMin(first.clock), Response = first.value };
            avail.Add(prev);
            var data = measures.OrderBy(m => m.clock).ToArray();
            for (int i = 1; i < data.Length; i++)
            {
                var curr = data[i];
                var d = RoundToMin(curr.clock);
                var diffInMin = (d - prev.Time).TotalMinutes;
                if (diffInMin > 3.5) //velka mezera, dopln null
                {
                    for (int j = 0; j < diffInMin; j++)
                    {
                        avail.Add(new ZabAvailability() { Time = d.AddMinutes(j), Response = null });
                    }
                }
                else if (diffInMin > 1.5) //mala mezera, zopakuju posledni stav
                {
                    for (int j = 0; j < diffInMin; j++)
                    {
                        avail.Add(new ZabAvailability() { Time = d.AddMinutes(j), Response = prev.Response, DownloadSpeed = prev.Response, HttpStatusCode = prev.HttpStatusCode });
                    }
                }
                else if (diffInMin > 1) //ok
                {
                    avail.Add(new ZabAvailability() { Time = d, Response = curr.value, DownloadSpeed = null, HttpStatusCode = null });
                }
                else if (diffInMin > 0) //mene nez 1 min
                {
                    //je nasleduji dal nez 1 min?
                    if (i < data.Length - 1 && (RoundToMin(data[i + 1].clock) - prev.Time).TotalMinutes > 2)
                    {
                        avail.Add(new ZabAvailability() { Time = d.AddMinutes(1), Response = curr.value, DownloadSpeed = null, HttpStatusCode = null});
                    }
                    //jinak to preskoc
                }

                prev = avail.Last();

            }
            this.Data = avail.ToArray();
        }
        private DateTime RoundToMin(DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0, d.Kind);
        }
        public ZabHost Host { get; set; }


        public ZabAvailability[] Data { get; set; }
    }
}

