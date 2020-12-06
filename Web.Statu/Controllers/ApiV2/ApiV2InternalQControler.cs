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
    }

    [SwaggerControllerTag("Voice 2 Text")]
    [RoutePrefix("api/v2/internalq")]
    public class ApiV2InternalQController : ApiV2AuthController
    {



        /// <summary>
        /// Vrátí taskID pro Voice2Text Docker image
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
        public string Voice2TextGetTask()
        {
            using (HlidacStatu.Q.Simple.Queue<Voice2text> sq = new Q.Simple.Queue<Voice2text>(Voice2text.QName, Devmasters.Config.GetWebConfigValue("RabbitMqConnectionString")))
            {
                var task = sq.Get();
                if (task == null)
                {
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NoContent, $"No taks available"));
                }
                return $"{task.dataset}/{task.itemid}";
            }
        }

        /// <summary>
        /// Potvrdí ukončení Voice2Text operace
        /// </summary>
        /// <returns>taskid</returns>
        [AuthorizeAndAudit(Roles = "Admin,internalQ")]
        [HttpGet, Route("Voice2TextDone/{taskId}")]
        public string Voice2TextDone(string taskId)
        {
            using (HlidacStatu.Q.Simple.Queue<Voice2text> sq = new Q.Simple.Queue<Voice2text>(Voice2text.QName+"_done", Devmasters.Config.GetWebConfigValue("RabbitMqConnectionString")))
            {
                var t = taskId.Split('/');
                sq.Send(new Voice2text() { dataset=t[0], itemid=t[1]  });
                return $"Thanks";
            }
        }


    }

}
