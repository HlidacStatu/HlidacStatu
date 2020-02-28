using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util.IO
{
    public class SimpleZipArchive : IDisposable
    {

        public string ArchiveFileName { get; set; }
        private string tmpFile { get { return this.ArchiveFileName + ".tmp"; } }


        FileStream zipFile = null;
        ZipArchive archive = null;
        ZipArchiveEntry entry = null;
        TextWriter writer = null;
        TextWriter swriter = null;
        public SimpleZipArchive(string archive, string packedFileName, bool overwrite = false)
        {
            this.ArchiveFileName = archive;
            var fi = new FileInfo(this.ArchiveFileName);
            if (overwrite == false && fi.Exists)
                throw new System.IO.IOException("File " + archive + " already exists.");
            else if (fi.Exists)
                fi.Delete();

            if (fi.Directory.Exists == false)
                fi.Directory.Create();

            this.zipFile = new FileStream(this.tmpFile, FileMode.Create);
            this.archive = new ZipArchive(zipFile, ZipArchiveMode.Create);
            this.entry = this.archive.CreateEntry(packedFileName, CompressionLevel.Optimal);
            this.writer = new StreamWriter(this.entry.Open());
            this.swriter = StreamWriter.Synchronized(this.writer);

        }

        public void Write(string text)
        {
            this.swriter.Write(text);
        }
        public void WriteLine(string text)
        {
            this.swriter.WriteLine(text);
        }

        private void Finish()
        {
            this.swriter.Flush();
            System.Threading.Thread.Sleep(50);
            CloseAll();
            System.Threading.Thread.Sleep(50);
            System.IO.File.Delete(this.ArchiveFileName);
            System.IO.File.Move(this.tmpFile, this.ArchiveFileName);

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void CloseAll()
        {
            if (this.swriter != null)
                this.Dispose();

            if (this.writer != null)
                this.writer.Dispose();

            if (this.archive != null)
                this.archive.Dispose();
            if (this.zipFile != null)
                this.zipFile.Dispose();

        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Finish();

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ZipArchive()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
