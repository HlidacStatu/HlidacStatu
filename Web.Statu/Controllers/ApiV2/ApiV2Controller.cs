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

namespace HlidacStatu.Web.Controllers
{
    [SwaggerControllerTag("Core")]
    [RoutePrefix("api/v2")]
    public class ApiV2Controller : ApiV2AuthController
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
        [HttpGet, Route("ping/{text}")]
        public string Ping(string text)
        {
            return "pong " + text;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [AuthorizeAndAudit]
        [HttpGet, Route("geterror/{id}")]
        public string GetError([FromUri]int? id = 200)
        {
            throw new HttpResponseException(new ErrorMessage((System.Net.HttpStatusCode)id, $"error {id}"));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [AuthorizeAndAudit]
        [HttpGet, Route("getmyip")]
        public string GetIp()
        {
            return this.ApiAuth.ApiCall.IP;
        }

        [AuthorizeAndAudit]
        [HttpGet, Route("dumps")]
        public Models.ApiV1Models.DumpInfoModel[] Dumps()
        {
            return Framework.Api.Dumps.GetDumps();
        }

        [AuthorizeAndAudit]
        [HttpGet, Route("dump/{datatype}/{date?}")]
        public HttpResponseMessage Dump(string datatype, string date)
        {

            DateTime? specificDate = Devmasters.DT.Util.ToDateTime(date, "yyyy-MM-dd");
            string onlyfile = $"{datatype}.dump" + (specificDate.HasValue ? "-" + specificDate.Value.ToString("yyyy-MM-dd") : "");
            string fn = HlidacStatu.Lib.StaticData.Dumps_Path + $"{onlyfile}" + ".zip";

            if (System.IO.File.Exists(fn))
            {
                long FileL = (new FileInfo(fn)).Length;
                byte[] bytes = new byte[1024 * 1024];
                try
                {
                    using (FileStream FS = System.IO.File.OpenRead(fn))
                    {
                        HttpResponseMessage response = new HttpResponseMessage();
                        response.StatusCode = HttpStatusCode.OK;
                        response.Content = new StreamContent(FS, 1024 * 64);
                        response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                        {
                            FileName = System.IO.Path.GetFileName(fn)
                        };
                        return response;

                    }

                }
                catch (System.Web.HttpException wex)
                {
                    if (wex.Message.StartsWith("The remote host closed the connection"))
                    {
                        //ignore
                    }
                    else
                        HlidacStatu.Util.Consts.Logger.Error("DUMP?" + date, wex);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Error("DUMP exception?" + date, e);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
        }
            else
            {
                Util.Consts.Logger.Error("API DUMP : not found file " + fn);
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Dump {datatype} for date:{date} nenalezen."));
            }


        }

    }
}
