using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.OCR.Api
{
    public partial class Client
    {
        public class MultipleFiles
        {
            public Api.Client.ProgressWithName MiningProgress { get; protected set; } = new Api.Client.ProgressWithName();

            IEnumerable<string> files = null;
            ProgressInterval pi = null;

            public int Priority { get; protected set; }
            public string Client { get; protected set; }
            public MiningIntensity Intensity { get; protected set; }
            public TimeSpan MaxWaitingTimeOfOneFile { get; set; } = TimeSpan.FromHours(1);
            public TimeSpan? RestartTaskIn { get; set; } = null;

            string apikey=null;
            decimal currProgress = 0;
            object objLock = new object();

            public MultipleFiles(string apikey, string client, int priority,
            MiningIntensity intensity, params string[] files)
                : this(apikey, client, priority, intensity, files as IEnumerable<string>)
            { }


            public MultipleFiles(string apikey, string client, int priority,
            MiningIntensity intensity, IEnumerable<string> files)
            {
                if (files == null)
                    throw new ArgumentNullException("files");
                if (string.IsNullOrEmpty( apikey ))
                    throw new ArgumentNullException("apikey");

                this.apikey = apikey;
                this.Client = client;
                this.Priority = priority;
                this.Intensity = intensity;
                this.files = files;

                if (this.files.Count()>0)
                    pi = new ProgressInterval(this.files.Count());

            }

            public async Task<HlidacStatu.Lib.OCR.Api.Result[]> Go()
            {
                List<Task<HlidacStatu.Lib.OCR.Api.Result>> tas = new List<Task<HlidacStatu.Lib.OCR.Api.Result>>();

                foreach (var fn in this.files)
                {
                    tas.Add(OneCall(apikey, fn));
                }

                var res = await Task.WhenAll(tas);
                return res;
            }

            private void AddProgress(decimal progress, string name)
            {
                lock (objLock)
                {
                    currProgress = currProgress + progress;
                }
                this.MiningProgress.SetProgress(currProgress, name);
            }

            private async Task<Result> OneCall(string apikey, string fn)
            {
                Result res = null;
                try
                {
                    AddProgress(
                        pi.SetProgressInPercent(1).Progress
                        , fn);
                    res = await HlidacStatu.Lib.OCR.Api.Client.TextFromFileAsync(apikey, fn, this.Client,
                            this.Priority, this.Intensity,
                            System.IO.Path.GetFileName(fn), this.MaxWaitingTimeOfOneFile,
                            this.RestartTaskIn);
                    AddProgress(
                        pi.SetProgressInPercent(100).Progress
                        , fn);
                    return res;

                }
                catch (ApiException e)
                {
                    logger.Error("TextFromFileAsync error", e);
                    return new Result() { Id=res?.Id,  IsValid = Result.ResultStatus.Invalid, Error = e.ToString() };
                }
                catch (Exception e)
                {
                    logger.Error("TextFromFileAsync error", e);
                    return new Result() { Id = res?.Id, IsValid = Result.ResultStatus.Invalid, Error = e.ToString() };

                    //throw;
                }
            }

        }
    }
}