using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vybory_PSP
{
    public class MP3
    {
        public MP3(string mp3path, string apikey)
        {
            Mp3Path = mp3path;
            Apikey = apikey;
        }

        public string Mp3Path { get; }
        public string Apikey { get; }

        public List<Devmasters.SpeechToText.VoiceToTextFormatter.TextWithTimestamp> CheckAndDownload(string datasetid, string recordid, string videourl)
        {
            return _checkDownloadAndStartV2TOrGet(false, datasetid, recordid, videourl);
        }

        public List<Devmasters.SpeechToText.VoiceToTextFormatter.TextWithTimestamp> CheckDownloadAndStartV2TOrGet(string datasetid, string recordid, string videourl)
        {
            return _checkDownloadAndStartV2TOrGet(true, datasetid, recordid, videourl);
        }
        private List<Devmasters.SpeechToText.VoiceToTextFormatter.TextWithTimestamp> _checkDownloadAndStartV2TOrGet(bool startV2T, string datasetid, string recordid, string videourl)
        {
            List<Devmasters.SpeechToText.VoiceToTextFormatter.TextWithTimestamp> blocks = null;

            string recId = recordid;
            string fnFile = $"{Mp3Path}\\{datasetid}\\{recId}";
            var MP3Fn = $"{fnFile}.mp3";
            var newtonFn = $"{fnFile}.mp3.raw_s2t";
            var dockerFn = $"{fnFile}.ctm";

            if (System.IO.File.Exists(MP3Fn) == false)
            {

                System.Diagnostics.ProcessStartInfo piv =
                            new System.Diagnostics.ProcessStartInfo("youtube-dl.exe",
                                $"--no-progress --extract-audio --audio-format mp3 --postprocessor-args \" -ac 1 -ar 16000\" -o \"{fnFile}.%(ext)s\" " + videourl
                                );
                Devmasters.ProcessExecutor pev = new Devmasters.ProcessExecutor(piv, 60 * 6 * 24);
                pev.StandardOutputDataReceived += (o, e) => { Devmasters.Logging.Logger.Root.Debug(e.Data); };

                Devmasters.Logging.Logger.Root.Info($"Starting Youtube-dl for {videourl} ");
                pev.Start();
            }
            bool exists_S2T = System.IO.File.Exists(newtonFn) || System.IO.File.Exists(dockerFn);
            if (exists_S2T == false && startV2T)
            {
                using (Devmasters.Net.HttpClient.URLContent net = new Devmasters.Net.HttpClient.URLContent(
                    $"https://www.hlidacstatu.cz/api/v2/internalq/Voice2TextNewTask/{datasetid}/{recId}")
                )
                {
                    net.Method = Devmasters.Net.HttpClient.MethodEnum.POST;
                    net.RequestParams.Headers.Add("Authorization", Apikey);
                    net.GetContent();
                }
            }

            if (exists_S2T)
            {
                if (System.IO.File.Exists(newtonFn))
                {
                    var tt = new Newton.SpeechToText.Cloud.FileAPI.VoiceToTerms(System.IO.File.ReadAllText(newtonFn));
                    blocks = new Devmasters.SpeechToText.VoiceToTextFormatter(tt.Terms)
                       .TextWithTimestamps(TimeSpan.FromSeconds(10), true);
                }
                else if (System.IO.File.Exists(dockerFn))
                {
                    var tt = new KaldiASR.SpeechToText.VoiceToTerms(System.IO.File.ReadAllText(dockerFn));
                    blocks = new Devmasters.SpeechToText.VoiceToTextFormatter(tt.Terms)
                       .TextWithTimestamps(TimeSpan.FromSeconds(10), true);

                }
            }

            return blocks;

        }
    }
}
