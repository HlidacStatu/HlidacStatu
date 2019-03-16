using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HlidacStatu.Lib.Data.External.Zabbix
{
    public class ZabAvailabilityInterval
    {
        public ZabAvailabilityInterval()
        {
        }
        public ZabAvailabilityInterval(DateTime from, DateTime to, decimal responseInSec)
        {
            this.From = from;
            this.To = to;
            this.Status = ZabAvailability.GetStatus(responseInSec);
        }
        public ZabAvailabilityInterval(DateTime from, DateTime to, Statuses status)
        {
            this.From = from;
            this.To = to;
            this.Status = status;
        }


        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Statuses Status { get; set; }
    }
}