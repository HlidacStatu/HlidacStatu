using System.Dynamic;

namespace ZabbixApi
{
    //Main
    public class Response
    {
        public Response()
        {
        }

        public string jsonrpc { get; set; }
        public dynamic result = new ExpandoObject();
        public int id { get; set; }
    }
}
