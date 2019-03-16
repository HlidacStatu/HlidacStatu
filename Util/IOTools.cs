using System;
using System.IO;
using System.Reflection;

namespace HlidacStatu.Util
{
    public static class IOTools
    {



        public static void MoveFile(string sourceFn, string destFn, bool overwrite = true)
        {
            if (overwrite && System.IO.File.Exists(sourceFn))
                DeleteFile(destFn);

            System.IO.File.Move(sourceFn, destFn);
        }

        public static bool DeleteFile(string fn)
        {
            if (System.IO.File.Exists(fn))
                try
                {
                    System.IO.File.Delete(fn);
                    return true;
                }
                catch (System.IO.IOException)
                {
                    System.Threading.Thread.Sleep(100);
                    try
                    {
                        System.IO.File.Delete(fn);
                        return true;
                    }
                    catch (System.IO.IOException)
                    {
                        System.Threading.Thread.Sleep(500);
                        try
                        {
                            System.IO.File.Delete(fn);
                            return true;
                        }
                        catch (Exception e)
                        {
                            Util.Consts.Logger.Warning("cannot delete " + fn, e);
                            return false;
                        }

                    }


                }
                catch (Exception e)
                {
                    Util.Consts.Logger.Warning("cannot delete " + fn, e);
                    return false;
                }
            return true;
        }

        public static string GetExecutingDirectoryName()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory.FullName;
        }


    }
}
