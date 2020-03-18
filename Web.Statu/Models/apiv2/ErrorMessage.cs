using Newtonsoft.Json;

namespace HlidacStatu.Web.Models.Apiv2
{
    /// <summary>
    /// 
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class ErrorMessage
    { 
        /// <summary>
        /// Gets or Sets error
        /// </summary>
        public string Error { get; set; }
        
        /// <summary>
        /// Object with more details about error.
        /// </summary>
        public dynamic Detail { get; set; }

        public ErrorMessage(string error)
        {
            Error = error;
        }

        public ErrorMessage(string error, dynamic obj)
        {
            Error = error;
            Detail = obj;
        }

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
