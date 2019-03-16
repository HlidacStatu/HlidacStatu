using Devmasters.Core.Collections;
using HlidacStatu.Lib;
using HlidacStatu.Lib.Enhancers;
using System;
using System.Diagnostics;
using System.Linq;

namespace HlidacStatu.Plugin.Enhancers
{
    public partial class TextMiner : IEnhancer
    {

        [Obsolete]
        private class toDelete
        {
            string pathToOcr = Devmasters.Core.Util.Config.GetConfigValue("ReadIrisMonitorPath");

            string[] ghostscriptSupportedTypes = new string[] {
                            "application/postscript",
                            "application/ps",
                            "application/x-postscript",
                            "application/x-ps",
                            "text/postscript",
                            "application/x-postscript-not-eps",
                            "application/pdf",
                            "application/x-pdf",
                            "application/acrobat",
                            "applications/vnd.pdf",
                            "text/pdf",
                            "text/x-pdf",
                            "application/postscript",
                            "application/eps",
                            "application/x-eps",
                            "image/eps",
                            "image/x-eps",
                            "application/postscript",
                            "application/ps",
                            "application/x-postscript",
                            "application/x-ps",
                            "text/postscript",
                            "application/x-postscript-not-eps",
                        };

            string[] readIrisSupportedTypes = new string[] {
                            "application/pdf",
                            "application/x-pdf",
                            "application/acrobat",
                            "applications/vnd.pdf",
                            "text/pdf",
                            "text/x-pdf",
                            "application/postscript",
                            "application/eps",
                            "application/x-eps",
                            "application/msword",
                            "application/vnd.ms-word*",
                            "application/x-mswrite*",
                            "application/vnd.ms-excel*",
                            "application/postscript",
                            "application/ps",
                            "application/x-postscript",
                            "application/x-ps",
                            "text/postscript",
                            "application/x-postscript-not-eps",
                            "*officedocument.wordprocessing*",
                            "image/tiff",
                            "image/jpeg",
                            "application/rtf",
                            "text/richtext",
                            "*pdf*",
                        };
            private bool isReadIrisRunning()
            {
                Process[] pname = Process.GetProcessesByName("readiris");
                return pname.Length > 0;
            }

            private bool isReadIrisSupportedType(string fromMime)
            {
                return isSupportedFormat(fromMime, readIrisSupportedTypes);
            }
            private bool isGhostScriptSupportedType(string fromMime)
            {
                return isSupportedFormat(fromMime, ghostscriptSupportedTypes);
            }

            private bool isSupportedFormat(string fromMime, string[] supportedTypes)
            {
                if (string.IsNullOrEmpty(fromMime))
                    return false;

                fromMime = fromMime.ToLower();

                foreach (var mime in supportedTypes)
                {
                    if (mime.Contains("*"))
                    {
                        string smime = mime.Replace("*", "");
                        if (mime.StartsWith("*") && mime.EndsWith("*"))
                        {
                            if (fromMime.Contains(smime))
                                return true;
                        }
                        else if (mime.StartsWith("*"))
                        {
                            if (fromMime.EndsWith(smime))
                                return true;
                        }
                        else if (mime.EndsWith("*"))
                        {
                            if (fromMime.StartsWith(smime))
                                return true;
                        }
                    }
                    else
                    {
                        if (fromMime == mime)
                        {
                            return true;
                        }
                    }

                }
                return false;
            }


            private int GetOCRQueueLength()
            {
                return System.IO.Directory.EnumerateFiles(pathToOcr).Count();
            }

            private string GetOCRFileName(string session, bool done)
            {
                if (done)
                    return string.Format(pathToOcr + "done\\{0}.txt", session);
                else
                    return string.Format(pathToOcr + "{0}.pdf", session);
            }

            private bool OCRDocumentWithReadIris(string downloadedFile, ref Lib.Data.Smlouva.Priloha att)
            {
                string session = Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(att.odkaz);

                if (!System.IO.File.Exists(downloadedFile))
                    return false;


                if (!System.IO.File.Exists(GetOCRFileName(session, true)))
                { // DO OCR with ReadIris

                    //check size of the queue, don't make it too big
                    if (GetOCRQueueLength() > MaxIrisQueueLenght)
                    {
                        do
                        {
                            System.Threading.Thread.Sleep(1000);
                        } while (GetOCRQueueLength() > MaxIrisQueueLenght);
                    }

                    System.IO.File.Copy(downloadedFile, GetOCRFileName(session, false));

                    //waiting for until all OCR is done
                    bool done = true;
                    do
                    {
                        done = System.IO.File.Exists(GetOCRFileName(session, true));
                        System.Threading.Thread.Sleep(1000);
                    } while (done == false);
                    System.Threading.Thread.Sleep(1000); //wait until is everything done
                }

            done:
                var plaintext = System.IO.File.ReadAllText(GetOCRFileName(session, true));

                att.PlainTextContent = HlidacStatu.Util.ParseTools.NormalizePrilohaPlaintextText(plaintext);
                att.Lenght = att.PlainTextContent.Length;
                att.WordCount = HlidacStatu.Util.ParseTools.CountWords(att.PlainTextContent);
                att.PlainTextContentQuality = DataQualityEnum.Estimated;
                att.LastUpdate = DateTime.Now;

                System.IO.File.Delete(GetOCRFileName(session, true));
                System.IO.File.Delete(GetOCRFileName(session, false));
                return true;
            }
        }
    }
}

