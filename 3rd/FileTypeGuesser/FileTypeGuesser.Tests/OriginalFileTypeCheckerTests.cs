namespace FileTypeGuesser.Tests
{
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    using NUnit.Framework;

    using Properties;

    [TestFixture]
    public class OriginalFileTypeCheckerTests
    {
        private MemoryStream bitmap;

        private MemoryStream pdf;

        private OriginalFileTypeGuesser.FileTypeChecker checker;

        [TestFixtureSetUp]
        public void SetUp()
        {
            bitmap= new MemoryStream();
            // LAND.bmp is from http://www.fileformat.info/format/bmp/sample/
            Resources.LAND.Save(bitmap, ImageFormat.Bmp);
            // http://boingboing.net/2015/03/23/free-pdf-advanced-quantum-the.html
            pdf = new MemoryStream(Resources.advancedquantumthermodynamics);
            checker = new OriginalFileTypeGuesser.FileTypeChecker();
        }

        [Test]
        [Repeat(1000)]
        public void TestPdf()
        {
            var fileTypes = checker.GetFileTypes(pdf);
            CollectionAssert.AreEquivalent(
                new[] { "Portable Document Format" },
                fileTypes.Select(fileType => fileType.Name));
        }

        [Test]
        [Repeat(1000)]
        public void TestBmp()
        {
            var fileTypes = checker.GetFileTypes(bitmap);
            CollectionAssert.AreEquivalent(
                new[] { "Bitmap" },
                fileTypes.Select(fileType => fileType.Name));
        }
    }
}
