using System;
using System.Runtime.Serialization;

namespace HlidacStatu.Lib.OCR.Api
{
    public partial class Client
    {
        public class ApiException : ApplicationException
        {
            public ApiException()
            {
            }

            public ApiException(string message) : base(message)
            {
            }

            public ApiException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected ApiException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }



    }
}
