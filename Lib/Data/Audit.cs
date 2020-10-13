using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{


    public partial class Audit
    {
        [Nest.Keyword]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        [Nest.Date]
        public System.DateTime date { get; set; }
        [Nest.Keyword]
        public string userId { get; set; }
        [Nest.Keyword]
        public string operation { get; set; }
        [Nest.Keyword]
        public string objectType { get; set; }
        [Nest.Keyword]
        public string objectId { get; set; }
        public string valueBefore { get; set; }
        public string valueAfter { get; set; }
        [Nest.Keyword]
        public string IP { get; set; }

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
            Search,
            UserSearch
        }
        public static Audit Add<T>(Operations operation, string user, T newObj, T prevObj)
            where T : IAuditable
        {
            return Add<T>(operation, user, null, newObj, prevObj);
        }
        public static Audit Add<T>(Operations operation, string user, string ipAddress, T newObj, T prevObj)
            where T : IAuditable
        {
            return Add(operation, user, ipAddress,
                newObj?.ToAuditObjectId(), newObj?.ToAuditObjectTypeName(),
                newObj?.ToAuditJson(), prevObj?.ToAuditJson()
                );
        }

        public static Audit Add(Operations operation, string user, string ipAddress,
            string objectId, string objectType,
            string newObjSer, string prevObjSer)
        {
            try
            {
                var a = new Audit();
                a.date = DateTime.Now;
                a.objectId = objectId;
                a.objectType = objectType;
                a.operation = operation.ToString();
                a.IP = ipAddress;
                a.userId = user ?? "";
                a.valueBefore = prevObjSer;
                a.valueAfter = newObjSer ?? "";

                if (operation == Operations.Search)
                {
                    return a;
                }

                var res = HlidacStatu.Lib.ES.Manager.GetESClient_Audit()
                .Index<Audit>(a, m => m.Id(a.Id));

                return a;
            }
            catch
            {
                return null;
            }

        }
    }
}
