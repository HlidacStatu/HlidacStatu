using static HlidacStatu.Lib.OCR.Api.Client;

namespace HlidacStatu.Lib.OCR.Api
{
    public class OCRClient
    {

        static string ApiUrl = "https://ocr.hlidacstatu.cz/";
        static OCRClient()
        {
            if (!string.IsNullOrEmpty(Devmasters.Core.Util.Config.GetConfigValue("OCR.ApiUrl")))
                ApiUrl = Devmasters.Core.Util.Config.GetConfigValue("OCR.ApiUrl");
        }



        static Devmasters.Core.Logging.Logger logger = new Devmasters.Core.Logging.Logger("HlidacStatu.Lib.OCR.Api.OCRClient");


        public static OCRTask GetNewTask(string server, Lib.OCR.Api.Client.TaskPriority minPriority = TaskPriority.Lowest, Lib.OCR.Api.Client.TaskPriority maxPriority = TaskPriority.Critical)
        {
            string tmpFile = null;
            try
            {
                using (WebOcr wc = new WebOcr())
                {
                    string param = "server=" + System.Net.WebUtility.UrlEncode(server)
                        + "&minPriority=" + (int)minPriority
                        + "&maxPriority=" + (int)maxPriority;

                    string fullUrl = ApiUrl + "task.ashx?" + param;
                    var data = wc.DownloadString(fullUrl);

                    if (string.IsNullOrEmpty(data))
                        return null;

                    OCRTask task = Newtonsoft.Json.JsonConvert.DeserializeObject<OCRTask>(data);
                    tmpFile= Client.TempIO.GetTemporaryFilename();
                    //get binaryData
                    task.localTempFile = tmpFile + task.OrigFilename;
                    fullUrl = ApiUrl + "task.ashx?taskid=" + task.TaskId;
                    wc.DownloadFile(fullUrl, task.localTempFile);
                    return task;
                }
            }
            catch (System.Exception e)
            {
                logger.Error("GetNewTask error", e);
                return null;
            }
            finally
            {
                System.Threading.Thread.Sleep(50);

                TempIO.DeleteFile(tmpFile);
            }

        }
        public static void MonitorData(string server, short cpu, int memory, int diskfree, short threads, short livethreads)
        {
            try
            {
                logger.Debug($"Sending monitoring of {server} ");

                string fullUrl = ApiUrl + $"ping.ashx?server={server}&cpu={cpu}&memory={memory}&diskfree={diskfree}&threads={threads}&livethreads={livethreads}";
                using (Devmasters.Net.Web.URLContent wc = new Devmasters.Net.Web.URLContent(fullUrl))
                {
                    wc.Method = Devmasters.Net.Web.MethodEnum.GET;
                    wc.TimeInMsBetweenTries = 10000;
                    wc.Timeout = 60000;
                    wc.Tries = 5;
                    var res = wc.GetContent();
                }
            }
            catch (System.Exception e)
            {

                logger.Error("Sending monitoring error", e);
            }

        }


        public static void ResetRunningTasks(string server)
        {
            try
            {
                logger.Debug($"ResetRunningTasks from server {server} ");

                string fullUrl = ApiUrl + "settask.ashx?method=reset";
                using (Devmasters.Net.Web.URLContent wc = new Devmasters.Net.Web.URLContent(fullUrl))
                {
                    wc.Method = Devmasters.Net.Web.MethodEnum.POST;
                    wc.RequestParams.Form.Add("data", server);
                    //wc.TimeInMsBetweenTries = 10000;
                    wc.Timeout = 10000;
                    wc.Tries = 1;
                    var res = wc.GetContent();
                }
            }
            catch (System.Exception e)
            {

                logger.Error("ResetRunningTasks error", e);
            }

        }

        public static void SetProgress(string taskId, decimal progress, string msg)
        {
            try
            {
                logger.Debug($"SetProgress for taskid:{taskId} ");

                string param = "taskId=" + taskId
                    + "&method=progress"
                    + "&msg=" + System.Net.WebUtility.UrlEncode(msg);
                string fullUrl = ApiUrl + "settask.ashx?" + param;
                using (Devmasters.Net.Web.URLContent wc = new Devmasters.Net.Web.URLContent(fullUrl))
                {
                    wc.Method = Devmasters.Net.Web.MethodEnum.POST;
                    wc.RequestParams.Form.Add("data", progress.ToString());
                    wc.TimeInMsBetweenTries = 10000;
                    wc.Timeout = 60000;
                    wc.Tries = 5;
                    var res = wc.GetContent();
                }
            }
            catch (System.Exception e)
            {

                logger.Error("SetProgress error ", e);
            }

        }
        public static void SetDone(string server, string taskId, Result result)
        {
            try
            {
                logger.Debug($"SetDone for taskid:{taskId} result:{result.IsValid.ToString()}");
                string param = "taskId=" + taskId
                    + "&method=done";
                string fullUrl = ApiUrl + "donetask.ashx?" + param;
                using (Devmasters.Net.Web.URLContent wc = new Devmasters.Net.Web.URLContent(fullUrl))
                {
                    wc.IgnoreHttpErrors = true;
                    wc.Method = Devmasters.Net.Web.MethodEnum.POST;
                    wc.RequestParams.Form.Add("data", Newtonsoft.Json.JsonConvert.SerializeObject(result));
                    wc.TimeInMsBetweenTries = 10000;
                    wc.Timeout = 60000;
                    wc.Tries = 15;
                    var res = wc.GetContent();
                }

            }
            catch (System.Exception e)
            {

                logger.Error("SetDone error", e);
            }

        }
    }
}
