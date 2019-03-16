namespace FileTypeGuesser
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class FileTypeChecker
    {
        private static readonly IList<FileType> knownFileTypes = new List<FileType>
        {
            //new FileType("Bitmap", ".bmp", new ExactFileTypeMatcher(new byte[] {0x42, 0x4d})),
            //new FileType("Portable Network Graphic", ".png",
            //    new ExactFileTypeMatcher(new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A})),
            //new FileType("JPEG", ".jpg",
            //    new FuzzyFileTypeMatcher(new byte?[] {0xFF, 0xD, 0xFF, 0xE0, null, null, 0x4A, 0x46, 0x49, 0x46, 0x00})),
            //new FileType("Graphics Interchange Format 87a", ".gif",
            //    new ExactFileTypeMatcher(new byte[] {0x47, 0x49, 0x46, 0x38, 0x37, 0x61})),
            //new FileType("Graphics Interchange Format 89a", ".gif",
            //    new ExactFileTypeMatcher(new byte[] {0x47, 0x49, 0x46, 0x38, 0x39, 0x61})),
            //new FileType("Portable Document Format", ".pdf", new RangeFileTypeMatcher(new ExactFileTypeMatcher(new byte[] { 0x25, 0x50, 0x44, 0x46 }), 1019))
            // ... Potentially more in future
            new FileType("ZIP", ".zip", new ExactFileTypeMatcher(new byte[] { 0x50, 0x4B, 0x03, 0x04 }))

    };

        public static FileType GetFileType(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return GetFileType(fs);
            }
        }
        public static FileType GetFileType(Stream fileContent)
        {
            return GetFileTypes(fileContent).FirstOrDefault() ?? FileType.Unknown;
        }

        public static IEnumerable<FileType> GetFileTypes(Stream stream)
        {
            return knownFileTypes.Where(fileType => fileType.Matches(stream));
        }
    }
}
