using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data
{
    public partial class ItemToOcrQueue
    {
        public enum ItemToOcrType
        {
            Smlouva,
            Insolvence,
            Dataset,
            VerejnaZakazka
        }

        public static IEnumerable<ItemToOcrQueue> TakeFromQueue(ItemToOcrType? itemType = null, string itemSubType = null, int maxItems = 30)
        {
            using (DbEntities db = new DbEntities())
            {
                using (var dbTran = db.Database.BeginTransaction())
                {
                    try
                    {
                        IQueryable<ItemToOcrQueue> sql = null;
                        sql = db.ItemToOcrQueue
                            .Where(m => m.done == null
                                    && m.started == null);

                        if (itemType != null)
                            sql = sql.Where(m => m.itemType == itemType.ToString());

                        if (!string.IsNullOrEmpty(itemSubType))
                            sql = db.ItemToOcrQueue
                            .Where(m => m.itemSubType == itemSubType);

                        sql = sql
                            .OrderByDescending(m => m.priority)
                            .ThenBy(m=>m.created)
                            .Take(maxItems);
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

        public static HlidacStatu.Lib.OCR.Api.Result AddNewTask(ItemToOcrType itemType, string itemId, string itemSubType = null, HlidacStatu.Lib.OCR.Api.Client.TaskPriority priority = OCR.Api.Client.TaskPriority.Standard)
        {
            using (DbEntities db = new DbEntities())
            {
                ItemToOcrQueue i = new ItemToOcrQueue();
                i.created = DateTime.Now;
                i.itemId = itemId;
                i.itemType = itemType.ToString();
                i.itemSubType = itemSubType;
                i.priority = (int)OCR.Api.Client.TaskPriority.Standard;
                db.ItemToOcrQueue.Add(i);
                db.SaveChanges();
                return new OCR.Api.Result()
                {
                    IsValid = OCR.Api.Result.ResultStatus.InQueueWithCallback,
                    Id = "uknown"
                };
            }
        }

        public static void ResetTask(int taskItemId)
        {
            using (DbEntities db = new DbEntities())
            {

                ItemToOcrQueue i = db.ItemToOcrQueue.Where(m => m.pk == taskItemId).FirstOrDefault();
                if (i != null)
                {
                    i.done = null;
                    i.started = null;
                    i.success = null;
                    i.result = null;
                    db.SaveChanges();
                }
            }
        }


        public static void SetDone(int taskItemId, bool success, string result = null)
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
