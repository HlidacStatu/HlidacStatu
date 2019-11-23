using System;
using System.Threading;

namespace HlidacStatu.Util.WebShot
{
    public class Worker_Chrome : IDisposable
    {

        HlidacStatu.Util.WebShot.Chrome chrome = null;
        Thread t = null;
        public string WorkerName = "";
        public Worker_Chrome(string workerName, CancellationToken cancelToken)
        {
            this.WorkerName = workerName;
            chrome = new Chrome(workerName);
            t = new Thread(() =>
            {
                chrome.WarmUp();
                HlidacStatu.Util.InfinitiveTask.Run((tag) =>
                {
                    string swr = null;
                    if (Workers.UrlToProcess.TryDequeue(out swr))
                    {
                        Chrome.logger.Debug(workerName + ": dequeued " + swr);
                        Workers.WorkRequest wr = Workers.WorkRequest.FromString(swr);
                        if (wr != null)
                        {
                            //Workers.Results[swr] = Url2Png.Screenshot(chrome, wr.Url, wr.cropWidth, wr.cropHeight);
                            Chrome.logger.Debug(workerName + ": getting screenshot " + swr);
                            byte[] result = Url2PngSingle_Chrome.Screenshot(chrome, wr.Url, wr.cropWidth, wr.cropHeight);
                            Workers.Results.AddOrUpdate(swr, result, (k, upd) => result);
                            Chrome.logger.Debug(workerName + ": saved screenshot " + swr);

                        }
                        else
                            Chrome.logger.Error(workerName + ": Wrong decryption of " + swr);
                    }
                    return InfinitiveTask.TaskNextRunStatus.Continue;
                }, workerName, TimeSpan.FromMilliseconds(200), cancelToken); // fire every 200ms
            }
            );
            t.Start();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Chrome.logger.Info(WorkerName + ": Disposing");

                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                try
                {
                    Chrome.logger.Debug(WorkerName + ": thread Disposing");
                    t.Abort();
                }
                catch (Exception)
                {

                }

                Chrome.logger.Debug(WorkerName + ": Chrome Disposing");
                chrome.Dispose();

                Chrome.logger.Info(WorkerName + ": Disposed");

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Worker() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			GC.SuppressFinalize(this);
		}
        #endregion
    }

}
