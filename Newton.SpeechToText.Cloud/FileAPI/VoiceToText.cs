using System;
using System.Linq;
using System.Collections.Generic;

namespace Newton.SpeechToText.Cloud.FileAPI
{
    public class VoiceToText
    {
        private AccessToken accessToken = null;
        private NtxToken ntxToken = null;


        public VoiceToText(string username, string password, string taskId, string taskLabel, string audience = "https://newton.nanogrid.cloud/")
        {
            accessToken = AccessToken.Login(username, password, audience);
            ntxToken = NtxToken.GetToken(accessToken, taskId, taskLabel);           
        }

        public VoiceToText(AccessToken accessToken, NtxToken ntxToken)
        {
            this.accessToken = accessToken;
            this.ntxToken = ntxToken;
        }

        public string FromMp3ToRaw(string fullPathToMP3File)
        {
            using (RESTCall wc = new RESTCall())
            {
                string url = this.accessToken.Audience + "api/v1/file/v2t";
                wc.Headers.Add("ntx-token", ntxToken.ntxToken);
                var resbyte = wc.UploadFile(url, "POST", fullPathToMP3File);
                var res = System.Text.Encoding.UTF8.GetString(resbyte);

                return res;
            }
        }

        public IEnumerable<Term> FromMp3ToTerms(string fullPathToMP3File)
        {
            return ChunksToTerms(
                RawToChunks(
                    FromMp3ToRaw(fullPathToMP3File)
                    )
                );
        }

        public string FromMp3ToText(string fullPathToMP3File)
        {
            return TermsToText(FromMp3ToTerms(fullPathToMP3File));
        }


        static decimal minDelay = 0.8m;
        public static string TermsToText(IEnumerable<Term> terms, bool withParagraphs = false)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(5*terms.Count());
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

                            if (delay> minDelay) //ticho mezi slovy
                                sb.AppendLine();
                        }
                        sb.Append(t.Value);

                        break;
                    case Term.TermCharacter.SpeakersNoise:
                    default:
                        break;
                }

            }
            var rawText= sb.ToString().ReplaceDuplicates("\n").ReplaceDuplicates("\r").ReplaceDuplicates("\r\n").ReplaceDuplicates(" ");
            return rawText;
        }

        private static decimal NoiseLength(List<Term> terms, int forItemNum)
        {
            if (forItemNum == 0)
                return terms[forItemNum].Timestamp;

            decimal noiseEnd = terms[forItemNum].Timestamp;
            if (terms.Count>forItemNum+1)
                noiseEnd = terms[forItemNum+1].Timestamp;

            decimal noiseStart = terms[forItemNum].Timestamp;
            for (int i = forItemNum-1; i > 0; i--)
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
                if (chTerms != null && chTerms.Count>0)
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

