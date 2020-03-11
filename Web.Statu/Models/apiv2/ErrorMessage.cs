using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace HlidacStatu.Web.Models.Apiv2
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class ErrorMessage
    { 
        /// <summary>
        /// Gets or Sets error
        /// </summary>
        [Required]
        [DataMember(Name="error")]
        public string Error { get; set; }

        public ErrorMessage(string error)
        {
            Error = error;
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
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}
