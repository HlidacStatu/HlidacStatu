using HlidacStatu.Lib.XSD;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{

    public class ErrorEnvelope
    {

        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string UserId { get; set; } = null;
        public string apiCallJson { get; set; } = null; 
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? LastUpdate { get; set; } = null;
        public string Error { get; set; } = null;
        public string Data { get; set; } = null;


        public void Save(ElasticClient client = null)
        {
            this.LastUpdate = DateTime.Now;
            var es = client ?? ES.Manager.GetESClient_VerejneZakazkyNaProfiluConverted();
            es.IndexDocument<ErrorEnvelope>(this);
        }

    }
}
