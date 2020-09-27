using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HlidacStatu.Lib.IO
{
    public abstract class DistributedFilePath<T>
         where T : class
    {

        int hashLen = 0;
        string root = "";
        protected Func<T, string> funcToGetId = null;
        public DistributedFilePath(int hashLength, string root, Func<T,string> getId)
        {
            this.hashLen = hashLength;
            this.root = root.Trim();
            if (!root.EndsWith("\\"))
                this.root = root + "\\";
            funcToGetId = getId;
        }

        public string Root { get { return root; } }

        public void InitPaths()
        {
            if (!System.IO.Directory.Exists(root))
            {
                System.IO.Directory.CreateDirectory(root);

                for (int i = 0; i < Math.Pow(16, this.hashLen); i++)
                {
                    string hash = i.ToString("X" + hashLen);
                    if (!System.IO.Directory.Exists(root + hash))
                        System.IO.Directory.CreateDirectory(root + hash);
                }
            }

        }

        public virtual string GetFullPath(T obj, string filename)
        {
            return GetFullDir(obj) +  filename;
        }

        public virtual string GetFullDir(T obj)
        {
            return string.Format("{0}{1}\\"
                , this.root, GetHash(obj));
        }

        public virtual string GetRelativeDir(T obj)
        {
            return GetHash(obj)+ "\\";

        }


        public virtual string GetRelativePath(T obj, string filename)
        {
            return GetRelativeDir(obj) + filename;

        }


        protected virtual string GetHash(T obj)
        {
            return Devmasters.Crypto.Hash.ComputeHashToHex(funcToGetId(obj)).Substring(0, this.hashLen);
        }





    }
}

