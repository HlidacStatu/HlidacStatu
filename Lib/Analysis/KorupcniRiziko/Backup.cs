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

        public static void CreateBackup(string comment, string ico)
        {
            KIndexData kidx = KIndexData.GetDirect(ico);
            if (kidx == null)
                return;
        }
        public static void CreateBackup(string comment, KIndexData kidx)
        {
            if (kidx == null)
                return;
            Backup b = new Backup();
            //calculate fields before saving
            b.Created = DateTime.Now;
            b.Id = $"{kidx.Ico}_{b.Created:s}";
            b.Comment = comment;
            b.KIndex = kidx;
            var res = ES.Manager.GetESClient_KIndexBackup().Index<Backup>(b, o => o.Id(b.Id)); //druhy parametr musi byt pole, ktere je unikatni
            if (!res.IsValid)
            {
                throw new ApplicationException(res.ServerError?.ToString());
            }
        }

    }
}
