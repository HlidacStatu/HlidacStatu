using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data;

namespace HlidacStatu.Web.Framework
{
    public interface IAuthenticableController
    {
        System.Security.Principal.IPrincipal User { get; }
        string HostIpAddress { get; }
        string AuthToken { get; }
        
        //used only in apiV2
        ApiAuth.Result ApiAuth { get; set; }

        AspNetUser AuthUser();
    }
}
