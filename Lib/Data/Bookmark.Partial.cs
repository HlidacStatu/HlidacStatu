using Devmasters;
using Devmasters.Enums;

using System;
using System.Linq;

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

        [Devmasters.Enums.ShowNiceDisplayName()]
        [Devmasters.Enums.Sortable(Devmasters.Enums.SortableAttribute.SortAlgorithm.BySortValue)]
        public enum ItemTypes
        {
            [Disabled()]
            SingleItem = 0,

            [Disabled()]
            Url = 1,

        }

        static string IdDelimiter = "|";

        public static string GetBookmarkId(IBookmarkable item)
        {
            return item.ToAuditObjectTypeName() + IdDelimiter + item.ToAuditObjectId();
        }

        public static bool IsItemBookmarked(IBookmarkable item, string userId)
        {
            return IsItemBookmarked(ItemTypes.SingleItem, GetBookmarkId(item), userId);
        }
        public static bool IsItemBookmarked(ItemTypes type, string itemId, string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
            {
                return db.Bookmark.Any(m => m.ItemType == (int)type && m.ItemId == itemId && m.UserId == userId);
            }
        }

        public static void SetBookmark(string name, string url, ItemTypes type, string itemId, string userId)
        {
            if (IsItemBookmarked(type, itemId, userId))
                return;

            Bookmark b = new Bookmark()
            {
                Name = Devmasters.TextUtil.ShortenText(name,250),
                Folder = "",
                Created=DateTime.Now,
                Url = url,
                BookmarkType = type,
                ItemId = itemId,
                UserId = userId
            };
            b.Save();

        }

        public static void DeleteBookmark(ItemTypes type, string itemId, string userId)
        {
            using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
            {
                var b = db.Bookmark.FirstOrDefault(m => m.ItemType == (int)type && m.ItemId == itemId && m.UserId == userId);
                if (b != null)
                {
                    db.Bookmark.Remove(b);
                    db.SaveChanges();
                }
            }
        }

        [Nest.Object(Ignore = true)]
        public ItemTypes BookmarkType
        {
            get { return (ItemTypes)this.ItemType; }
            set { this.ItemType = (int)value; }
        }

        public void Save()
        {
            using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
            {
                if (this.Id == default(Guid))
                {
                    this.Id = Guid.NewGuid();
                    db.Bookmark.Add(this);
                }
                else
                {
                    db.Bookmark.Attach(this);
                    db.Entry(this).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }
        }
    }
}
