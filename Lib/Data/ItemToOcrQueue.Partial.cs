using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data
{
    public partial class ItemToOcrQueue
    {

        public static IEnumerable<ItemToOcrQueue> TakeFromQueue(Type itemType, string itemSubType = null, int maxItems = 30)
        {
            using (DbEntities db = new DbEntities())
            {
                using (var dbTran = db.Database.BeginTransaction())
                {
                    try
                    {
                        IQueryable<ItemToOcrQueue> sql = null;
                        if (string.IsNullOrEmpty(itemSubType))
                            sql = db.ItemToOcrQueue
                                .Where(m => m.done == null
                                        && m.started == null
                                        && m.itemType == itemType.Name
                                        )
                                .OrderBy(m => m.created)
                                .Take(maxItems)
                                ;
                        else
                            sql = db.ItemToOcrQueue
                            .Where(m => m.done == null
                                    && m.started == null
                                    && m.itemType == itemType.Name
                                    && (m.itemSubType == itemSubType)
                                    )
                            .OrderBy(m => m.created)
                            .Take(maxItems)
                            ;
                        var res = sql.ToArray();
                        foreach (var i in res)
                        {
                            i.started = DateTime.Now;
                        }
                        db.SaveChanges();
                        dbTran.Commit();
                        return res;
                    }
                    catch (Exception e)
                    {
                        dbTran.Rollback();
                        throw e;
                    }
                }
            }
        }

        public static void AddNewTask(Type itemType, string itemId, string itemSubType = null)
        {
            using (DbEntities db = new DbEntities())
            {
                ItemToOcrQueue i = new ItemToOcrQueue();
                i.created = DateTime.Now;
                i.itemId = itemId;
                i.itemType = itemType.Name;
                i.itemSubType = itemSubType;
                db.ItemToOcrQueue.Add(i);
                db.SaveChanges();
            }
        }

        public void SetDone(int taskItemId, bool success, string result = null)
        {
            using (DbEntities db = new DbEntities())
            {
                
                ItemToOcrQueue i = db.ItemToOcrQueue.Where(m => m.pk == taskItemId).FirstOrDefault();
                if (i != null)
                {
                    i.done = DateTime.Now;
                    i.success = success ? 1 : 0;
                    i.result = result;
                    db.SaveChanges();
                }
            }
        }

    }
}
