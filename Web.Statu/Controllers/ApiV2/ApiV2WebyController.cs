using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using HlidacStatu.Util;
using System;
using System.IO;
using System.Web.Http.Description;
using HlidacStatu.Web.Framework;
using System.Linq;
using HlidacStatu.Lib.Data.External.Zabbix;
using Devmasters.Core;
using Devmasters.Enums;

namespace HlidacStatu.Web.Controllers
{
    [SwaggerControllerTag("Weby")]
    [RoutePrefix("api/v2/Weby")]
    public class ApiV2WebyController : ApiV2AuthController
    {
        public const int DefaultResultPageSize = 25;
        public const int MaxResultsFromES = 5000;

        /*
        Atributy pro API
        [SwaggerOperation(Tags = new[] { "Beta" })] - zarazeni metody do jine skupiny metod, pouze na urovni methody
        [ApiExplorerSettings(IgnoreApi = true)] - neni videt v dokumentaci, ani ve swagger file
        [SwaggerControllerTag("Core")] - Tag pro vsechny metody v controller
        */




        // /api/v2/{id}
        [AuthorizeAndAudit]
        [HttpGet, Route()]
        [GZipOrDeflate()]
        public Lib.Data.External.Zabbix.ZabHost[] List()
        {
            return HlidacStatu.Lib.Data.External.Zabbix.ZabTools.Weby().ToArray();
        }

        [AuthorizeAndAudit]
        [HttpGet, Route("{id?}")]
        [GZipOrDeflate()]
        public WebStatusExport Status(string id = null)
        {
            if (string.IsNullOrEmpty(id))
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest,
                    $"Web nenalezen"
                    ));

            ZabHost host = ZabTools.Weby().Where(w => w.hostid == id.ToString() & w.itemIdResponseTime != null).FirstOrDefault();
            if (host == null)
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest,
                    $"Web nenalezen"
                    ));

            try
            {
                ZabHostAvailability data = ZabTools.GetHostAvailabilityLong(host);
                ZabHostSslStatus webssl = ZabTools.SslStatusForHostId(host.hostid);
                var ssldata = new WebStatusExport.SslData()
                {
                    Grade = webssl?.Status().ToNiceDisplayName(),
                    LatestCheck = webssl?.Time,
                    Copyright = "(c) © Qualys, Inc. https://www.ssllabs.com/",
                    FullReport = "https://www.ssllabs.com/ssltest/analyze.html?d=" + webssl?.Host?.UriHost()
                };
                if (webssl == null)
                {
                    ssldata = null;
                }
                return
                    new WebStatusExport()
                    {
                        Availability = data,
                        SSL = ssldata
                    };

            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error($"_DataHost id ${id}", e);
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest,
                    $"Interní chyba při načítání systému."
                    ));
            }
        }


    }
}
