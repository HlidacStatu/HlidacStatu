using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace HlidacStatu.Lib.Data.VZ
{
    public class ProfilZadavatele
    {
        public string Id
        {
            get
            {
                return this.DataSet + "-" + this.EvidencniCisloProfilu;
            }
        }

        [Keyword]
        public string EvidencniCisloFormulare { get; set; }
        [Keyword]
        public string EvidencniCisloProfilu { get; set; }
        public VerejnaZakazka.Subject Zadavatel { get; set; } = new VerejnaZakazka.Subject();
        [Keyword]
        public string Url { get; set; }
        [Date()]
        public DateTime? DatumUverejneni { get; set; }

        [Date()]
        public DateTime? LastUpdate { get; set; }

        [Date()]
        public DateTime? LastAccess { get; set; }
        public enum LastAccessResults
        {
            OK = 0,
            HttpError = 1,
            XmlError = 2,
            OtherError = 3
        }

        //0=OK
        public LastAccessResults? LastAccessResult { get; set; }


        [Keyword]
        public string DataSet { get; set; }

        public void Save(ElasticClient client = null)
        {

            if (
                !string.IsNullOrEmpty(this.EvidencniCisloFormulare)
                && !string.IsNullOrEmpty(this.EvidencniCisloFormulare)
                && !string.IsNullOrEmpty(this.Url)
                )
            {
                var es = (client ?? ES.Manager.GetESClient_ProfilZadavatele())
                    .IndexDocument<ProfilZadavatele>(this);

            }
        }

        public static ProfilZadavatele GetByUrl(string url, ElasticClient client = null)
        {

            var f = (client ?? ES.Manager.GetESClient_ProfilZadavatele())
                .Search<ProfilZadavatele>(s => s
                    .Query(q => q
                        .Term(t => t.Field(ff => ff.Url).Value(url))
                    )
                );
            if (f.IsValid)
            {
                if (f.Total == 0)
                    return null;
                else
                    return f.Hits.First()?.Source;
            }
            else
                throw new ApplicationException("ES error\n\n" + f.ServerError.ToString());
        }
        public static ProfilZadavatele GetById(string profileId, ElasticClient client = null)
        {
            if (string.IsNullOrEmpty(profileId))
                return null;
            var f = (client ?? ES.Manager.GetESClient_ProfilZadavatele())
                .Get<ProfilZadavatele>(profileId);
            if (f.Found)
                return f.Source;
            else
                return null;
        }

        public static ProfilZadavatele[] GetByIco(string ico, ElasticClient client = null)
        {
            try
            {
                var f = (client ?? ES.Manager.GetESClient_ProfilZadavatele())
                    .Search<ProfilZadavatele>(s => s
                        .Query(q => q
                            .Term(t => t.Field(ff => ff.Zadavatel.ICO).Value(ico))
                        )
                    );
                if (f.IsValid)
                    return f.Hits.Select(m => m.Source).ToArray();
                else
                    return new ProfilZadavatele[] { };

            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("ERROR ProfilZadavatele.GetByIco for ICO " + ico, e);
                return new ProfilZadavatele[] { };
            }
        }

        public static ProfilZadavatele GetByRawId(string datasetname, string profileId, ElasticClient client = null)
        {
            var id = datasetname + "-" + profileId;
            return GetById(id, client);

        }

    }

}
