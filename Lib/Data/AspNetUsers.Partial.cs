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
                using (Devmasters.Core.PersistLib p = new Devmasters.Core.PersistLib())
                {
                    var res = p.ExecuteScalar(Devmasters.Core.Util.Config.GetConfigValue("CnnString"),
                         System.Data.CommandType.Text, "select id from aspnetroles  where name = @role",
                         new System.Data.IDataParameter[] { new System.Data.SqlClient.SqlParameter("role", role) });
                    if (Devmasters.Core.PersistLib.IsNull(res))
                        return false;
                }

                return db.AspNetUserRoles.Any(m => m.UserId == this.Id && m.RoleId == roleId);
            }
        }
    }
}
