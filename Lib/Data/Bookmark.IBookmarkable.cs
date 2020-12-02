namespace HlidacStatu.Lib.Data
{
    public partial class Bookmark
    {
        public interface IBookmarkable
            : Audit.IAuditable
        {
            string GetUrl(bool local);
            string GetUrl(bool local, string foundWithQuery);
            string BookmarkName();
        }
    }
}
