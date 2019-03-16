namespace FileTypeGuesser
{
    using System.IO;

    public class RangeFileTypeMatcher : FileTypeMatcher
    {
        private readonly FileTypeMatcher matcher;

        private readonly int maximumStartLocation;

        public RangeFileTypeMatcher(FileTypeMatcher matcher, int maximumStartLocation)
        {
            this.matcher = matcher;
            this.maximumStartLocation = maximumStartLocation;
        }

        protected override bool MatchesPrivate(Stream stream)
        {
            for (var i = 0; i < this.maximumStartLocation; i++)
            {
                stream.Position = i;
                if (matcher.Matches(stream, resetPosition: false))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
