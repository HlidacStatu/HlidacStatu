using System.Net.Http;
using Newtonsoft.Json;

namespace HlidacStatu.Web.Models.Apiv2
{
    /// <summary>
    /// 
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class ErrorMessage : HttpResponseMessage
    {

        private ErrorMessage() { }
        public ErrorMessage(Lib.Data.External.DataSets.ApiResponseStatus apiresponse) 
            : this(System.Net.HttpStatusCode.BadRequest, apiresponse.error?.ToString(), apiresponse.value)
        { }

        public ErrorMessage(System.Net.HttpStatusCode statusCode, string error, dynamic detail = null)
        {
            this.Error = error;
            if (detail != null)
                this.Detail = detail;

            this.Content =new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(new { Error = this.Error, Detail = this.Detail })
                );
        }
        /// <summary>
        /// Gets or Sets error
        /// </summary>
        public string Error { get; private set; }
        
        /// <summary>
        /// Object with more details about error.
        /// </summary>
        public dynamic Detail { get; private set; }
       

        /// <summary>
        /// Returns error message
        /// </summary>
        /// <returns>string message</returns>
        public override string ToString()
        {
            return Error;
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
