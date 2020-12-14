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
using HlidacStatu.Q.Simple.Tasks;

namespace HlidacStatu.Web.Controllers
{


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
            using (HlidacStatu.Q.Simple.Queue<Voice2Text> sq = new Q.Simple.Queue<Voice2Text>(
                Voice2Text.QName,
                Devmasters.Config.GetWebConfigValue("RabbitMqConnectionString"))
                )
            {
                sq.Send(new Voice2Text() { dataset = datasetId, itemid = itemId });
                return $"OK";
            }
        }


        /// <summary>
        /// Vrátí taskID pro Voice2Text Docker image
        /// </summary>
        /// <returns>taskid</returns>
        [AuthorizeAndAudit(Roles = "Admin,internalQ")]
        [HttpGet, Route("Voice2TextGetTask")]
        public Voice2Text Voice2TextGetTask()
        {
            using (HlidacStatu.Q.Simple.Queue<Voice2Text> sq = new Q.Simple.Queue<Voice2Text>(Voice2Text.QName, Devmasters.Config.GetWebConfigValue("RabbitMqConnectionString")))
            {
                var task = sq.GetAndAck();
                if (task == null)
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
        public string Voice2TextDone([FromBody] Voice2Text task)
        {
            using (HlidacStatu.Q.Simple.Queue<TaskResult<Voice2Text>> sq
                = new Q.Simple.Queue<TaskResult<Voice2Text>>(Voice2Text.QName + "_done", Devmasters.Config.GetWebConfigValue("RabbitMqConnectionString")))
            {
                task.internaltaskid = 0;
                TaskResult<Voice2Text> result = new TaskResult<Voice2Text>()
                {
                    Payload = task,
                    Created = DateTime.Now,
                    Result = "done",
                    User = AuthUser().UserName,
                    FromIP = this.HostIpAddress
                };
                sq.Send(result);
            }


            return $"OK";
        }

        /// <summary>
        /// Potvrdí ukončení Voice2Text operace
        /// </summary>
        /// <returns>taskid</returns>
        [AuthorizeAndAudit(Roles = "Admin,internalQ")]
        [HttpPost, Route("Voice2TextFailed/{requeueAsTheLast}")]
        public string Voice2TextFailed(bool requeueAsTheLast, [FromBody] Voice2Text task)
        {
            using (HlidacStatu.Q.Simple.Queue<TaskResult<Voice2Text>> sq
                = new Q.Simple.Queue<TaskResult<Voice2Text>>(Voice2Text.QName + "_failed", Devmasters.Config.GetWebConfigValue("RabbitMqConnectionString")))
            {
                TaskResult<Voice2Text> result = new TaskResult<Voice2Text>()
                {
                    Payload = task,
                    Created = DateTime.Now,
                    Result = "failed",
                    User = AuthUser().UserName,
                    FromIP = this.HostIpAddress
                };
                sq.Send(result);
            }

            return "OK";
        }
    }

}
