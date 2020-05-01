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
                public DataSet Ds = null;
                public string ItemId = null;
                IO.DatasetSavedFile filesystem = null;
                public SavedFile(DataSet ds, string itemId)
                {
                    this.Ds = ds;
                    this.ItemId = itemId;
                    filesystem = new IO.DatasetSavedFile(this.Ds);
                }

                private string GetFullPath(string filename)
                {
                    return filesystem.GetFullPath(this, NormalizeFilename(filename));
                }

                public byte[] GetData(string filename)
                {
                    return System.IO.File.ReadAllBytes(GetFullPath(filename));
                }

                public byte[] GetData(Uri url)
                {

                    return System.IO.File.ReadAllBytes(GetFullPath(url.LocalPath));

                }

                private string NormalizeFilename(string filename)
                {
                    string validFilename = System.IO.Path.GetFileName(filename);

                    if (string.IsNullOrWhiteSpace(validFilename) || validFilename.Length < 3)
                        validFilename = Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(filename);

                    var generatedFileName = this.ItemId + "_" + validFilename;
                    if (generatedFileName.Length > 80)
                        generatedFileName = Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(generatedFileName);

                    return generatedFileName;
                }

                public bool Save(string fullPath)
                {
                    return Save(System.IO.File.ReadAllBytes(fullPath), fullPath);
                }
                public bool Save(Uri url)
                {
                    using (Devmasters.Net.Web.URLContent net = new Devmasters.Net.Web.URLContent(url.AbsoluteUri))
                    {
                        net.Timeout = 180 * 1000;
                        net.Tries = 3;
                        net.TimeInMsBetweenTries = 10000;
                        var data = net.GetBinary().Binary;
                        return Save(data, url.LocalPath);
                    }
                }

                public bool Save(byte[] data, string filename)
                {

                    var fullname = this.GetFullPath(filename);
                    try
                    {
                        System.IO.File.WriteAllBytes(fullname, data);
                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
