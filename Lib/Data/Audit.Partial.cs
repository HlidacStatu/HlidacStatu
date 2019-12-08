using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{

    public partial class Audit
    {

        public interface IAuditable
        {
            string ToAuditJson();
            string ToAuditObjectTypeName();
            string ToAuditObjectId();
        }

        public enum Operations
        {
            Read,
            Update,
            Delete,
            Create,
            Other,
            InvalidAccess,
            Call,
            Search
        }
        public static Audit Add<T>(Operations operation, string user, T newObj, T prevObj)
            where T : IAuditable
        {
            return Add<T>(operation, user, null, newObj, prevObj);
        }
        public static Audit Add<T>(Operations operation, string user, string ipAddress, T newObj, T prevObj)
            where T : IAuditable
        {
            Add(operation, user, ipAddress, 
                newObj?.ToAuditObjectId(), newObj?.ToAuditObjectTypeName(),
                newObj.ToAuditJson(), prevObj.ToAuditJson()
                );
            using (DbEntities db = new DbEntities())
            {
                var a = new Audit();
                a.date = DateTime.Now;
                a.objectId = newObj?.ToAuditObjectId();
                a.objectType = newObj?.ToAuditObjectTypeName();
                a.operation = operation.ToString();
                a.IP = ipAddress;
                a.userId = user ?? "";
                a.valueBefore = prevObj?.ToAuditJson() ?? null;
                a.valueAfter = newObj?.ToAuditJson() ?? null;
                db.Audit.Add(a);
                db.SaveChanges();
                return a;
            }
        }

        public static Audit Add(Operations operation, string user, string ipAddress, 
            string objectId, string objectType,
            string newObjSer, string prevObjSer)
        {
            using (DbEntities db = new DbEntities())
            {
                var a = new Audit();
                a.date = DateTime.Now;
                a.objectId = objectId;
                a.objectType = objectType;
                a.operation = operation.ToString();
                a.IP = ipAddress;
                a.userId = user ?? "";
                a.valueBefore = prevObjSer;
                a.valueAfter = newObjSer;
                db.Audit.Add(a);
                db.SaveChanges();
                return a;
            }
        }
    }
}
