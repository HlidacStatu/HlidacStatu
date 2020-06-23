using HlidacStatu.Lib.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;


namespace HlidacStatu.Web.Framework
{


    public class ApiCall : Audit.IAuditable
    {
        public class CallParameter
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public CallParameter() { }
            public CallParameter(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }

            public static implicit operator CallParameter(KeyValuePair<string, string> kv)
            {
                return new CallParameter() { Name = kv.Key, Value = kv.Value };
            }

        }
        public string Method { get; set; }
        public string Id { get; set; }
        public IEnumerable<CallParameter> Parameters { get; set; }

        /// <summary>
        /// User's email
        /// </summary>
        public string User { get; set; }
        public string UserId { get; set; }

        public string IP { get; set; }

        public string[] UserRoles { get; set; } = new string[] { };


        public string ToAuditJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string ToAuditObjectId()
        {
            return this.Id;
        }

        public string ToAuditObjectTypeName()
        {
            return "ApiCall";
        }
    }


    public class ApiAuth
    {
        private static string[] dontAuditMethods = new string[] { "getforclassification", "classificationlist" };
        public class Result
        {
            public static ApiAuth.Result Valid(ApiCall apiCall)
            {

                if (!dontAuditMethods.Contains(apiCall.Method.ToLower()))
                {
                    Audit.Add(Audit.Operations.Call, apiCall.User, apiCall.IP, apiCall, null);
                }
                return new ApiAuth.Result(true, apiCall);
            }
            public static ApiAuth.Result Invalid(ApiCall apiCall)
            {
                if (!string.IsNullOrEmpty(apiCall?.User))
                {
                    Audit.Add(Audit.Operations.InvalidAccess, apiCall.User, apiCall.IP, apiCall, null);
                }
                else
                {
                    Audit.Add(Audit.Operations.InvalidAccess, apiCall.User, apiCall.IP, apiCall, null);
                }
                return new ApiAuth.Result(false, apiCall);
            }

            private Result(bool valid, ApiCall apiCall)
            {
                Authentificated = valid;
                ApiCall = apiCall;
            }
            public bool Authentificated { get; private set; } = false;
            public ApiCall ApiCall { get; private set; } = null;

        }




        public static ApiAuth.Result IsApiAuth(IAuthenticableController c, string validRole = null, IEnumerable<ApiCall.CallParameter> parameters = null, [CallerMemberName] string method = "")
        {
            if (string.IsNullOrEmpty(validRole))
                return IsApiAuth(c, new string[] { }, parameters, method);
            else
                return IsApiAuth(c, validRole.Split(','), parameters, method);
        }

        public static ApiAuth.Result IsApiAuth(IAuthenticableController c, string[] validRoles, IEnumerable<ApiCall.CallParameter> parameters = null, [CallerMemberName] string method = "")
        {
            var usrmgr = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            string login = null;
            if (IsApiAuthHeader(c.AuthToken, out login))
            {
                Microsoft.AspNet.Identity.EntityFramework.IdentityUser user = usrmgr.FindByEmail(login);
                if (user == null)
                    return ApiAuth.Result.Invalid(new ApiCall() { IP = c.HostIpAddress, UserId=null, User = null, Id = method, Method = method, Parameters = parameters });
                else
                {
                    string[] userroles = usrmgr.GetRoles(user.Id).ToArray();

                    if (validRoles == null)
                        return ApiAuth.Result.Valid(new ApiCall() { IP = c.HostIpAddress, UserId = user.Id, User = user.Email, Id = method, Method = method, Parameters = parameters, UserRoles = userroles });
                    else if (validRoles.Count() == 0)
                        return ApiAuth.Result.Valid(new ApiCall() { IP = c.HostIpAddress, UserId = user.Id, User = user.Email, Id = method, Method = method, Parameters = parameters, UserRoles = userroles });
                    else
                    {
                        foreach (var role in validRoles)
                        {
                            if (usrmgr.IsInRole(user.Id, role.Trim()))
                                return ApiAuth.Result.Valid(new ApiCall() { IP = c.HostIpAddress, UserId = user.Id, User = user.Email, Id = method, Method = method, Parameters = parameters, UserRoles = userroles });
                        }
                        return ApiAuth.Result.Invalid(new ApiCall() { IP = c.HostIpAddress, UserId = user.Id, User = user.Email, Id = method, Method = method, Parameters = parameters, UserRoles = userroles });
                    }

                }

            }
            else if (c.User?.Identity?.IsAuthenticated == true)
            {
                Microsoft.AspNet.Identity.EntityFramework.IdentityUser user = usrmgr.FindByEmail(c.User.Identity.Name);

                string[] userroles = usrmgr.GetRoles(user.Id).ToArray();

                if (validRoles == null)
                    return ApiAuth.Result.Valid(new ApiCall() { IP = c.HostIpAddress, UserId = user?.Id, User = c.User.Identity.Name, Id = method, Method = method, Parameters = parameters, UserRoles = userroles });
                else if (validRoles.Count() == 0)
                    return ApiAuth.Result.Valid(new ApiCall() { IP = c.HostIpAddress, UserId = user?.Id, User = c.User.Identity.Name, Id = method, Method = method, Parameters = parameters, UserRoles = userroles });
                else
                {
                    foreach (var role in validRoles)
                    {
                        if (c.User.IsInRole(role.Trim()))
                            return ApiAuth.Result.Valid(new ApiCall() { IP = c.HostIpAddress, UserId = user?.Id, User = c.User.Identity.Name, Id = method, Method = method, Parameters = parameters, UserRoles = userroles });
                    }
                    return ApiAuth.Result.Invalid(new ApiCall() { IP = c.HostIpAddress, UserId = user?.Id, User = c.User.Identity.Name, Id = method, Method = method, Parameters = parameters, UserRoles = userroles });
                }
            }
            else
                return ApiAuth.Result.Invalid(new ApiCall() { IP = c.HostIpAddress, UserId=null, User = null, Id = method, Method = method, Parameters = parameters });


        }


        private static bool IsApiAuthHeader(string authToken, out string login)
        {
            login = "";
            
            if (string.IsNullOrEmpty(authToken))
                return false;
            authToken = authToken.Replace("Token ", "").Trim();

            Guid t = Guid.Empty;

            if (Guid.TryParse(authToken, out t))
            {
                using (HlidacStatu.Lib.Data.DbEntities db = new DbEntities())
                {
                    var user = db.AspNetUserTokens.Where(m => m.Token == t).FirstOrDefault();
                    if (user != null)
                        login = db.AspNetUsers.Where(m => m.Id == user.Id).FirstOrDefault()?.Email;
                    return user != null;
                }
            }
            else
                return false;
        }
    
    }
}