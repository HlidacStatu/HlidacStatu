using System;

namespace HlidacStatu.Lib.Data.External.DataSets
{


    public class ApiResponseStatus
    {
        public static ApiResponseStatus DatasetRegistered = Error(-1, "dataset is already registered.");
        public static ApiResponseStatus DatasetNotFound = Error(-2, "dataset not found.");
        public static ApiResponseStatus DatasetNoPermision = Error(-3, "You don't have permissions for this operation.");
        public static ApiResponseStatus DatasetJsonSchemaMissing = Error(-4, "JSON Schema missing.");
        public static ApiResponseStatus DatasetJsonSchemaError = Error(-7, "Invalid JSON Schema.");
        public static ApiResponseStatus DatasetSearchTemplateError = Error(-5, "Invalid search template. Errors: ");
        public static ApiResponseStatus DatasetDetailTemplateError = Error(-6, "Invalid detail template. Errors: ");

        public static ApiResponseStatus InvalidSearchQuery = Error(-10, "Invalid search query .");
        public static ApiResponseStatus DatasetItemInvalidFormat = Error(-11, "Json schema validation failed.");
        public static ApiResponseStatus DatasetItemNotFound = Error(-12, "dataset Item not found.");
        public static ApiResponseStatus DatasetItemNoSetID = Error(-13, "Object ID isn't set.");
        public static ApiResponseStatus DatasetItemSaveError = Error(-14, "Item is not set. Some error during saving.");

        public static ApiResponseStatus InvalidFormat = Error(-97, "Invalid format .");
        public static ApiResponseStatus ApiUnauthorizedAccess = Error(-98, "Unauthorized access.");
        public static ApiResponseStatus GeneralExceptionError(Exception ex)
        {
            return Error(-99, "General exception. Something wrong deep inside.", ex?.ToString());
        }

        public static ApiResponseStatus StatniWebNotFound = Error(-91, "Web not found.");

        public static ApiResponseStatus Valid(object value = null) { return new ApiResponseStatus() { valid = true, value=value }; }
        public static ApiResponseStatus Error(int number, string description, string errorDetail = null)
        {
            return Error(new ErrorInfo()
            {
                number = number,
                description = description,
                errorDetail = errorDetail
            });
        }

        public static ApiResponseStatus Error(ErrorInfo error)
        {
            return new ApiResponseStatus()
            {
                valid = false,
                error = error
            };
        }

        public bool valid { get; set; }
        public object value { get; set; }
        public ErrorInfo error { get; set; }
        public class ErrorInfo
        {
            public int number { get; set; }
            public string description { get; set; }
            public string errorDetail { get; set; }

            public override string ToString()
            {
                return $"{number}: {description}"
                    + (string.IsNullOrEmpty(errorDetail) ? "" : "\n" + errorDetail);
            }
        }

        public override string ToString()
        {
            return $@"{(valid ? "VALID response" : "INVALID response")}
Value: {value?.ToString()}
ErrorNumber: {error?.number}
ErrorDescription: {error?.description}
ErrorDetail: {error?.errorDetail}
";
        }
    }



}
