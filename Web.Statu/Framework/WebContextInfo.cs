using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HlidacStatu.Web.Framework
{
    public class WebContextInfo
    {
        public static string GetFullInfoString(HttpContextBase context)
        {
            return GetFullInfo(context)
                .Select(m => m.Key + ": " + m.Value)
                .Aggregate((f, s) => f + "\n" + s);
        }
        
        public static List<KeyValuePair<string, string>> GetFullInfo(HttpContextBase context)
        {
            List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>(50);
            if (context == null)
                return ret;

            DateTime errorTime = DateTime.UtcNow;

            ret.Add(new KeyValuePair<string, string>("Date:", errorTime.ToString("r")));
            ret.Add(new KeyValuePair<string, string>("Local Date: ", errorTime.ToLocalTime().ToString("F")));
            ret.Add(new KeyValuePair<string, string>("Host: ", System.Environment.MachineName));
            ret.Add(new KeyValuePair<string, string>("Url", context.Request.HttpMethod + " " + context.Request.Url.AbsoluteUri));
            string hostname = context.Request.ServerVariables["REMOTE_ADDR"];
            try
            {
                hostname = System.Net.Dns.GetHostEntry(hostname).HostName;
            }
            catch
            {
            }

            ret.Add(new KeyValuePair<string, string>("Remote Host", context.Request.ServerVariables["REMOTE_ADDR"] + " " + hostname));
            {
                if (context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated)
                    ret.Add(new KeyValuePair<string, string>("Logged User",context.User.Identity.Name));
                else
                    ret.Add(new KeyValuePair<string, string>("Logged User", "Not Logged"));
            }

            if (context.Request != null)
            {

                try
                {
                    foreach (string key in context.Request.Headers.AllKeys)
                    {
                        if (key.ToLower() != "cookie")
                        {
                            ret.Add(new KeyValuePair<string, string>("HTTP Header " + key,context.Request.Headers[key]));
                        }
                    }
                }
                catch
                {
                }

                try
                {
                    foreach (string key in context.Request.Form.AllKeys)
                    {
                        ret.Add(new KeyValuePair<string, string>("POST " + key , context.Request.Form[key]));
                    }

                }
                catch
                {
                }
            

                try
                {
                    foreach (string key in context.Request.Cookies.AllKeys)
                    {
                        ret.Add(new KeyValuePair<string, string>("HTTPCookie " + key ,context.Request.Cookies[key].Value.ToString()));
                    }

                }
                catch
                {
                }

                try
                {
                    foreach (string key in context.Request.ServerVariables.AllKeys)
                    {
                        ret.Add(new KeyValuePair<string, string>("SERVERVar " + key, context.Request.ServerVariables[key].ToString()));
                    }

                }
                catch
                {
                }

                try
                {
                    string tmpf = System.IO.Path.GetTempFileName();
                    context.Request.SaveAs(tmpf, true);
                    ret.Add(new KeyValuePair<string, string>("RAW", System.IO.File.ReadAllText(tmpf)));
                    Devmasters.IO.IOTools.DeleteFile(tmpf);
                }
                catch (Exception)
                {
                }            
            }

            return ret;

        }
    }
}