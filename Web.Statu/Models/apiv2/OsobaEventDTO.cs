using System;

namespace HlidacStatu.Web.Models.Apiv2
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