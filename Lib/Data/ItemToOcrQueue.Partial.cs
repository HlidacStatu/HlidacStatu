﻿using System;
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
       
        private static IQueryable<ItemToOcrQueue> CreateQuery(DbEntities db, ItemToOcrType? itemType, string itemSubType)
        {
            IQueryable<ItemToOcrQueue> sql = null;
            sql = db.ItemToOcrQueue
                .Where(m => m.done.Equals(null)
                        && m.started.Equals(null));

            if (itemType != null)
                sql = sql.Where(m => m.itemType == itemType.ToString());

            if (!string.IsNullOrEmpty(itemSubType))
                sql = sql.Where(m => m.itemSubType == itemSubType);

            return sql;
        }
        public static bool AreThereItemsToProcess(ItemToOcrType? itemType = null, string itemSubType = null)
        {
            using (DbEntities db = new DbEntities())
            {
                var sql = CreateQuery(db, itemType, itemSubType);
                return sql.Any();
            }
        }

        static object lockTakeFromQueue = new object();
        public static IEnumerable<ItemToOcrQueue> TakeFromQueue(ItemToOcrType? itemType = null, string itemSubType = null, int maxItems = 30)
        {
            using (DbEntities db = new DbEntities())
            {
                lock (lockTakeFromQueue)
                {
                    using (var dbTran = db.Database.BeginTransaction())
                    {
                        try
                        {
                            IQueryable<ItemToOcrQueue> sql = CreateQuery(db, itemType, itemSubType);

                            sql = sql
                                .OrderByDescending(m => m.priority)
                                .ThenBy(m => m.created)
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
        }

        public static HlidacStatu.Lib.OCR.Api.Result AddNewTask(ItemToOcrType itemType, string itemId, string itemSubType = null, HlidacStatu.Lib.OCR.Api.Client.TaskPriority priority = OCR.Api.Client.TaskPriority.Standard)
        {
            return AddNewTask(itemType, itemId, itemSubType, (int)priority);
        }
        public static HlidacStatu.Lib.OCR.Api.Result AddNewTask(ItemToOcrType itemType, string itemId, string itemSubType = null, int priority = 5)
        {
            using (DbEntities db = new DbEntities())
            {
                IQueryable<ItemToOcrQueue> sql = CreateQuery(db, itemType, itemSubType);
                sql = sql.Where(m => m.itemId == itemId );
                if (sql.Any()) //already in the queue
                    return new OCR.Api.Result()
                    {
                        IsValid = OCR.Api.Result.ResultStatus.InQueueWithCallback,
                        Id = "uknown"
                    };

                ItemToOcrQueue i = new ItemToOcrQueue();
                i.created = DateTime.Now;
                i.itemId = itemId;
                i.itemType = itemType.ToString();
                i.itemSubType = itemSubType;
                i.priority = priority;
                db.ItemToOcrQueue.Add(i);
                db.SaveChanges();
                return new OCR.Api.Result()
                {
                    IsValid = OCR.Api.Result.ResultStatus.InQueueWithCallback,
                    Id = "uknown"
                };
            }
        }

        public static void ResetTask(int taskItemId, bool decreasePriority = true)
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
                    if (decreasePriority)
                    {
                        i.priority--;
                        if (i.priority < 1)
                            i.priority = 1;
                    }
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
