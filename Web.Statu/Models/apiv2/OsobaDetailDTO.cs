using HlidacStatu.Lib.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Web.Models.Apiv2
{
    public class OsobaDetailDTO
    {
        public OsobaDetailDTO(Osoba o)
        {
            this.TitulPred = o.TitulPred;
            this.Jmeno = o.Jmeno;
            this.Prijmeni = o.Prijmeni;
            this.TitulPo = o.TitulPo;
            this.Narozeni = o.Narozeni;
            this.NameId = o.NameId;
            this.Profile = o.GetUrl();
            this.Sponzoring = o.Events(ev => ev.Type == 3)
                .Select(ev => new OsobaEventDTO()
                {
                    Castka = ev.AddInfoNum,
                    DatumOd = ev.DatumOd,
                    DatumDo = ev.DatumDo,
                    Typ = "sponzor",
                    Organizace = ev.Organizace
                }).ToList() ;
        }
        public string TitulPred { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public string TitulPo { get; set; }

        public DateTime? Narozeni { get; set; }
        public string NameId { get; set; }
        public string Profile { get; set; }
        public List<OsobaEventDTO> Sponzoring { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}