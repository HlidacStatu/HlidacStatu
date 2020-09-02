using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using System.Web.Http;
using System.Net.Http;
using System.Linq;
using System.Net;
using HlidacStatu.Util;
using System;
using System.IO;
using System.Web.Http.Description;
using HlidacStatu.Web.Framework;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;

namespace HlidacStatu.Web.Controllers
{
    [SwaggerControllerTag("KIndex")]
    [RoutePrefix("api/v2/kindex")]
    public class ApiV2KIndexController : ApiV2AuthController
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
        [AuthorizeAndAudit(Roles = "Admin")]
        [HttpGet, Route("stats")]
        [SwaggerOperation(Tags = new[] { "Private" })]
        public string Stats(int rok)
        {
            var stat = HlidacStatu.Lib.Analysis.KorupcniRiziko.Statistics.GetStatistics(rok);
            if (stat != null)
            {
                List<string> lines = new List<string>();
                foreach (var item in stat.SubjektOrderedListKIndexAsc)
                {
                    lines.Add(
                        item.ico + "\t"
                        + item.kindex
                        );
                }
                return lines.Aggregate((f, s) => f + "\n" + s);
            }
            else
                return "";
        }


    }
}
