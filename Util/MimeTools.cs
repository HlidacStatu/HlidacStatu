using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public static class MimeTools
    {

        public static string MimetypeFromExtension(string filename)
        {
            return MimeMapping.MimeUtility.GetMimeMapping(filename);
        }

        
        public static string ExtensionFromMime(string mime)
        {
            string ext = MimeMapping.MimeUtility.TypeMap 
                .Where(m => m.Value == mime)
                .Select(m=>m.Key)
                .OrderBy(m=>m?.Length ?? 0)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(ext))
                return ".bin";
            return ext;
        }
    }
}
