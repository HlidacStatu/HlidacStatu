using System;

namespace HlidacStatu.Lib.OCR.Api
{
    public partial class Client
    {


        public static class TempIO
        {
            static string TempDirectory = null;
            static object lockObj = new object();
            static TempIO()
            {
                lock (lockObj)
                {
                    TempDirectory = Devmasters.Config.GetWebConfigValue("OCR.Temp.Directory");
                    if (string.IsNullOrEmpty(TempDirectory))
                    {
                        TempDirectory = System.IO.Path.GetTempPath();
                    }
                    if (!TempDirectory.EndsWith(@"\"))
                        TempDirectory = TempDirectory + @"\";
                    if (!System.IO.Directory.Exists(TempDirectory))
                        System.IO.Directory.CreateDirectory(TempDirectory);
                }
            }

            static object getTmpFilenameLock = new object();
            public static string GetTemporaryFilename()
            {
                do
                {
                    string tempName = Devmasters.TextUtil.GenRandomString(12);
                    string fn = TempDirectory + tempName;
                    try
                    {
                        lock (getTmpFilenameLock)
                        {
                            if (!System.IO.File.Exists(fn))
                            {
                                System.IO.File.WriteAllText(fn, "");
                                return fn;
                            }
                        }
                    }
                    catch (Exception){}
                } while (true);
            }


            public static void DeleteDir(string dirName)
            {
                if (!string.IsNullOrEmpty(dirName))
                {
                    if (System.IO.Directory.Exists(dirName))
                    {
                        try
                        {
                            System.IO.Directory.Delete(dirName, true);
                        }
                        catch (Exception)
                        {
                            System.Threading.Thread.Sleep(1000);
                            try
                            {
                                System.IO.Directory.Delete(dirName, true);
                            }
                            catch (Exception)
                            {
                                System.Threading.Thread.Sleep(1000);
                                try
                                {
                                    System.IO.Directory.Delete(dirName, true);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                }

            }

            public static void DeleteFile(string fnName)
            {
                if (!string.IsNullOrEmpty(fnName))
                {
                    if (System.IO.File.Exists(fnName))
                    {
                        try
                        {
                            System.IO.File.Delete(fnName);
                        }
                        catch (Exception)
                        {
                            System.Threading.Thread.Sleep(1000);
                            try
                            {
                                System.IO.File.Delete(fnName);
                            }
                            catch (Exception)
                            {
                                System.Threading.Thread.Sleep(1000);
                                try
                                {
                                    System.IO.File.Delete(fnName);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                }

            }


        }


    }
}
