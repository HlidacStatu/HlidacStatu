using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class AspNetUser
    {

        public static AspNetUser GetByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;
            using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
            {
                return db.AspNetUsers.AsNoTracking().FirstOrDefault(m=>m.Email == email);
            }
        }
        public string GetAPIToken()
        {
            return HlidacStatu.Lib.Data.AspNetUserToken.GetToken(this.Email).Token.ToString("N");
        }

        public bool IsInRole(string role)
        {
            using (DbEntities db = new DbEntities())
            {
                //var roleId = db.AspNetRoles.Where(m => m.Name == role).FirstOrDefault();
                string roleId = null;
                using (Devmasters.PersistLib p = new Devmasters.PersistLib())
                {
                    var res = p.ExecuteScalar(Devmasters.Config.GetWebConfigValue("CnnString"),
                         System.Data.CommandType.Text, "select id from aspnetroles  where name = @role",
                         new System.Data.IDataParameter[] { new System.Data.SqlClient.SqlParameter("role", role) });
                    if (Devmasters.PersistLib.IsNull(res))
                        return false;
                }

                return db.AspNetUserRoles.Any(m => m.UserId == this.Id && m.RoleId == roleId);
            }
        }


        object _watchdogAllInOneLock = new object();
        private WatchdogAllInOne _watchdogAllInOne = null;
        public bool SentWatchdogOneByOne
        {
            get
            {
                InitWatchdogAllInOne();
                return _watchdogAllInOne.GetValue();
            }
            set {
                InitWatchdogAllInOne();
                _watchdogAllInOne.SetValue(value);
                _watchdogAllInOne.Save();
            }
        }
        private void InitWatchdogAllInOne()
        {
            if (_watchdogAllInOne == null)
            {
                lock (_watchdogAllInOneLock)
                {
                    if (_watchdogAllInOne == null)
                        _watchdogAllInOne = new WatchdogAllInOne(this);
                }
            }
        }

        public class WatchdogAllInOne : HlidacStatu.Lib.Data.UserOptions<bool>
        {

            public WatchdogAllInOne(AspNetUser user)
                : base(user, ParameterType.WatchdogAllInOne, null)
            {
            }

            protected override string SerializeToString(bool value)
            {
                return value ? "1" : "0";
            }

            protected override bool DeserializeFromString(string value)
            {
                if (value == null)
                    return false;

                if (value == "1")
                    return true;
                else 
                    return false;
            }
        }

    }
}
