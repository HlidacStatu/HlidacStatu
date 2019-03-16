namespace FileTypeGuesser
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class ExactFileTypeMatcher : FileTypeMatcher
    {
        private readonly byte[] bytes;

        public ExactFileTypeMatcher(IEnumerable<byte> bytes)
        {
            this.bytes = bytes.ToArray();
        }

        protected override bool MatchesPrivate(Stream stream)
        {
            foreach (var b in bytes)
            {
                if (stream.ReadByte() != b)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
