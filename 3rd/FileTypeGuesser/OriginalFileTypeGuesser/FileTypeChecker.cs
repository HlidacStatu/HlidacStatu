namespace OriginalFileTypeGuesser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class FileTypeChecker
    {
        private static readonly IList<FileType> knownFileTypes = new List<FileType>
            {
                new FileType("Bitmap", ".bmp", new byte?[] { 0x42, 0x4d }),
                new FileType("Portable Network Graphic", ".png", new byte?[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
                new FileType("JPEG", ".jpg", new byte?[] { 0xFF, 0xD, 0xFF, 0xE0, null, null, 0x4A, 0x46, 0x49, 0x46, 0x00 }),
                new FileType("Graphics Interchange Format 87a", ".gif", new byte?[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }),
                new FileType("Graphics Interchange Format 89a", ".gif", new byte?[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }),
                new FileType("Portable Document Format", ".pdf", new byte?[] { 0x25, 0x50, 0x44, 0x46 }, 1019)
                // ... Potentially more in future
            }
            .OrderBy(f => f.MaximumStartLocation)
            .ThenBy(f => f.MagicSequence.Length).ToList();

        private static ByteWithWildcardComparer _wildcardComparer = new ByteWithWildcardComparer();

        public FileType GetFileType(Stream fileContent)
        {
            if (fileContent == null)
            {
                throw new ArgumentNullException("fileContent");
            }
            if (!fileContent.CanRead || (fileContent.Position != 0 && !fileContent.CanSeek))
            {
                throw new ArgumentException("File contents must be a readable stream", "fileContent");
            }
            if (fileContent.Position != 0)
            {
                fileContent.Seek(0, SeekOrigin.Begin);
            }
            var startOfFile = new byte[1024];
            fileContent.Read(startOfFile, 0, startOfFile.Length);
            var result = knownFileTypes.FirstOrDefault(f => StartOfFileContainsFileType(f, startOfFile));

            return result ?? FileType.Unknown;
        }

        public IEnumerable<FileType> GetFileTypes(Stream fileContent)
        {
            if (fileContent == null)
            {
                throw new ArgumentNullException("fileContent");
            }
            if (!fileContent.CanRead || (fileContent.Position != 0 && !fileContent.CanSeek))
            {
                throw new ArgumentException("File contents must be a readable stream", "fileContent");
            }
            if (fileContent.Position != 0)
            {
                fileContent.Seek(0, SeekOrigin.Begin);
            }
            var startOfFile = new byte[1024];
            fileContent.Read(startOfFile, 0, startOfFile.Length);
            return knownFileTypes.Where(f => StartOfFileContainsFileType(f, startOfFile));
        }

        private static bool StartOfFileContainsFileType(FileType fileType, byte[] startOfFile)
        {
            var counter = 0;
            do
            {
                if (startOfFile.Skip(counter)
                        .Take(fileType.MagicSequence.Length)
                        .Cast<byte?>()
                        .SequenceEqual(fileType.MagicSequence, _wildcardComparer))
                {
                    return true;
                }
            }
            while (++counter <= fileType.MaximumStartLocation);
            return false;
        }

        private class ByteWithWildcardComparer : IEqualityComparer<byte?>
        {
            public bool Equals(byte? x, byte? y)
            {
                // null is a wildcard - it matches the other regardless of value.
                return x == null || y == null || x.GetValueOrDefault() == y.GetValueOrDefault();
            }

            public int GetHashCode(byte? obj)
            {
                return obj.GetValueOrDefault().GetHashCode();
            }
        }
    }
}
