using Newtonsoft.Json;

namespace HlidacStatu.Lib.Data.Insolvence
{
	public class InsolvenceDetail : Bookmark.IBookmarkable //, ISocialInfo
	{
		public Rizeni Rizeni { get; set; }

		public string BookmarkName() => $"Insolvenční řízení {Rizeni.SpisovaZnacka}";

        public string GetUrl(bool local = true)
        {
            return GetUrl(local, string.Empty);
        }

        public string GetUrl(bool local, string foundWithQuery)
        {
			var url = "/insolvence/rizeni/" + Rizeni.UrlId();
            if (!string.IsNullOrEmpty(foundWithQuery))
                url = url + "?qs=" + System.Net.WebUtility.UrlEncode(foundWithQuery);
			return local ? url : "https://www.hlidacstatu.cz" + url;
		}
		public string ToAuditJson() => JsonConvert.SerializeObject(this);
		public string ToAuditObjectId() => Rizeni.SpisovaZnacka;
		public string ToAuditObjectTypeName() => "Insolvenční řízení";




    }
}
