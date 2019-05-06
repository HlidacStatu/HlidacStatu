using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External.DataSets
{
    public class DataSetException : ArgumentException
    {
        public string DatasetId { get; set; }
        public static DataSetException GetExc(string datasetId, int number, string description, string errorDetail = null)
        {
            return new DataSetException(datasetId, ApiResponseStatus.Error(number, description, errorDetail));
        }

        public ApiResponseStatus APIResponse { get; set; } = null;

        public DataSetException(string datasetId, ApiResponseStatus apiResp)
            : base()
        {
            this.DatasetId = datasetId;
            this.APIResponse = apiResp;
        }

        public DataSetException(string datasetId, string message, ApiResponseStatus apiResp)
            : base(message)
        {
            this.DatasetId = datasetId;
            this.APIResponse = apiResp;
        }

        public DataSetException(string datasetId, string message, ApiResponseStatus apiResp, Exception innerException)
            : base(message, innerException)
        {
            this.DatasetId = datasetId;
            this.APIResponse = apiResp;
        }

        protected DataSetException(string datasetId, ApiResponseStatus apiResp, SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.DatasetId = datasetId;
            this.APIResponse = apiResp;
        }
        public override string Message {
            get {
                return $"Dataset:{this.DatasetId}\nApiResponse:{this.APIResponse.ToString()}\n{base.Message}";
                }
        }
    
        
    }
}
