using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External.DataSets
{
    public partial class DataSet
    {
        public class Item
        {
            public class SavedFile
            {
                public class FileAttributes
                {
                    public string Source { get; set; }
                    public DateTime Downloaded { get; set; }
                    public string ContentType { get; set; }
                    public int Version { get; set; } = 0;
                    public long Size { get; set; }
                    public string Filename { get; set; }
                }

                public DataSet Ds = null;
                public string ItemId = null;
                IO.DatasetSavedFile filesystem = null;
                public SavedFile(DataSet ds, string itemId)
                {
                    this.Ds = ds;
                    this.ItemId = itemId;
                    filesystem = new IO.DatasetSavedFile(this.Ds);
                }

                private string GetFullPath(Uri url)
                {
                    return GetFullPath(url.PathAndQuery);
                }
                private string GetFullPath(string filename)
                {
                    return filesystem.GetFullPath(this, NormalizeFilename(filename));
                }

                //don't use
                private byte[] GetData(string filename)
                {
                    return System.IO.File.ReadAllBytes(GetFullPath(filename));
                }

                public bool Exists(Uri url)
                {
                    var fullname = GetFullPath(url);
                    var fullnameAttrs = fullname + ".attrs";
                    if (!System.IO.File.Exists(fullnameAttrs))
                        return false;

                    var attrs = Newtonsoft.Json.JsonConvert.DeserializeObject<FileAttributes>(System.IO.File.ReadAllText(fullnameAttrs));
                    return System.IO.File.Exists(VersionedFilename(fullname, attrs.Version));

                }

                public byte[] GetData(Uri url)
                {
                    var fullname = GetFullPath(url);
                    var fullnameAttrs = fullname + ".attrs";

                    var attrs = Newtonsoft.Json.JsonConvert.DeserializeObject<FileAttributes>(System.IO.File.ReadAllText(fullnameAttrs));
                    return System.IO.File.ReadAllBytes(VersionedFilename(fullname, attrs.Version));

                }

                private string NormalizeFilename(string filename)
                {
                    string validFilename = HlidacStatu.Util.IOTools.MakeValidFileName(filename);

                    if (string.IsNullOrWhiteSpace(validFilename) || validFilename.Length < 3)
                        validFilename = Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(filename);

                    var generatedFileName = this.ItemId + "_" + validFilename;
                    if (generatedFileName.Length > 80)
                        generatedFileName = Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(generatedFileName);

                    return generatedFileName;
                }

                //private now, don't use
                private bool __Save(string originalFilePath)
                {

                    System.IO.FileInfo fi = new System.IO.FileInfo(originalFilePath);

                    FileAttributes fa = new FileAttributes()
                    {
                        ContentType = HlidacStatu.Util.MimeTools.MimetypeFromExtension(originalFilePath),
                        Source = originalFilePath,
                        Size = fi.Length,
                        Downloaded = DateTime.UtcNow
                    };
                    var data = System.IO.File.ReadAllBytes(originalFilePath);
                    return SaveMeOnDisk(ref data, this.GetFullPath(originalFilePath), fa);
                }
                public bool Save(Uri url)
                {
                    using (Devmasters.Net.Web.URLContent net = new Devmasters.Net.Web.URLContent(url.AbsoluteUri))
                    {
                        net.Timeout = 180 * 1000;
                        net.Tries = 3;
                        net.TimeInMsBetweenTries = 10000;
                        var data = net.GetBinary().Binary;
                        var fn = net.ResponseParams.Headers["Content-Disposition"];
                        if (!string.IsNullOrWhiteSpace(fn))
                            fn = HlidacStatu.Util.ParseTools.GetRegexGroupValue(fn, "filename=\"(?<fn>.*)\"", "fn");

                        var attrs = new FileAttributes()
                        {
                            Downloaded = DateTime.UtcNow,
                            Source = url.AbsoluteUri,
                            ContentType = net.ContentType,
                            Size = data.Length,
                            Filename = fn
                        };

                        return Save(ref data, url, attrs);
                    }
                }


                private static string VersionedFilename(string fullname, int version)
                {
                    if (version == 0)
                        return fullname;
                    else
                        return fullname + "." + version;
                }
                public bool Save(ref byte[] data, Uri url, FileAttributes attrs)
                {
                    var fullname = this.GetFullPath(url);
                    return SaveMeOnDisk(ref data, fullname, attrs);
                }
                private bool SaveMeOnDisk(ref byte[] data, string fullname, FileAttributes attrs)
                {

                    var fullnameAttrs = fullname + ".attrs";
                    try
                    {
                        if (System.IO.File.Exists(fullname))
                        {
                            int nextVersion = FindNextVersion(fullname);
                            int lastVersion = nextVersion - 1;
                            System.IO.FileInfo fiLast = new System.IO.FileInfo(VersionedFilename(fullname, lastVersion));
                            if (HlidacStatu.Util.IO.BinaryCompare.FilesContentsAreEqual(fiLast, data))
                            {
                                data = null;
                                return true;
                            }
                            attrs.Version = nextVersion;
                        }
                        System.IO.File.WriteAllBytes(VersionedFilename(fullname, attrs.Version), data);
                        System.IO.File.WriteAllText(fullnameAttrs, Newtonsoft.Json.JsonConvert.SerializeObject(attrs));
                        data = null;
                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                private static int FindNextVersion(string fullname)
                {
                    int maxVersions = 99;
                    int version = 0;
                    while (true)
                    {
                        version++;
                        var tmpFn = fullname + "." + version.ToString();
                        if (!System.IO.File.Exists(tmpFn))
                            return version;

                        if (version == maxVersions)
                            return maxVersions;
                    }
                }



            }
        }
    }
}
