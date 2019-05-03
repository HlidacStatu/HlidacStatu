using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HlidacStatu.Util.WebShot
{
    public class Workers
    {
        public class WorkRequest {
            public string Url { get; set; }
            public int cropWidth { get; set; }
            public int cropHeight { get; set; }
            public override string ToString()
            {
                return $"{cropWidth}|{cropHeight}|{Url}";
            }
            public static WorkRequest FromString(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return null;
                string[] parts = str.Split('|');
                if (parts.Length != 3)
                    return null;
                else
                    return new WorkRequest()
                    {
                        cropWidth = Convert.ToInt32(parts[0]),
                        cropHeight = Convert.ToInt32(parts[1]),
                        Url = parts[2]
                    };
            }
        }
        internal static System.Collections.Concurrent.ConcurrentQueue<string> UrlToProcess = new System.Collections.Concurrent.ConcurrentQueue<string>();
        internal static System.Collections.Concurrent.ConcurrentDictionary<string, byte[]> Results = new System.Collections.Concurrent.ConcurrentDictionary<string, byte[]>();

        static CancellationTokenSource cancel = null;
        static List<Worker_Chrome> workers = new List<Worker_Chrome>();
        public static void CreateAndStartWorkers(int num, CancellationTokenSource cancel)
        {
            for (int i = 0; i < num; i++)
                workers.Add(new Worker_Chrome("Worker" + i.ToString(),cancel.Token));
        }

        static bool canceling = false;
        public static void CancelWorkers()
        {

            Chrome.logger.Debug("called CancelWorkers: start ");
            if (canceling == false)
            {
                canceling = true;
                Chrome.logger.Info("CancelWorkers: canceling");
                cancel?.Cancel(true);
                foreach (var w in workers)
                {
                    try
                    {
                        Chrome.logger.Debug("CancelWorkers: canceling " + w.WorkerName);
                        w.Dispose();
                    }
                    catch (Exception e)
                    {
                        Chrome.logger.Debug("CancelWorkers: canceling " + w.WorkerName + " error:", e);

                    }
                }
            }
        }

        ~Workers()
        {
            CancelWorkers();
        }

    }
}
