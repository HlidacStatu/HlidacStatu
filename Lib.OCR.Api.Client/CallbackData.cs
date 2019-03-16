using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.OCR.Api
{
    public class CallbackData
    {
        public enum CallbackType
        {
            Url,
            LocalElastic
        }

        public CallbackData(Uri url, String body, CallbackType type)
        {
            this.Url = url.AbsoluteUri;
            this.Type = type;
            this.CallbackDataBody = body;
        }

        public CallbackType Type { get; set; } = CallbackType.Url;
        public string Url { get; set; }
        public string CallbackDataBody { get; set; }

        
        public static string PrepareElasticCallbackDataForOCRReq(string itemPathText, bool addMore = false)
        {
            var ss = @"{
            ""script"": {
                ""source"": ""ctx._source.#ITEMPATHTEXT# = params.txt;#ADDMORE#"",
                ""lang"": ""painless"",
                ""params"" : { ""txt"" : #TEXT# }
            }
        }";

            var res = ss
                .Replace("#ITEMPATHTEXT#", itemPathText);

            if (!addMore)
                res = res.Replace("#ADDMORE#", "");


            return res;
        }
    }
}
