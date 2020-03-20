using Newtonsoft.Json;
using System.Collections.Generic;

namespace HlidacStatu.Web.Models.Apiv2
{
    [JsonObject(MemberSerialization.OptOut)]
    public class SearchResultDTO
    {
        public long Total { get; set; }
        public int Page { get; set; }
        public IEnumerable<dynamic> Results { get; set; }

        public SearchResultDTO(long total, int page, IEnumerable<dynamic> obj)
        {
            Total = total;
            Page = page;
            Results = obj;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}