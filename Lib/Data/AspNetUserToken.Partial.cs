using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class AspNetUserToken
    {
        public static AspNetUserToken CreateNew(IdentityUser user)
        {
            return CreateNew(user.Id);
        }
        public static AspNetUserToken CreateNew(string userId)
        {
            using (Lib.Data.DbEntities db = new DbEntities())
            {

                var t = new AspNetUserToken() { Id = userId, Count = 0, Created = DateTime.Now, LastAccess = null, Token = Guid.NewGuid() };
                db.AspNetUserTokens.Add(t);
                db.SaveChanges();
                return t;
            }
        }

        public static AspNetUserToken GetToken(string username)
        {
            using (Lib.Data.DbEntities db = new DbEntities())
            {

                var user = db.AspNetUsers
                    .Where(m => m.UserName == username)
                    .FirstOrDefault();
                if (user == null)
                    return CreateNew(user.Id);

                var token = db.AspNetUserTokens
                    .Where(m => m.Id == user.Id)
                    .FirstOrDefault();
                if (token == null)
                    return CreateNew(user.Id);
                else
                    return token;
            }

        }

    }
}
