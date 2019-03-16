using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public class Subject
    {
        #region ciselniky


        #endregion

     

        public Guid Id { get; set; } = Guid.NewGuid();

        [Keyword()]
        public string FirmoId { get; set; } //db_id z Firmo.cz

        [Keyword()]
        public string ObchodniJmeno { get; set; }
        public string SidloAdresa { get; set; }
        public string Stat { get; set; }

        [Keyword()]
        public string ICO { get; set; }

        [Keyword()]
        public string DIC { get; set; }

        public int Typ { get; set; }

        [Keyword()]
        public string DatovaSchranka { get; set; }

        public DateTime? DatumVzniku { get; set; }
        public DateTime? DatumZaniku { get; set; }

        [Object(Enabled = false)]
        public string Zdroj { get; set; }


        public static bool ExistsInDb(Subject subj)
        {
            var res = Lib.ES.Manager.GetESClient()
                    .Search<Subject>(s => s
                        .Size(1)
                        .Source(ss => ss.ExcludeAll())
                        .Query(q => q
                             .Term(f => f.ICO, subj.ICO)
                        )

                );
            if (res.IsValid == false)
                Lib.ES.Manager.LogQueryError<Subject>(res);

            return res.Total > 0;
        }

    }


}
