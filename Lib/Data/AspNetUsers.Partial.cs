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

    }
}
