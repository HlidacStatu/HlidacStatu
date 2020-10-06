using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Dynamic;

namespace ZabbixApi
{


    //Main
    public class Zabbix
    {
        public Zabbix(string user, string password, string zabbixURL, bool basicAuth)
        {
            this.user = user;
            this.password = password;
            this.zabbixURL = zabbixURL;
            if (basicAuth) this.basicAuth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(this.user + ":" + this.password));
            auth = null;
        }

        public Zabbix(string user, string password, string zabbixURL) : this(user, password, zabbixURL, false) { }

        private string user;
        private string password;
        private string zabbixURL;
        private string auth;
        private string basicAuth = null;
        public bool loggedOn = false;

        public void login()
        {
            dynamic userAuth = new ExpandoObject();
            userAuth.user = user;
            userAuth.password = password;
            Response zbxResponse = objectResponse("user.login", userAuth);

            if (zbxResponse.result != null && zbxResponse.result.GetType().Name == "String")
            {
                auth = zbxResponse.result;
                if (!string.IsNullOrEmpty(auth))
                {
                    loggedOn = true;
                    return;
                }

            }

            loggedOn = false;

        }

        public bool logout()
        {
            Response zbxResponse = objectResponse("user.logout", new string[] { });
            var result = zbxResponse.result;
            return result;
        }

        public string jsonResponse(string method, object parameters)
        {
            Request zbxRequest = new Request("2.0", method, 1, auth, parameters);
            string jsonParams = JsonConvert.SerializeObject(zbxRequest);
            return sendRequest(jsonParams);
        }


        public Response objectResponse(string method, object parameters)
        {
            Request zbxRequest = new Request("2.0", method, 1, auth, parameters);
            string jsonParams = JsonConvert.SerializeObject(zbxRequest);
            return createResponse(sendRequest(jsonParams));
        }



        private Response createResponse(string json)
        {
            Response zbxResponse = JsonConvert.DeserializeObject<Response>(json);
            return zbxResponse;
        }

        private string sendRequest(string jsonParams)
        {
            try
            {

                WebRequest request = WebRequest.Create(zabbixURL);
                if (basicAuth != null) request.Headers.Add("Authorization", "Basic " + basicAuth);
                request.ContentType = "application/json-rpc";
                request.Method = "POST";
                string jsonResult;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jsonParams);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                WebResponse response = request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    jsonResult = streamReader.ReadToEnd();
                }

                return jsonResult;
            }
            catch
            {

                throw;
            }
        }

    }
}
