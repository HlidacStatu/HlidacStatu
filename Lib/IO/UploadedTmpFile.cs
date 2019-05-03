using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data;
using System.Security.Cryptography;

namespace HlidacStatu.Lib.IO
{
    public class UploadedTmpFile : DistributedFilePath<string>
    {
        public UploadedTmpFile()
            : this(Devmasters.Core.Util.Config.GetConfigValue("PrilohyDataPath") + "\\_TmpUploaded")
        { }

        public UploadedTmpFile(string root)
        : base(1, root, (s) => s)
        {
            InitPaths();
        }
        
    }
}
