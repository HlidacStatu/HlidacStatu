using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class Backup
    {
        [Nest.Keyword]
        public string Id { get; set; }

        public DateTime Created { get; set; }

        public string Comment { get; set; }
        public KIndexData KIndex { get; set; }

        public static void CreateBackup(string comment, string ico, bool? useTempDb = null)
        {
            KIndexData kidx = KIndexData.GetDirect(ico);
            if (kidx == null)
                return;
            CreateBackup(comment, kidx, useTempDb);
        }
        public static void CreateBackup(string comment, KIndexData kidx, bool? useTempDb=null)
        {
            useTempDb = useTempDb ?? !string.IsNullOrEmpty(Devmasters.Config.GetWebConfigValue("UseKindexTemp"));

            if (kidx == null)
                return;
            Backup b = new Backup();
            //calculate fields before saving
            b.Created = DateTime.Now;
            b.Id = $"{kidx.Ico}_{b.Created:s}";
            b.Comment = comment;
            b.KIndex = kidx;
            var client = ES.Manager.GetESClient_KIndexBackup();
            if  (useTempDb.Value)
                client = ES.Manager.GetESClient_KIndexBackupTemp();

            var res = client.Index<Backup>(b, o => o.Id(b.Id)); //druhy parametr musi byt pole, ktere je unikatni
            if (!res.IsValid)
            {
                Util.Consts.Logger.Error("KIndex backup save error\n" + res.ServerError?.ToString());
                throw new ApplicationException(res.ServerError?.ToString());
            }
        }

    }
}
