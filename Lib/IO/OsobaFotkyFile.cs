using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data;
using System.Security.Cryptography;

namespace HlidacStatu.Lib.IO
{
    public class OsobaFotkyFile : DistributedFilePath<Lib.Data.Osoba>
    {
        public OsobaFotkyFile()
            : this(Devmasters.Config.GetWebConfigValue("OsobaFotkyDataPath"))
        { }

        public OsobaFotkyFile(string root)
        : base(2, root, (s) => { return Devmasters.Crypto.Hash.ComputeHashToHex(s.NameId).Substring(0, 2) + "\\" + s.NameId; })
        {
            InitPaths();
        }

        protected override string GetHash(Osoba obj)
        {
            return this.funcToGetId(obj);
        }


        public override string GetFullDir(Osoba obj)
        {
            var path = base.GetFullDir(obj);
            return path.Substring(0, path.Length - 1) + "-";
        }

        public override string GetRelativeDir(Osoba obj)
        {
            var path = base.GetRelativeDir(obj);
            return path.Substring(0, path.Length - 1) + "-";
        }

    }
}
