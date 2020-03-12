using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HlidacStatu.Web.Models.apiv2
{
    public class OsobaEventDTO
    {
        public string Typ { get; set; }
        public string Organizace { get; set; }
        public string Role { get; set; }
        public decimal? Castka { get; set; }
        public DateTime? DatumOd { get; set; }
        public DateTime? DatumDo { get; set; }

    }
}