using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
namespace HlidacStatu.Lib.Data.Logs
{
    interface ILogs
    {
        DateTime Date { get; set; }
        string ProfileId { get; set; }
    }

    public class ProfilZadavateleDownload : ILogs
    {
        public DateTime Date { get; set; }

        [Keyword]
        public string ProfileId { get; set; }

        public long ResponseMs { get; set; }

        [Boolean]
        public bool? HttpValid { get; set; } = null;


        public string HttpError { get; set; }
        public int? HttpErrorCode { get; set; } = null;

        public bool? XmlValid { get; set; } = null;
        public string XmlError { get; set; }
        public string XmlInvalidContent { get; set; }

        [Keyword]
        public string RequestedUrl { get; set; }

        public void Save(ElasticClient client = null)
        {
            var es = (client ?? ES.Manager.GetESClient_Logs())
                .IndexDocument<ProfilZadavateleDownload>(this);
        }
    }

}
