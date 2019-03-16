using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data;
using System.Security.Cryptography;

namespace HlidacStatu.Lib.IO
{
    public class PrilohaFile : DistributedFilePath<Lib.Data.Smlouva>
    {
        public PrilohaFile()
            : this(Devmasters.Core.Util.Config.GetConfigValue("PrilohyDataPath"))
        { }

        public PrilohaFile(string root)
        : base(3, root, (s) => { return Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(s.Id).Substring(0, 3) + "\\" + s.Id; })
        {
        }
        public override string GetFullDir(Smlouva obj)
        {
            return base.GetFullDir(obj) + obj.Id + "\\";
        }
        public string GetFullPath(Smlouva obj, Smlouva.Priloha priloha)
        {
            return this.GetFullPath(obj, priloha.odkaz);
        }

        public override string GetFullPath(Smlouva obj, string prilohaUrl)
        {
            if (string.IsNullOrEmpty(prilohaUrl) || obj == null)
                return string.Empty;
            return this.GetFullDir(obj)  + Encode(prilohaUrl);
        }

        public override string GetRelativeDir(Smlouva obj)
        {
            return base.GetRelativeDir(obj) + obj.Id + "\\";
        }
        public string GetRelativePath(Smlouva obj, Smlouva.Priloha priloha)
        {
            return this.GetRelativePath(obj, priloha.odkaz);
        }
        public override string GetRelativePath(Smlouva obj, string prilohaUrl)
        {
            if (string.IsNullOrEmpty(prilohaUrl) || obj == null)
                return string.Empty;
            return this.GetRelativeDir(obj) + Encode(prilohaUrl);

            //return base.GetRelativePath(obj, Devmasters.Core.CryptoLib.Hash.ComputeHash(prilohaUrl));
        }


        public static string Encode(string prilohaUrl)
        {
            return Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(prilohaUrl);
            //using (MD5 md5Hash = MD5.Create())
            //{
            //    byte[] md5= md5Hash.ComputeHash(Encoding.UTF8.GetBytes(prilohaUrl));
            //    return System.Convert.ToBase64String(md5.ToString("X")).Replace("=","-");
            //}
        }



    }
}
