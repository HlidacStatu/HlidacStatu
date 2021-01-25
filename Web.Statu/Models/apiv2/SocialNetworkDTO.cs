using System;

namespace HlidacStatu.Web.Models.Apiv2
{
    public class SocialNetworkDTO
    {
        private string _type;
        public string Type 
        {
            get => _type;
            set => _type = value.Trim().ToLower();
        }
        public string Id { get; set; }
        public string Url
        {
            get
            {
                switch (this.Type)
                {
                    case "twitter":
                        return "https://twitter.com/" + Id;
                    case "facebook_page":
                        return "https://www.facebook.com/" + Id;
                    case "facebook_profile":
                        return "https://www.facebook.com/" + Id;
                    case "facebook":
                        return "https://www.facebook.com/" + Id;
                    case "instagram":
                        return "https://twitter.com/" + Id;
                    case "www":
                        return Id;
                    default:
                        return "";
                }
            }
        }
    }
}