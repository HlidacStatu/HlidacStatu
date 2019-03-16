using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HlidacStatu.Lib.Data.External.Zabbix
{
    public class ZabHostAvailability_Intervals
    {
        public ZabHostAvailability_Intervals(ZabHost host, IEnumerable<ZabHistoryItem> measures, DateTime lastDate)
        {
            this.Host = host;
            List<ZabAvailabilityInterval> interv = new List<ZabAvailabilityInterval>();
            DateTime debugt = new DateTime(2017, 12, 7, 17, 08, 00);
            var data = measures.OrderBy(m => m.clock).ToArray();
            for (int i = 0; i < data.Length-1; i++)
            {
                var m = data[i];
                Statuses status = ZabAvailability.GetStatus(m.value);

                if (i > 0 && (m.clock - data[i - 1].clock).TotalMinutes > 3) //pokud chybi data za vice nez 3 min, oznat je cervene
                {
                    interv.Add(new ZabAvailabilityInterval(data[i - 1].clock, m.clock, Statuses.Nedostupné));
                }

                //hledej kdy tento status konci
                var j = 1;
                while (
                    i+j<data.Length-1 
                    && ZabAvailability.GetStatus(data[i + j].value) == status
                    && (i>0 && (data[i +j].clock - data[i+j - 1].clock).TotalMinutes < 3)
                    )
                {
                    j++;
                }
                interv.Add(new ZabAvailabilityInterval(m.clock, data[i + j].clock, status));
                i = i + (j - 1);



            }
            this.Intervals = interv.ToArray();
        }
        public ZabHost Host { get; set; }
        public ZabAvailabilityInterval[] Intervals { get; set; }
    }
}