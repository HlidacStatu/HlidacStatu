using System;
using System.Linq;
using System.Collections.Generic;

namespace Newton.SpeechToText.Cloud.FileAPI
{
    public class VoiceToText
    {
        private AccessToken accessToken = null;
        private NtxToken ntxToken = null;
        private string mp3file = null;

        private bool converted = false;


        public VoiceToText(string fullPathToMP3File,
            string username, string password,
            string taskId, string taskLabel, string audience = "https://newton.nanogrid.cloud/")
        {
            accessToken = AccessToken.Login(username, password, audience);
            ntxToken = NtxToken.GetToken(accessToken, taskId, taskLabel);
            mp3file = fullPathToMP3File;
        }

        public VoiceToText(string fullPathToMP3File, AccessToken accessToken, NtxToken ntxToken)
        {
            this.accessToken = accessToken;
            this.ntxToken = ntxToken;
            mp3file = fullPathToMP3File;
        }

        public bool Convert()
        {
            using (RESTCall wc = new RESTCall())
            {
                string url = this.accessToken.Audience + "api/v1/file/v2t";
                wc.Headers.Add("ntx-token", ntxToken.ntxToken);
                var resbyte = wc.UploadFile(url, "POST", this.mp3file);
                var res = System.Text.Encoding.UTF8.GetString(resbyte);
                converted = true;
                this.Raw = res;
                this.Chunks = RawToChunks(this.Raw);
                this.Terms = ChunksToTerms(this.Chunks);
            }
            return true;
        }

        private string _raw;
        public string Raw
        {
            get
            {
                if (converted == false)
                    throw new ApplicationException("Start Conversion with Convert() method first.");
                else return _raw;
            }
            private set => _raw = value;
        }

        private IEnumerable<ChunkLine> _chunks;
        public IEnumerable<ChunkLine> Chunks
        {
            get
            {
                if (converted == false)
                    throw new ApplicationException("Start Conversion with Convert() method first.");
                else return _chunks;
            }
            private set => _chunks = value;
        }

        private IEnumerable<Term> _terms;
        public IEnumerable<Term> Terms
        {
            get
            {
                if (converted == false)
                    throw new ApplicationException("Start Conversion with Convert() method first.");
                else return _terms;
            }

            private set => _terms = value;
        }

        public string Text(bool withParagraphs)
        {
            return TermsToText(this.Terms, withParagraphs);
        }


        static decimal minDelay = 0.8m;

        public static string TermsToText(IEnumerable<Term> terms, bool withParagraphs = false)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(5 * terms.Count());
            var lTerms = terms.ToList();
            for (int i = 0; i < lTerms.Count; i++)
            {

                Term t = lTerms[i];

                switch (t.Character)
                {
                    case Term.TermCharacter.Separator:
                        sb.Append(t.Value);
                        break;
                    case Term.TermCharacter.Noise:
                        if (withParagraphs)
                        {
                            var nl = NoiseLength(lTerms, i);
                            if (nl > minDelay)
                                sb.AppendLine();
                        }
                        break;
                    case Term.TermCharacter.Word:
                        if (withParagraphs)
                        {
                            var delay = 0m;
                            if (i > 0)
                                delay = t.Timestamp - lTerms[i - 1].Timestamp;

                            if (delay > minDelay) //ticho mezi slovy
                                sb.AppendLine();
                        }
                        sb.Append(t.Value);

                        break;
                    case Term.TermCharacter.SpeakersNoise:
                    default:
                        break;
                }

            }
            var rawText = sb.ToString().ReplaceDuplicates("\n").ReplaceDuplicates("\r").ReplaceDuplicates("\r\n").ReplaceDuplicates(" ");
            return rawText;
        }

        private static decimal NoiseLength(List<Term> terms, int forItemNum)
        {
            if (forItemNum == 0)
                return terms[forItemNum].Timestamp;

            decimal noiseEnd = terms[forItemNum].Timestamp;
            if (terms.Count > forItemNum + 1)
                noiseEnd = terms[forItemNum + 1].Timestamp;

            decimal noiseStart = terms[forItemNum].Timestamp;
            for (int i = forItemNum - 1; i > 0; i--)
            {
                if (terms[i].Character == Term.TermCharacter.Noise
                    //|| terms[i].Character == Term.TermCharacter.SpeakersNoise
                    )
                    noiseStart = terms[i].Timestamp;
                else
                    break;
            }


            return noiseEnd - noiseStart;
        }

        public static IEnumerable<Term> ChunksToTerms(IEnumerable<ChunkLine> chunks)
        {
            List<Term> terms = new List<Term>();
            Term prev = null;
            foreach (var ch in chunks)
            {
                var chTerms = ch?.push?.events?.ToTerms(prev);
                if (chTerms != null && chTerms.Count > 0)
                {
                    terms.AddRange(chTerms);
                    prev = chTerms.Last();
                }

            }

            return terms;
        }

        public static IEnumerable<ChunkLine> RawToChunks(string rawApiResult)
        {
            List<ChunkLine> lines = new List<ChunkLine>();
            foreach (var line in rawApiResult.Split('\r', '\n'))
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var ev = Newtonsoft.Json.JsonConvert.DeserializeObject<ChunkLine>(line);
                    if (ev?.push != null)
                        lines.Add(ev);
                }
            }

            return lines;

        }
    }
}

