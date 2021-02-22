using HlidacStatu.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HlidacStatu.Web.Models.Apiv2
{
    public class FirmaSocialDTO
    {
        public FirmaSocialDTO(Firma firma, List<OsobaEvent> events)
        {
            this.Jmeno = firma.Jmeno;
            this.Ico = firma.ICO;
            this.Profile = firma.GetUrl();
            this.SocialniSite = events.Select(e => new SocialNetworkDTO(e)).ToList();
        }
        public string Ico { get; set; }
        public string Jmeno { get; set; }
        public string Profile { get; set; }
        public List<SocialNetworkDTO> SocialniSite { get; set; }
    }
}