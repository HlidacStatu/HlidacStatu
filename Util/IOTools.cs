using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HlidacStatu.Util
{
    public static class IOTools
    {

        /// <summary>
        /// Strip illegal chars and reserved words from a candidate filename (should not include the directory path)
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        /// </remarks>

        public static string MakeValidFileName(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename");
            //remove path from UNC or URL
            if (filename.EndsWith("/"))
                filename = filename.Substring(0, filename.Length - 1);
            if (filename.EndsWith("\\"))
                filename = filename.Substring(0, filename.Length - 1);
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename");

            int found = filename.LastIndexOfAny(new char[] { '\\', '/' });
            if (found < filename.Length)
                found++;

            string fn = filename.Substring(found);

            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()) + "=()") + @"\s"; //pridano =()\s
            var invalidReStr = string.Format(@"[{0}]+", invalidChars);

            var reservedWords = new[]
            {
        "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
        "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
        "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };

            var sanitisedNamePart = Regex.Replace(fn, invalidReStr, "_");
            foreach (var reservedWord in reservedWords)
            {
                var reservedWordPattern = string.Format("^{0}\\.", reservedWord);
                sanitisedNamePart = Regex.Replace(sanitisedNamePart, reservedWordPattern, "_reservedWord_.", RegexOptions.IgnoreCase);
            }

            return Devmasters.Core.TextUtil.RemoveDiacritics(sanitisedNamePart);
        }

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

        public static string GetExecutingDirectoryName(bool endsWithBackslash)
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            var dir = new FileInfo(location.AbsolutePath).Directory.FullName + (endsWithBackslash ? @"\" : "");
            return dir;
        }


    }
}
