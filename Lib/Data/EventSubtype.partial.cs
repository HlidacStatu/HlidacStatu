using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HlidacStatu.Lib.Data
{
    public partial class EventSubType
    {
        public static List<EventSubType> GetListItems()
        {
            using (DbEntities db = new DbEntities())
            {
                return db.EventSubType.ToList();

            }
        }

        public static bool IsValidSubtype(int typeId, int? subtypeId)
        {
            if (subtypeId is null) return true;

            using (DbEntities db = new DbEntities())
            {
                return db.EventSubType.Where(st => st.EventTypeId == typeId && st.Id == subtypeId).Any();
            }
        }
    }
}
