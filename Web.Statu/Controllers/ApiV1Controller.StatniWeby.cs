using Devmasters.Core;
using HlidacStatu.Lib.Data.External.DataSets;
using HlidacStatu.Lib.Data.External.Zabbix;
using HlidacStatu.Web.Framework;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{

    [GZipOrDeflate()]
    public partial class ApiV1Controller : Controllers.GenericAuthController
    {


        [GZipOrDeflate()]
        public ActionResult WebList()
        {
            if (!Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                })
                .Authentificated)
            {
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                    HlidacStatu.Lib.Data.External.Zabbix.ZabTools.Weby()
                    ), "text/json");
            }
        }

        [GZipOrDeflate()]
        public ActionResult WebStatus(string id, string h)
        {
            if (!Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("id", id)
                })
                .Authentificated)
            {
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (Devmasters.Core.TextUtil.IsNumeric(id))
                    return _DataHost(Convert.ToInt32(id), h);
                else
                    return Json(ApiResponseStatus.StatniWebNotFound, JsonRequestBehavior.AllowGet);
            }
        }

        private ActionResult _DataHost(int id, string h)
        {
            ZabHost host = ZabTools.Weby().Where(w => w.hostid == id.ToString() & w.itemIdResponseTime != null).FirstOrDefault();
            if (host == null)
                return Json(ApiResponseStatus.StatniWebNotFound, JsonRequestBehavior.AllowGet);

            if (host.ValidHash(h))
            {
                try
                {
                    var data = ZabTools.GetHostAvailabilityLong(host);
                    var webssl = ZabTools.SslStatusForHostId(host.hostid);
                    var ssldata = new
                    {
                        grade = webssl?.Status().ToNiceDisplayName(),
                        time = webssl?.Time,
                        copyright = "(c) © Qualys, Inc. https://www.ssllabs.com/",
                        fullreport = "https://www.ssllabs.com/ssltest/analyze.html?d=" + webssl?.Host?.UriHost()
                    };
                    if (webssl == null)
                    {
                        ssldata = null;
                    }
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                        new
                        {
                            availability = data,
                            ssl = ssldata
                        })
                        , "text/json");

                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Error($"_DataHost id ${id}", e);
                    return Json(ApiResponseStatus.GeneralExceptionError(e), JsonRequestBehavior.AllowGet);
                }

            }
            else
                return Json(ApiResponseStatus.StatniWebNotFound, JsonRequestBehavior.AllowGet);
        }




    }
}


