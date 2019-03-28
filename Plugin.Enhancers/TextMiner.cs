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
        public static int MaxIrisQueueLenght = 50;
        bool asyncOCR = false;
        bool skipOCR = false;
        bool forceAlreadyMined = false;

        public enum OCREngines
        {
            IrisOnly,
            GhostscriptOnly,
            All,
        }

        string pathToOcr = Devmasters.Core.Util.Config.GetConfigValue("ReadIrisMonitorPath");
        public TextMiner()
        {
            if (!pathToOcr.EndsWith("\\"))
                pathToOcr += "\\";
        }

        public TextMiner(bool skipOCR, bool forceAlreadyMined,  bool asyncOCR = false)
            : this()
        {
            this.skipOCR = skipOCR;
            this.forceAlreadyMined = forceAlreadyMined;
            this.asyncOCR = asyncOCR;
        }

        public string Description
        {
            get
            {
                return "Vytahnout texty z prilozenych smluv";
            }
        }

        public string Name
        {
            get
            {
                return "TextMiner";
            }
        }


        DateTime history = new DateTime(2016, 1, 1);

        public void Update(ref Lib.Data.Smlouva item)
        {

            if (item.Prilohy != null)
            {
                for (int i = 0; i < item.Prilohy.Count(); i++)
                {

                    var att = item.Prilohy[i];
                    if (!this.forceAlreadyMined && att.LastUpdate > history)
                        continue;
                    if (!this.forceAlreadyMined && att.PlainTextContentQuality != DataQualityEnum.Unknown) //already parsed
                    {
                        att.LastUpdate = DateTime.Now.AddDays(-7);
                        continue;
                    }
                    string downloadedFile = Lib.Data.Smlouva.Priloha.GetFileFromPrilohaRepository(att, item);
                    if (downloadedFile != null)
                    {
                        try
                        {
                            HlidacStatu.Lib.OCR.Api.Client.MiningIntensity intensity = HlidacStatu.Lib.OCR.Api.Client.MiningIntensity.Maximum;
                            if (skipOCR)
                                intensity = HlidacStatu.Lib.OCR.Api.Client.MiningIntensity.SkipOCR;

                            Base.Logger.Debug($"STARTING TextMiner Client.TextFromFile Id:{item.Id} att:{att.nazevSouboru}  async:{asyncOCR}  skipOCR:{intensity.ToString()}");

                            HlidacStatu.Lib.OCR.Api.Result res = null;
                            if (asyncOCR)
                                //res = HlidacStatu.Lib.OCR.Api.Client.TextFromFile(
                                //    Devmasters.Core.Util.Config.GetConfigValue("OCRServerApiKey"),
                                //    downloadedFile, "TextMiner",
                                //    HlidacStatu.Lib.OCR.Api.Client.TaskPriority.High, intensity
                                //    ); //TODOcallBackData: item.CallbackDataForOCRReq(i) );
                                res = Lib.Data.ItemToOcrQueue.AddNewTask( Lib.Data.ItemToOcrQueue.ItemToOcrType.Smlouva, item.Id);
                            else
                                res = HlidacStatu.Lib.OCR.Api.Client.TextFromFile(
                                    Devmasters.Core.Util.Config.GetConfigValue("OCRServerApiKey"),
                                    downloadedFile, "TextMiner",
                                HlidacStatu.Lib.OCR.Api.Client.TaskPriority.High, intensity);


                            if (res.IsValid == HlidacStatu.Lib.OCR.Api.Result.ResultStatus.InQueueWithCallback)
                            {
                                Base.Logger.Debug($"Queued TextMiner Client.TextFromFile Id:{item.Id} att:{att.nazevSouboru}  async:{asyncOCR}  taskid:{res.Id}");
                            }
                            else if (res.IsValid == HlidacStatu.Lib.OCR.Api.Result.ResultStatus.Invalid)
                            {
                                Base.Logger.Error($"ERROR TextMiner Client.TextFromFile Id:{item.Id} att:{att.nazevSouboru}  async:{asyncOCR}  taskid:{res.Id}  error:{res.Error}");
                            }
                            else if (res.IsValid == HlidacStatu.Lib.OCR.Api.Result.ResultStatus.Unknown)
                            {
                                Base.Logger.Error($"UNKNOWN Status TextMiner Client.TextFromFile Id:{item.Id} att:{att.nazevSouboru}  async:{asyncOCR}  taskid:{res.Id}  error:{res.Error}");
                            }
                            else if (res.IsValid == HlidacStatu.Lib.OCR.Api.Result.ResultStatus.Valid)
                            {
                                if (res.Documents.Count > 1)
                                {
                                    Base.Logger.Error("More documents inside attachment");
                                }
                                else if (res.Documents.Count == 1)
                                {
                                    att.PlainTextContent = HlidacStatu.Util.ParseTools.NormalizePrilohaPlaintextText(res.Documents[0].Text);
                                    att.Lenght = att.PlainTextContent.Length;
                                    att.WordCount = HlidacStatu.Util.ParseTools.CountWords(att.PlainTextContent);
                                    if (res.Documents[0].UsedOCR)
                                        att.PlainTextContentQuality = DataQualityEnum.Estimated;
                                    else
                                        att.PlainTextContentQuality = DataQualityEnum.Parsed;

                                    att.LastUpdate = DateTime.Now;

                                    if (att.EnoughExtractedText)
                                    {
                                        if (att.PlainTextContentQuality == DataQualityEnum.Estimated)
                                            item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Text přílohy extrahován z OCR dokumentu ", "", "item.Prilohy[" + i.ToString() + "].PlainTextContent", "", "", this));
                                        else
                                            item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Text přílohy extrahován z obsahu dokumentu ", "", "item.Prilohy[" + i.ToString() + "].PlainTextContent", "", "", this));

                                    }
                                }
                            }

                            Base.Logger.Debug("Done TextMiner Client.TextFromFile Id:" + item.Id + " att:" + att.nazevSouboru);

                        }
                        finally
                        {
                            HlidacStatu.Util.IOTools.DeleteFile(downloadedFile);
                        }

                    }

                }
            }
        }




    }
}

