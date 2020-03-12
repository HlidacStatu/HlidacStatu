using HlidacStatu.Lib.Data;
using System;

namespace HlidacStatu.Web.Models.apiv2
{
    public class OsobaDTO
    {
        public OsobaDTO(Osoba o)
        {
            this.TitulPred = o.TitulPred;
            this.Jmeno = o.Jmeno;
            this.Prijmeni = o.Prijmeni;
            this.TitulPo = o.TitulPo;
            this.Narozeni = o.Narozeni;
            this.NameId = o.NameId;
            this.Profile = o.GetUrl();
        }
        public string TitulPred { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public string TitulPo { get; set; }

        public DateTime? Narozeni { get; set; }
        public string NameId { get; set; }
        public string Profile { get; set; }
    }
}