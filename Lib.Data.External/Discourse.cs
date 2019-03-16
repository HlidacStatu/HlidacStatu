using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HlidacStatu.Lib.Data.External
{
    public class Discourse
    {
        //TODO
        static string defaultApiUsername = Devmasters.Core.Util.Config.GetConfigValue("DiscourseAPIUsername"); //
        static string defaultApiKey = Devmasters.Core.Util.Config.GetConfigValue("DiscourseAPIKey");
        static string defaultBaseUrl = FixRootUrl(Devmasters.Core.Util.Config.GetConfigValue("DiscourseAPIBaseUrl")); //"{0}";
        string baseUrlQS = null; //baseUrl + "&api_username={1}&api_key={2}";
        string baseUrl = null;
        //using(var client = new System.Net.WebClient()) {  client.UploadData(address,"PUT",data);}

        string apiUsername = "";
        string apiKey = "";

        private static string FixRootUrl(string url)
        {
            if (!url.EndsWith("/"))
                return url + "/";
            else return url;
        }

        public Discourse(string baseUrl = null)
            : this(defaultApiUsername, defaultApiKey, baseUrl)
        {
        }
        public Discourse(string apiUsrn, string apiKey, string baseUrl = null)
        {
            this.apiKey = apiKey;
            this.apiUsername = apiUsrn;
            baseUrl = baseUrl ?? defaultBaseUrl;
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException("baseUrl");
            baseUrl += "{0}";
            baseUrlQS = baseUrl + "&api_username={1}&api_key={2}"; ;
        }

        public bool IsFreeUsername(string username)
        {
            try
            {
                System.Net.WebClient net = new System.Net.WebClient();
                var ret = Newtonsoft.Json.Linq.JObject.Parse(net.DownloadString(GetUrl("users/" + username + ".json", true)));
                if (ret["error_type"] == null)
                    return false;
                else
                    return true;

            }
            catch (System.Net.WebException ex)
            {
                System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)ex.Response;
                if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {

                throw;
            }

        }
        private string GetUrl(string command, bool withQS)
        {
            if (withQS)
                return string.Format(baseUrlQS, command, this.apiUsername, this.apiKey);
            else
                return string.Format(baseUrl, command);
        }

        public bool InviteNewUser(string email)
        {

            using (Devmasters.Net.Web.URLContent net = new Devmasters.Net.Web.URLContent(GetUrl("invites", false)))
            {

                try
                {

                    //net.ContentType = "application/json";

                    net.Method = Devmasters.Net.Web.MethodEnum.POST;
                    net.RequestParams.Form.Add("api_key", apiKey);
                    net.RequestParams.Form.Add("api_username", apiUsername);

                    net.RequestParams.Form.Add("group_names", "Hlidac-team-members");
                    net.RequestParams.Form.Add("email", email);
                    net.RequestParams.Form.Add("custom_message", @"Ahoj.
Toto je pozvámka do Platforma.Hlidacstatu.cz - veřejné diskuzní platformy s privátní částí pro naše teamové diskuze.
Interně je to vhodný systém pro diskuzi nad konkrétními projekty, nápady, kdy jsou jednotlivá témata diskuze od sebe oddělena (narozdíl od Slacku).\n
Současně je to platforma pro veřejnou diskuzi a kontakt s veřejností.

Michal Bláha");



                    var s = net.GetContent().Text;
                    var ret = Newtonsoft.Json.Linq.JObject.Parse(s);
                    if (ret["success"].ToObject<string>() == "OK")
                        return true;
                    else
                        return false;
                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Error("platforma invitation error", e);
                    return false;
                }
            }
        }

        public bool AddUserIntoHlidacGroup(string username)
        {

            var data = JsonConvert.SerializeObject(
                    new
                    {
                        api_key = apiKey,
                        api_username = "michalblaha",
                        usernames = username
                    }
                    );
            System.Net.WebClient client = new System.Net.WebClient();
            string s = client.UploadString(GetUrl("groups/43/members.json", false), "PUT", data);

            var ret = Newtonsoft.Json.Linq.JObject.Parse(s);
            if (ret["success"].ToObject<bool>())
                return true;
            else
                return false;

        }


        public bool PostToTopic(int topicId, string post)
        {
            using (Devmasters.Net.Web.URLContent net = new Devmasters.Net.Web.URLContent(GetUrl("posts", false)))
            {

                try
                {

                    //net.ContentType = "application/json";

                    net.Method = Devmasters.Net.Web.MethodEnum.POST;
                    net.RequestParams.Form.Add("api_key", apiKey);
                    net.RequestParams.Form.Add("api_username", apiUsername);

                    net.RequestParams.Form.Add("topic_id", topicId.ToString());
                    net.RequestParams.Form.Add("raw", post);


                    var s = net.GetContent().Text;
                    var ret = Newtonsoft.Json.Linq.JObject.Parse(s);
                    return true;
                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Error("PostToTopic error", e);
                    return false;
                }
            }
        }
    }
}
