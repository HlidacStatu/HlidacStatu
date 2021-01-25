using HlidacStatu.Lib.Data;
using System;

namespace HlidacStatu.Web.Models.Apiv2
{
    public class SocialNetworkDTO
    {
        public SocialNetworkDTO(OsobaEvent osobaEvent)
        {
            this.Id = osobaEvent.AddInfo;
            this.Type = osobaEvent.Organizace;
        }

        public string Type { get; set; }
        public string Id { get; set; }
        public string Url
        {
            get
            {
                if(Enum.TryParse<OsobaEvent.SocialNetwork>(Type, out var socialNetwork))
                {
                    switch (socialNetwork)
                    {
                        case OsobaEvent.SocialNetwork.Twitter:
                            return "https://twitter.com/" + Id;
                        case OsobaEvent.SocialNetwork.Facebook_page:
                            return "https://www.facebook.com/" + Id;
                        case OsobaEvent.SocialNetwork.Facebook_profile:
                            return "https://www.facebook.com/" + Id;
                        case OsobaEvent.SocialNetwork.Instagram:
                            return "https://www.instagram.com/" + Id;
                        case OsobaEvent.SocialNetwork.www:
                            return Id;
                        default:
                            return "";
                    }
                }
                return "";
            }
        }
    }
}