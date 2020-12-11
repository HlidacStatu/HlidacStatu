using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using Newtonsoft.Json;
using HlidacStatu.Lib.Data.External.DataSets;
using System;
using System.Linq;
using System.Web.Http;
using System.Collections.Generic;
using System.Web.Http.Description;
using Swashbuckle.Swagger.Annotations;
using HlidacStatu.Web.Framework;

namespace HlidacStatu.Web.Controllers
{

    public class Voice2text
    {
        public const string QName = "voice2text";
        public string dataset { get; set; }
        public string itemid { get; set; }
        public ulong internaltaskid { get; set; }
    }

    [SwaggerControllerTag("Voice 2 Text")]
    [RoutePrefix("api/v2/internalq")]
    public class ApiV2InternalQController : ApiV2AuthController
    {



        /// <summary>
        ///  Vytvori novy task
        /// </summary>
        /// <returns>taskid</returns>
        [AuthorizeAndAudit(Roles = "Admin,internalQ")]
        [HttpPost, Route("Voice2TextNewTask/{datasetId}/{itemId}")]
        public string Voice2TextNewTask(string datasetId, string itemId)
        {
            using (HlidacStatu.Q.Simple.Queue<Voice2text> sq = new Q.Simple.Queue<Voice2text>(
                Voice2text.QName,
                Devmasters.Config.GetWebConfigValue("RabbitMqConnectionString"))
                )
            {
                sq.Send(new Voice2text() { dataset = datasetId, itemid = itemId });
                return $"OK";
            }
        }


        /// <summary>
        /// Vrátí taskID pro Voice2Text Docker image
        /// </summary>
        /// <returns>taskid</returns>
        [AuthorizeAndAudit(Roles = "Admin,internalQ")]
        [HttpGet, Route("Voice2TextGetTask")]
        public Voice2text Voice2TextGetTask()
        {
            using (HlidacStatu.Q.Simple.Queue<Voice2text> sq = new Q.Simple.Queue<Voice2text>(Voice2text.QName, Devmasters.Config.GetWebConfigValue("RabbitMqConnectionString")))
            {
                var task = sq.GetAndAck();
                if (task == null )
                {
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NoContent, $"No taks available"));
                }
                return task;

            }
        }

        /// <summary>
        /// Potvrdí ukončení Voice2Text operace
        /// </summary>
        /// <returns>taskid</returns>
        [AuthorizeAndAudit(Roles = "Admin,internalQ")]
        [HttpPost, Route("Voice2TextDone")]
        public string Voice2TextDone([FromBody] Voice2text task)
        {
            using (HlidacStatu.Q.Simple.Queue<Voice2text> sq
                = new Q.Simple.Queue<Voice2text>(Voice2text.QName + "_done", Devmasters.Config.GetWebConfigValue("RabbitMqConnectionString")))
            {
                task.internaltaskid = 0;
                sq.Send(task);
            }
            

            return $"OK";
        }

        /// <summary>
        /// Potvrdí ukončení Voice2Text operace
        /// </summary>
        /// <returns>taskid</returns>
        [AuthorizeAndAudit(Roles = "Admin,internalQ")]
        [HttpPost, Route("Voice2TextFailed/{requeueAsTheLast}")]
        public string Voice2TextFailed(bool requeueAsTheLast, [FromBody] Voice2text task)
        {
            using (HlidacStatu.Q.Simple.Queue<Voice2text> sq
                = new Q.Simple.Queue<Voice2text>(Voice2text.QName + "_failed", Devmasters.Config.GetWebConfigValue("RabbitMqConnectionString")))
            {
                sq.Send(task);
            }

            return "OK";
        }
    }

}
