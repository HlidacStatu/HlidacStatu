using HlidacStatu.Lib.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Devmasters.Enums;

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
            this.Sponzoring = o.Events(ev => ev.Type == (int)OsobaEvent.Types.Sponzor)
                .Select(ev => new OsobaEventDTO()
                {
                    Castka = ev.AddInfoNum,
                    DatumOd = ev.DatumOd,
                    DatumDo = ev.DatumDo,
                    Typ = "sponzor",
                    Organizace = ev.Organizace
                }).ToList() ;

            var unwantedEvents = new int[]
            {
                (int)OsobaEvent.Types.Osobni,
                (int)OsobaEvent.Types.CentralniRegistrOznameni,
                (int)OsobaEvent.Types.SocialniSite,
                (int)OsobaEvent.Types.Sponzor
            };

            this.Udalosti = o.NoFilteredEvents(ev => !unwantedEvents.Contains(ev.Type))
                .Select(ev => new OsobaEventDTO()
                {
                    Castka = ev.AddInfoNum,
                    DatumOd = ev.DatumOd,
                    DatumDo = ev.DatumDo,
                    Role = ev.AddInfo,
                    Typ = ((OsobaEvent.Types)ev.Type).ToNiceDisplayName(),
                    Organizace = ev.Organizace
                }).ToList();

            this.SocialniSite = o.NoFilteredEvents(ev => ev.Type == (int)OsobaEvent.Types.SocialniSite)
                .Select(ev => new SocialNetworkDTO()
                {
                    Id = ev.AddInfo,
                    Type = ev.Organizace
                }).ToList();
        }
        public string TitulPred { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public string TitulPo { get; set; }

        public DateTime? Narozeni { get; set; }
        public string NameId { get; set; }
        public string Profile { get; set; }
        public List<OsobaEventDTO> Sponzoring { get; set; }
        public List<OsobaEventDTO> Udalosti { get; set; }
        public List<SocialNetworkDTO> SocialniSite { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}