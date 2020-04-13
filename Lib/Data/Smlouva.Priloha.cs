using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devmasters.Core;
using System.Text.RegularExpressions;
using HlidacStatu.Util;

namespace HlidacStatu.Lib.Data
{
    public partial class Smlouva
    {
        public class Priloha : XSD.tPrilohaOdkaz
        {
            public class KeyVal
            {
                [Keyword()]
                public string Key { get; set; }
                [Keyword()]
                public string Value { get; set; }
            }

 
            public KeyVal[] FileMetadata = new KeyVal[] { };


            string _plainTextContent = "";
            [Text()]
            public string PlainTextContent
            {
                get { return _plainTextContent; }
                set
                {
                    _plainTextContent = value;
                    this.Lenght = this.PlainTextContent?.Length ?? 0;
                    this.WordCount = ParseTools.CountWords(this.PlainTextContent);
                }
            }


            public DataQualityEnum PlainTextContentQuality { get; set; } = DataQualityEnum.Unknown;

            [Date]
            public DateTime LastUpdate { get; set; } = DateTime.MinValue;

            public byte[] LocalCopy { get; set; } = null;

            [Keyword()]
            public string ContentType { get; set; }
            public int Lenght { get; set; }
            public int WordCount { get; set; }
            public int Pages { get; set; }




            [Object(Ignore = true)]
            public bool EnoughExtractedText
            {
                get
                {
                    return !(this.Lenght <= 20 || this.WordCount <= 10);
                }
            }

            public Priloha()
            {
            }
            public Priloha(XSD.tPrilohaOdkaz tpriloha)
            {
                this.hash = tpriloha.hash;
                this.nazevSouboru = tpriloha.nazevSouboru;
                this.odkaz = tpriloha.odkaz;
                //if (tpriloha.data != null)
                //{
                //    //priloha je soucasti tpriloha, uloz


                //}
            }

            public string LimitedAccessSecret(string forEmail)
            {
                if (string.IsNullOrEmpty(forEmail))
                    throw new ArgumentNullException("forEmail");
                return Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(forEmail + this.hash + "__" + forEmail);
            }


            public static string GetFileFromPrilohaRepository(HlidacStatu.Lib.Data.Smlouva.Priloha att,
                Lib.Data.Smlouva smlouva)
            {
                var ext = ".pdf";
                try
                {
                    ext = new System.IO.FileInfo(att.nazevSouboru).Extension;

                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Warning("invalid file name " + (att?.nazevSouboru ?? "(null)"));
                }


                string localFile = Lib.Init.PrilohaLocalCopy.GetFullPath(smlouva, att);
                var tmpPath = System.IO.Path.GetTempPath();
                HlidacStatu.Util.IOTools.DeleteFile(tmpPath);
                if (!System.IO.Directory.Exists(tmpPath))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(tmpPath);

                    }
                    catch
                    {

                    }
                }
                string tmpFnSystem = System.IO.Path.GetTempFileName();
                string tmpFn = tmpFnSystem + HlidacStatu.Lib.OCR.DocTools.PrepareFilenameForOCR(att.nazevSouboru);
                try
                {

                    //System.IO.File.Delete(fn);
                    if (System.IO.File.Exists(localFile))
                    {
                        //do local copy
                        Consts.Logger.Debug($"Copying priloha {att.nazevSouboru} for smlouva {smlouva.Id} from local disk {localFile}");
                        System.IO.File.Copy(localFile, tmpFn, true);
                    }
                    else
                    {

                        try
                        {
                            Consts.Logger.Debug($"Downloading priloha {att.nazevSouboru} for smlouva {smlouva.Id} from URL {att.odkaz}");
                            byte[] data = null;
                            using (Devmasters.Net.Web.URLContent web = new Devmasters.Net.Web.URLContent(att.odkaz))
                            {
                                web.Timeout = web.Timeout * 10;
                                data = web.GetBinary().Binary;
                                System.IO.File.WriteAllBytes(tmpFn, data);
                            }
                            Consts.Logger.Debug($"Downloaded priloha {att.nazevSouboru} for smlouva {smlouva.Id} from URL {att.odkaz}");
                        }
                        catch (Exception)
                        {
                            try
                            {
                                byte[] data = null;
                                Consts.Logger.Debug($"Second try: Downloading priloha {att.nazevSouboru} for smlouva {smlouva.Id} from URL {att.odkaz}");
                                using (Devmasters.Net.Web.URLContent web = new Devmasters.Net.Web.URLContent(att.odkaz))
                                {
                                    web.Tries = 5;
                                    web.IgnoreHttpErrors = true;
                                    web.TimeInMsBetweenTries = 1000;
                                    web.Timeout = web.Timeout * 20;
                                    data = web.GetBinary().Binary;
                                    System.IO.File.WriteAllBytes(tmpFn, data);
                                }
                                Consts.Logger.Debug($"Second try: Downloaded priloha {att.nazevSouboru} for smlouva {smlouva.Id} from URL {att.odkaz}");
                                return tmpFn;
                            }
                            catch (Exception e)
                            {

                                HlidacStatu.Util.Consts.Logger.Error(att.odkaz, e);
                                return null;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Error(att.odkaz, e);
                    throw;
                }
                finally
                {
                    HlidacStatu.Util.IOTools.DeleteFile(tmpFnSystem);
                }
                return tmpFn;


            }



        }

    }
}
