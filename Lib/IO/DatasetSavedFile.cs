using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data;
using System.Security.Cryptography;
using HlidacStatu.Lib.Data.External.DataSets;

namespace HlidacStatu.Lib.IO
{
    public class DatasetSavedFile : DistributedFilePath<DataSet.Item.SavedFile>
    {
        public DatasetSavedFile(DataSet ds)
            : this(ds, Devmasters.Core.Util.Config.GetConfigValue("DatasetFilePath"))
        { }

        public DatasetSavedFile(DataSet ds, string root)
        : base(2, 
              root.EndsWith("\\") ? root+ds.DatasetId : root+"\\"+ds.DatasetId, 
              (s) => { return Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(s.ItemId).Substring(0, 2); })
        {
            InitPaths();
        }
        
        protected override string GetHash(DataSet.Item.SavedFile obj)
        {
            return this.funcToGetId(obj);
        }


    }
}
