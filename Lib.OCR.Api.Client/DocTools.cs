using System;
using System.Text.RegularExpressions;

namespace HlidacStatu.Lib.OCR
{
    public static class DocTools
    {

        public static Devmasters.Core.Logging.Logger baselogger = new Devmasters.Core.Logging.Logger("HlidacStatu.Lib.OCR");

        public static string OrigFilenameDelimiter = "_!_";

        public static string PrepareFilenameForOCR(string origFilename)
        {
            if (origFilename == null)
                origFilename = "";
            var fn = System.IO.Path.GetFileName(origFilename) ?? "";

            Regex myRegex = new Regex("[^a-z0-9-.]", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
            fn = myRegex.Replace(fn, "");
            //fn = HlidacStatu.Util.ParseTools.ReplaceWithRegex(fn, "", "[^a-z0-9-.]");

            var ext = System.IO.Path.GetExtension(fn);
            var name = System.IO.Path.GetFileNameWithoutExtension(fn) ?? "";
            if (string.IsNullOrEmpty(ext) || ext == ".")
                ext = ".bin";
            if (name.Length < 1)
                name = name + Devmasters.Core.TextUtil.GenRandomString(5);
            if (name.Length > 40)
                name = name.Substring(0, 30) + Devmasters.Core.TextUtil.GenRandomString(9);
            return OrigFilenameDelimiter + name + ext;
        }

        public static string GetFilename(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return filename;

            var fn = System.IO.Path.GetFileName(filename);
            if (fn?.Contains(OrigFilenameDelimiter) == true)
            {
                var nalez1 = fn.IndexOf(OrigFilenameDelimiter);
                fn = fn.Substring(nalez1 + OrigFilenameDelimiter.Length);
            }
            return fn;
        }

        public static bool IsPDFContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType) && contentType.Contains("pdf");
        }

        public static bool HasPDFHeader(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;
            if (!System.IO.File.Exists(filename))
                return false;

            byte[] b = new byte[4];
            using (var r = System.IO.File.OpenRead(filename))
            {
                r.Read(b, 0, 4);
            }
            byte[] pdfheader = new byte[] { 37, 80, 68, 70 };
            bool valid = true;
            for (int i = 0; i < 4; i++)
            {
                valid = valid && b[i] == pdfheader[i];
            }
            return valid;
        }

        public static int CountWords(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }
        public static string ToElasticDate(DateTime date)
        {
            switch (date.Kind)
            {
                case DateTimeKind.Unspecified:
                case DateTimeKind.Local:
                    return date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                case DateTimeKind.Utc:
                    return date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                default:
                    return date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            }
        }
    }
}
